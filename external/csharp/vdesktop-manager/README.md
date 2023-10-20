### Info

this directory contains a replica of subset of

[VDesk](https://github.com/eksime/VDesk) which is an C# wrapper for IVirtualDesktopManager on Windows 10. This c# application being converred to Powershell script

with the dependency pulled via nuget. It appears that with version `4.0.0` of `VirtualDesktop` packages the application does not work on  Windows 10 and Windows 11:
This may be why the upstream project used a specific commit revision of the dependency: `https://github.com/Grabacr07/VirtualDesktop/tree/1e7f823076f090f90d35024e5592fbbff4614afc`

```sh
vdesk create:1
```
fails with

![runtime error](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/vdesktop-manager/screenshots/capture-runtime-error.png)

installing the __VDesk__ msi from the release direcory at latest available versioh [1.2.0](https://github.com/eksime/VDesk/releases/tag/v1.2.0) installs it to
32 bit 

```text
c:\Program Files (x86)\VDesk
```

trying it

```cmd
"c:\Program Files (x86)\VDesk\VDesk.exe" create:1
```
is not failing but no additional desktops are created
and the 
```cmd
"c:\Program Files (x86)\VDesk\VDesk.exe" on:2 run:notepad "C:\some file.txt"
```
does not start the application as described


when run the commands individually through new start menu

![windows 11 launch](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/vdesktop-manager/screenshots/capture-run-windows11.png)

a help message is shown 

![help message](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/vdesktop-manager/screenshots/capture-help.png)

### Note

The Powershell command to try on Windows 10/11 is:
```poweshell
(ps notepad)[0].MainWindowHandle |
 Move-Window (Get-Desktop 2) | Out-Null
```
### See Also

   * https://superuser.com/questions/956884/windows-10-assigning-application-to-specific-desktop
   * https://stackoverflow.com/questions/32491872/starting-programs-on-multiple-desktops-using-powershell-in-windows-10 
   * https://superuser.com/questions/956884/windows-10-assigning-application-to-specific-desktop
