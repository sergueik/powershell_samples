### Into
This directory contains replica of

origin: [AlexAsplund/PowershellStarter](https://github.com/AlexAsplund/PowershellStarter)

### Usage

This appplication is only work if the  user has persmission to create the service
```powershell
new-service -Name PowershellStarter -DisplayName PowershellStarter -BinaryPathName  (resolve-path PowershellStarter\bin\debug\PowershellStarter.exe) -Description "Powershellstarter service" -StartupType Automatic
```

```powershell
start-service PowershellStarter
```
Still the `PowershellStarter` logs an *error* EventLog message when self-terminating after obsering that the script no longer is running:
```text
Application: PowershellStarter.exe
Framework Version: v4.0.30319
Description: The application requested process termination through System.Environment.FailFast(string message).
Message: Script no longer running, service stopped.
Stack:
   at System.Environment.FailFast(System.String)
   at PowershellStarter.PowershellStarterService.onScriptExited(System.Object, System.EventArgs)
   at System.Diagnostics.Process.RaiseOnExited()
   at System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
   at System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
   at System.Threading._ThreadPoolWaitOrTimerCallback.PerformWaitOrTimerCallback(System.Object, Boolean)
```

NOTE: the example Powershell `c:\temp\pstest.ps1`` has to be console appplication
e.g.

```posweshell
write-output 'test data' | out-file c:\temp\pstest.log -encoding ascii -append
```
when the app is a GUI,
the error says
```text
box or form when the application is not running in UserInteractive mode is not 

```
### See Also
 * [discussion](https://stackoverflow.com/questions/20561990/how-to-solve-the-specified-service-has-been-marked-for-deletion-error)
 of harmless but confusing side effect of removing the Winwows service

*  [creating a Basic Windows Service in C#](https://www.codeproject.com/Articles/14353/Creating-a-Basic-Windows-Service-in-C)

