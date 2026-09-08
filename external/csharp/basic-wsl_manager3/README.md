### Info
replica of [WSL Manager](https://github.com/MoyashiWithDevice/WSL_Manager)
 - Windows Form based custom shell for
Windows Subsystem for Linux (WSL)

![Original App](screenshots/capture-app.png)

![App](screenshots/capture-app-console.png)

![App](screenshots/capture-app-store.png)

![App Tray](screenshots/capture-app-tray.png)

![App Tray](screenshots/capture-app-tray2.png)

### Usage


check - straw Registry keys can be found:


```powershell
get-childitem -path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Lxss' | select-object -expandproperty Name| select-object -first 1
```
```txt
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Lxss\{a7b1b735-87cb-4545-bd1b-b29b129dbc47}
```
```powershell
get-itemproperty -path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Lxss\{a7b1b735-87cb-4545-bd1b-b29b129dbc47}' -name '*'
```
```text
State             : 1
DistributionName  : Ubuntu
Version           : 2
BasePath          : C:\Users\kouzm\AppData\Local\Packages\CanonicalGroupLimited
                    .Ubuntu_79rhkp1fndgsc\LocalState
Flags             : 15
DefaultUid        : 0
PackageFamilyName : CanonicalGroupLimited.Ubuntu_79rhkp1fndgsc
PSPath            : Microsoft.PowerShell.Core\Registry::HKEY_CURRENT_USER\Softw
                    are\Microsoft\Windows\CurrentVersion\Lxss\{a7b1b735-87cb-45
                    45-bd1b-b29b129dbc47}
PSParentPath      : Microsoft.PowerShell.Core\Registry::HKEY_CURRENT_USER\Softw
                    are\Microsoft\Windows\CurrentVersion\Lxss
PSChildName       : {a7b1b735-87cb-4545-bd1b-b29b129dbc47}
PSDrive           : HKCU
PSProvider        : Microsoft.PowerShell.Core\Registry

```
```cmd
dir "%LOCALAPPDATA%\Packages\CanonicalGroupLimited.Ubuntu_79rhkp1fndgsc\LocalState"
```
```text
 Volume in drive C is Windows-SSD
 Volume Serial Number is 3C37-0A74

 Directory of C:\Users\kouzm\AppData\Local\Packages\CanonicalGroupLimited.Ubuntu_79rhkp1fndgsc\LocalState

02/13/2025  08:19 PM    <DIR>          .
02/13/2025  08:19 PM    <DIR>          ..
04/23/2023  05:16 PM     1,197,473,792 ext4.vhdx
               1 File(s)  1,197,473,792 bytes
               2 Dir(s)  150,638,940,160 bytes free
```
NOTE: the last modified date of the disk image and the directory are both in the past, but not identical.

perform Windows component inventory check (requires elevated prompt):
```posershell
Get-WindowsOptionalFeature -Online |
Where-Object FeatureName -match 'Subsystem-Linux|VirtualMachinePlatform' |
Select-Object -property FeatureName,State
```

```text
FeatureName                          State
-----------                          -----
VirtualMachinePlatform            Disabled
Microsoft-Windows-Subsystem-Linux Disabled
```

it means, it is possible for a machine to legitimately have a leftover, fully populated Microsoft Store Linux image(s) (Ubuntu in this case) __WSL2__ registration and its corresponding hard disk (`VHDX`), but the __WSL__ runtime currently isn't available.


A non-blocking status check is simple in console:
```cmd
wsl.exe --status
```
```text
The Windows Subsystem for Linux is not installed. You can install by running 'wsl.exe --install'.
For more information please visit https://aka.ms/wslinstall
```
```cmd
echo %ERRORLEVEL%
```
```text
50
```

```
The blind attempt to list resources when __WSL__ is not installed (or installed, but subsequently removed) may lead to delay with returning the error of longer than a minute:

```cmd
wsl.exe --list --running
```
```text
The Windows Subsystem for Linux is not installed. You can install by running 'wsl.exe --install'.
For more information please visit https://aka.ms/wslinstall

Press any key to install Windows Subsystem for Linux.
Press ESC or CTRL-C to cancel.
This prompt will time out in 60 seconds.
```
thenThe Windows Subsystem for Linux is not installed. You can install by running 'wsl.exe --install'.
For more information please visit https://aka.ms/wslinstall

Press any key to install Windows Subsystem for Linux.
Press ESC or CTRL-C to cancel.
This prompt will time out in 60 seconds.
```
then

```text
Operation aborted
```



The application never receives the full message:

The
```text
Press any key to install...
Press ESC or CTRL-C...
This prompt will time out...
```
is part of the *interactive fallback behavior* of the WSL stub, rather than ordinary diagnostic output that caller's processInfo `StandardError.ReadToEnd()` is guaranteed to receive


----

### Author
