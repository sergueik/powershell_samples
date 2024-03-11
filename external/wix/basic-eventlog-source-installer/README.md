### Info

this project contains __Installer for Creating a brand new Event Source under Windows event viewer__ example from [chapter 13 recipe 2](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_13/code/recipe_2/eventsourceinstaller/EventSourceInstaller)
from [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book

### Usage


#### Build the Test App

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe .\Program\Program.csproj
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
$xml.Save($name)
```
* NOTE: will switch to Windows line endings


* compile the package

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe .\Setup\Setup.wixproj
```
the `Setup.msi` will be in `Setup\bin\Debug`.

### Install

in elevated prompt

* install
```cmd
msiexec.exe /l*v a.log /quiet /i Setup\bin\Debug\Setup.msi
```
### confirm

![Applications and Services Event Logs](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-eventlog-applications-and-services.png)

  * observe the custom log file:

```text
c:\Windows\System32\winevt\Logs\mycustomlog2.evtx
```

```cmd
del c:\Windows\System32\winevt\Logs\mycustomlog2.evtx
```
```text
c:\Windows\System32\winevt\Logs\mycustomlog2.evtx
The process cannot access the file because it is being used by another process.
```
  * check the log inventory

```cmd
wevtutil.exe get-log mycustomlog2
```
```text
name: mycustomlog2
enabled: true
type: Admin
owningPublisher:
isolation: Application
channelAccess: O:BAG:SYD:(A;;0xf0007;;;SY)(A;;0x7;;;BA)(A;;0x7;;;SO)(A;;0x3;;;IU)(A;;0x3;;;SU)(A;;0x3;;;S-1-5-3)(A;;0x3;;;S-1-5-33)(A;;0x1;;;S-1-5-32-573)
logging:
  logFileName: %SystemRoot%\System32\Winevt\Logs\mycustomlog2.evtx
  retention: false
  autoBackup: false
  maxSize: 1052672
publishing:
  fileMax: 1
```

after the project uninstalled will see
```text
Failed to read configuration for log mycustomlog2. The specified channel could not be found. Check channel configuration.
```
the file `c:\Windows\System32\winevt\Logs\mycustomlog2.evtx` may remain if there were logs added

```cmd
wevtutil.exe get-publisher MyCustomEventSource
```
```text
name: MyCustomEventSource
guid: 00000000-0000-0000-0000-000000000000
helpLink: http://go.microsoft.com/fwlink/events.asp?CoName=Microsoft%20Corporation&ProdName=Microsoft%c2%ae%20.NET%20Framework&ProdVer=4.0.30319.0&FileName=EventLogMessages.dll&FileVer=4.8.3761.0
messageFileName: C:\Program Files\EventSourceInstaller\EventLogMessages.dll
message:
channels:
  channel:
    name: mycustomlog2
    id: 16
    flags: 1
    message:
levels:
opcodes:
tasks:
  task:
    name: %1

    value: 1
    eventGUID: 00000000-0000-0000-0000-000000000000
    message: 1
keywords:
```


  * generate new log events

```cmd
Program\bin\Debug\EventSourceTestApp.exe
```
  * repeat a few times	```
  * check the logs have been added

```powershell
get-eventlog -logname Application -source MyCustomEventSource -newest 2| format-list
```
will return
```text
get-eventlog : No matches found
```
update with relevant `logname`:
```powershell
get-eventlog -logname mycustomlog2 -source MyCustomEventSource -newest 2| format-list
```


```text
Index              : 14
EntryType          : Information
InstanceId         : 1
Message            : some information was logged
Category           : %1
CategoryNumber     : 1
ReplacementStrings : {some information was logged}
Source             : MyCustomEventSource
TimeGenerated      : 3/11/2024 7:19:49 AM
TimeWritten        : 3/11/2024 7:19:49 AM
UserName           :

Index              : 13
EntryType          : Error
InstanceId         : 2
Message            : there was an error logged
Category           : %1
CategoryNumber     : 1
ReplacementStrings : {there was an error logged}
Source             : MyCustomEventSource
TimeGenerated      : 3/11/2024 7:19:49 AM
TimeWritten        : 3/11/2024 7:19:49 AM
UserName           :
```
(the `Index` will be different)

![Custom Event Log](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-custom-eventlog.png)

#### Build Stub Message Resource Dll from Source (Optional)

To build resource dll one needs to author a `mc` file:

```c
;// HEADER SECTION
MessageIdTypedef=DWORD
;// CATEGORY DEFINITIONS
;// MESSAGE DEFINITIONS
MessageId=101
Language=English
Service utilization: %1
.

```

The compilers needed to generaete the resource-only dlls:

```cmd
mc.exe -u %RESOURCE_FILENAME%.mc
rc.exe -r -fo %RESOURCE_FILENAME%.res %RESOURCE_FILENAME%.rc
link.exe -dll -noentry -out:%RESOURCE_FILENAME%.dll %RESOURCE_FILENAME%.res /MACHINE:X86
```
are part of Visual Studio (`link.exe`) and Windows SDK (`rc.exe`, `mc.exe`	) - these tools are paltform and Windows release specific:
```
c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\amd64\link.exe	c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\amd64\link.exe
c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\x86_amd64\link.exe	c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\x86_amd64\link.exe
c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\x86_arm\link.exe	c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\x86_arm\link.exe
```
```text
c:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\MC.Exe
c:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\x64\MC.Exe
c:\Program Files (x86)\Windows Kits\8.0\bin\x64\mc.exe
c:\Program Files (x86)\Windows Kits\8.0\bin\x86\mc.exe
c:\Program Files (x86)\Windows Kits\8.1\bin\x64\mc.exe
```
```text
c:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\RC.Exe
c:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\x64\RC.Exe
c:\Program Files (x86)\Windows Kits\8.0\bin\x64\rc.exe
c:\Program Files (x86)\Windows Kits\8.0\bin\x86\rc.exe
c:\Program Files (x86)\Windows Kits\8.1\bin\x64\rc.exe
```

The `EventMessageFile` attribute is required - removing the attribute leads to build error:
```text
CNDL0010: The util:EventSource/@EventMessageFile attribute was not found; it is required.` ) and can not be assigned a blank value (doing that leads to build error: `CNDL0006: The util:EventSource/@EventMessageFile attribute's value cannot be an empty string.  
If a value is not required, simply remove the entire attribute
```

The only working workaround is to supply a blank file
```cmd
copy NUL messages.dll
```
this will lead to a warning `LGHT1076: ICE71: The Media table has no entries` at build time, but the installer will be functional: it will create orremove log event resource on install/uninstall.

If the stub `messages.dll` is used when creating the installer, the logs end up being written to `Application` log (unverified).

The log info show that something is missing but something is still logged:

```text
Index              : 6
EntryType          : Information
InstanceId         : 101
Message            : The description for Event ID '101' in Source
                     'MyCustomEventSource' cannot be found.  The local
                     computer may not have the necessary registry information
                     or message DLL files to display the message, or you may
                     not have permission to access them.  The following
                     information is part of the event:'75 percent'
Category           : (1)
CategoryNumber     : 1
ReplacementStrings : {75 percent}
Source             : MyCustomEventSource
TimeGenerated      : 3/15/2023 4:18:18 PM
TimeWritten        : 3/15/2023 4:18:18 PM
UserName           :

Index              : 5
EntryType          : Error
InstanceId         : 100
Message            : The description for Event ID '100' in Source
                     'MyCustomEventSource' cannot be found.  The local
                     computer may not have the necessary registry information
                     or message DLL files to display the message, or you may
                     not have permission to access them.  The following
                     information is part of the event:
Category           : (1)
CategoryNumber     : 1
ReplacementStrings : {}
Source             : MyCustomEventSource
TimeGenerated      : 3/15/2023 4:18:18 PM
TimeWritten        : 3/15/2023 4:18:18 PM
UserName           :

```
Also there was an error observed during uninstall:

![Errror in Uninstall](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-error-uninstall.png)
### Note

if instead of git clone, a zip of the project sources was downloaded, start with unlocking the files

```powershell
get-childitem . - file -recurse | foreach-objecg { 
  unlock-file -path. $_.fullname 
} 
```

### TestLog

in another project the custom log named `TestLog` was successfully created and messages with Event ID  1,2,3 have been logged


```powershell
wevtutil.exe enum-publishers | findstr -i testlog
```
```powershell
wevtutil.exe get-publisher TestLog
```
NOTE: `wevtutil.exe` supports abbreviated immemorable aliases like `gp` `ep` etc.
```text
name: TestLog
guid: 00000000-0000-0000-0000-000000000000
helpLink: http://go.microsoft.com/fwlink/events.asp?CoName=Microsoft%20Corporation&ProdName=Microsoft%c2%ae%20.NET%20Framework&ProdVer=4.0.30319.0&FileName=EventLogMessages.dll&FileVer=4.8.3761.0
messageFileName: C:\Windows\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll
message:
channels:
  channel:
    name: TestLog
    id: 16
    flags: 1
    message:
levels:
opcodes:
tasks:
keywords:

```

The message dll  `C:\Windows\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll` is actually owned by Microsoft and is part of __Microsoft.NET Framework__ but it accepts message Event ID 1 and 2.
The file size of version __4.0.30319.33440__ is 786KB and of version __4.8.3761.0__ is 785 KB. There is no .Net Assembly Manifest in this dll - it is resource-only

when open in `reshacker` the `EventLogMessages` shows a message table with trivial templates for every Event ID:

![EventLogMessages.dll in reshacker](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-reshacker.png)

indicating it is safe to use with custom logging without explicitly copying a replica when installing event log message file. It is not entirely clear from the Wix Util Extension EventSource Element [documentation](https://wixtoolset.org/docs/v3/xsd/util/eventsource) how to use the %ENVIRONMENT_VARIABLE% syntax to refer to a file already present on the user's machine -  this is a work in progress.

### See Also

   * `EventSource` element (Wix Toolset 3.x Util Extension) [documentation](https://wixtoolset.org/docs/v3/xsd/util/eventsource/)
   * `EventManifest` element (Wix Toolset 3.x Util Extension) [documentation](https://wixtoolset.org/docs/v3/xsd/util/eventmanifest/)
   * [create a Custom Event Log in the Event Viewer through Wix Toolset](https://bzzzt.io/post/2013-06/2013-06-05-wix-wednesday-1-4-create-a-custom-event-log-in-the-event-viewer/)
   * [discussion on compiling a small app to produce an event log source using WiX](https://itecnote.com/tecnote/r-how-to-create-an-event-log-source-using-wix/)
   * [how to create a .NET event log source using WiX](https://itecnote.com/tecnote/how-to-create-a-net-event-log-source-using-wix/) 
   * [creating a resource-only DLL](https://learn.microsoft.com/en-us/cpp/build/creating-a-resource-only-dll)
   * [build Resource-only DLL in Visual Studio 2015](https://www.originlab.com/doc/OriginC/odk/Build-Resource-only-DLL-in-Visual-Studio-2015)
   * [creating a Resource-Only DLL](https://www.codeproject.com/Tips/5349617/Creating-a-Resource-Only-DLL)
   * [EventLog and resource-only DLL orchestration using](https://www.codeproject.com/Articles/9889/EventLog-and-resource-only-DLL-orchestration) coveging cresating custom resource-only dll using message text file (.mc), resouece compiler, message compiler and linker and referencing the `EventLogMessages.dll` ( which existed in __Microsoft.NET Framework__  __1.1.4322__ too)
   * [Message compiler](http://msdn.microsoft.com/library/default.asp?url=/library/en-us/tools/tools/message_compiler.asp)
   * [Resource Compiler](http://msdn.microsoft.com/library/default.asp?url=/library/en-us/tools/tools/resource_compiler.asp)
   * __Resource Hacker__  a freeware resource compiler & decompiler for Windows applications [download](https://www.angusj.com/resourcehacker/) 
   * MinGW has [windmc.exe](https://manpages.debian.org/testing/binutils-mingw-w64-x86-64/x86_64-w64-mingw32-windmc.1.en.html) and [windres.exe](https://manpages.debian.org/testing/binutils-mingw-w64-x86-64/x86_64-w64-mingw32-windres.1.en.html) for same purposes
   * https://stackoverflow.com/questions/74273900/is-there-a-way-to-create-a-resource-only-dll-using-mingw-compiler-tools

   * https://stackoverflow.com/questions/49508242/how-to-compile-resource-file-with-mingw-windres





### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)



