### Info
This directory contains a replica of [App to convert Markdown (.md) files to RTF Rich Text files](https://github.com/snjo/MarkdownToRtf), translated from C# __6.x__, __7.x__, and __9.x__ to plain C# __5.0__ syntax / __.NET Framework 4.5__.  
The driver can also be compiled as a dependency to display __Markdown__ files in a __Rich Text__ field in a C# Windows Forms application.  

Surprisingly, __Markdig__ favors [XAML](https://en.wikipedia.org/wiki/Extensible_Application_Markup_Language)/[WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation) over [RichText](https://en.wikipedia.org/wiki/Rich_Text_Format) and __HTML__ output.

![app1](screenshots/app1.jpg)

### Notes

- `$` - *interpolated strings* - C# __6.0__
- *Tuple return types and deconstruction* - C# __7.0__ (required multiple parser and compiler adjustments):
  - Assignment grammar
  - Return type syntax
  - Overload resolution
  - Deconstruction rules
- *Target-typed* `new` - C# __9.0__
- `AsSpan` instead of `Substring` – not supported in C# __5.0__ due to compiler dependencies

### See Also

- [RichText Builder (StringBuilder for RTF)](https://www.codeproject.com/articles/RichText-Builder-StringBuilder-for-RTF-) – relatively easy to implement.
- [Extended RichTextBox (RichTextBoxEx)](https://www.codeproject.com/articles/EXTENDED-Version-of-Extended-Rich-Text-Box-RichTex#comments-section) – adds toolbar, ruler, and extended functionality to a standard WinForms `RichTextBox`.
- [MarkdownToRtf](https://github.com/ReneeHuh/MarkdownToRtf) – static utility converting __Markdown__ to __RTF__ using [Markdig](https://github.com/xoofx/markdig) parser via [NuGet](https://www.nuget.org/packages/Markdig/). Generates WordPad-compatible RTF with proper headers, font tables, and formatting codes. Supports headings, paragraphs, lists, tables, code blocks, emphasis, and hyperlinks.
- [GustavoHennig/MarkdownToRtf](https://github.com/GustavoHennig/MarkdownToRtf) – basic Markdown to RTF converter.
- [Avalon Renderer](https://github.com/Kryptos-FR/markdig.wpf) – WPF renderer using [Markdig](https://github.com/xoofx/markdig).
- [Markdown Basic Syntax](https://www.markdownguide.org/basic-syntax/)
- [RTF Spec](https://latex2rtf.sourceforge.net/RTF-Spec-1.2.pdf) – the original published specification was developed by __Microsoft Corporation__ in __1987__.
- [The RTF Cookbook](https://metacpan.org/dist/RTF-Writer/view/lib/RTF/Cookbook.pod) – note: not a Perl module

### WIP

- Tag the __RTF__ generated from __Markdown__ with **hidden markers** for [Paragraphs](https://www.markdownguide.org/basic-syntax/#paragraphs-1), [Headings](https://www.markdownguide.org/basic-syntax/#headings), [Images](https://www.markdownguide.org/basic-syntax/#images), and [Code Blocks](https://www.markdownguide.org/basic-syntax/#code-blocks).  
  *(Markers remain invisible during normal rendering and do not affect the displayed content.)*
- Implement **forward / backward navigation buttons** to scroll to the following/preceding anchor, enabling vi-style movement *by paragraph*, *section*, or *code block* logical units.
- Implement a **toggle button** in the UI to instantly reveal or hide the markers for debugging or inspection purposes.

#### Example

- Fragment *without* markers:
```rtf
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24
This is visible text. This is also visible text.
}
```

-- Same fragment *with* **hidden marker**:
The hidden text will not be displayed; visually, it renders identically.
```rtf
{\rtf1\ansi\deff0
{\fonttbl{\f0\fswiss\fcharset0 Arial;}}
\pard\sa200\sl276\slmult1\f0\fs24
This is visible text. {\v This text is hidden.} This is also visible text.
}
```

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
