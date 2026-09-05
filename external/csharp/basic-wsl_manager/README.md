### Info
Replica of the [gekkom/WSL-Manager](https://github.com/gekkom/WSL-Manager)
Windows Subsystem for Linux Manager GUI that does not require [.NET Core](https://docs.microsoft.com/dotnet/core/) (an open-source, cross-platform successor to __.NET Framework__ aiming on make it to Mac OS Linux platrorms. 
>NOTE `https://docs.microsoft.com/dotnet/core/` now redirects to https://learn.microsoft.com/en-us/dotnet/fundamentals/ and there is no way back:
> ```code
> 301 Moved Permanently
> ```

#### History of the Original Project

> Deprecated please use: https://github.com/bostrot/wsl2-distro-manager

> Works on Windows 10 1803, 1809, 1903, 1909, 2004 and every corresponding Windows Server version.
> Older Windows versions may have limitations.

There is ofiginal vendor [Installer executable](https://github.com/visdauas/WSL-Manager/releases/download/v1.1.1/wsl-manager-installer-x64.exe)(untested)

The key  WSL Manager dependency - [LxRunOffline](https://github.com/DDoSolitary/LxRunOffline) -thin layer around Windows Registry and `wsl.exe` calls - abandoned code -offering the following ultra basic functionality:

+ Install any Linux distro to any directory on your computer.
+ Move an existing installation to another directory.
+ Duplicate(copy) an existing installation.
+ Register an existing installation directory. This enables you to install to a USB stick and use it on different computers.
+ Run arbitrary Linux commands in a specified installation.
+ Configure default user, environment variables and various flags.
+ Export configuration to an XML file and import from the file.
+ Export an installation to a tar file.

> Original project has been abandoned as far as I know [Old Repo](https://github.com/wslhub/WSL-DistroManager)

### Usage

copy the dependency we do not like to store under source control

```
mkdir -p Program/bin/x64/Debug/External
curl -skLo Program/bin/x64/Debug/External/LxRunOffline.exe https://github.com/gekkom/WSL-Manager/raw/refs/heads/master/WSL%20Manager/External/LxRunOffline.exe
```
> NOTE: consider to use the dependency release location https://github.com/DDoSolitary/LxRunOffline/releases

build for x64 Debug

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
msbuild.exe .\basic-wsl_manager.sln "/p:Platform=x64" /detailedsummary /t:clean,build
```

> NOTE: app need to run with administrator token (manifest?)

![capture elevation](screenshots/capture-elevation.png)

```code
Can not start process. 
The requested operation requires elevation. (Exception from HRESULT: 0x800702E4)
```
after run through filtered/non-elevated token to full , appliction behaves as expected

![capture app](screenshots/capture-app.png)

> NOTE: iit uses `LxRunOffline.exe`  for virtually everything even to launch the shell in the VM:

![capture app](screenshots/capture-app-launch.png)
### See Also
 
  * [WPF WSL Manager](https://github.com/wslhub/WslManager) - __.Net__ __6__
  * [WSL Maui Universal](https://github.com/Forz70043/bridge) - reuires VS 2022 build schema and
----

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

