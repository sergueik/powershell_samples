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
```cmd
msiexec.exe  /l*v "install.log" /i bin\VSCodeCustomInstaller.msi
```

![MSI fatal error](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-vscode-customized/screenshots/msi_error.png)
```txt
Property(S): VSCodeInstallDir = C:\Program Files\Microsoft VS Code\
Property(S): VSIXFolder = C:\Users\sergueik\AppData\Local\Temp\VSIX\
Property(S): CodeConfigDir = C:\Users\sergueik\AppData\Roaming\Code\User\
Property(S): CodeUserDir = C:\Users\sergueik\AppData\Roaming\Code\
Property(S): VSCodeBinDir = C:\Program Files\Microsoft VS Code\bin\
Property(S): ProgramFilesFolder = C:\Program Files\
Property(S): TARGETDIR = C:\
Property(S): AppDataFolder = C:\Users\sergueik\AppData\Roaming\
Property(S): TempFolder = C:\Users\sergueik\AppData\Local\Temp\
Property(S): SourceDir = C:\developer\sergueik\powershell_samples\external\wix\basic-vscode-customized\bin\
Property(S): ALLUSERS = 1
Property(S): Manufacturer = MyCompany
Property(S): ProductCode = {8793DC7C-EED2-480C-A4AE-FF16B94C413A}
Property(S): ProductLanguage = 1033
Property(S): ProductName = VSCode Custom Installer
Property(S): ProductVersion = 1.0.0.0
Property(S): UpgradeCode = {4F63A60C-8958-4990-9CAD-AF6357B8DA8C}
Property(S): MsiLogFileLocation = C:\developer\sergueik\powershell_samples\external\wix\basic-vscode-customized\install.log
Property(S): PackageCode = {5E2BA9D5-1762-4139-A59B-495F8DD03329}
Property(S): ProductState = -1
Property(S): PackagecodeChanging = 1
Property(S): CURRENTDIRECTORY = C:\developer\sergueik\powershell_samples\external\wix\basic-vscode-customized
Property(S): CLIENTUILEVEL = 0
Property(S): CLIENTPROCESSID = 2224
Property(S): USERNAME = sergueik
Property(S): VersionDatabase = 200
Property(S): ROOTDRIVE = C:\
Property(S): EXECUTEACTION = INSTALL
Property(S): ACTION = INSTALL
Property(S): INSTALLLEVEL = 1
Property(S): SECONDSEQUENCE = 1
Property(S): ADDLOCAL = MainFeature
Property(S): VersionMsi = 5.00
Property(S): VersionNT = 601
Property(S): WindowsBuild = 7601
Property(S): ServicePackLevel = 1
Property(S): ServicePackLevelMinor = 0
Property(S): MsiNTProductType = 1
Property(S): MsiNTSuitePersonal = 1
Property(S): WindowsFolder = C:\Windows\
Property(S): WindowsVolume = C:\
Property(S): SystemFolder = C:\Windows\system32\
Property(S): System16Folder = C:\Windows\system\
Property(S): RemoteAdminTS = 1
Property(S): CommonFilesFolder = C:\Program Files\Common Files\
Property(S): FavoritesFolder = C:\Users\sergueik\Favorites\
Property(S): NetHoodFolder = C:\Users\sergueik\AppData\Roaming\Microsoft\Windows\Network Shortcuts\
Property(S): PersonalFolder = C:\Users\sergueik\Documents\
Property(S): PrintHoodFolder = C:\Users\sergueik\AppData\Roaming\Microsoft\Windows\Printer Shortcuts\
Property(S): RecentFolder = C:\Users\sergueik\AppData\Roaming\Microsoft\Windows\Recent\
Property(S): SendToFolder = C:\Users\sergueik\AppData\Roaming\Microsoft\Windows\SendTo\
Property(S): TemplateFolder = C:\ProgramData\Microsoft\Windows\Templates\
Property(S): CommonAppDataFolder = C:\ProgramData\
Property(S): LocalAppDataFolder = C:\Users\sergueik\AppData\Local\
Property(S): MyPicturesFolder = C:\Users\sergueik\Pictures\
Property(S): AdminToolsFolder = C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Administrative Tools\
Property(S): StartupFolder = C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\
Property(S): ProgramMenuFolder = C:\ProgramData\Microsoft\Windows\Start Menu\Programs\
Property(S): StartMenuFolder = C:\ProgramData\Microsoft\Windows\Start Menu\
Property(S): DesktopFolder = C:\Users\Public\Desktop\
Property(S): FontsFolder = C:\Windows\Fonts\
Property(S): GPTSupport = 1
Property(S): OLEAdvtSupport = 1
Property(S): ShellAdvtSupport = 1
Property(S): Intel = 6
Property(S): PhysicalMemory = 3584
Property(S): VirtualMemory = 3042
Property(S): LogonUser = sergueik
Property(S): UserSID = S-1-5-21-440999728-2294759910-2183037890-1000
Property(S): UserLanguageID = 1033
Property(S): ComputerName = SERGUEIK42
Property(S): SystemLanguageID = 1033
Property(S): ScreenX = 1024
Property(S): ScreenY = 768
Property(S): CaptionHeight = 22
Property(S): BorderTop = 1
Property(S): BorderSide = 1
Property(S): TextHeight = 16
Property(S): TextInternalLeading = 3
Property(S): ColorBits = 32
Property(S): TTCSupport = 1
Property(S): Time = 5:13:14
Property(S): Date = 11/26/2025
Property(S): MsiNetAssemblySupport = 4.8.3761.0
Property(S): MsiWin32AssemblySupport = 6.1.7601.17514
Property(S): RedirectedDllSupport = 2
Property(S): AdminUser = 1
Property(S): MsiRunningElevated = 1
Property(S): Privileged = 1
Property(S): DATABASE = C:\Windows\Installer\172de6.msi
Property(S): OriginalDatabase = C:\developer\sergueik\powershell_samples\external\wix\basic-vscode-customized\bin\VSCodeCustomInstaller.msi
Property(S): UILevel = 5
Property(S): Preselected = 1
Property(S): CostingComplete = 1
Property(S): OutOfDiskSpace = 0
Property(S): OutOfNoRbDiskSpace = 0
Property(S): PrimaryVolumeSpaceAvailable = 0
Property(S): PrimaryVolumeSpaceRequired = 0
Property(S): PrimaryVolumeSpaceRemaining = 0
Property(S): SOURCEDIR = C:\developer\sergueik\powershell_samples\external\wix\basic-vscode-customized\bin\
Property(S): SourcedirProduct = {8793DC7C-EED2-480C-A4AE-FF16B94C413A}
Property(S): ProductToBeRegistered = 1
MSI (s) (A4:E4) [05:13:14:171]: MainEngineThread is returning 1603
MSI (s) (A4:70) [05:13:14:171]: RESTART MANAGER: Session closed.
MSI (s) (A4:70) [05:13:14:171]: No System Restore sequence number for this installation.
MSI (s) (A4:70) [05:13:14:171]: User policy value 'DisableRollback' is 0
MSI (s) (A4:70) [05:13:14:171]: Machine policy value 'DisableRollback' is 0
MSI (s) (A4:70) [05:13:14:171]: Incrementing counter to disable shutdown. Counter after increment: 0
MSI (s) (A4:70) [05:13:14:171]: Note: 1: 1402 2: HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Installer\Rollback\Scripts 3: 2 
MSI (s) (A4:70) [05:13:14:171]: Note: 1: 1402 2: HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Installer\Rollback\Scripts 3: 2 
MSI (s) (A4:70) [05:13:14:171]: Decrementing counter to disable shutdown. If counter >= 0, shutdown will be denied.  Counter after decrement: -1
MSI (s) (A4:70) [05:13:14:171]: Restoring environment variables
MSI (c) (B0:B4) [05:13:14:171]: Back from server. Return value: 1603
```
MSI error `1603` is a generic "fatal error" that occurs during a Windows 
Installer operation due to a variety of system-related issues, such as insufficient permissions, corrupted files, or a conflicting application

### Docker 

#### Run [VS Code Server]() in the browser
```sh
docker image pull ruanbekker/vscode-server:slim
```

```sh
docker run -it -p 8443:8443 -p 8080:8080 ruanbekker/vscode-server:slim
```
```sh
ID=$(docker ps | grep 'ruanbekker/vscode-server'  |cut -f 1 -d ' ')
```

```sh
docker exec -it $ID sh -c 'cat /home/$(whoami)/.config/code-server/config.yaml'
```
NOTE: use single quotes

```text
bind-addr: 127.0.0.1:8080
auth: password
password: 955141ef9c82051e9db612a2
cert: false
```
* authenticate 

![Visual Studio Code Login](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-vscode-customized/screenshots/login.png)

![Visual Studio in the Browser](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-vscode-customized/screenshots/code.png)
#### Run [VS Code with X Server](https://github.com/pubkey/vscode-in-docker/blob/master/docker/Dockerfile) In Container

WIP

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

