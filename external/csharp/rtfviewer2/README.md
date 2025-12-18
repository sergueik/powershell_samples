### Info

This directory contains a replica of [App to convert Markdown (.md) files to RTF Rich Text files](https://github.com/snjo/MarkdownToRtf).

![app1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/app1.jpg)

### Downlevel C# Compiler Feature Removals

Translated translated from C# __6.x__, __7.x__, and __9.x__ to plain C# __5.0__ syntax / __.NET Framework 4.5__to plain C# __5.0__ syntax / __.NET Framework 4.5__

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
- *value tuples* and *tuple literal* syntax - C# __7.0__

Surprisingly, __Markdig__ favors [XAML](https://en.wikipedia.org/wiki/Extensible_Application_Markup_Language)/[WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation) over [RichText](https://en.wikipedia.org/wiki/Rich_Text_Format) and __HTML__ output.

### Usage

1. Launch the Tool
2. The markdown payload to render can be pasted into the left pane text area window. This the only currently supported loading process. Earlier available "File Load" functionality has been hidden from te toolbar
3. The tool will convert the Markdown payload into RTF while adding invisible yet [searchable](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox.find?view=windowsdesktop-10.0#system-windows-forms-richtextbox-find(system-string-system-int32-system-windows-forms-richtextboxfinds))  [markers]() - the `\u8203?` which is the RTF representarion of `\u200B` (the Unicode [Zero Width Space](https://en.wikipedia.org/wiki/Zero-width_space) [code point](https://en.wikipedia.org/wiki/List_of_Unicode_characters))

![debug1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/rtfviewer2/screenshots/debug1.jpg)

after every section of kind in the original document to facilitate navigation:

* [Paragraphs](https://www.markdownguide.org/basic-syntax/#paragraphs-1)
* [Headings](https://www.markdownguide.org/basic-syntax/#headings)
* [Images](https://www.markdownguide.org/basic-syntax/#images)
* [Lists](https://www.markdownguide.org/basic-syntax/#lists-1)
* [Tables](https://www.markdownguide.org/extended-syntax/#tables)
* [Code Blocks](https://www.markdownguide.org/basic-syntax/#code-blocks).


The **forward / backward navigation buttons** scroll to the following/preceding anchor, enabling vi-style movement.

NOTE :*the Markers remain invisible during normal rendering and do not affect the displayed content.)*d

### Example to Navigate

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |

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

### TODO

Implement rendering of [code](https://www.markdownguide.org/basic-syntax/#code) with switching the background color and code font and color by extending the `SetStyle`

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
  * __CommonMark.NET__ [repository](https://github.com/Knagis/CommonMark.NET) and [nuget package](https://www.nuget.org/packages/CommonMark.NET) - rendeting markdown to HTML.


### Example to Navigate

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
