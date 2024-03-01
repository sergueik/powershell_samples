### Info


this directory contains code from [codeproject article](https://www.codeproject.com/Articles/511653/Using-WIX-With-Managed-Custom-Action)
updated to run per-user install. The custom action `MyCustomActionMethod` was originally used to create log file

In addition to writing to log file, for which the user running the installer may not have sufficient access permissions, the custom action creates an event log entry.


The second custom action `GetTimeZone` generates a value for WIX MSbuild "session" property `TIME_ZONE` with the data that is accurate on the target machine. This session property can be used in later custom actions, e.g. in command arguments, and in the log messages

### Usage

#### Build the Test App

*  make sure the prerequisite MSBuild projects are present:

```txt
 Directory of c:\Program Files\MSBuild\Microsoft\WiX\v3.x

02/26/2023  05:24 PM    <DIR>          .
02/26/2023  05:24 PM    <DIR>          ..
09/15/2019  07:13 AM             4,233 lux.targets
09/16/2019  02:23 PM             9,067 wix.ca.targets
09/15/2019  07:13 AM             1,731 wix.nativeca.targets
09/16/2019  02:23 PM             1,097 wix.targets
09/16/2019  02:23 PM           145,601 wix200x.targets
09/16/2019  02:23 PM           146,067 wix2010.targets
```
on a 64 bit Windows machine this will be in `Program Files (x86)`
```cmd
mkdir "c:\Program Files (x86)\MSBuild\Microsoft\\WiX\v3.x"

```

```powershell

$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe .\CustomAction\CustomAction.csproj /t:clean
MSBuild.exe .\CustomAction\CustomAction.csproj
```
in the bin directory will observe additional artifacts:
```text
    Directory: CustomAction\bin\Debug


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        4/11/2023   3:21 PM         251296 CustomAction.CA.dll
-a----        4/11/2023   3:21 PM           4608 CustomAction.dll
-a----        9/16/2019  11:31 PM         184240 Microsoft.Deployment.WindowsInstaller.dll
```

the purpose of `Microsoft.Deployment.WindowsInstaller.dll` is unknown but the `CustomAction.CA.dll` is referenced in the `Product.wxs` as
```XML
<Binary Id="CustomActionBinary" SourceFile="$(var.CustomAction.TargetDir)$(var.CustomAction.TargetName).CA.dll"/>
```
#### Package

* compile package

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
cd Setup
MSBuild.exe Setup.wixproj
```

the `Setup.msi` will be in `Setup\bin\Debug`.
if open in the `7-zip` it will show only `dummy.txt` - the dll will be hidden inside the cabinet:

```cmd
"c:\Program Files\7-Zip\7z.exe" l Setup.msi
```
```text
7-Zip 9.20  Copyright (c) 1999-2010 Igor Pavlov  2010-11-18

Listing archive: Setup.msi

--
Path = Setup.msi
Type = Compound
Cluster Size = 4096
Sector Size = 64
----
Path = Product.cab
Folder = -
Size = 113
Packed Size = 128
--
Path = Product.cab
Type = Cab
Method = MSZip
Blocks = 1
Volumes = 1

```			
#### Install

* in regular prompt

```cmd
msiexec.exe /l*v a.log /i bin\Debug\Setup.msi
```
observe in the MSI log (`a.log`):

```text
MSI (s) (C4!94) [13:48:01:410]: PROPERTY CHANGE: Adding TIME_ZONE property. Its value is 'Pacific Standard Time'.
```
observe the text file created

```cmd
dir C:\temp\installed.txt

```
```text
    Directory: C:\temp


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        2/29/2024   1:18 PM              0 installed.txt
```
* To write to Event Log, will need to create the source first. This requires the following command being call with elevated user rights:
```powershell
new-eventlog -source MyCustomEventSource1 -LogName Application
```
the cmdlet [creates](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.management/new-eventlog?view=powershell-5.1) a new classic event log and can also register an event source that writes to the new log or to an existing log


* To remove the custom log source run
```powershell
remove-eventlog -source MyCustomEventSource1
```

* To see the entries run

```powershell
get-eventlog -logname Application -source MyCustomEventSource1 -newest 2| format-list
```
this will print

```text
Index              : 91266
EntryType          : Information
InstanceId         : 701
Message            : The description for Event ID '701' in Source
                     'MyCustomEventSource' cannot be found.  The local
                     computer may not have the necessary registry information
                     or message DLL files to display the message, or you may
                     not have permission to access them.  The following
                     information is part of the event:'Written log file:
                     c:\temp\installed.txt'
Category           : (1)
CategoryNumber     : 1
ReplacementStrings : {Written log file: c:\temp\installed.txt}
Source             : MyCustomEventSource
TimeGenerated      : 4/12/2023 8:51:30 AM
TimeWritten        : 4/12/2023 8:51:30 AM
UserName           :
```

Alternatively use an existing event source e.g. `MsiInstaller` or `EventSystem`

### Uninstall
```powershell
msiexec.exe /l*v a.log /qn /x bin\Debug\Setup.msi
```
confirm the prompt to uninstall the product

### See Also 

  * Creating and referencing a C# custom action __Wix Cookbook__ [book extract](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_6/code/recipe_1/customactioninstaller/CustomActionInstaller)
  * [WIX custom action to prompt user to close applications during install / unintall](https://www.codeproject.com/Articles/584105/Prompt-user-to-close-applications-on-install-unins)
  * long discussion about ask to [use WiX Toolset on Linux](https://github.com/wixtoolset/issues/issues/4381) 
  * [linux equivalent of WIX Installer](https://stackoverflow.com/questions/13290035/linux-equivalent-of-wix-installer-needed) - links to some answers are dead
  * [use Wine to build MSI](https://stackoverflow.com/questions/10240484/build-msi-in-wine)
  * [list of available WIX alternatives for Linux](https://alternativeto.net/software/wix/?platform=linux)(NOTE: none of the applications actually tried)
  * about [run the WiX tools on Linux using Wine and Mono](https://wiki.gnome.org/Projects/GTK/Win32/WiX)
  * [Windows Installer Error Messages (for Developers)](https://bit.ly/msi-error-codes)
  * [Debug system error codes](https://bit.ly/windows-error-codes)
  * [COM Error Codes](https://bit.ly/com-error-codes)
  * [Enable Windows Installer logging](https://support.microsoft.com/kb/223300) - explains the flags in the value `KEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\Installer\Logging`
  * [Windows Installer Microsoft Documentation](https://bit.ly/msidocs)
  * [wine](https://github.com/wine-mirror/wine/tree/master/dlls/msi)
  * [Wix Toolset Sample project with skeleton custom actions](https://github.com/Chanyong-Park/wixtoolsetsample)
  * [Docker Image hosting Wix Toolset on Wine and Debian slim i386](https://github.com/utilitywarehouse/docker-wixtoolset)
   * [powershell commands](https://github.com/icuxika/WiXToolset3Builder) to generate MSI from `.wixobj` using basic Wix build console tools without invoking `msbuild.exe`
   * [kenhys/wixtoolset-examples](https://github.com/kenhys/wixtoolset-examples)
   * [use Group Policy to remotely install software](https://learn.microsoft.com/en-us/troubleshoot/windows-server/group-policy/use-group-policy-to-install-software)
   * [deploy Software using Group Policy](https://activedirectorypro.com/deploy-software-using-group-policy)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
