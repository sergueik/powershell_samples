### Info

This directory contains the setup project of a basic Windows tray app with deskt
op shortcut using the examples:
### Note

Visual Studio Code no longer officially supports Windows 32-bit versions. Support for Windows 32-bit VS Code ended with the October 2023 (version 1.84) release.
If a 32-bit system is being used, it is recommended to update to a 64-bit version of Windows to run the latest versions of Visual Studio Code.
If a 32-bit version of Visual Studio Code is absolutely necessary, it would require locating and installing an older version of VS Code released prior to October 2023, which might be found in archived release notes or older download pages.
e.g. 
`https://www.filepuma.com/download/visual_studio_code_32bit_1.43.2-25054/download/`

Latest releases (64 only) can be found in `https://code.visualstudio.com/Download`

Extensons are on [visualstidio mrketplae](https://marketplace.visualstudio.com/)

For testing will install `https://marketplace.visualstudio.com/items?itemName=formulahendry.CodeRunner`
it is glorified zip file with binaries:
```
   Date      Time    Attr         Size   Compressed  Name
------------------- ----- ------------ ------------  ------------------------
2021-08-19 21:58:12 .....         2061          693  extension.vsixmanifest
2021-08-19 21:58:12 .....        44032        17468  CodeRunner.dll
2021-08-19 21:58:12 .....        19968         3010  CodeRunner.pdb
2021-08-19 21:58:12 .....       182008        77909  Microsoft.ApplicationInsights.dll
2021-08-19 21:58:12 .....          938          334  CodeRunner.pkgdef
2021-08-19 21:58:12 .....        23472        23254  Resources\logo.png
2021-08-19 21:58:12 .....          496          209  [Content_Types].xml
2021-08-19 21:58:12 .....          655          320  manifest.json
2021-08-19 21:58:12 .....          923          435  catalog.json
------------------- ----- ------------ ------------  ------------------------
```
ZOWE extensions can be found on `https://www.zowe.org/download`
### 
```powerhsll
[guid]::NewGuid()
```

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Setup.wixproj
```
After install extension are copied to
```txt
%LocalAppData%\Microsoft\VisualStudio\{version}\Extensions
```
```sh
msiexec /i "bin\VSCodeCustomInstaller.msi" /l*v "a.log"
```
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

