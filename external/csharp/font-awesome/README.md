### Info

Replica of [Font Awesome Windows Forms](https://github.com/denwilliams/FontAwesome-WindowsForms) project 
embedding [Font Awesome Desktop](https://docs.fontawesome.com/desktop) packaged 
as a [TrueType Font](https://en.wikipedia.org/wiki/TrueType)  
into an Utility dll - remediating the need to install the font explicitly on the client machine(s) - 
with additional utilities to avoid being tied to `Properties.Resources` auto-generated proxy created
by __Visual Studio__ along with `resx` file (XML serialization of the resources)
and the `Resources.Designer.cs` class which handle strongly-typed access to the embedded resources.

### Historical Context

- Resources are embedded into the assembly at compile time.
- `ResourceManager.GetObject()` deserializes them into .NET types like `byte[]`, `Bitmap`, `Icon`.
- The strongly-typed `Resources.Designer.cs` serves simply as a convenience accessor
  for __Visual Studio__ specific `.resx`  resource(s).
- This approach originated in memory-starved environments (mainframes, early desktops) when
loading vanilla resources dynamically from files in disk was slow and costly and violation of the predictable memory layout was a no-no.


Conceptually, `.resx` + embedded resources 
and [MUI](https://en.wikipedia.org/wiki/Multilingual_User_Interface) and
[Satellite Assemblies](https://en.wikipedia.org/wiki/Assembly_(CLI)#Satellite_assemblies)
in __.NET__ serve the same role
as the [Classic Mac OS](https://en.wikipedia.org/wiki/Classic_Mac_OS) __resource fork__ — 
bundling application code and structured UI assets/resources together,
simplifying deployment and localization. Like resource forks, this approach encapsulates fonts, icons, strings,
images under a single binary *package*, 
avoiding external file dependencies and making resource lookup efficient 
and consistent.

The `Properties.Resources` approach feels heavy and strngly __SOAP__ / __WCF__ era vibe today because it is a product of memory scarcity and
compile-time type safety requirements — similar to old-school `EventHandler` classes in UI frameworks.

---

### Usage

* download the resource
```
VERSION=7.1.0
curl -ksLo "$HOME/Downloads/fontawesome-free-desktop.zip" https://use.fontawesome.com/releases/v$VERSION/fontawesome-free-$VERSION-desktop.zip
unzip -l ~/Downloads/fontawesome-free-desktop.zip | grep -E '(ttf|otf)'
```
```text
        0  2025-09-29 21:06   fontawesome-free-7.1.0-desktop/otfs/
   199352  2025-09-29 21:06   fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Brands-Regular-400.otf
   410592  2025-09-29 21:06   fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Solid-900.otf
    87332  2025-09-29 21:06   fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf
```
* extract the available opentype font(s):
```sh
unzip -x  ~/Downloads/fontawesome-free-desktop.zip         fontawesome-free-7.1.0-desktop/otfs/*
```
```text
Archive:  /c/Users/kouzm/Downloads/fontawesome-free-desktop.zip
  inflating: fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Brands-Regular-400.otf
  inflating: fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Solid-900.otf
  inflating: fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf

```
* run the utility to confirm the font resource is loadble:
```sh
Utils/bin/Debug/FontResource.exe --noop 'fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf'
``` 
```text
OK: Font 'Font Awesome 7 Free-Regular-400.otf' is loadable.
```
and dump it as a `byte[]` array  or `base64`-encoded blob:
```sh
Utils/bin/Debug/FontResource.exe 'fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf' | tee fragment.cs
```
*examine the ``fragment.cs`:
```c#
/* Byte array literal */
byte[] fontData = new byte[] {
0x4F, 0x54, 0x54, 0x4F, 0x00, 0x0A, 0x00, 0x80, 0x00, 0x03, 
...
0x1E, 0x00, 0x0F, 0x00, 0x18, 0x00, 0x11, 0x00, 0x00};

/* Base64 string */
string fontBase64 = @"
T1RUTwAKAIAAAwAgQ0ZGIE4d+jsAAIFcAAC2jkdTVUKmi7LqAAE37AAAHTZPUy8yYOnhowAAARAAAABg
...
Y21hcKEk7M8AAAi8AAB4fmhlYWQvD4wXAAAArAAAADZoaGVhBEMC6gAAAOQAAAAkaG10eH3AAAAAAAFw
...
AAEABAA1AAwAGQANAB4AFwACAA8AFQAeAA8AGAARAAA=
";

```
* try alternative free ttf icon font

```sh
unzip -l ~/Downloads/lineicons5-free-pakage.zip | grep ttf
 ```
```text
   156780  2024-10-22 04:55   regular-icon-font-free/fonts/Lineicons.ttf
```

```sh
unzip -x ~/Downloads/lineicons5-free-pakage.zip  regular-icon-font-free/fonts/Lineicons.ttf
```
```text
Archive:  /c/Users/kouzm/Downloads/lineicons5-free-pakage.zip
  inflating: regular-icon-font-free/fonts/Lineicons.ttf
```
use a newer version of the Utility
```sh
./Utils/bin/Debug/FontResource.exe  regular-icon-font-free/fonts/Lineicons.ttf  --noop
```
```text
OK: Font 'Lineicons.ttf' is loadable.
```

```sh
./Utils/bin/Debug/FontResource.exe  --file regular-icon-font-free/fonts/Lineicons.ttf  --noop
```

```text
OK: Font 'Lineicons.ttf' is loadable.
```
### Note
ready ro catch the `System.ArgumentException`
with message one of:
```
Font file format is not supported.
```
```
Parameter is not valid.
```
### WIP

```powershell
.\Tools\bin\Debug\FontResource.exe --file="regular-icon-font-free\fonts\Lineicons.ttf" --list
```
```text
OK: Font 'Lineicons.ttf' is loadable.
```
```text
U+0020-U+007F
U+0081-U+0081
U+008D-U+008D
U+008F-U+0090
U+009D-U+009D
U+00A0-U+00FF
U+0152-U+0153
U+0160-U+0161
U+0178-U+0178
U+017D-U+017E
U+0192-U+0192
U+02C6-U+02C6
U+02DC-U+02DC
U+2013-U+2014
U+2018-U+201A
U+201C-U+201E
U+2020-U+2022
U+2026-U+2026
U+2030-U+2030
U+2039-U+203A
U+20AC-U+20AC
U+2122-U+2122
```
```powershell
.\Tools\bin\Debug\FontResource.exe --file="regular-icon-font-free\fonts\Lineicons.ttf" --enum --start 0xFEE6 --end 0xFFFC
```
```text
OK: Font 'Lineicons.ttf' is loadable.
```
```text

public enum true
{
    glyph_FEE6 = 0xFEE6,
    glyph_FEE7 = 0xFEE7,
    glyph_FEE8 = 0xFEE8,
    glyph_FEE9 = 0xFEE9,
    glyph_FEEA = 0xFEEA,
    glyph_FEEB = 0xFEEB,
    glyph_FEEC = 0xFEEC,
    glyph_FEED = 0xFEED,
    glyph_FEEE = 0xFEEE,
    glyph_FEEF = 0xFEEF,
    glyph_FEF0 = 0xFEF0,
    glyph_FEF1 = 0xFEF1,
    glyph_FEF2 = 0xFEF2,
    glyph_FEF3 = 0xFEF3,
    glyph_FEF4 = 0xFEF4,
    glyph_FEF5 = 0xFEF5,
    glyph_FEF6 = 0xFEF6,
    glyph_FEF7 = 0xFEF7,
    glyph_FEF8 = 0xFEF8,
    glyph_FEF9 = 0xFEF9,
    glyph_FEFA = 0xFEFA,
    glyph_FEFB = 0xFEFB,
    glyph_FEFC = 0xFEFC,
    glyph_FFFC = 0xFFFC,
}

```

```sh
 curl -sLko bootstrap-icons.zip https://github.com/twbs/icons/releases/download/v1.12.0/bootstrap-icons-1.12.0.zip
```
```sh
unzip -l bootstrap-icons.zip | grep -E '(ttf|otf)'
```
```text
      423  2025-05-04 02:04   bootstrap-icons-1.12.0/filetype-ttf.svg
      904  2025-05-04 02:04   bootstrap-icons-1.12.0/filetype-otf.svg
```
```sh
unzip bootstrap-icons.zip bootstrap-icons-1.12.0/filetype-ttf.svg
```
```sh
Archive:  bootstrap-icons.zip
  inflating: bootstrap-icons-1.12.0/filetype-ttf.svg
```
```sh
file bootstrap-icons-1.12.0/filetype-ttf.svg
```
```text
bootstrap-icons-1.12.0/filetype-ttf.svg: SVG Scalable Vector Graphics image, ASCII text
```

This needs to be fixed.
#### FlatIcon
```sh
curl -skLO https://github.com/ColorlibHQ/Breed/raw/refs/heads/master/assets/fonts/Flaticon.ttf 
```
```sh
file Flaticon.ttf
```
```text
Flaticon.ttf: TrueType Font data, 13 tables, 1st "FFTM", 14 names, Macintosh
```
```sh
curl -skLO https://raw.githubusercontent.com/freepik-company/flaticon-uicons/refs/heads/main/src/uicons/css/regular/straight.css
```

the code points are shown on individual icon pages in `https://icons.getbootstrap.com/icons/list` `https://icons.getbootstrap.com/icons/envelope-arrow-down-fill/` etc.

to load the  font Unicode Code  / Name data into C# program download the CSS

```sh
curl -skLO  https://icons.getbootstrap.com/assets/font/bootstrap-icons.css
```
```powershell
invoke-webrequest -Uri https://icons.getbootstrap.com/assets/font/bootstrap-icons.css -outfile "bootstrap-icons.css" -usebasicparsing

```
and one can parse it:
```powershell
. .\parse_css.ps1 -debug
DEBUG: Processing CSS:
```
```text
...\bootstrap-icons.css

Name                           Value
----                           -----
f1ae                           border-middle
f6d8                           usb-c-fill
f249                           chat-dots-fill
...
f6a6                           pc-display
f6c4                           terminal-plus
f6c9                           ticket-perforated-fill
f17b                           bar-chart-line-fill

```
or

```powershell
. .\parse_css4.ps1 -debug -namespace IconFonts -name BootstrapIcons
```

```text
namespace IconFonts {
    public enum BootstrapIcons {
        _123 = 0xf67f,
        AlarmFill = 0xf101,
        Alarm = 0xf102,
        AlignBottom = 0xf103,
        AlignCenter = 0xf104,
        AlignEnd = 0xf105,
        AlignMiddle = 0xf106,
        AlignStart = 0xf107,
        AlignTop = 0xf108,
        Alt = 0xf109,
        ...
        GlobeAmericasFill = 0xf91b,
        GlobeAsiaAustraliaFill = 0xf91c,
        GlobeCentralSouthAsiaFill = 0xf91d,
        GlobeEuropeAfricaFill = 0xf91e,
    }
}
```
the __Bootstrap CSS__ style sheet defines very simple  classes like:
```css
.bi-person-fill-dash::before { content: "\f89f"; }
```
### See Also

  * __Font Awesome__ [download](https://fontawesome.com/download) page 
  * __Font Awesome__ [glyph names](https://fontawesome.com/v4/cheatsheet/)
  * `System.Drawing.Text` [API](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.text.privatefontcollection?view=netframework-4.5) for loading fonts
  * __Icon Font__ Alternatives (a.k.a. __FA Substitutes__ )
   + https://lineicons.com/download (the resource needs to be downloaded manually from the website)
   + [bootstrap icons](https://github.com/twbs/icons/releases)
  * https://github.com/googlefonts/noto-emoji/blob/main/fonts/NotoColorEmoji.ttf
  * https://gitlab.winehq.org/wine/wine/-/merge_requests/411
  * `Typography.OpenFont` [source](https://github.com/QL-Win/QuickLook.Typography.OpenFont) and [package](https://www.nuget.org/packages/QuickLook.Typography.OpenFont/1.0.1)
  * [Unicode Character Database](https://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt) this is where Unicode Character Names are stored - the cut-down internal copy of UCD data packaged in Windows

### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
