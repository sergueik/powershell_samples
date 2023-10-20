### Info


this directory contains code from [codeproject article](https://www.codeproject.com/Articles/511653/Using-WIX-With-Managed-Custom-Action)
updated to run per-user install

In addition to writing to log file,for which the user running the isntaller may not have sufficient access permissions, the custom action creates an event log entry.
### Usage

#### Build the Test App

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
get-eventlog -logname Application -source MyCustomEventSource -newest 2| format-list
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
### See Also 
  * [WIX custom action to prompt user to close applications during install / unintall](https://www.codeproject.com/Articles/584105/Prompt-user-to-close-applications-on-install-unins)
  * long discussion about ask to [use WiX Toolset on Linux](https://github.com/wixtoolset/issues/issues/4381) 
  * [linux equivalent of WIX Installer](https://stackoverflow.com/questions/13290035/linux-equivalent-of-wix-installer-needed) - links to some answers are dead
  * [use Wine to build MSI](https://stackoverflow.com/questions/10240484/build-msi-in-wine)
  * [list of available WIX alternatives for Linux](https://alternativeto.net/software/wix/?platform=linux)(NOTE: none of the applications actually tried)
  * about [run the WiX tools on Linux using Wine and Mono](https://wiki.gnome.org/Projects/GTK/Win32/WiX)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
