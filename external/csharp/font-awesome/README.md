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

```sh
 curl -skLo wycliffeassociates.typography.openfont.zip  https://www.nuget.org/api/v2/package/WycliffeAssociates.Typography.OpenFont/1.0.0
```
```sh
$ unzip -l wycliffeassociates.typography.openfont.zip
````
```text
Archive:  wycliffeassociates.typography.openfont.zip
  Length      Date    Time    Name
---------  ---------- -----   ----
      532  2020-07-23 09:04   _rels/.rels
      934  2020-07-23 09:04   WycliffeAssociates.Typography.OpenFont.nuspec
   632320  2020-07-23 13:04   lib/netstandard2.0/Typography.OpenFont.dll
     2323  2020-07-20 19:07   LICENSE.md
      527  2020-07-23 09:04   [Content_Types].xml
      795  2020-07-23 09:04   package/services/metadata/core-properties/833c7d9785a34538bca00b85d54ad42c.psmdcp
     9474  2020-07-23 06:05   .signature.p7s
---------                     -------
   646905                     7 files
```
```sh
pip install fontTools
```
```text
Defaulting to user installation because normal site-packages is not writeable
Requirement already satisfied: fontTools in c:\users\kouzm\appdata\roaming\python\python311\site-packages (4.59.0)

```
```sh
python met.py --file  'fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf'
```
this will output
```text
39 zero-width-space
40 folder-closed
41 heart
42 star
43 user
44 house
45 clock
46 rectangle-list
47 flag
48 headphones
49 bookmark
50 camera
51 image
52 pen-to-square
53 circle-xmark
54 circle-check
55 circle-question
56 eye
57 eye-slash
58 calendar-days
59 comment
60 folder
61 folder-open
62 chart-bar
63 comments
64 star-half
65 lemon
66 credit-card
67 hard-drive
68 hand-point-right
69 hand-point-left
70 hand-point-up
71 hand-point-down
72 cloud
73 copy
74 floppy-disk
75 square
76 truck
77 envelope
78 paste
79 lightbulb
80 bell
81 hospital
82 square-plus
83 circle
84 face-smile
85 face-frown
86 face-meh
87 keyboard
88 calendar
89 circle-play
90 square-minus
91 square-check
92 share-from-square
93 compass
94 square-caret-down
95 square-caret-up
96 square-caret-right
97 file
98 file-lines
99 thumbs-up
100 thumbs-down
101 sun
102 moon
103 square-caret-left
104 circle-dot
105 building
106 file-pdf
107 file-word
108 file-excel
109 file-powerpoint
110 file-image
111 file-zipper
112 file-audio
113 file-video
114 file-code
115 life-ring
116 paper-plane
117 futbol
118 newspaper
119 bell-slash
120 copyright
121 closed-captioning
122 object-group
123 object-ungroup
124 note-sticky
125 clone
126 hourglass-half
127 hourglass
128 hand-back-fist
129 hand
130 hand-scissors
131 hand-lizard
132 hand-spock
133 hand-pointer
134 hand-peace
135 registered
136 calendar-plus
137 calendar-minus
138 calendar-xmark
139 calendar-check
140 map
141 message
142 circle-pause
143 circle-stop
144 font-awesome
145 handshake
146 envelope-open
147 address-book
148 address-card
149 circle-user
150 id-badge
151 id-card
152 window-maximize
153 window-minimize
154 window-restore
155 snowflake
156 trash-can
157 images
158 clipboard
159 alarm-clock
160 circle-down
161 circle-left
162 circle-right
163 circle-up
164 gem
165 money-bill-1
166 rectangle-xmark
167 chess-bishop
168 chess-king
169 chess-knight
170 chess-pawn
171 chess-queen
172 chess-rook
173 square-full
174 comment-dots
175 face-smile-wink
176 face-angry
177 face-dizzy
178 face-flushed
179 face-frown-open
180 face-grimace
181 face-grin
182 face-grin-wide
183 face-grin-beam
184 face-grin-beam-sweat
185 face-grin-hearts
186 face-grin-squint
187 face-grin-squint-tears
188 face-grin-stars
189 face-grin-tears
190 face-grin-tongue
191 face-grin-tongue-squint
192 face-grin-tongue-wink
193 face-grin-wink
194 face-kiss
195 face-kiss-beam
196 face-kiss-wink-heart
197 face-laugh
198 face-laugh-beam
199 face-laugh-squint
200 face-laugh-wink
201 face-meh-blank
202 face-rolling-eyes
203 face-sad-cry
204 face-sad-tear
205 face-smile-beam
206 star-half-stroke
207 face-surprise
208 face-tired
```
(approx 600 glyphs listed for `regular-icon-font-free/fonts/Lineicons.ttf`, less than 300 for `fontawesome-free-7.1.0-desktop/otfs/Font Awesome 7 Free-Regular-400.otf`)

```powershell
.\Utils\bin\Debug\FontResource.exe --file="regular-icon-font-free\fonts\Lineicons.ttf" --list
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
.\Utils\bin\Debug\FontResource.exe --file="regular-icon-font-free\fonts\Lineicons.ttf" --enum
```
```text
OK: Font 'Lineicons.ttf' is loadable.
```
```text


public enum true
{
    glyph_0020 = 0x0020,
    glyph_0021 = 0x0021,
    glyph_0022 = 0x0022,
    glyph_0023 = 0x0023,
    glyph_0024 = 0x0024,
    glyph_0025 = 0x0025,
    ...
    glyph_FEF9 = 0xFEF9,
    glyph_FEFA = 0xFEFA,
    glyph_FEFB = 0xFEFB,
    glyph_FEFC = 0xFEFC,
    glyph_FFFC = 0xFFFC,
}
```
### See Also

  * __Font Awesome__ [download](https://fontawesome.com/download) page 
  * __Font Awesome__ [glyph names](https://fontawesome.com/v4/cheatsheet/)
  * `System.Drawing.Text` [API](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.text.privatefontcollection?view=netframework-4.5) for loading fonts
  * __Icon Font__ Alternatives (a.k.a. __FA Substitutes__ )
   + https://lineicons.com/download (the resource needs to be downloaded manually from the website)

  * https://github.com/googlefonts/noto-emoji/blob/main/fonts/NotoColorEmoji.ttf
  * https://gitlab.winehq.org/wine/wine/-/merge_requests/411
  * `Typography.OpenFont` [source](https://github.com/QL-Win/QuickLook.Typography.OpenFont) and [package](https://www.nuget.org/packages/QuickLook.Typography.OpenFont/1.0.1)


### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
