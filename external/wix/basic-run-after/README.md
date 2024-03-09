### Info

This directory contains the variation of WIX
setup project of a [basic Windows tray app with desktop shortcut](https://github.com/sergueik/powershell_samples/tree/master/external/wix/basic-systemtray-installer)
using the examples

  * [run application after install](https://wixtoolset.org/docs/v3/howtos/ui_and_localization/run_program_after_install/)

  * [stackoverflow](https://stackoverflow.com/questions/19271862/wix-how-to-run-exe-files-after-installation-from-installed-directory) discussion on starting the appafter install


to ask the user to decide the application to be launched after the install
	
### Usage

#### Build the Test App (required when installing the Application)

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
* NOTE: this update will convert the `Product.wxs` to Windows line endings

* compile package

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe Setup.wixproj
```

* ignore the linker warnings:

```text
(AddSolutionDefineConstants target) ->
  C:\Program Files\MSBuild\Microsoft\WiX\v3.x\wix2010.targets(1199,5): warning : Solution properties are only available during IDE builds or when building the solution file from the command line. To turn off this warning set <DefineSolutionProperties>false</DefineSolutionProperties> in your .wixproj file. [\Setup\Setup.wixproj]


"Setup\Setup.wixproj" (default target) (1) ->
(Link target) ->
  ...\Setup\Product.wxs(101): warning LGHT1076: ICE69: Mismatched component reference. Entry 'AppDesktopShortcut' of the Shortcut table belongs to component 'ComponentDesktopShortcut'. However, the formatted string in column 'Target' references file 'Application' which belongs to component 'Engine'. Components are in the same feature. [\Setup\Setup.wixproj]
  ...\Setup\Product.wxs(119): warning LGHT1076: ICE91: The file 'Manual' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [\Setup\Setup.wixproj]
  ...\Setup\Product.wxs(112): warning LGHT1076: ICE91: The file 'Application' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [\Setup\Setup.wixproj]
  ...\Setup\Product.wxs(127): warning LGHT1076: ICE91: The file 'configuration' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [\Setup\Setup.wixproj]
  ...\Setup\Product.wxs(134): warning LGHT1076: ICE91: The file 'Setting' will be installed to the per user directory 'INSTALLFOLDER' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [\Setup\Setup.wixproj]
  ...\Setup\Product.wxs(101): warning LGHT1076: ICE91: The shortcut 'AppDesktopShortcut' will be installed to the per user directory 'DesktopFolder' that doesn't vary based on ALLUSERS value. This file won't be copied to each user's profile even if a per machine installation is desired. [\Setup\Setup.wixproj]
```

the `Setup.msi` will be in `Setup\bin\Debug`.

Alternatively,

```powershell
$env:PATH = "${env:PATH};${env:ProgramFiles}\WiX Toolset v3.11\bin"
if (${env:ProgramFiles(x86)}) { $env:PATH = "${env:PATH};${env:ProgramFiles(x86)}\WiX Toolset v3.11\bin"}
$candle = get-Command candle.exe | select-object -expandproperty source 
$light = get-Command light.exe | select-object -expandproperty source 
```
```powershell
cd Setup
& $candle Product.wxs
& $light Product.wixobj -ext WixUIExtension -ext WixUtilExtension -sval
```
NOTE: the `$candle` will need no extra quotes around the path, the following is not needed:
```powershell
$candle = get-Command candle.exe | select-object -ExpandProperty Source | foreach-object { ("""{0}""" -f $_) }
```
the `Program.msi` will be in the `Setup` directory

#### Installing

* in regular prompt, or admin Prompt, based on the type of the install

```cmd
msiexec.exe /l*v a.log /i bin\Debug\Setup.msi
```

after the "Launch my Application Name" checkbox is checked
![User Choice](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-run-after/screenshots/capture-launch-application.png)


the application will be launched

![App Running](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-run-after/screenshots/capture-application-launched.png)

If this is not the case, inspect the MSI log `a.log` for  messages like

```text
Action 14:46:39: LaunchApplication.
Action start 14:46:39: LaunchApplication.
MSI (c) (70:54) [14:46:39:097]: Invoking remote custom action. DLL: C:\Users\sergueik\AppData\Local\Temp\MSI49D6.tmp, Entrypoint: WixShellExec
MSI (c) (70:24) [14:46:39:097]: Cloaking enabled.
MSI (c) (70:24) [14:46:39:097]: Attempting to enable all disabled privileges before calling Install on Server
MSI (c) (70:24) [14:46:39:097]: Connected to service for CA interface.
MSI (c) (70!8C) [14:46:39:159]: Note: 1: 2715 2: SystemTrayApp.exe
MSI (c) (70!8C) [14:46:39:159]: Note: 1: 2715 2: SystemTrayApp.exe
Action ended 14:46:39: LaunchApplication. Return value 3.
MSI (c) (70:2C) [14:46:39:159]: Note: 1: 2205 2:  3: Error
MSI (c) (70:2C) [14:46:39:159]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 2896
DEBUG: Error 2896:  Executing action LaunchApplication failed.
The installer has encountered an unexpected error installing this package. This may indicate a problem with this package. The error code is 2896. The arguments are: LaunchApplication, ,
Action ended 14:46:39: ExitDialog. Return value 3.
Action ended 14:46:39: INSTALL. Return value 1.
MSI (c) (70:E8) [14:46:39:159]: Destroying RemoteAPI object.
MSI (c) (70:24) [14:46:39:159]: Custom Action Manager thread ending.

```
NOTE, the code `3` (`ERROR_PATH_NOT_FOUND`) indicates that the application has been referneced improperly. The reference attribute
```xml
<Property Id="WixShellExecTarget" Value="[#Application]" />
```
must be the same as `Id` attribute of the `File`
```xml
<File Id="Application" Name="SystemTrayApp.exe" DiskId="1" Source="..\Program\bin\$(var.Configuration)\SystemTrayApp.exe" />
```
if the `Name` was used
```xml
 Value="[#SystemTrayApp.exe]"
```
by mistake it will lead to the above error

#### Uninstall

can be done by using the `Setup.msi` or __Programs and Features__

* NOTE: if the application was not stopped before uninstall, the uninstall will request a reboot:


![Uninstall Warning](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-run-after/screenshots/capture-uninstall-choice.png)


![Uninstall Unable to Stop](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-run-after/screenshots/capture-uninstall-conclusion.png)

### NOTES:

* if the
```xml
<UIRef Id="WixUI_Advanced" />
```
is used instead of
```xml
<UIRef Id="WixUI_Minimal" />
```
in `Setup.wixproj`, observed the following error
```text
  C:\agent\_work\66\s\src\ext\UIExtension\wixlib\WixUI_Advanced.wxs(30):
error LGHT0094:
Unresolved reference to symbol 'Property:ApplicationFolderName' in section 'Fragment:'.
```

* if the `dll` reference is omitted
```xml
  <ItemGroup>
    <WixExtension Include="WixUIExtension">
      <HintPath>$(WixExtDir)\WixUIExtension.dll</HintPath>
      <Name>WixUIExtension</Name>
    </WixExtension>
  </ItemGroup>
```
observed the error
```text
NOTE: error LGHT0094: Unresolved reference to symbol 'WixUI:WixUI_Minimal' in section 'Product:{19F1EF00-48B4-4ACF-B07F-E9EBAAA64C18}'. 
```

* there is no argument support in `WixShellExecTarget`


```xml
<Property Id="WixShellExecTarget" Value="notepad.exe [#Application]"/>
```
will open `notepad.exe` without the argument. Many other attempts will fail to launch anything with no helpful logging even with `l*vx` flag is provided


### See Also:

  * [how To: Run the Installed Application After Setup](http://wixtoolset.org/documentation/manual/v3/howtos/ui_and_localization/run_program_after_install.html)
  * [stackoverflow](https://stackoverflow.com/questions/31305483/switches-in-wixshellexectarget) explanation how to call with arguments
  * [WixShellExec custom actions](https://wixtoolset.org/docs/tools/wixext/util/#wixshellexec-custom-actions)
  * [Windows Installer Error Messages (for Developers)](https://bit.ly/msi-error-codes)
  * [Debug system error codes](https://bit.ly/windows-error-codes)
  * [COM Error Codes](https://bit.ly/com-error-codes)
  * [Enable Windows Installer logging](https://support.microsoft.com/kb/223300) - explains the flags in the value `KEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\Installer\Logging`
  * [icuxika/WiXToolset3Builder](https://github.com/icuxika/WiXToolset3Builder) - `light` and `candle` command options example


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

