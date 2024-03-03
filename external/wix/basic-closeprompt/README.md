### Info
this directory contains a replica of the
[source](https://github.com/IvanLeonenko/ClosePromptCA)  of custom action prompt dialog for Wix project
covered in __Prompt user to close applications on install/uninstall with WIX custom action__
[article](https://www.codeproject.com/Articles/584105/Prompt-user-to-close-applications-on-install-unins)

with added WiX project
### Usage


```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe .\basic-closeprompt.sln /t:clean,compile
msbuild.exe .\basic-closeprompt.sln```
```


```cmd
msiexec.exe /l*v a.log /i Setup\bin\Debug\Setup.msi
```

![prompt](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-closeprompt/screenshots/capture-close-prompt.png)

once all notepad windows are closed the installer continues

### Cleanup


### See Also

   * Misc

     + https://www.codeproject.com/Articles/511653/Using-WIX-with-Managed-Custom-Action
     + https://www.codeproject.com/Articles/335585/Building-Installation-Packages-with-Windows-Instal
     + https://www.codeproject.com/Articles/865604/Writing-a-Wix-Extension-to-Read-XML-Files
     + https://www.codeproject.com/Articles/44191/Drivers-Installation-With-WiX

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
