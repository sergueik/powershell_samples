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
