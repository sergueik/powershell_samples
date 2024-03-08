### Info

https://resources.oreilly.com/examples/9781784393212/-/blob/master/chapter_1/code/recipe_2/consoleapplicationinstaller/ConsoleApplicationInstaller/Product.wxs
### Usage

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe Setup.wixproj
```
#### Install

* in regular prompt

```cmd
msiexec.exe /l*v a.log /qn /i bin\Debug\Setup.msi
```


* exampine `a.log` against errors like below and troubleshoot:

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
### Cleanup

```powershell
msiexec.exe /l*v a.log /qn /x bin\Debug\Setup.msi
```
### See Also

  * https://wixtoolset.org/docs/tools/wixext/quietexec/
  * https://stackoverflow.com/questions/6044069/how-to-execute-wix-custom-action-after-installation
   
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
