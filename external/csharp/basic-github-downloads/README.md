### Info

replica of [github-downloads](https://github.com/OlegChe77/github-downloads) by OlegChe77 -
Windows app: download statistics of your GitHub releases, stars, forks, traffic and growth history
### Usage

#### Console 

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /optimize+ /codepage:65001 /out:GhDownloads.exe /win32icon:src\app.ico /resource:src\app.ico,app.ico /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll /r:System.Security.dll /r:System.Core.dll src\GhDownloads.cs
```

### Misc

```
ls -1 *.vsdx | xargs -IX echo   "<ItemGroup>\n<None Include=\"X\">\n<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\n</None>\n</ItemGroup>"

```
### See Also

  * [original article](https://habr.com/ru/articles/1101148/) (in Russian)

