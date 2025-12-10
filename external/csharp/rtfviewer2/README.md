### Info

This directory contains a replica of [App to convert Markdown (.md) files to RTF Rich Text files](https://github.com/snjo/MarkdownToRtf), translated from C# __6.x__, __7.x__, and __9.x__ to plain C# __5.0__ syntax / __.NET Framework 4.5__.
The driver can also be compiled as a dependency to display __Markdown__ files in a __Rich Text__ field in a C# Windows Forms application.

Surprisingly, __Markdig__ favors [XAML](https://en.wikipedia.org/wiki/Extensible_Application_Markup_Language)/[WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation) over [RichText](https://en.wikipedia.org/wiki/Rich_Text_Format) and __HTML__ output.

![app1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/app1.jpg)

### Notes

- `$` - *interpolated strings* - C# __6.0__
- *Tuple return types and deconstruction* - C# __7.0__ (required multiple parser and compiler adjustments):
  - Assignment grammar
  - Return type syntax
  - Overload resolution
  - Deconstruction rules
- *Target-typed* `new` - C# __9.0__
- `AsSpan` instead of `Substring` – not supported in C# __5.0__ due to compiler dependencies
- `using static` directive - C# __6.0__
- *discard*-style unused variables`_` - C# __7.0__
- *async* / *await* and `Task` - C# __5.0__
- *null-conditional operator* `?.` - C# __6.0__
- *null-coalescing operator* `x ?? y` - C# __2.0__ (often confused with *null-conditional operator* )



### Usage

* Launch the Tool

* The markdown payload to render can be pasted into the left pane text area window. This the only currently supported loading process. Earlier available "File Load" functionality has been hidden from te toolbar

* The tool will convert the Markdown payload into RTF while adding invisible yet [searchable](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox.find?view=windowsdesktop-10.0#system-windows-forms-richtextbox-find(system-string-system-int32-system-windows-forms-richtextboxfinds))  [markers]() - the `\u8203?` which is the RTF representarion of `\u200B` (the Unicode [Zero Width Space](https://en.wikipedia.org/wiki/Zero-width_space) [code point](https://en.wikipedia.org/wiki/List_of_Unicode_characters))

![debug1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/debug1.jpg)

after every

* [Paragraphs](https://www.markdownguide.org/basic-syntax/#paragraphs-1)
* [Headings](https://www.markdownguide.org/basic-syntax/#headings)
* [Images](https://www.markdownguide.org/basic-syntax/#images)
* [Lists](https://www.markdownguide.org/basic-syntax/#lists-1)
* [Tables](https://www.markdownguide.org/extended-syntax/#tables)
* [Code Blocks](https://www.markdownguide.org/basic-syntax/#code-blocks).

section of the original document to facilitate navigation. i


- Implemented the **forward / backward navigation buttons** to scroll to the following/preceding anchor, enabling vi-style movement *by paragraph*, *section*, or *code block* logical units - 
the __step-forward__ and __step-backward__ are moving the selection from one hidden to the following or preceding one.



NOTE :*the Markers remain invisible during normal rendering and do not affect the displayed content.)*

- Implement a **toggle button** in the UI to instantly reveal or hide the markers for debugging or inspection purposes. (WIP)

### Example to Navigate

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |
#### Example of Invisible Text Formatted as Hidden with the \v Character-Formatting Control Word

- Fragment *without* markers:

```rtf
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24
This is visible text. This is also visible text.
}

![app1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/form1.jpg)
```

-- Same fragment *with* **hidden marker**:

```rtf
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24 This is visible text. {\v This text is hidden.} This is also visible text.}
```

![form2](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/form2.jpg)

The hidden text will not be displayed; visually, it renders identically.

### Error Processing

If processing of the markdown has encountered errors these are displayed in a custom dialog

![error3](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/error3.jpg)

and affected markup elements are rendered verbatim with `PARSE ERROR` prefix

![error4](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/error4.jpg)

By pressing __Suppress__ button one stops this dialog from showing

### ⚠️ Bugs: Rendering RTF inside Markdown

#### Problem

Placing *real* RTF markup inside Markdown code blocks—either by indentation or fenced code blocks—causes the entire document to fail to render when passed through `RichTextBox` or `RichTextBoxControl`.

This renders correctly when embedded *as plain text*:

```
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24
This is visible text. This is also visible text.
}
```

But turning the same text into a code block (indented or fenced) results in malformed or unreadable output.

#### Why this happens

When Markdown is converted to RTF, the renderer must treat code blocks as **literal text**, not as actual RTF content.
However, unescaped RTF control characters inside a code block:

- `{` and `}` (RTF group delimiters)
- backslash `\` introducing control words
- sequences like `\pard`, `\fs24`, etc.

are still interpreted by the Windows RichText control as real RTF commands.

If the Markdown → RTF layer does **not escape** these characters, the output becomes invalid RTF.

#### Example of a failure case

Even placing raw RTF inside a Markdown HTML comment produces partial corruption:

```markdown
<!--
   {\rtf1\ansi\deff0
    {\fonttbl{\f0\fswiss\fcharset0 Arial;}}
    \pard\sa200\sl276\slmult1\f0\fs24
    This is visible text. This is also visible text.
   }
-->
```

The comment is removed by the Markdown parser, but the intermediate AST still contains raw control sequences that the RTF writer outputs without escaping.
`RichTextBox.Rtf` then misinterprets them and fails.

#### Layer where things go wrong

1. **Markdown parsing** — OK; the code block is interpreted correctly.
2. **AST → RTF conversion** — ❌ escaping incomplete; raw `{`, `}`, and `\` survive.
3. **RichTextBox rendering** — attempts to interpret the unintended RTF commands and fails.

#### Workaround

Do **not** fence real RTF using triple backticks with a language tag such as `rtf`.
Instead, force plain-text mode:

```text
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24
This is visible text. This is also visible text.
}
```

or escape braces and backslashes manually before embedding them in Markdown.

---

### RTF Groups

the [WinForms](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/System/Windows/Forms/Controls/RichTextBox/RichTextBox.cs) `RichTextBox` class does not remove __RTF__ groups in C# code.

All __RTF__ *parsing*, *group filtering*, *hidden-text suppression*, *table collapsing*, etc. are implemented inside the native __Windows__ __RichEdit__ control ( hosted in `msftedit.dll` or legacy `riched20.dll`).

The C# class is only a thin *proxy wrapper* around that native control via Win32 messages.

The idiosyncrasies of `RichTextBox` behavior(loss of groups, normalization, re-serialization) is __native__, not __managed__:

+ `riched32.dll`: RichEdit __1.0__, plain RTF
+ `riched20.dll`: RichEdit __2.0__, __3.0__
+ `msftedit.dll`: RichEdit __4.1–8+__

---

### See Also

- [RichText Builder (StringBuilder for RTF)](https://www.codeproject.com/articles/RichText-Builder-StringBuilder-for-RTF-) – relatively easy to implement.
- [Extended RichTextBox (RichTextBoxEx)](https://www.codeproject.com/articles/EXTENDED-Version-of-Extended-Rich-Text-Box-RichTex#comments-section) – adds toolbar, ruler, and extended functionality to a standard WinForms `RichTextBox`.
- [MarkdownToRtf](https://github.com/ReneeHuh/MarkdownToRtf) – static utility converting __Markdown__ to __RTF__ using [Markdig](https://github.com/xoofx/markdig) parser via [NuGet](https://www.nuget.org/packages/Markdig/). Generates WordPad-compatible RTF with proper headers, font tables, and formatting codes. Supports headings, paragraphs, lists, tables, code blocks, emphasis, and hyperlinks.
- [GustavoHennig/MarkdownToRtf](https://github.com/GustavoHennig/MarkdownToRtf) – basic Markdown to RTF converter.
- [Avalon Renderer](https://github.com/Kryptos-FR/markdig.wpf) – WPF renderer using [Markdig](https://github.com/xoofx/markdig).
- [Markdown Basic Syntax](https://www.markdownguide.org/basic-syntax/)
- [RTF Spec](https://latex2rtf.sourceforge.net/RTF-Spec-1.2.pdf) – the original published specification was developed by __Microsoft Corporation__ in __1987__ (The __Windows OS__ was first launched in __1985__).
- [The RTF 1.x Specification](https://www.biblioscape.com/rtf15_spec.htm)
- [The RTF Cookbook](https://metacpan.org/dist/RTF-Writer/view/lib/RTF/Cookbook.pod) – note: not a Perl module
  * Misc. `RichTextBox` articles (note traditionally the `RichTextBox` examples are writen in [VB.Net](https://en.wikipedia.org/wiki/Visual_Basic_(.NET))):
   + [Scrolling Around with the RichTextBox Contro](https://www.codeproject.com/articles/Scrolling-Around-with-the-RichTextBox-Control)
   + [Numbering lines of RichTextBox](https://www.codeproject.com/articles/Numbering-lines-of-RichTextBox-in-NET-2-0) (no source)
   + [Insert Plain Text and Images into RichTextBox at Runtime](https://www.codeproject.com/articles/Insert-Plain-Text-and-Images-into-RichTextBox-at-R#comments-section) (no source)
   + [RicherTextBox](https://www.codeproject.com/articles/RicherTextBox) (no source)
   + [Line Numbering of RichTextBox in .NET 2.0](https://www.codeproject.com/articles/Line-Numbering-of-RichTextBox-in-NET-2-0)
   + [Changing the line spacing in a RichTextBox control](https://www.codeproject.com/articles/Changing-the-line-spacing-in-a-RichTextBox-control)
   + [Changing the line spacing in a RichTextBox control](https://www.codeproject.com/articles/EXTENDED-Version-of-Extended-Rich-Text-Box-RichTex)
  * [C# project replica and Powershell port](https://github.com/sergueik/powershell_samples/tree/master/csharp/render_markdown) of a Markdown RTF covertor operating [MarkDig](https://github.com/xoofx/markdig) library

### Example to Navigate

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
