Exactly. The missing input isn't really another search criterion. It is a search budget / termination policy.
bounded asynchronous filesystem traversal engine with pluggable result sinks
NOTE: cancellation and inaccessible folders/files as first-class outcomes,
immediate children of the selected root:
concept it becomes a set of global counters + events.
the traversal itself doesn't need to know why it has stopped

What to search

* Root folder — `BrowseForFolder`
* File mask —  e.g. `*.xml`, `invoice*.pdf`, etc.
* Content condition — literal text, regex, perhaps eventually metadata/size/date conditions.


**When to stop**

|Mode|Input|Meaning|
|----|-----|-------|
|File limit|N files|Stop after examining N files|
|Folder limit|N folders|Stop after visiting N top level directories, recursively|
|Time limit|N seconds/minutes|Stop when the elapsed-time budget expires|
|Unlimited|—|Search until the entire reachable tree is exhausted|

these condition could be composable, rather than mutually exclusive:


*Stop when any limit is reached*.

For example:

Search `\\server\share\department`, recursively, for `*.xml` containing `CUSTOMER_ID`, for up to 5,000 files, 10 top level folders, or 60 seconds — *whichever occurs first*.

That is much more appropriate for an SMB environment than pretending the user is doing a normal local Windows Explorer search.

### NPOI

> NOTE: __NPOI__ cannot read or process `.PDF` files (is designed exclusively to read, write, and manipulate Office documents without requiring Microsoft Office Interop) and despite what its official project description claims, is weak in handling `.PPT`/`.PPTX` files

### PdfPig
```sh
export V=0.1.7
curl -skLo ~/Downloads/pdfpig.$V.zip  https://www.nuget.org/api/v2/package/PdfPig/$V
unzip -ql ~/Downloads/pdfpig.$V.zip lib/net45/*
```
```text
  Length      Date    Time    Name
---------  ---------- -----   ----
    11776  2022-12-13 01:14   lib/net45/UglyToad.PdfPig.Package.pdb
    38400  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Core.dll
  4078080  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.dll
   242688  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.DocumentLayoutAnalysis.dll
  1068032  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Fonts.dll
    19968  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Tokenization.dll
    42496  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Tokens.dll
    13936  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Core.pdb
    59973  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Core.xml
    83760  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.DocumentLayoutAnalysis.pdb
   383110  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.DocumentLayoutAnalysis.xml
   101580  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Fonts.pdb
   246421  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Fonts.xml
     9032  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Tokenization.pdb
     9962  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Tokenization.xml
    11024  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.Tokens.pdb
    37338  2022-12-13 01:13   lib/ne
   249992  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.pdb
   577595  2022-12-13 01:13   lib/net45/UglyToad.PdfPig.xml
     4608  2022-12-13 01:14   lib/net45/UglyToad.PdfPig.Package.dll
      148  2022-12-13 01:14   lib/net45/UglyToad.PdfPig.Package.xml
```
```sh
mkdir -p packages/PdfPig.0.1.7/lib/net45
unzip -d packages/PdfPig.0.1.7 ~/Downloads/pdfpig.$V.zip lib/net45/*
```
```sh
find  packages/PdfPig.0.1.7/lib/net45/ -iname '*dll' -exec basename {} \;
```
```text
UglyToad.PdfPig.Core.dll
UglyToad.PdfPig.dll
UglyToad.PdfPig.DocumentLayoutAnalysis.dll
UglyToad.PdfPig.Fonts.dll
UglyToad.PdfPig.Package.dll
UglyToad.PdfPig.Tokenization.dll
UglyToad.PdfPig.Tokens.dll
```
For this tool it is not necessary to tarrget 64 bit, otherwise update to
```cmd
path=%path%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
msuilb.exe WordFile.sln /T:Clean,Build
```

```cmd
Program\bin\Debug\WordFile.exe
```
### Packaging Notes

* The scanner/searcher has a deliberately narrow responsibility: traverse files, inspect what is necessary, and dump runs/results as plain text.
* It does not need to become an Office automation framework, document-generation framework, PDF-processing platform, etc.
* Therefore every *additional* dependency should have a concrete *reason to exist* in that component.
* If NPOI is sufficient for the Office-reading/writing portion, there is little architectural value in continuously chasing every new NPOI release merely because releases exist.
* Also, A PDF dependency can remain undecided until the actual PDF requirement is established.
* The fact that NPOI continues to evolve does not automatically mean that the scanner should continuously evolve with it
* the actual scanner logic may be almost embarrassingly small:
```
document
   │
   ▼
POI proxy
   │
   └── ToString()
         │
         ├── useful text → emit
         │
         └── otherwise → known Regex matcher
```

When filling the candidate slot for PDF, it is good to remember:

**PDF reading ≠ PDF engineering.**

If the QA tool already has a PDF helper library to confirm “yes, this downloaded object is a valid PDF,” that same library is quite possibly capable of producing a textual dump of a PDF.

No reason to consider a heavyweight PDF creation/manipulation framework merely because there is a need to “grep” a PDF.

### Running From Powershell ISE

```sh
cp WordFileTool/packages/NPOI.2.5.0/lib/net45/*dll .
cp WordFileTool/packages/SharpZipLib.1.2.0/lib/net45/*dll .
cp WordFileTool/packages/PdfPig.0.1.7/lib/net45/*dll .
```
launch Powershell console

```powershell
. .\wordfiletool.ps1
```
ignore the error message:
```text
Add-Type : Unable to load one or more of the requested types. Retrieve the LoaderExceptions property for more information.
At ..\wordfiletool.ps1:32 char:5
+     Add-Type -Path $_
+     ~~~~~~~~~~~~~~~~~
    + CategoryInfo          : NotSpecified: (:) [Add-Type], ReflectionTypeLoad
   Exception
    + FullyQualifiedErrorId : System.Reflection.ReflectionTypeLoadException,Microsoft.PowerShell.Commands.AddTypeCommand

```
> NOTE: only first launch succeeds drawing theform. The subsqeuent runs print error
```text
Exception calling "Main" with "0" argument(s): "SetCompatibleTextRenderingDefault must be called before the first IWin32Window object is created in the application."
At ..\wordfiletool.ps1:460 char:1
+ [Program.Program]::Main()
```
Open Powershell ISE. Paste the script into Edit pane and run. 
> NOTE: screen dimensions are wrong:
![powershell ISE run](screenshots/capture-powershell-ise.png)


### See Also

  * https://github.com/hanzhaoxin/ExcelReport - currently on .netstandard, but commit [f3988](https://github.com/hanzhaoxin/ExcelReport/tree/f3988ec14003d2a167552144f458d2a774bcd4bd/ExcelReport) - is .net 4.0 version and see also [project documentation](http://www.cnblogs.com/hanzhaoxin/tag/ExcelReport)
  * [nuget npoi 2.8.1](https://www.nuget.org/packages/npoi/#supportedframeworks-body-tab) supports .Net __4.7.2__. For [Npoi.Extend](https://www.nuget.org/packages/NPOI.Extend/1.0.4) one has to use much older version __1.0.4__ - latest is __1.1.3__ but only list __netstandard 2.0__ which is basically the same but incompatible

  * https://github.com/WuLex/WordFileTool 
  * https://github.com/ToolsByXLG/NPOI.Word2Html
  * https://github.com/IS4Code/npoi - 
  * https://www.nuget.org/packages/npoi/
  * [nissl-lab/POIFSExplorer](https://github.com/nissl-lab/POIFSExplorer) - NPOI based tool helps you explore internal structure of OLE2(ActiveX) documents, including
    + `.xls` __Excel__ document
    + `.doc` __Word__ document
    + `.ppt` __Powerpoint__ document
  * [nissl-lab/OLE2Storage](https://github.com/nissl-lab/OLE2Storage) -  straight (pure-no COM interop) .NET IStorge interface library to read/write __OLE2__(ActiveX) document

  * https://hackernoon.com/comparing-apache-npoi-and-ironxl-in-c-a-complete-guide 
  * For Text Extraction & Parsing - best option is [PdfPig](https://www.nuget.org/packages/PdfPig/0.1.7#supportedframeworks-body-tab)
  * [PdfPig Wiki](https://github.com/UglyToad/PdfPig/wiki)

### TLDR
[Neuschwanstein Castle](https://en.wikipedia.org/wiki/Neuschwanstein_Castle) in southern Germany is the famous
fairytale palace that inspired Disney's [Cinderella](https://en.wikipedia.org/wiki/Cinderella_Castle) and [Sleeping Beauty](https://en.wikipedia.org/wiki/Sleeping_Beauty_Castle) Magic
Kindom Orlando Disney World castles

---
### Author
[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
