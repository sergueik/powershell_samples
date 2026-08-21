### Info 

replica of [renamer](https://github.com/SetsunaF/Renamer)
an  easy to use massive file renamer with custom  Windows Form like UI.
useful for mormalizing media filenames for a constrained playback/display device

### Background

Document is a massive directory of everything and a kitchen sink, which includes the Visuo drawing Word itself can not handle directly
```
document.docx
│
├── [Content_Types].xml
│
├── _rels/
│   └── .rels
│
├── docProps/
│   ├── app.xml
│   ├── core.xml
│   └── custom.xml                 # optional
│
└── word/
    ├── document.xml               # main document body
    │
    ├── _rels/
    │   └── document.xml.rels      # relationships from document.xml
    │
    ├── styles.xml
    ├── settings.xml
    ├── fontTable.xml
    ├── webSettings.xml
    │
    ├── numbering.xml              # lists / numbering
    ├── comments.xml               # optional
    ├── footnotes.xml              # optional
    ├── endnotes.xml               # optional
    │
    ├── theme/
    │   └── theme1.xml
    │
    ├── media/
    │   ├── image1.png
    │   ├── image2.jpeg
    │   └── ...
    │
    ├── embeddings/
    │   ├── oleObject1.bin
    │   └── ...
    │
    └── charts/
        ├── chart1.xml
        └── ...
```
Older Office:

```code
document.doc
│
├── Root Entry
│
├── WordDocument
├── 0Table / 1Table
├── Data
│
├── ObjectPool/
│   ├── _123456789/
│   │   ├── CompObj
│   │   ├── Ole
│   │   └── ...
│   └── ...
│
└── other storages / streams
```
the entries in the hieratchy above  weren't true filesystem *directories* and *files*. They were *storages* and *streams* inside a single *Compound Binary* File (CFB).

Older binary Office documents used OLE Structured Storage, which presented the contents of a single file as a hierarchical collection of storages and streams through interfaces such as IStorage and IStream


```
Vintage .doc                         Modern .docx
────────────                         ────────────

Compound File                    ZIP / OPC package
     │                                  │
     ├── Storage                        ├── directories
     │                                  │
     └── Stream                         └── parts/files
            │                                  │
            └── binary data                    └── XML/binary resources
```
packing dilemma

```code
┌───────────────────────────────┐
│       BUSINESS PROCESS        │
│                               │
│  Word document                │
│    └── embedded Visio         │
│         { proprietary schema }│
│          └── diagram          │
└───────────────┬───────────────┘
                │
                │  extract payload
                ▼
┌───────────────────────────────┐
│        VISIO RESOURCE         │
│                               │
│ shapes + connectors + context │
│ + IDE details (zoom,font,color│
│ + internal relationships      │
└───────────────┬───────────────┘
                │
                │  semantic extraction
                ▼
┌───────────────────────────────┐
│      MERMAID REPRESENTATION   │
│                               │
│        A ───► B               │
│         │                     │
│         └──► C                │
│                               │
│   business-relevant           │
│   deliberately lossy          │
└───────────────┬───────────────┘
                │
                │  reason / modify / test
                ▼
┌───────────────────────────────┐
│            EXPERIMENT         │
│                               │
│    "Is it calling foo?"       │
│   "Does it really need bar?"  │
│ "Can it interact with baz?"   │
└───────────────┬───────────────┘
                │
                │  final verdict
                ▼
        ┌──────────────────┐
        │ ORIGINAL SOURCE  │
        │   = authority    │
        └──────────────────┘
```

|OLE Structured Storage | OPC / Open XML|
|-----------------------|---------------|
|`StgOpenStorage()`     | open ZIP package|
|`IStorage`                 | package / part hierarchy|
|`IStorage::OpenStream()`   | open package part|
|`IStorage::OpenStorage()`  | navigate relationships/parts|
|`IStream`                  | stream access|

|Expensive path|Proven cheap path|
|--------------|-----------------|
|Word → OLE → Visio → inspect|Word → extract Visio payload|
|BP/UiPath → proprietary internals → inspect|BP/UiPath → extract payload|
|Work directly on system of record|Work on semantic projection|
|Every experiment touches original|Experiments touch Mermaid|
|High operational cost|Low-cost hypothesis testing|
|Final answer still requires judgment|Final answer checked against original|

Given __Visio__ is installed locally → one can *ask* Visio itself about its drawing

```code
' interart with Visio asking it to interpret its own document.

Dim visApp, visDoc, visPage, visShape
Dim filename: filename = "drawing.vsdx"

Dim visApp:Set visApp = CreateObject("Visio.Application"): visApp.Visible = False

Dim visDoc: Set visDoc = visApp.Documents.Open(filename)

For Each visPage In visDoc.Pages
    For Each visShape In visPage.Shapes

        WScript.Echo _
            "  [" & visShape.ID & "] " & _
            "Name=" & visShape.Name & _
            "  Text=" & visShape.Text & _
            "       Type=" & visShape.Type & _
            "  Master=" & visShape.Master.Name

    Next
Next

visDoc.Close: visApp.Quit: Set visShape = Nothing: Set visPage = Nothing: Set visDoc = Nothing: Set visApp = Nothing
```
### Usage
The scenario


```
(05) - Art Farmer, Donald Byrd, Idrees Sulieman - Three Trumpets (1957, 1992, Prestige-OJC) - [You Gotta Dig It to Dig It]. mp3
    │
    ▼
05 - You Gotta Dig It - Art Farmer…Three Trumpets.mp3
```
explains why the regex/capture-group approach is attractive: the filename itself contains semi-structured metadata, but the device doesn't need all of it

After the capturing and reordering retained groups, an additional transformation is performed, e.g. 
* artist/album information is compressed, remains distinct 
* resulting alias fits on a small-screen / weak-display playback device

PS/C# point. The interesting proposition isn't merely “C# can rename files.” It's that the toolchain gives you a nice incremental engineering environment:

That is quite different from the Python/Perl experience of:
* populate directory with files with funny names.
* compose command line
* run script
* examine files
i

I'd keep it almost exactly as you have it:

populate directory with files with funny names
compose command line
run script
examine files

The phrase “files with funny names” is particularly effective here. It makes the problem concrete and slightly playful, while also describing exactly what the test fixture is.
i

he key distinction is that the filesystem becomes merely the input/output boundary. The actual rename logic can be tested entirely in memory:

                    JUnit
                      │
              ┌───────┴────────┐
              │                │
          test fixture      test case
          (string)          (string)
              │                │
              ▼                ▼
        "funny filename" → transformation
                                │
                                ▼
                         expected alias

So instead of:

populate directory
    ↓
invoke script
    ↓
look at renamed files
    ↓
decide whether it worked

you get atomic tests such as:

@Test
void extractsTrackTitle() {
    assertEquals(
        "05 - You Gotta Dig It - Art Farmer…Three Trumpets.mp3",
        transform(
            "(05) - Art Farmer, Donald Byrd, Idrees Sulieman - " +
            "Three Trumpets (1957, 1992, Prestige-OJC) - " +
            "[You Gotta Dig It to Dig It].mp3"
        )
    );
}

And then separately:

@Test
void removesCatalogMetadata() { ... }

@Test
void preservesTrackNumber() { ... }

@Test
void preservesExtension() { ... }

@Test
void compressesArtistList() { ... }

@Test
void rejectsAliasTooLongForDevice() { ... }

That's a very different development model.

And yes, JUnit is particularly attractive here because this isn't some home-grown test harness. You're standing on decades of established machinery: lifecycle annotations, assertions, parameterized tests, fixtures, IDE integration, failure reporting, etc.

The really nice conceptual progression is:

First make the transformation deterministic. Then make it testable. Only then make it touch the filesystem.

That would also make the PowerShell/C# choice more compelling: PowerShell can remain the operational shell, while C# contains a small, strongly testable transformation library.

### Note

The Unicode __horizontal ellipsis__ (`HORIZONTAL ELLIPSIS`) character …  is `U+2026`, it is also known as __HTML Entity__ `&hellip;` or `&#8230;`

the app heavily uses custom UX
![capture app](screenshots/capture-app.png)


```text
Program/bin/Debug/DropdownButton.dll
Program/bin/Debug/MediaInfo.dll
Program/bin/Debug/MediaInfo64.dll
Program/bin/Debug/MetroFramework.dll
Program/bin/Debug/ModernFolderBrowserDialog.dll
Program/bin/Debug/Newtonsoft.Json.dll
Program/bin/Debug/ObjectListView.dll
```
#### Refactoring
```sh
grep -lr ..\\\\Resources ./Program
grep -lr \.\.\\\\Resources ./Program

```
```text
./Program/Program.csproj
./Program/Properties/Resources.resx
```
```sh
grep -lr \.\.\\\\Resources ./Program | xargs -IX sed -i 's|\.\.\\Resources|Resources|g' X

```
copy the depednencies which the original project does not describe as nuget dependncies from original project `Resource/References`
```sh
mkdir Program/Resources/References
pushd Program/Resources
for F in MediaInfo.dll MediaInfo64.dll;  do curl -skLO https://github.com/SetsunaF/Renamer/raw/refs/heads/master/Renamer/$F ;done
cd References
for F in DropdownButton.dll MetroFramework.dll MetroFramework.Design.dll ModernFolderBrowserDialog.dll Newtonsoft.Json.dll ObjectListView.dll ;  do curl -skLO https://github.com/SetsunaF/Renamer/raw/refs/heads/master/Resources/References/$F ;done
popd
```

```powershell
pushd Program/Resources
get-childitem -path '.' -filter '*dll' -file | select-object -expandproperty VersionInfo | select-object -property OriginalFilename,FileVersion | format-list
cd References
get-childitem -path '.' -filter '*dll' -file | select-object -expandproperty VersionInfo | select-object -property OriginalFilename,FileVersion | format-list
popd
```

```text

OriginalFilename : MediaInfo.dll
FileVersion      : 0.7.72.0

OriginalFilename : MediaInfo.dll
FileVersion      : 0.7.72.0


OriginalFilename : DropdownButton.dll
FileVersion      : 1.2.0.0

OriginalFilename : MetroFramework.Design.dll
FileVersion      : 1.3.0.0

OriginalFilename : MetroFramework.dll
FileVersion      : 1.3.0.0

OriginalFilename : ModernFolderBrowserDialog.dll
FileVersion      : 1.0.0.0

OriginalFilename : Newtonsoft.Json.dll
FileVersion      : 6.0.8.18111

OriginalFilename : ObjectListView.dll
FileVersion      : 2.8.0.0
```
### TODO

```
"\\((?<index>[AB0-9][0-9]+)\\)\\s+(?<artist>)\\s+(?<title>)","<index> - <title> - <artist>"

```
### NOTE:

the app heavily uses custom UX

![capture app](screenshots/capture-app.png)

#### Refactoring
```sh
grep -lr ..\\\\Resources ./Program
grep -lr \.\.\\\\Resources ./Program

```
```text
./Program/Program.csproj
./Program/Properties/Resources.resx
```
```sh
grep -lr \.\.\\\\Resources ./Program | xargs -IX sed -i 's|\.\.\\Resources|Resources|g' X

```
### TODO

```
"\\((?<index>[AB0-9][0-9]+)\\)\\s+(?<artist>)\\s+(?<title>)","<index> - <title> - <artist>"

```

### See Also 

  * https://www.nuget.org/packages/MediaInfoDLL
  * https://www.nuget.org/packages/FolderBrowserEx
  * https://www.nuget.org/packages/BetterFolderBrowser
---

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
