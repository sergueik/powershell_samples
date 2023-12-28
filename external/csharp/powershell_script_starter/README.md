### Into
This directory contains replica of

origin: [AlexAsplund/PowershellStarter](https://github.com/AlexAsplund/PowershellStarter)

### Usage

* create Powershell script for PowershellStarter to launch `c:\temp\pstest.ps1`

* install PowershellStarter service
NOTE: This appplication is only work if the  user has persmission to create the service

```powershell
new-service -Name PowershellStarter -DisplayName PowershellStarter -BinaryPathName  (resolve-path PowershellStarter\bin\debug\PowershellStarter.exe) -Description "Powershellstarter service" -StartupType Automatic
```

```powershell
get-service -name PowershellStarter
```
```text
Status   Name               DisplayName
------   ----               -----------
Stopped  PowershellStarter  PowershellStarter
```


```powershell
start-service PowershellStarter
```
```cmd
dir c:\temp\pstest.log
```
```text
```
NOTE: `PowershellStarter` logs an *error* EventLog message when self-terminating after obsering that the script no longer is running:
```text
Application: PowershellStarter.exe
Framework Version: v4.0.30319
Description: The application requested process termination through
System.Environment.FailFast(string message).
Message: Script no longer running, service stopped.
Stack:
   at System.Environment.FailFast(System.String)
   at PowershellStarter.PowershellStarterService.onScriptExited(System.Object, System.EventArgs)
   at System.Diagnostics.Process.OnExited()
   at System.Diagnostics.Process.RaiseOnExited()
   at System.Diagnostics.Process.CompletionCallback(System.Object, Boolean)
   at System.Threading._ThreadPoolWaitOrTimerCallback.WaitOrTimerCallback_Context(System.Object, Boolean)
```

and an errror
```text
Faulting application name: PowershellStarter.exe, version: 1.0.0.0, time stamp: 0x658deb99
Faulting module name: unknown, version: 0.0.0.0, time stamp: 0x00000000
Exception code: 0x80131623
Fault offset: 0x00431802
Faulting process id: 0x654
Faulting application start time: 0x01da39d7fbbc460e
Faulting application path: C:\developer\sergueik\powershell_samples\external\csharp\powershell_script_starter\PowershellStarter\bin\debug\PowershellStarter.exe
Faulting module path: unknown
Report Id: 462fec40-a5cb-11ee-9386-08002783cd0a


```

the error code `0x80131623` means simply 
```text
Runtime operation halted by call to System.Environment.FailFast()
```

there will be Windows Error Reporting event logs created because of the way the service termiates itself.

NOTE: the example Powershell `c:\temp\pstest.ps1` has to be a console appplication
e.g.

```posweshell
write-output 'test data' | out-file c:\temp\pstest.log -encoding ascii -append
```
when the Powershell script is a GUI app,
the error says
```text
box or form when the application is not running in UserInteractive mode is not 

```
### Cleanup
```cmd
sc.exe delete PowershellStarter
```
* NOTE: the `Remove-Service` cmdlet was introduced in PowerShell 6.0.

### See Also

  * [discussion](https://stackoverflow.com/questions/20561990/how-to-solve-the-specified-service-has-been-marked-for-deletion-error) of harmless but confusing side effect of removing the Windows service
  * https://learn.microsoft.com/en-us/dotnet/api/system.environment.failfast?view=netframework-4.5 
  * [creating a Basic Windows Service in C#](https://www.codeproject.com/Articles/14353/Creating-a-Basic-Windows-Service-in-C)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

