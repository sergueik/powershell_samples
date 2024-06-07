
### Info
this directory contains two
replicas of the [dummy service installer project](https://github.com/harlannorth/ServiceAndInstaller) and [basic Timeservice service installer](https://github.com/walkingriver/TimeServiceWixDemo)
merged with [Windows service Installer Example](https://github.com/PacktPublishing/WiX-3.6-A-Developer-s-Guide-to-Windows-Installer-XML/tree/master/Chapter%2011/Windows%20service%20example/WindowsServiceExample) from the book __WiX 3.6: A Developer's Guide to Windows Installer XML__ published by Packt.

There is only 9 projects in github showing Wix installers for Windows Service.

To load ithe projects in Visual Studion 2019, one has first install [Visual Studio Wix 3 extension](https://marketplace.visualstudio.com/items?itemName=WixToolset.WiXToolset)



It is an "VSIX installer" installer package of itself and being processed by Visual Studio itself

![wix extension installer](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-service-installer/screenshots/capture-wix-extension-installer.png)

After the Wix Toolset Extension is installed into Visual Studio, one of two project still fails to load with the verdict:
*application is not installed -  incompatible* 

In addition the WiX Toolset v3.11 build tools needs be installed  from [Wix Toolset 3.11 release directory](https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm)to build this project
 
![wix toolset installer](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-service-installer/screenshots/capture-wix-toolset-installer.png)
If this is not done, the Visual Studio Error is:
```text
Could not find wix.targets at 'C:\Program Files\MSBuild\Microsoft\WiX\v3.x\'. 
```
After the package is built the basic 
MSI prompts for privileged user password when operated by non privileged:

![perfmon](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-service-installer/screenshots/capture-elevation-dialog.png)

The uninstall prompts for a reboot which it claims will be scheduled


### WiX Tools

Note that [sharpdevelop](https://github.com/icsharpcode) __3.0__ comes with an old but still fully capable release of __WiX__ __3.0__:

```cmd
c:\Program Files\SharpDevelop\3.0\bin\Tools\Wix

01/07/2021  05:47 PM    <DIR>          .
01/07/2021  05:47 PM    <DIR>          ..
01/07/2021  05:47 PM    <DIR>          Bitmaps
10/06/2009  01:46 PM            24,576 candle.exe
10/06/2009  01:46 PM               232 candle.exe.config
10/06/2009  01:46 PM            11,666 CPL.txt
10/06/2009  01:46 PM           662,016 darice.cub
10/06/2009  01:46 PM            24,576 dark.exe
10/06/2009  01:46 PM               490 dark.exe.config
10/06/2009  01:46 PM           304,824 difxapp_x64.wixlib
10/06/2009  01:46 PM           208,525 difxapp_x86.wixlib
01/07/2021  05:47 PM    <DIR>          doc
10/06/2009  01:46 PM            28,672 heat.exe
10/06/2009  01:46 PM               360 heat.exe.config
10/06/2009  01:46 PM            36,864 light.exe
10/06/2009  01:46 PM               232 light.exe.config
10/06/2009  01:46 PM            24,576 lit.exe
10/06/2009  01:46 PM               232 lit.exe.config
10/06/2009  01:46 PM            24,576 melt.exe
10/06/2009  01:46 PM               271 melt.exe.config
10/06/2009  01:46 PM           501,248 mergemod.cub
10/06/2009  01:46 PM           150,016 mergemod.dll
10/06/2009  01:46 PM            28,672 Microsoft.Tools.WindowsInstallerXml.NAntTasks.dll
10/06/2009  01:46 PM            61,952 mspatchc.dll
10/06/2009  01:46 PM            28,672 pyro.exe
10/06/2009  01:46 PM               232 pyro.exe.config
10/06/2009  01:46 PM            24,576 smoke.exe
10/06/2009  01:46 PM               232 smoke.exe.config
10/06/2009  01:46 PM            28,672 torch.exe
10/06/2009  01:46 PM               232 torch.exe.config
10/06/2009  01:46 PM            24,576 wconsole.dll
10/06/2009  01:46 PM            95,232 winterop.dll
10/06/2009  01:46 PM         1,404,928 wix.dll
10/06/2009  01:46 PM            95,792 wix.targets
10/06/2009  01:46 PM           425,984 WixComPlusExtension.dll
10/06/2009  01:46 PM            81,920 WixCop.exe
10/06/2009  01:46 PM            24,576 WixDifxAppExtension.dll
10/06/2009  01:46 PM            57,344 WixDirectXExtension.dll
10/06/2009  01:46 PM            81,920 WixFirewallExtension.dll
10/06/2009  01:46 PM            77,824 WixGamingExtension.dll
10/06/2009  01:46 PM           335,872 WixIIsExtension.dll
10/06/2009  01:46 PM           106,496 WixIsolatedAppExtension.dll
10/06/2009  01:46 PM           131,072 WixMsmqExtension.dll
10/06/2009  01:46 PM           192,512 WixNetFxExtension.dll
10/06/2009  01:46 PM            94,208 WixOfficeExtension.dll
10/06/2009  01:46 PM            32,768 WixPSExtension.dll
10/06/2009  01:46 PM           225,280 WixSqlExtension.dll
10/06/2009  01:46 PM            57,344 WixTasks.dll
10/06/2009  01:46 PM         1,355,776 WixUIExtension.dll
10/06/2009  01:46 PM           598,016 WixUtilExtension.dll
10/06/2009  01:46 PM           790,528 WixVSExtension.dll
10/06/2009  01:46 PM            98,304 wui.dll
```

### NOTE:

Visual Studio 2019 reports failure in "reloading" the application project if the `.nuget` directory is missing:

```text
TimeService.csproj : error  : The imported project "TimeService\.nuget\nuget.targets" was not found. Confirm that the expression in the Import declaration "TimeService\\.nuget\nuget.targets" is correct, and that the file exists on disk.  
```
the directory
```

 Directory of ..\TimeService\.nuget

02/05/2023  01:32 AM    <DIR>          .
02/05/2023  01:32 AM    <DIR>          ..
01/19/2023  03:28 PM               164 NuGet.Config
01/19/2023  03:28 PM           651,264 NuGet.exe
01/19/2023  03:28 PM             7,452 NuGet.targets
```

and the dependency were inherited from the sample project.

### Building without Visual Studio

* copy `nuget.exe` from its install location, e.g. from Sharp Develop:
```powershell.exe
copy "c:/Program Files/SharpDevelop/5.0/AddIns/Misc/PackageManagement/NuGet.exe" .nuget
```

the NuGet Version: 2.7.40906.75 is OK.


#### Build the Test App

```powershell
cd Program
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Program.csproj
cd ..
```

#### Package

* update guid, version and other attributes (optional)

```powershell
cd Setup
$name = 'Product.wxs' 
$xml = [xml](get-content -path $name )
$xml.Normalize()
$guid = [guid]::NewGuid()
$xml.Wix.Product.Id = $guid.ToString()
$xml.Save(('Setup/' + $name))
cd ..
```
* NOTE: will switch to Windows line endings




* compile package
```powershell
cd Setup
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\Setup.wixproj
```
the `Setup.msi` will be in `Setup\bin\Debug`.


### Install

in elevated prompt

* install
```cmd
cd Setup\bin\Debug
msiexec.exe /quiet /i Setup.msi
```
* confirm
```cmd
sc.exe query "TimeService"
```
```text
SERVICE_NAME: TimeService
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```
```cmd
sc.exe qc "TimeService"
```
```text
[SC] QueryServiceConfig SUCCESS

SERVICE_NAME: TimeService
        TYPE               : 10  WIN32_OWN_PROCESS
        START_TYPE         : 2   AUTO_START
        ERROR_CONTROL      : 0   IGNORE
        BINARY_PATH_NAME   : "C:\Program Files\TimeService\TimeService.exe"
        LOAD_ORDER_GROUP   :
        TAG                : 0
        DISPLAY_NAME       : Time Service
        DEPENDENCIES       :
        SERVICE_START_NAME : LocalSystem
```
NOTE: the executable from the install dir is running

Alternatively inspect service from Poweshell:
```powershell
get-service -name 'TimeService' | select-object -property Name,Status,StartType|format-list

```
```text
Name      : TimeService
Status    : Running
StartType : Automatic
```
```powershell
get-wmiobject -Class Win32_Service -filter 'Name="TimeService"' -Property * -ErrorAction Stop | select-object -ExcludeProperty __* -Property Name,StartMode,State,Status,PathName | format-list
```

```text
Name      : TimeService
StartMode : Auto
State     : Running
Status    : OK
PathName  : "C:\Program Files\TimeService\TimeService.exe"
```
* uninstall

```cmd
msiexec /quiet /x Setup.msi
```

*  confirm service is removed
```cmd
sc.exe query "TimeService"
```

```text
[SC] EnumQueryServicesStatus:OpenService FAILED 1060:

The specified service does not exist as an installed service.

```
* run with logs 

```cmd
msiexec /l*v a.log /quiet /i Setup.msi
```
* NOTE:  the program will end up being put into `c:\Program Files\TimeService` on 32 bit Windows and in `c:\Program Files (x86)\TimeService` on 64 bit Windows:

```text
 Directory of c:\Program Files\TimeService

02/05/2023  09:03 PM    <DIR>          .
02/05/2023  09:03 PM    <DIR>          ..
02/04/2023  08:39 PM             1,963 log4net.config
02/16/2012  11:12 AM           288,768 log4net.dll
11/20/2012  05:45 PM           391,680 Newtonsoft.Json.dll
08/08/2012  12:58 PM           180,832 System.Net.Http.dll
08/08/2012  12:58 PM           168,544 System.Net.Http.Formatting.dll
08/08/2012  12:58 PM            16,480 System.Net.Http.WebRequest.dll
08/08/2012  12:58 PM           323,168 System.Web.Http.dll
09/20/2012  06:52 AM           105,584 System.Web.Http.SelfHost.dll
02/05/2023  09:00 PM            10,240 TimeService.exe
02/04/2023  08:39 PM               256 TimeService.exe.config
02/05/2023  09:00 PM            24,064 TimeService.pdb
              11 File(s)      1,511,579 bytes
```

also from installer log:

```text
PROPERTY CHANGE: Adding WixPerUserFolder property. Its value is 'C:\Users\sergueik\AppData\Local\Apps\TimeService'.
PROPERTY CHANGE: Adding WixPerMachineFolder property. Its value is 'C:\Program Files\TimeService'.
```

when installer is run by non-elevated user, the following failure will be logged:

```text
Property(S): ProductToBeRegistered = 1
MSI (s) (F4:8C) [20:59:34:179]: Note: 1: 1708 
MSI (s) (F4:8C) [20:59:34:179]: Note: 1: 2205 2:  3: Error 
MSI (s) (F4:8C) [20:59:34:179]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1708 
MSI (s) (F4:8C) [20:59:34:179]: Note: 1: 2205 2:  3: Error 
MSI (s) (F4:8C) [20:59:34:179]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1709 
MSI (s) (F4:8C) [20:59:34:179]: Product: TimeService -- Installation failed.

```
and possibly some other errors - the msi log is cryptic

### TrustedInstaller

to find the sid of the `TrustedInstaller` user, use Service Control Manager and services utility `sc.exe`:

```cmd
sc.exe showsid TrustedInstaller
```
```text
NAME: TrustedInstaller
SERVICE SID: S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464
STATUS: Active
```
the WMI query to `Win32_UserAccount` appears to not return anything about this one and neither does `get-localuser` cmdlet


### See Also
  * https://github.com/Robs79/How-to-create-a-Windows-Service-MSI-Installer-Using-WiX
  * https://github.com/salyh/elasticsearch-msi-installer - relies on powershell and batch scripts to run `candle.exe` `light.exe` etc.
  * [managed advertised application installations for auto-elevation](https://learn.microsoft.com/en-us/windows/win32/msi/installing-a-package-with-elevated-privileges-for-a-non-admin) - possible because Windows Installer service
always has elevated privileges while doing installs. locked  heavily into Windows Registry, domain, and group policies - does not appear to set any option in the MSI itself
  * [documentation](https://learn.microsoft.com/en-us/windows/win32/msi/alwaysinstallelevated) on `AlwaysInstallElevated` policy to install a Windows Installer package with elevated (system) privileges.

  * [codeproject article](https://www.codeproject.com/Tips/105638/A-quick-introduction-Create-an-MSI-installer-with) on __quick introduction: Create an MSI installer with WiX__, shows the usage of `candle.exe`, `light.exe` etc.
  * [blog](https://www.advancedinstaller.com/versus/wix-toolset/wix-installer-add-windows-services.html) on __How To Add Windows Services with WiX Installer__
  * __Wix 3.x__ [repository](https://github.com/wixtoolset/wix3) and [release directory](https://github.com/wixtoolset/wix3/releases) 
  * [how to Setup the WiX Toolset](https://hackernoon.com/how-to-setup-the-wix-toolset)
  * [WiX Toolset Tutorial](https://www.firegiant.com/wix/tutorial/)
  * [WIX for Install DataBase](https://www.codeproject.com/Articles/331368/WIXDataBase)
  * [stackoverflow](https://stackoverflow.com/questions/23217740/use-wix-without-visual-studio) on use WIX without visual studio
  * __Wix Setup Samples__ [projects](https://github.com/deepak-rathi/Wix-Setup-Samples) - 10 sample projects illustrating various install scenarion implementation
  * __quick introduction: Create an MSI installer with WiX__ [article](https://www.codeproject.com/Tips/105638/A-quick-introduction-Create-an-MSI-installer-with)
  * __Wix Tricks__ [article](https://www.codeproject.com/Articles/43564/WiX-Tricks)
  * WiX Toolset Tutorial [Creating a Simple Setup](https://wixtoolset.org/docs/v3/votive/authoring_first_votive_project/)
  * [Using wix to schedule a task as the local system](https://kamivaniea.com/?p=632)
  * [source code](https://github.com/PacktPublishing/WiX-3.6-A-Developer-s-Guide-to-Windows-Installer-XML) of the  __WiX: A Developer's Guide to Windows Installer XML__ book
  * `ServiceConfig` element (Wix Toolset 3.x Util Extension) [documentation](https://wixtoolset.org/docs/v3/xsd/util/serviceconfig/)
  * [WIX Builder for Jenkins](https://github.com/jenkinsci/wix-plugin)
  * [WiX 3 Tutorial: Solution/Project structure and Dev resources](https://weblogs.sqlteam.com/mladenp/2010/02/11/wix-3-tutorial-solutionproject-structure-and-dev-resources/)
  * [Wix recognized project References and Variables](https://wixtoolset.org/docs/v3/votive/votive_project_references/)
  * https://serverfault.com/questions/561246/environment-variable-for-net-framework-location
  * https://manfredlange.blogspot.com/2010/02/using-environment-variables-in-wix.html
  * https://blog.bartdemeyer.be/2013/10/create-an-installer-for-website-with-wix-part-1/
  * [history](https://en.wikipedia.org/wiki/Windows_Installer)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)



