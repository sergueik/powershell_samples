### Info

This directory contains the setup project of a basic Windows tray app with desktop shortcut using the examples:


  *  __Wix Cookbook__ book [chapter 4 Recipe 3](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_4/code/recipe_4/foldershortcutinstaller/FolderShortcutInstaller) showing how to install a shortcut to the Desktop

  * [chapter 5 recipe 2](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_5/code/recipe_2/xmlattributeinstaller/XmlAttributeInstaller) showing altering an XML application configuration file settig and attribute at install time


The tray app is a replica of [System Tray Application skeleton project](https://github.com/sergueik/powershell_samples/tree/master/external/csharp/form_systemtray_app)
with few static files (`Manual.pdf`, `MainIcon.ico`  and applocation config deployed explicitly into the application dir.


### Usage

#### Build the Test App

```powershell

$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe .\Program\Program.csproj
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
# Note: saves into wrong directory
$xml.Save(('{0}\{1}' -f 'Setup', $name))
```
* NOTE: this update will switch `Product.wxs` to Windows line endings

* compile package

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe Setup.wixproj
```

* ignore the linker warnings:

```text
  Product.wxs(43): warning LGHT1076: ICE69: Mismatched component reference. Entry 'AppDesktopShortcut' of the Shortcut table belongs to component 'ComponentDesktopShortcut'. However, the formatted string in column 'Target' references file 'Application' which belongs to component 'Engine'. Components are in the same feature. [Setup.wixproj]
  Setup.msi : warning LGHT1076: ICE74: The UpgradeCode property is not authored in the Property table. It is strongly recommended that authors of installation packages specify an UpgradeCode for their application. [Setup.wixproj]
  Product.wxs(61): warning LGHT1076: ICE91: The file 'Manual' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [Setup.wixproj]
  Product.wxs(54): warning LGHT1076: ICE91: The file 'Application' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [Setup.wixproj]
  Product.wxs(69): warning LGHT1076: ICE91: The file 'configuration' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [Setup.wixproj]
  Product.wxs(76): warning LGHT1076: ICE91: The file 'Setting' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [Setup.wixproj]
  Product.wxs(43): warning LGHT1076: ICE91: The shortcut 'AppDesktopShortcut' will be installed to the per user directory 'DesktopFolder' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [Setup.wixproj]
```

the `Setup.msi` will be in `Setup\bin\Debug`.

### Install

* in regular prompt

```cmd
msiexec.exe /l*v a.log /i bin\Debug\Setup.msi
```

after the install, few registry entries are created:
```powershell
Get-ItemProperty -path 'HKCU:\Software\Manufacturer\System Tray App' -name 'MainExe' -erroraction silentlycontinue| select-object -expandproperty MainExe
```
```text
1
```
There is an update in WMI database, it shows

```cmd
wmic:root\cli>path win32_product where (caption = "System Tray App") get * /format:list
```
```text
AssignmentType=0
Caption=System Tray App
Description=System Tray App
HelpLink=
HelpTelephone=
IdentifyingNumber={015F2E2C-6B6B-4E18-B595-930635F04380}
InstallDate=20230331
InstallDate2=
InstallLocation=
InstallSource=C:\developer\sergueik\powershell_samples\external\wix\basic-systemtray-installer\Setup\bin\Debug\
InstallState=5
Language=1033
LocalPackage=C:\Windows\Installer\17e20f3.msi
Name=System Tray App
PackageCache=C:\Windows\Installer\17e20f3.msi
PackageCode={688A0028-C03C-4E72-A1C4-72F2024A687F}
PackageName=Setup.msi
ProductID=
RegCompany=
RegOwner=
SKUNumber=
Transforms=
URLInfoAbout=
URLUpdateInfo=
Vendor=Manufacturer
Version=1.2.3
WordCount=10

```
* Note: offten but not always the package is cached in a new directory under `%ALLUSERSPROFILE%`:
```cmd
dir "${env:ALLUSERSPROFILE}\Package Cache"| Sort-Object LastWriteTime | select-object -last 1
```

```text
    Directory: C:\ProgramData\Package Cache


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
d-----        4/25/2021   2:56 PM                C2EC438DA75EA01B28C669BDDA0E5B2E9E9CAEB0
```
	
The `Setup.msi` is produced. After installed, the product can be uninstalled and the App can be started from desktop shortcut and run as expected, 

![Application in the tray](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-application.png)

the launcher is on Desktop:

![Application desktop icon](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-desktop-icon.png)

![Application New desktop icon](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-new-desktop-icon.png)


The application is installed under `LOCALAPPDATA` folder 

![Application directory](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-application-dir.png)

The install shows no UAC dialog and is possible for regular user to install or remove


The launcher is on Desktop:


![Application Launcher](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-launcher.png)

### Edit Configuration

one can modify the application configuration file `config.ini` deployed to application directory, by using the "Config" menu. The tray application will wait for the editor to be closed:


![modify the application configuration file](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-edit-config.png)


while the editor is running, the application will wait

![waiting the application configuration file to be done](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-edit-config2.png)

If configuration has been modified, it is still removed during uninstall (this is possibly a bug)

### Upgrade

* build the app
* build the installer package
* Save the installer, say as `Setup-1.0.0.msi`
* Install package
* run app and check version
* Update `[assembly: AssemblyVersion("1.0.0.0")]` in `Program/Properties/AssemblyInfo.cs` 
* Uninstall the package (the binary may sometimes remain in `%APPDATA%`
* Update `Wix/Product/@Id`in `Setup /Product.wxs`
* repeat the steps. Use a different name for new version of installer package e.g. `Setup-1.1.0.msi`

If no upgrade observed, manually remove the leftover `C:\Users\sergueik\AppData\Local\System Tray App\SystemTrayApp.exe` and repeat to confirm the intended behavior

NOTE: on localized versions of Windows the folder will not be named in English, so it is recommended to compose the path manualy (`c:\Users\sergueik\AppData\Local`)

NOTE: if application was detected running, and reboot option was selected, the empty application dir will remain under `c:\Users\sergueik\AppData\Local`

### ToDo

* There is no real check that app is running before uninstall. 

When the executable was put (incorrectly) directly into `C:\Program Files`, there was this check resulting in warning dialog:

![warning during removal](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-detect-app-running.png)

* when the `Icon` element is used along with `Shortcut`,  the launcher is placed into a wrong directory (rolled back the breaking change):

```text
c:\Program Files\System Tray App Manual.lnk
c:\Program Files\System Tray Application.lnk
```

the UAC dialog shows during the install and  there was occasionally a problem with uninstalling the app:

![regression failure to uninstall](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-regression-problem-after-icon.png)

There is no real check that app is running before uninstall. 
When the executable was put (incorrectly) directly into `C:\Program Files`, there was this check resulting in warning dialog:

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-systemtray-installer/screenshots/capture-detect-app-running.png)

The application executable is not removed during the uninstall (this is a bug)


### See Also


  * MSI [Installer](https://github.com/binjr/binjr/blob/master/distribution/bundlers/win_msi/binjr.wxs) of RRD Time Series Data Browser [](https://github.com/binjr/binjr) 
  * [Installer](https://github.com/Windos/BurntToast/blob/main/Installer/src/BurntToast.wxs) for [Module](https://github.com/Windos/BurntToast) for creating and displaying Toast Notifications on Microsoft Windows 10
  * [converting per-machine install to per-user](https://stackoverflow.com/questions/12102771/how-do-i-install-to-localappdata-folder)
  * `InstallPrivileges` and `InstallScope` attribute of `Package` element [discussion](https://wix-users.narkive.com/pOkFtyxs/creating-msi-for-non-admin-user)
  * [stackoverflow](https://stackoverflow.com/questions/28320441/is-it-possible-to-install-the-same-application-either-per-user-or-per-machine) about challenges of per-use install and  combining both
  * [stackoverflow discussion](https://stackoverflow.com/questions/54602947/wix-msi-installer-successfully-runs-for-uninstalling-an-app-but-the-app-has-not) for MSI installer successfully runs for uninstalling an app but the app has not been uninstalled situation

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

