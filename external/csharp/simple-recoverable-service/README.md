### Info

Code from custom serviceInstaller Extension that Enables Recovery and Autostart Configuration [article](https://www.codeproject.com/Articles/6164/A-ServiceInstaller-Extension-That-Enables-Recovery)


### Usage
* build the complex project, all the"utility" dlls will be in the same folder with the main program executable:
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -uninstall Program\bin\Debug\WindowsService.NET.exe
```
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -install Program\bin\Debug\WindowsService.NET.exe
```
to use interactive user credentials add the following (see [also](https://www.aspsnippets.com/Articles/Install-Windows-Service-silently-without-entering-Username-and-Password-using-InstallUtilexe-from-Command-Prompt-Line.aspx)):


```cmd
set PASSWORD=....
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe  -uninstall Program\bin\Debug\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% /unattend Program\bin\Debug\WindowsService.NET.exe
```

![Recovery Property Screen](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-recoverable-service/screenshots/capture_recovery_options.png)

### See Also
  * VB.net app to "install a Windows service the way one's likes" [article](https://www.codeproject.com/Articles/5338/Install-a-Windows-service-the-way-YOU-want-to) -  probably similar, but with a VB.Net p/invoke syntax


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
