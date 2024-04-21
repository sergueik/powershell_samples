### Info

this project contains __Installer for Creating a brand new Event Source under Windows event viewer__ example from [chapter 13 recipe 2](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_13/code/recipe_2/eventsourceinstaller/EventSourceInstaller)
from [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book

### Usage


* Build the Test App

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe basic-eventlog-tool.sln
```

* build Package

* update guid, version and other attributes (optional)

```powershell
$name = 'Setup\Product.wxs' 
$xml = [xml](get-content -path $name )
$xml.Normalize()
$guid = [guid]::NewGuid()
$xml.Wix.Product.Id = $guid.ToString()
$xml.Save($name)
```
* NOTE: this operation will convert the XML resource to Windows line endings


* compile the package

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe .\Setup\Setup.wixproj
```
ignore the warning
```text
(Link target) ->
  C:\developer\sergueik\powershell_samples\external\wix\basic-eventlog-source-installer\Setup\Product.wxs(24): warning LGHT1076: ICE69: Mismatched component reference. Entry 'regA097EB236FF6790846B37AC63E0BD38F' of the Registry table belongs to component 'cmpEventSource'. However, the formatted string in column 'Value' references file 'fileMessagesDLL' which belongs to component 'cmpMessagesDLL'. Components are in the same feature. [C:\developer\sergueik\powershell_samples\external\wix\basic-eventlog-source-installer\Setup\Setup.wixproj]
```
the `Setup.msi` will be in `Setup\bin\Debug`.

### Install

in elevated prompt

* install
```cmd
msiexec.exe /l*v a.log /quiet /i Setup\bin\Debug\Setup.msi
```
### confirm


```powershell
$name = 'mycustomlog2' ; 
get-childitem -path HKLM:\SYSTEM\CurrentControlSet\services\eventlog\$name


    Hive:
    HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\mycustomlog2


Name                           Property
----                           --------
MyCustomEventSource            EventMessageFile    : C:\Program Files\EventSourceInstaller\EventLogMessages.dll
                               CategoryMessageFile : C:\Program Files\EventSourceInstaller\EventLogMessages.dll
                               CategoryCount       : 1

```
NOTE: at this time the cmdlet 
```powrshell
get-eventlog -logname $name
```
returns error:
```text
get-eventlog : No matches found
```
To add event log entries run the supplied program as discussed below.
NOTE: the installer causes system level dummy EventLog category message
file resource `c:\Windows\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll`
be copied into its directory.
How to make it set the Registry value to just the path `%windir%\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll` is a WIP


![Applications and Services Event Logs](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-eventlog-applications-and-services.png)

  * observe the freshly updated custom log file:

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
get-eventlog -logname Application -source MyCustomEventSource
```
will raise an error
```text
get-eventlog : No matches found
```
update with command specifying the custom `logname` and add formatting:
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

To restore the Resource directory one will need to sync to [0ee13bb](https://github.com/sergueik/powershell_samples/commit/0ee13bb01eae7f366025e0d0fea5e432fa141463) or earlier commit and do the following:

To build resource dll one needs to author or modify the `mc` file:

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

The following Visual Studio compilers needed to generate the resource-only dlls:

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

There exist equivalent `windmc.exe` and `windres.exe` tools in MinGW environment but those were not tested.

The `EventMessageFile` attribute is required in the `Product.wsx` - removing the attribute leads to build error:
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

The log info will contain warning information about missing description but the intended messages are still logged:

```text
Index              : 6
EntryType          : Information
InstanceId         : 101
Message            : The description for Event ID '100' in Source 'MyCustomEventSource' cannot be found.  The local computer may not have the necessary registry information or message DLL files to display the message, or you may not have permission to access them.  The following information is part of the event: '75 percent'
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
Message            : The description for Event ID '100' in Source 'MyCustomEventSource' cannot be found.  The local computer may not have the necessary registry information or message DLL files to display the message, or you may not have permission to access them.  The following information is part of the event:
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

The message dll `C:\Windows\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll` is actually owned by Microsoft and is part of __Microsoft.NET Framework__ but it accepts message Event ID 1 and 2.
The file size of version __4.0.30319.33440__ is 786KB and of version __4.8.3761.0__ is 785 KB. There is no .Net Assembly Manifest in this dll - it is resource-only

when open in `reshacker` the `EventLogMessages` shows a message table with trivial templates for every Event ID:

![EventLogMessages.dll in reshacker](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-reshacker.png)

indicating it is safe to use with custom logging without explicitly copying a replica when installing event log message file. It is not entirely clear from the Wix Util Extension EventSource Element [documentation](https://wixtoolset.org/docs/v3/xsd/util/eventsource) how to use the %ENVIRONMENT_VARIABLE% syntax to refer to a file already present on the user's machine -  this is a work in progress.

### MSI Log

apparently installer actions

```text
MSI (s) (A8:BC) [17:30:21:724]: Executing op: ActionStart(Name=RemoveRegistryValues,Description=Removing system registry values,Template=Key: [1], Name: [2])
MSI (s) (A8:BC) [17:30:21:724]: Executing op: ProgressTotal(Total=3,Type=1,ByteEquivalent=13200)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: RegOpenKey(Root=-2147483646,Key=SYSTEM\CurrentControlSet\Services\EventLog\mycustomlog2\MyCustomEventSource,,BinaryType=0,,)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: RegRemoveValue(Name=EventMessageFile,Value=#%[#fileMessagesDLL],)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: RegRemoveValue(Name=CategoryMessageFile,Value=#%[#fileMessagesDLL],)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: RegRemoveValue(Name=CategoryCount,Value=#1,)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: ActionStart(Name=RemoveFiles,Description=Removing files,Template=File: [1], Directory: [9])
MSI (s) (A8:BC) [17:30:21:724]: Executing op: ProgressTotal(Total=1,Type=1,ByteEquivalent=175000)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: SetTargetFolder(Folder=C:\Program Files\EventSourceInstaller\)
MSI (s) (A8:BC) [17:30:21:724]: Executing op: FileRemove(,FileName=EventLogMessages.dll,,ComponentId={F868FE8E-8F1E-4AEC-82AE-B5AB012E152F})
MSI (s) (A8:BC) [17:30:21:724]: Verifying accessibility of file: EventLogMessages.dll

```
are wrapped in database transactions

```txt
MSI (s) (A8:BC) [17:30:21:677]: Note: 1: 2228 2:  3: Patch 4: SELECT `Patch`.`File_`, `Patch`.`Header`, `Patch`.`Attributes`, `Patch`.`Sequence`, `Patch`.`StreamRef_` FROM `Patch` WHERE `Patch`.`File_` = ? AND `Patch`.`#_MsiActive`=? ORDER BY `Patch`.`Sequence` 
MSI (s) (A8:BC) [17:30:21:677]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1302 

```
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



### Task Scheduler Default Logging

is very verbose

![Applications and Services Event Logs](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-taskmanager-eventlog.png)

```cmd
wevtutil.exe el| findstr -i tasksch
```
or 
```cmd
wevtutil.exe enum-logs | findstr -i TaskScheduler
```
this will return
```text
Microsoft-Windows-TaskScheduler/Debug
Microsoft-Windows-TaskScheduler/Diagnostic
Microsoft-Windows-TaskScheduler/Operational
```

narrowing down
to `Microsoft-Windows-TaskScheduler/Operational`:
```cmd
wevtutil.exe get-log Microsoft-Windows-TaskScheduler/Operational
name: Microsoft-Windows-TaskScheduler/Operational
enabled: true
type: Operational
owningPublisher: Microsoft-Windows-TaskScheduler
isolation: Application
channelAccess: O:BAG:SYD:(A;;0xf0007;;;SY)(A;;0x7;;;BA)(A;;0x7;;;SO)(A;;0x3;;;IU)(A;;0x3;;;SU)(A;;0x3;;;S-1-5-3)(A;;0x3;;;S-1-5-33)(A;;0x1;;;S-1-5-32-573)
logging:
  logFileName: %SystemRoot%\System32\Winevt\Logs\Microsoft-Windows-TaskScheduler%4Operational.evtx
  retention: false
  autoBackup: false
  maxSize: 1179648
publishing:
  fileMax: 1


```
```cmd
wevtutil.exe query-events  Microsoft-Windows-TaskScheduler/Operational /c:100 /f:text > a.log
```
shows enormous amount of operational events with technical data
```text
Event[97]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-03-21T11:57:37.555
  Event ID: 201
  Task: Action completed
  Level: Information
  Opcode: Stop
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler successfully completed task "\Microsoft\Windows\Windows Error Reporting\QueueReporting" , instance "{4B2A3BA3-DC24-4078-A0DF-20CE6D6AF42D}" , action "C:\Windows\system32\wermgr.exe" with return code 0.

Event[98]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-03-21T11:57:37.555
  Event ID: 102
  Task: Task completed
  Level: Information
  Opcode: Stop
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler successfully finished "{4B2A3BA3-DC24-4078-A0DF-20CE6D6AF42D}" instance of the "\Microsoft\Windows\Windows Error Reporting\QueueReporting" task for user "sergueik42\sergueik".

Event[99]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-03-21T12:06:48.727
  Event ID: 107
  Task: Task triggered on scheduler
  Level: Information
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler launched "{D709EB52-D77F-4F42-997E-4BA33417CFCA}"  instance of task "\Microsoft\Windows\RAC\RacTask" due to a time trigger condition.


```
NOTE: `wevutil.exe` does not offer simple way to specify query arguments):
```text
Read events from an event log, log file or using structured query.

Usage:

wevtutil { qe | query-events } <PATH> [/OPTION:VALUE [/OPTION:VALUE] ...]

<PATH>
By default, you provide a log name for the <PATH> parameter. However, if you use

the /lf option, you must provide the path to a log file for the <PATH> parameter
.
If you use the /sq parameter, you must provide the path to a file containing a
structured query.

Options:

You can use either the short (for example, /f) or long (for example, /format) version of the option names. Options and their values are not case-sensitive.

/{lf | logfile}:[true|false]
If true, <PATH> is the full path to a log file.

/{sq | structuredquery}:[true|false]
If true, <PATH> is the full path to a file that contains a structured query.

/{q | query}:VALUE
VALUE is an XPath query to filter events read. If not specified, all events will be returned. This option is not available when /sq is true.

/{bm | bookmark}:VALUE
VALUE is the full path to a file that contains a bookmark from a previous query.


/{sbm | savebookmark}:VALUE
VALUE is the full path to a file in which to save a bookmark of this query. The file extension should be .xml.

/{rd | reversedirection}:[true|false]
Event read direction. If true, the most recent events are returned first.

/{f | format}:[XML|Text|RenderedXml]
The default value is XML. If Text is specified, prints events in an easy to read text format, rather than in XML format. If RenderedXml, prints events in XML format with rendering information. Note that printing events in Text or RenderedXml formats is slower than printing in XML format.

/{l | locale}:VALUE
VALUE is a locale string to print event text in a specific locale. Only available when printing events in text format using the /f option.

/{c | count}:<n>
Maximum number of events to read.

/{e | element}:VALUE
When outputting event XML, include a root element to produce well-formed XML.
VALUE is the string you want within the root element. For example, specifying
/e:root would result in output XML with the root element pair <root></root>.

```
NOTE: the equivalent Powershell command 
```powershell
get-eventlog -logname 'Microsoft-Windows-TaskScheduler/Operational'
```
is failing with
```
get-eventlog : The event log 'Microsoft-Windows-TaskScheduler/Operational' on computer '.' does not exist.
```
clearing all logs from the Task Scheduler Event Log, and installing a dummy scheduled task using the 
developed Microsoft  Installer letting it run a few rounds
produces
```text
Event[0]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:49:17.011
  Event ID: 310
  Task: Task Engine started
  Level: Information
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler started Task Engine "S-1-5-18:NT AUTHORITY\System:Service:"  process. Command="taskeng.exe" , ProcessID=3764, ThreadID=3176

Event[2]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:49:17.136
  Event ID: 317
  Task: Task Engine started
  Level: Information
  Opcode: Start
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler started Task Engine "S-1-5-18:NT AUTHORITY\System:Service:"  process.


Event[7]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:49:25.370
  Event ID: 106
  Task: Task registered
  Level: Information
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
User "S-1-5-18"  registered Task Scheduler task "\AUTOMATION\ATASK"


Event[9]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:49:30.730
  Event ID: 102
  Task: Task completed
  Level: Information
  Opcode: Stop
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler successfully finished "{FD1E2E8D-4507-46FD-A61D-4F4C48BEA206}" instance of the "\GoogleUpdateTaskMachineUA" task for user "WORKGROUP\SERGUEIK42$".

Event[10]:  
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:49:30.730
  Event ID: 314
  Task: Task Engine idle
  Level: Information
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler has no tasks running for Task Engine "S-1-5-18:NT AUTHORITY\System:Service:" , and the idle timer has started.

Event[11]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:51:00.011
  Event ID: 326
  Task: Launch condition not met, computer on batteries
  Level: Warning
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler did not launch task "\AUTOMATION\ATASK"  because computer is running on batteries. User Action: If launching the task on batteries is required, change the respective flag in the task configuration.

Event[12]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:51:00.011
  Event ID: 101
  Task: Task Start Failed
  Level: Error
  Opcode: Launch Failure
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler failed to start "\AUTOMATION\ATASK" task for user "NT AUTHORITY\System". Additional Data: Error Value: 2147750692.

Event[13]:
  Log Name: Microsoft-Windows-TaskScheduler/Operational
  Source: Microsoft-Windows-TaskScheduler
  Date: 2024-04-08T16:51:00.011
  Event ID: 107
  Task: Task triggered on scheduler
  Level: Information
  Opcode: Info
  Keyword: N/A
  User: S-1-5-18
  User Name: NT AUTHORITY\SYSTEM
  Computer: sergueik42
  Description: 
Task Scheduler launched "{2CB05819-EC0B-4E54-BF45-FA2B6B38E610}"  instance of task "\AUTOMATION\ATASK" due to a time trigger condition.

```
These logs are more than somewhat excessive - the event log is filtered and rendered by __Task Scheduler__ itself, though performance is mediocre

![Applications and Services Event Logs](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-taskscheduler-task-history.png)


### See Also

  * [history](https://en.wikipedia.org/wiki/Windows_Installer)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)



