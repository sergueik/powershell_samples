### Info

this directory contains installer of a set of cross invoking Powershell scripts which are invoked during the product install in one or two ways offered by Wix Toolset
for the side effect (e.g. in this exxamle some information logged to Windows Event Log or custom Scheduled Task is created

based on [chapter_1 Wix Code example](https://resources.oreilly.com/examples/9781784393212/-/blob/master/chapter_1/code/recipe_2/consoleapplicationinstaller/ConsoleApplicationInstaller/Product.wxs) from the book __WiX 3.6: A Developer's Guide to Windows Installer XML__ published by Packt and misc. stackoverflow posts listed at the end.
### Usage

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
msbuild.exe Setup.wixproj
```

* Check that the log is present:
```powershell
wevtutil.exe enum-logs | findstr.exe -i TestLog
```

* Get Log information


```powershell
wevtutil.exe get-log TestLog
```
```text
name: TestLog
enabled: true
type: Admin
owningPublisher:
isolation: Application
channelAccess: O:BAG:SYD:(A;;0xf0007;;;SY)(A;;0x7;;;BA)(A;;0x7;;;SO)(A;;0x3;;;IU)(A;;0x3;;;SU)(A;;0x3;;;S-1-5-3)(A;;0x3;;;S-1-5-33)(A;;0x1;;;S-1-5-32-573)
logging:
  logFileName: %SystemRoot%\System32\Winevt\Logs\TestLog.evtx
  retention: false
  autoBackup: false
  maxSize: 1052672
publishing:
  fileMax: 1


```

* Clear log entries

```powersshell
wevtutil.exe clear-log  TestLog
```

NOTE: `wevtutil.exe` supports abbreviated immemorable aliases like `el`, `gl`, `cl` etc.

#### Install

* in elevated prompt

```cmd
msiexec.exe /l*v a.log /qn /i bin\Debug\Setup.msi
```
#### Successful Install

After the succesful install, check the messages added to the eventlog:
```poweshell
get-eventlog -logname testlog
```
```text

   Index Time          EntryType   Source                 InstanceID Message
   ----- ----          ---------   ------                 ---------- -------
   13555 Mar 07 20:18  Information testlog                         1 message...

```
this eventlog entry is produced by running the powershell script `launcher.ps1` during the install:
```powershell
"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"
```
eventually calling system cmdlet
```powershell
write-eventlog -logname testlog -source testlog -eventid 1 -entrytype  information -message 'message from the script'
```

![add remove programs](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-powershell-run/screenshots/capture-add-remove-programs.png)

The scripts will be installed to 
```text
 Directory of c:\Program Files\Powershell Script Runner

03/08/2024  07:20 AM    <DIR>          .
03/08/2024  07:20 AM    <DIR>          ..
03/08/2024  04:32 AM               196 dependency.ps1
03/08/2024  06:47 AM               215 launcher.ps1
               2 File(s)            411 bytes
```
and removed during uninstall
#### Error Troubleshooting

* examine `a.log` against errors like below and troubleshoot:

```text
MSI (s) (50:5C) [15:05:06:362]: Hello, I'm your 32bit Impersonated custom action server.
WixQuietExec:  Error 0x80070057: Failed to get command line data
WixQuietExec:  Error 0x80070057: Failed to get Command Line
WixQuietExec:  Error 0x80070057: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)
Action ended 15:05:06: InvokeTestPS1. Return value 3.
MSI (s) (50:BC) [15:05:06:377]: Machine policy value 'DisableRollback' is 0

```
```text
MSI (s) (44:20) [04:40:46:051]: Hello, I'm your 32bit Impersonated custom action server.
MSI (s) (44!28) [04:40:46:067]: PROPERTY CHANGE: Deleting WixQuietExecCmdLine property. Its current value is '"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Users\sergueik\AppData\Local\Powershell Script Runner Installer\launcher.ps1" "testlog" "message text"'.
WixQuietExec:  The argument 'C:\Users\sergueik\AppData\Local\Powershell Script Runner Installer\launcher.ps1' to the -File parameter does not exist. Provide the path to an existing '.ps1' file as an argument to the -File parameter.
WixQuietExec:  Windows PowerShell 
WixQuietExec:  Copyright (C) 2016 Microsoft Corporation. All rights reserved.
WixQuietExec:  
WixQuietExec:  Error 0xfffd0000: Command line returned an error.
WixQuietExec:  Error 0xfffd0000: QuietExec Failed
WixQuietExec:  Error 0xfffd0000: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)

```

```text
MSI (s) (08:A8) [06:11:01:630]: PROPERTY CHANGE: Adding WixQuietExecCmdLine property. Its value is 'cmd /k echo "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"'.
WixQuietExec:  Command string must begin with quoted application name.
WixQuietExec:  Error 0x80070057: invalid command line property value
WixQuietExec:  Error 0x80070057: Failed to get Command Line
WixQuietExec:  Error 0x80070057: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)

```

```text
MSI (s) (08:44) [06:17:17:357]: Invoking remote custom action. DLL: C:\Windows\Installer\MSI2111.tmp, Entrypoint: WixQuietExec
MSI (s) (08:C8) [06:17:17:357]: Generating random cookie.
MSI (s) (08:C8) [06:17:17:388]: Created Custom Action Server with PID 2544 (0x9F0).
MSI (s) (08:5C) [06:17:17:481]: Running as a service.
MSI (s) (08:5C) [06:17:17:481]: Hello, I'm your 32bit Impersonated custom action server.
MSI (s) (08!58) [06:17:17:496]: PROPERTY CHANGE: Deleting WixQuietExecCmdLine property. Its current value is '"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"'.
WixQuietExec:  The argument 'C:\Program Files\Powershell Script Runner\launcher.ps1' to the -File parameter does not exist. Provide the path to an existing '.ps1' file as an argument to the -File parameter.
WixQuietExec:  Windows PowerShell 
WixQuietExec:  Copyright (C) 2016 Microsoft Corporation. All rights reserved.
WixQuietExec:  
WixQuietExec:  Error 0xfffd0000: Command line returned an error.
WixQuietExec:  Error 0xfffd0000: QuietExec Failed
WixQuietExec:  Error 0xfffd0000: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)

```
```text
MSI (s) (B8:A0) [08:29:03:201]: Hello, I'm your 32bit Impersonated custom action server.
MSI (s) (B8!3C) [08:29:03:201]: PROPERTY CHANGE: Deleting WixQuietExecCmdLine property. Its current value is '"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"'.
WixQuietExec:  . : The term '.\dependency.ps1' is not recognized as the name of a cmdlet, 
WixQuietExec:  function, script file, or operable program. Check the spelling of the name, or 
WixQuietExec:  if a path was included, verify that the path is correct and try again.
WixQuietExec:  At C:\Program Files\Powershell Script Runner\launcher.ps1:8 char:3
WixQuietExec:  + . .\dependency.ps1 -message $message -logname $logname
WixQuietExec:  +   ~~~~~~~~~~~~~~~~
WixQuietExec:      + CategoryInfo          : ObjectNotFound: (.\dependency.ps1:String) , Co 
WixQuietExec:     mmandNotFoundException
WixQuietExec:      + FullyQualifiedErrorId : CommandNotFoundException
WixQuietExec:   
Action ended 8:29:03: InvokeTestPS1. Return value 1.
Action ended 8:29:03: INSTALL. Return value 1.
Property(S): UpgradeCode = {0105D0B1-94A7-456F-8D01-C8767596625B}
Property(S): POWERSHELLEXE = C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe
Property(S): INSTALLFOLDER = C:\Program Files\Powershell Script Runner\
Property(S): ProgramFilesFolder = C:\Program Files\
Property(S): TARGETDIR = C:\

```

```text
MSI (s) (B8:08) [08:35:41:604]: Hello, I'm your 32bit Impersonated custom action server.
MSI (s) (B8!74) [08:35:41:604]: PROPERTY CHANGE: Deleting WixQuietExecCmdLine property. Its current value is '"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"'.
WixQuietExec:  . : The term 'C:\Windows\system32\dependency.ps1' is not recognized as the 
WixQuietExec:  name of a cmdlet, function, script file, or operable program. Check the 
WixQuietExec:  spelling of the name, or if a path was included, verify that the path is 
WixQuietExec:  correct and try again.
WixQuietExec:  At C:\Program Files\Powershell Script Runner\launcher.ps1:9 char:3
WixQuietExec:  + . ((resolve-path -path '.').path + '\' + 'dependency.ps1') -message $ ...
WixQuietExec:  +   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
WixQuietExec:      + CategoryInfo          : ObjectNotFound: (C:\Windows\system32\dependency. 
WixQuietExec:     ps1:String) , CommandNotFoundException
WixQuietExec:      + FullyQualifiedErrorId : CommandNotFoundException
WixQuietExec:   
Action ended 8:35:41: InvokeTestPS1. Return value 1.
Action ended 8:35:41: INSTALL. Return value 1.
Property(S): UpgradeCode = {0105D0B1-94A7-456F-8D01-C8767596625B}

```

```text
WixQuietExec:  . : The term 'C:\Program Files\Powershell Script 
WixQuietExec:  Runner\launcher.ps1\dependency.ps1' is not recognized as the name of a cmdlet, 
WixQuietExec:  function, script file, or operable program. Check the spelling of the name, or 
WixQuietExec:  if a path was included, verify that the path is correct and try again.
WixQuietExec:  At C:\Program Files\Powershell Script Runner\launcher.ps1:10 char:3
WixQuietExec:  + . ($script_path + '\' + 'dependency.ps1') -message $message -logname  ...
WixQuietExec:  +   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
WixQuietExec:      + CategoryInfo          : ObjectNotFound: (C:\Program File...\dependency.p 
WixQuietExec:     s1:String) , CommandNotFoundException
WixQuietExec:      + FullyQualifiedErrorId : CommandNotFoundException

```
```text

MSI (s) (E0:AC) [13:06:52:271]: Hello, I'm your 32bit Impersonated custom action server.
MSI (s) (E0!54) [13:06:52:271]: PROPERTY CHANGE: Deleting WixQuietExecCmdLine property. Its current value is '"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -noprofile -noninteractive -file "C:\Program Files\Powershell Script Runner\launcher.ps1" "testlog" "message text"'.
Action start 13:06:52: InvokeTestPS1.
WixQuietExec:  C:\Program Files\Powershell Script Runner\launcher.ps1 : Cannot bind 
WixQuietExec:  positional parameters because no names were given.
WixQuietExec:      + CategoryInfo          : InvalidArgument: (:) , ParentConta 
WixQuietExec:     insErrorRecordException
WixQuietExec:      + FullyQualifiedErrorId : AmbiguousPositionalParameterNoName,launcher.ps1
WixQuietExec:   
WixQuietExec:  Error 0x80070001: Command line returned an error.
WixQuietExec:  Error 0x80070001: QuietExec Failed
WixQuietExec:  Error 0x80070001: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)

```
### Cleanup

```powershell
msiexec.exe /l*v a.log /qn /x bin\Debug\Setup.msi
```
### See Also

  * https://wixtoolset.org/docs/tools/wixext/quietexec/
  * https://stackoverflow.com/questions/6044069/how-to-execute-wix-custom-action-after-installation
  * https://github.com/wixtoolset/issues/issues/1265
  * https://wixtoolset.org/docs/tools/wixexe/   
  * https://stackoverflow.com/questions/27499072/split-a-folder-path-and-file-name
   * https://wixtoolset.org/docs/v3/customactions/shellexec/
   * https://wixtoolset.org/docs/v3/customactions/qtexec/
   * https://wixtoolset.org/docs/tools/wixext/quietexec/
   * https://stackoverflow.com/questions/31305483/switches-in-wixshellexectarget

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
