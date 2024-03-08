### Usage

```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319"
MSBuild.exe Setup.wixproj
```
#### Install

* in regular prompt

```cmd
msiexec.exe /l*v a.log /i bin\Debug\Setup.msi
```


* exampine `a.log` against errors like:

```text
MSI (s) (50:5C) [15:05:06:362]: Hello, I'm your 32bit Impersonated custom action server.
WixQuietExec:  Error 0x80070057: Failed to get command line data
WixQuietExec:  Error 0x80070057: Failed to get Command Line
WixQuietExec:  Error 0x80070057: Failed in ExecCommon method
CustomAction InvokeTestPS1 returned actual error code 1603 (note this may not be 100% accurate if translation happened inside sandbox)
Action ended 15:05:06: InvokeTestPS1. Return value 3.
MSI (s) (50:BC) [15:05:06:377]: Machine policy value 'DisableRollback' is 0

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

this eventlog entry is produced by running the powershell script `test.ps1` during the install:
```powershell
write-eventlog -logname testlog -source testlog -eventid 1 -entrytype  information -message 'message from the script'
```
### See Also

  * https://wixtoolset.org/docs/tools/wixext/quietexec/


### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
