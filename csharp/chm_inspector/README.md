### Info

Program to examine the contents of a compiled help file (CHM) without fully extracting it, based on the [Get CHM Title](https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title) snippet by *Castorix31*.

Uses the **Microsoft InfoTech IStorage System** COM server `{5D02926A-212E-11D0-9DF9-00A0C922E6EC}` to read storage directly without extracting the compiled HTML archive. **MSITFS** is the HTML Help/CHM runtime’s storage implementation (`itss.dll`).

Notably, [7-Zip](https://www.7-zip.org) can unpack/extract CHM files (listed as an “unpacking only” format). 7-Zip does **not** provide CHM creation or repacking functionality. Source code and downloads are available from the official 7-Zip site.

---

### Status

* **Implemented**
  * Reading the title of the compiled help file and listing component HTML files via `StgOpenStorage` and `7z.exe` commands
  * Loading the file list into a `DataGrid` for selection of files to extract

* **Work in progress**
  * Extracting selected files
  * Peeking for additional metadata to help select files — useful for large business-grade help files with tens of thousands of component documents

![wip1](screenshots/app1.jpg)
![wip2](screenshots/app2.jpg)
![wip3](screenshots/app3.jpg)

---
### Testing


Minimal dummy CHM generation using official, free [HTML Help Workshop](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/htmlhelp/microsoft-html-help-downloads)


Steps:

  * Add 2–3 HTML files (topic1.htm, topic2.htm)
  
  
  * Create `dummy.hhp` project file with:
```ini
[OPTIONS]
Title=Dummy CHM
Default Topic=topic1.htm

[FILES]
topic1.htm
topic2.htm
```


* Compile with 
```cmd
hhc.exe dummy.hhp
```

this produces `dummy.chm` with __Titles__ in `#STRINGS` and __URL__ list in `#URLSTR`

---


### Out Of Memory Exception


The stream-reading logic for extracting data from Structured Storage had an implementation flaw in an earlier revision, resulting in an
__OutOfMemoryException__ at
[Chm.cs#L391]	(https://github.com/sergueik/powershell_samples/blob/debug-oom/csharp/chm_inspector/Utils/Chm.cs#L391) 
This happened because the code attempted to read the entire storage stream into memory using a fixed-size buffer without checking how many bytes were actually read, causing the loop to repeatedly append unbounded data to the `MemoryStream` until the process exhausted its available contiguous virtual memory segment.

```c#
public static List<TocEntry> toc_structured(string filePath) {
  logger.Info("toc_structured: starting");
    object obj = null;
    IITStorage iit = null;
    IStorage storage = null;
    IEnumSTATSTG enumStat = null;
    IStream stream = null;

    var result = new List<TocEntry>();

    try {
        obj = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_ITStorage, true));
        iit = (IITStorage)obj;

        HRESULT hresult = iit.StgOpenStorage(filePath, null, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), IntPtr.Zero, 0, out storage);
        if (hresult != HRESULT.S_OK || storage == null)
        throw new Exception(String.Format("Failed to open CHM file\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));

        hresult = storage.EnumElements(0, IntPtr.Zero, 0, out enumStat);
        if (hresult != HRESULT.S_OK || enumStat == null)
        throw new Exception(String.Format("Failed to enumerate CHM elements\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));

        var stat = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
        uint fetched;

        while (enumStat.Next(1, stat, out fetched) == HRESULT.S_OK && fetched == 1) {
            // We are looking for "toc.hhc"
            if (string.Equals(stat[0].pwcsName, "toc.hhc", StringComparison.OrdinalIgnoreCase)) {
                hresult = storage.OpenStream(stat[0].pwcsName, IntPtr.Zero, (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_READ), 0, out stream);
                if (hresult != HRESULT.S_OK || stream == null)
                  throw new Exception(String.Format("Failed to open toc.hhc stream,\nError: 0x{0}\n{1}", hresult.ToString("X"), MessageHelper.Msg(hresult)));

                // Read full stream
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[4096];
                IntPtr pcb = IntPtr.Zero;
        // NOTE: do not be logging every iteration of the read loop
        int loopCounter = 0;
                while (true) {
                    stream.Read(buffer, buffer.Length, pcb);
                    loopCounter++;	                    
            if (loopCounter % 500 == 0)
                      logger.Info(String.Format( "Memory {0} MB", GC.GetTotalMemory(false) / (1024 * 1024)));
                    // Assume buffer fully read; could refine with actual bytes read
                    ms.Write(buffer, 0, buffer.Length);
                    // For simplicity, break when less than buffer size (optional refinement)
                    if (buffer.Length < 4096) break;
                }

                string tocContent = Encoding.UTF8.GetString(ms.ToArray());

                // Regex parse OBJECT nodes
                var matches = Regex.Matches(tocContent,
                    @"<OBJECT[^>]*>.*?<param name=""Name"" value=""(.*?)"">.*?<param name=""Local"" value=""(.*?)"">.*?</OBJECT>",
                    RegexOptions.Singleline);

                foreach (Match m in matches) {
                    result.Add(new TocEntry {
                        Name = m.Groups[1].Value,
                        Local = m.Groups[2].Value
                    });
                }

                break; // done with toc.hhc
            }
        }
    } finally {
        if (stream != null) Marshal.ReleaseComObject(stream);
        if (enumStat != null) Marshal.ReleaseComObject(enumStat);
        if (storage != null) Marshal.ReleaseComObject(storage);
        if (iit != null) Marshal.ReleaseComObject(iit);
        if (obj != null) Marshal.ReleaseComObject(obj);
    }

    return result;

  }
```

A more resilient approach (achieved in the commit [37fa6dbfe](https://github.com/sergueik/powershell_samples/commit/37fa6dbfe44d94856ec0fa8c35aed558a10f01b6))involves:

  * Checking the exact number of bytes returned by `IStream.Read`,
  * Stopping when fewer than `buffer.Length` bytes are read,
  * processing the stream incrementally to avoid building a full in-memory copy.


Memory allocation and segmentation ensure that your process only receives a limited, contiguous block of usable RAM/virtual memory, and an `OutOfMemoryException` simply means the system could not reserve a sufficiently large continuous segment for your in-memory buffer—even if total free memory still existed.

#### Telemetry and Log Collection

The sample includes optional instrumentation (e.g., periodic `GC.GetTotalMemory()`  [metric](https://learn.microsoft.com/en-us/dotnet/api/system.gc.gettotalmemory?view=netframework-4.5) logging) that can be forwarded to a 
log collector such as [Seq](https://datalust.co/), or to telemetry stacks such as [Prometheus](https://prometheus.io/download/)/ [Grafana](https://grafana.com/grafana/download) to observe memory pressure and stream read behavior 

The sample includes optional instrumentation (e.g., periodic GC.GetTotalMemory() logging) that can be forwarded to a log collector such as Seq, Prometheus, or Grafana to observe memory pressure and stream read behavior.


By examining this metric, whose values typically spike sharply just before failure, 


```c#
logger.Info(String.Format( "Memory {0} MB", GC.GetTotalMemory(false) / (1024 * 1024)));
```
![oom2](screenshots/oom2.jpg)


![metric](screenshots/metric1.jpg)


one can clearly visualize heap exhaustion and the conditions leading up to an impending `OutOfMemoryException`	.


NOTE: Seq is primarily a lightweight 
fine-grained, application-level log collection and indexing platform, 
not a telemetry indicator or metrics system, and it does not provide 
built-in capabilities for transforming log message content.
![wip3](screenshots/seq2.jpg)

In an ideal setup for collecting and rendering application telemetry, the Seq event stream—received via a Serilog sink—would be forwarded to Grafana for visualization.
The other option is to use [ECS Logging .NET](https://www.elastic.co/docs/reference/ecs/logging/dotnet/setup)
This, however, requires a compatible Seq-JSON data source or 
Grafana plugin, which is still under investigation and not yet fully supported.

Another viable option is to integrate the ELK stack, 
using the `Serilog.Sinks.Elasticsearch` 
[NuGet package](https://www.nuget.org/packages/Serilog.Sinks.Elasticsearch) for .NET, or 
the [Logback ECS Encoder](https://mvnrepository.com/artifact/co.elastic.logging/logback-ecs-encoder)/[Logstash Logback Encoder](https://mvnrepository.com/artifact/net.logstash.logback/logstash-logback-encoder)
dependencies for Java, allowing log events 
to be indexed directly into Elasticsearch and transformed and visualized through Kibana.

---

### TOC

Information about the topics covered in individual files is stored in the file named `toc.hhc` as a set of objects:

```html
<OBJECT type="text/sitemap">
  <param name="Name" value="Starting a new VM for the first time">
  <param name="Local" value="ch01s09.html#idp8051664">
</OBJECT>

The toc.hhc is a plain HTML-like file inside the CHM (MS ITSS) archive. For a selection helper grid, the relevant attributes are Name and Local.

#### Extracting toc.hhc with `7-Zip`

```cmd
7z.exe e ${chm_file} toc.hhc -o${output_folder}
```

#### Reading `toc.hhc` via MSITFS COM Server

Using the **COM** **API** allows reading the file directly from the `CHM` archive without extracting:
```c#
IStorage storage;
HRESULT hr = StgOpenStorage(
    "myfile.chm",
    null,
    STGM_READ | STGM_SHARE_EXCLUSIVE,
    IntPtr.Zero,
    0,
    out storage
);

```
This is the same approach used internally by **MS HTML Help Workshop** or libraries such as **ChmLib**.

#### Parsing `toc.hhc`


`OBJECT` nodes can be parsed using a lightweight DOM parser (e.g., standalone [embedded DOM parser]() ) or even a simple regex:

```c#
var matches = Regex.Matches(
    tocContent,
    @"<OBJECT[^>]*>.*?<param name=""Name"" value=""(.*?)"">.*?<param name=""Local"" value=""(.*?)"">.*?</OBJECT>",
    RegexOptions.Singleline
);

foreach(Match match in matches) {
    string name = match.Groups[1].Value;
    string local = match.Groups[2].Value;
    // add to Dictionary<string, string> or a List<TocEntry> for the grid datasource
}

```
This regex approach avoids **DOM** parsing overhead, which is typically unnecessary since `toc.hhc` is usually very small.

### TODO

For full CHM introspection (beyond listing HTML filenames), `#URLSTR` alone is insufficient.  
The CHM internal structure requires cross-referencing several internal streams to reconstruct:

* Topic filenames
* Topic numeric IDs
* Topic titles
* Table of Contents (TOC)
* Index associations
* “Home” page
* Window definitions

---

### See Also

* [Original snippet](https://learn.microsoft.com/en-us/answers/questions/1358539/get-chm-title)
* [CodeProject example](https://www.codeproject.com/articles/Decompiling-CHM-help-files-with-C-) of decompiling CHM files with C# by [Yuriy Maksymenko](https://forum.codeproject.com/user/yuriy-maksymenko) (source code appears to be missing). Possible LinkedIn profile: [Yuriy Maksymenko](https://www.linkedin.com/in/yuri-canada/?originalSubdomain=ca)
* [StackOverflow discussion](https://stackoverflow.com/questions/9391424/how-to-get-a-list-of-topics-from-a-chm-file-in-c-sharp)
* [MSDN: IStorage and the compound file (structured storage) model](https://learn.microsoft.com/en-us/windows/win32/api/objidl/nn-objidl-istorage)
* [Open-source CHM spec and practical decompiler examples](https://www.nongnu.org/chmspec/latest/Miscellaneous.html) — CHM is a compound/LZX/LIT-style archive containing HTML/MHT, LZX compression, and index structures
* Partial implementation of `itss.dll` in [Wine](https://bugs.winehq.org/show_bug.cgi?id=7517)  
* `IStorage` compound file implementation (`StgOpenStorageEx` / `StgCreateStorageEx`) — [MSDN](https://learn.microsoft.com/en-us/windows/win32/stg/istorage-compound-file-implementation)
* Wine [source tree](https://gitlab.winehq.org/skitt/wine/-/tree/master/dlls/itss) for `itss` implementation
* sample [chm file](https://submain.com/ghostdoc/samples/PowerCollections/CHM/PowerCollectionsCHM.zip) from PowerCollections 
---

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
[
