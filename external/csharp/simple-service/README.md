
### Info
https://github.com/Darkseal/WindowsService.NET

### Usage
* remove custom log file
```cmd
del /q Program\bin\debug\WindowsService.NET_*.txt
```
* build the complex project, all the"utility" dlls will be in the same folder with the main program executable:
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -uninstall Program\bin\Debug\WindowsService.NET.exe
```
* remove the custom Event Log (in __Administrator:Window Powershell__ console:
```powershell
remove-eventlog  -logname TestLog
```
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -install Program\bin\Debug\WindowsService.NET.exe
```

You may  need to start service and then restart it to see it write log entries to custom  event log `TestLog`.

![creation of Event Log](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/capture_creation_eventlog.png)

```cmd
type Program\bin\Debug\WindowsService.NET_*.txt
```
- the log creation may need to me moved from the constructor of `Service1` class to event handler.

* to use interactive user credentials add the following (see [also](https://www.aspsnippets.com/Articles/Install-Windows-Service-silently-without-entering-Username-and-Password-using-InstallUtilexe-from-Command-Prompt-Line.aspx)):

```cmd
set PASSWORD=....
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe  -uninstall Program\bin\Debug\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% /unattend Program\bin\Debug\WindowsService.NET.exe
```
this does not appear to work. When credentials are incorrect the process is aborted however when the password is valid, it is logged as if it was successful:
```text
Creating EventLog source WindowsService.NET in log Application...

The Install phase completed successfully, and the Commit phase is beginning.
See the contents of the log file for the c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe assembly's progress.
The file is located at c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.InstallLog.
Committing assembly 'c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe'.
Affected parameters are:
   logfile = c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.InstallLog
   username = sergueik42\sergueik
   assemblypath = c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe
   install =
   logtoconsole =
   password = ********
```
but the service remains assigned to Local System account
```text
get-service -name 'WindowsService.NET'|select-object -property *


Name                : WindowsService.NET
RequiredServices    : {}
CanPauseAndContinue : False
CanShutdown         : False
CanStop             : False
DisplayName         : WindowsService.NET
DependentServices   : {}
MachineName         : .
ServiceName         : WindowsService.NET
ServicesDependedOn  : {}
ServiceHandle       :
Status              : Stopped
ServiceType         : Win32OwnProcess
Site                :
Container           :
```
```
$servicename = 'WindowsService.NET'
get-wmiobject win32_service -filter "name='$($servicename)'" | format-list *

get-wmiobject win32_service -filter "name='$($servicename)'" |select-object -property startname

startname
---------
LocalSystem
```
- see [also](https://devblogs.microsoft.com/scripting/the-scripting-wife-uses-powershell-to-find-service-accounts/)

```
get-itemproperty -name 'objectname' -path "HKLM:\System\CurrentControlSet\Services\${ServiceName}"
this will show the user

```
to set password?
- use
```powershell
$domain = $env:USERDOMAIN
$username = $env:USERNAME	
$user = "${domain}\${username}"
$credential = Get-Credential -username $user  -message  ('Enter password for {0}, please' -f $username   )
set-service -name $servicename -Credential $credential
```
NOTE:  according to documentation this parameter is supported by Powershell [7.1](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/set-service?view=powershell-7.1)
but not [5.1](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/set-service?view=powershell-5.1)

in case of success the following will be logged  (among other things):

```cmd
Installing service WindowsService.NET...
Service WindowsService.NET has been successfully installed.
Creating EventLog source WindowsService.NET in log Application...
```

```cmd
sc.exe start WindowsService.NET
```
will print
```text
SERVICE_NAME: WindowsService.NET
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 2  START_PENDING
                                (NOT_STOPPABLE, NOT_PAUSABLE, IGNORES_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x7d0
        PID                : 520
        FLAGS              :
```
confirm
```cmd
sc.exe query WindowsService.NET
```

will print
```text
SERVICE_NAME: WindowsService.NET
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0

```

stop
```cmd
sc.exe start WindowsService.NET
```
this will print
```text
SERVICE_NAME: WindowsService.NET
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 3  STOP_PENDING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```

### Signing the Script

* follow the [tutorial](https://www.youtube.com/watch?v=_I5TcWTHo8g)
* in an elevated powershell prompt
```powershell
cd cert:
cd currentuser\my
new-selfsignedCertificate -subject "E=serguei,CN=WellsFargo,CN=wellsfargo" -textExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3") -keyalgorithm RSA -keylength 2048 -friendlyname "Borg Key"  -notAfter 01-01-2022
```
(update the subject and expiration date as neded)
this will lead to creation of the file:

```powershell
PSParentPath: Microsoft.PowerShell.Security\Certificate::CurrentUser\my

Thumbprint                                Subject
----------                                -------
AC951DCD45903B3F000B000BDD811CE0CFC70563  E=serguei, CN=WellsFargo, CN=wells...

```
* navigate to the `SystemCertificates` directory to cofirm
```cmd
cd %USERPPROFILE%\AppData\Roaming\Microsoft\SystemCertificates\My\Certificates
```
note: the directories and files in the above path are hidden
observe the same thumbprint hash:
```cmd
attrib *
```
```text
A  S    I            C:\Users\sergueik\AppData\Roaming\Microsoft\SystemCertificates\My\Certificates\AC951DCD45903B3F000B000BDD811CE0CFC70563
```
* examine the certificate
```powershell
cd cert:
cd currentuser\my
$cert = get-childitem -path 'AC951DCD45903B3F000B000BDD811CE0CFC70563'
$cert.enhancedkeyUsagelist
```
will display
```text
FriendlyName ObjectId
------------ --------
Code Signing 1.3.6.1.5.5.7.3.3
```

NOTE: do not use full physical drive path:
```powershell
$cert_path = 'C:\Users\sergueik\AppData\Roaming\Microsoft\SystemCertificates\My\Certificates\AC951DCD45903B3F000B000BDD811CE0CFC70563'
$cert = get-childitem -path $cert_path
```
this way the `enhancedkeyUsagelist` property will be empty:

```text
$cert.enhancedkeyUsagelist
```
will show nothing

NOTE: these cmdlets are Windows-version specific.


* Authenticode sign the script (does not need an elevated powershell prompt):
```powershell
cd cert:
cd currentuser\my
$cert_path = 'AC951DCD45903B3F000B000BDD811CE0CFC70563'
$cert = get-childitem -path $cert_path
set-authenticodesignature -certificate:$cert -filepath C:\developer\sergueik\work_script.ps1
```
* the script willl get modified with the signature:

```powershell
# SIG # Begin signature block
# MIIF5QYJKoZIhvcNAQcCoIIF1jCCBdICAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
...
```
NOTE: on Windows 7 machine, the certificates [created via IIS Site Manager](https://help.zenoti.com/en/articles/3692291-worldpay-create-a-self-signed-certificate-on-windows-7-pc),'create a self-signed cerfificate', that are placed into `cert:\LocalMachine\My`, are not valid for code signing.
One needs to use `makecert.exe` for [creating those certificates](https://community.spiceworks.com/how_to/122368-windows-7-signing-a-powershell-script-with-a-self-signed-certificate)
via command like below
```cmd
makecert.exe -n CN=PowerShell Local Certificate Root -a sha1 -eku 1.3.6.1.5.5.7.3.3 -r -sv root.pvk root.cer -ss Root -sr localMachine
```

* update the scheduled task action command `executionpolicy` argument:

```txt
-executionpolicy allsigned -noprofile -file "C:\developer\sergueik\work_script.ps1" -send -datafile "C:\developer\sergueik\data.txt"
```

run the script interactively one time:
from cmd window to imitate the Task Scheduler:
```cmd
powershell.exe -executionpolicy allsigned -noprofile -file "C:\developer\sergueik\work_script.ps1" -send -datafile "C:\developer\sergueik\data.txt"
```

it will prompt for confirmation:
```txt

Do you want to run software from this untrusted publisher?
File C:\developer\sergueik\work_script.ps1 is published by E=serguei,
CN=WellsFargo, CN=wellsfargo and is not trusted on your system. Only run
scripts from trusted publishers.
[V] Never run  [D] Do not run  [R] Run once  [A] Always run  [?] Help
(default is "D"):
```

confirm with 'A'.
* this at least was needed after failing attempt to immediately run the just signed code under `Local System` account
One can still see the error in running the scheduled task:

```txt
Task Scheduler successfully completed task "\automation\example_task_service" , instance "{ff47d502-9189-4ddd-a752-71572dfa5a9b}" , action "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" with return code 2147942401.
```
which is

```txt
Inspecting error code: 2147942401
Incorrect function. (Exception from HRESULT: 0x80070001)
```

when task is executed under `Local System` account but the script is authenticode signed with interacive user certificate. When the scheduled tasl is run as the same interactie user, everything works

### Note
Check the home-brewed plaintext log written by service e.g.

```text
Program\bin\Debug\WindowsService.NET_20220122.txt
```
You may have to close `services.msc` which is displaying the service if reports failure to uninstall or install. Such errors are observed occasionally

```text
[12:15:14] - Average: 1,55172413793103
[12:15:14] - procesing Timer1TimeStamp=25 Feb2022 12:15:14, Value=3
[12:15:15] - procesing Timer1TimeStamp=25 Feb2022 12:15:15, Value=2
[12:15:16] - procesing Timer1TimeStamp=25 Feb2022 12:15:16, Value=2
[12:15:17] - procesing Timer1TimeStamp=25 Feb2022 12:15:17, Value=3
[12:15:18] - procesing Timer1TimeStamp=25 Feb2022 12:15:18, Value=2
[12:15:19] - procesing Timer1TimeStamp=25 Feb2022 12:15:19, Value=2
[12:15:20] - procesing Timer1TimeStamp=25 Feb2022 12:15:20, Value=2
[12:15:21] - procesing Timer1TimeStamp=25 Feb2022 12:15:21, Value=3
[12:15:22] - procesing Timer1TimeStamp=25 Feb2022 12:15:22, Value=5
[12:15:23] - procesing Timer1TimeStamp=25 Feb2022 12:15:23, Value=1
[12:15:24] - procesing Timer1TimeStamp=25 Feb2022 12:15:24, Value=1
[12:15:25] - procesing Timer1TimeStamp=25 Feb2022 12:15:25, Value=1
[12:15:26] - procesing Timer1TimeStamp=25 Feb2022 12:15:26, Value=1
[12:15:27] - procesing Timer1TimeStamp=25 Feb2022 12:15:27, Value=1
[12:15:28] - procesing Timer1TimeStamp=25 Feb2022 12:15:28, Value=1
[12:15:29] - procesing Timer1TimeStamp=25 Feb2022 12:15:29, Value=1
[12:15:30] - procesing Timer1TimeStamp=25 Feb2022 12:15:30, Value=1
[12:15:31] - procesing Timer1TimeStamp=25 Feb2022 12:15:31, Value=1
[12:15:32] - procesing Timer1TimeStamp=25 Feb2022 12:15:32, Value=1
[12:15:33] - procesing Timer1TimeStamp=25 Feb2022 12:15:33, Value=1
[12:15:34] - procesing Timer1TimeStamp=25 Feb2022 12:15:34, Value=4
[12:15:35] - procesing Timer1TimeStamp=25 Feb2022 12:15:35, Value=1
[12:15:36] - procesing Timer1TimeStamp=25 Feb2022 12:15:36, Value=1
[12:15:37] - procesing Timer1TimeStamp=25 Feb2022 12:15:37, Value=4
[12:15:38] - procesing Timer1TimeStamp=25 Feb2022 12:15:38, Value=4
[12:15:39] - procesing Timer1TimeStamp=25 Feb2022 12:15:39, Value=3
[12:15:40] - procesing Timer1TimeStamp=25 Feb2022 12:15:40, Value=1
[12:15:41] - procesing Timer1TimeStamp=25 Feb2022 12:15:41, Value=1
[12:15:42] - procesing Timer1TimeStamp=25 Feb2022 12:15:42, Value=1
[12:15:43] - procesing Timer1TimeStamp=25 Feb2022 12:15:43, Value=4
[12:15:44] - procesing Timer1TimeStamp=25 Feb2022 12:15:44, Value=1
[12:15:45] - procesing Timer1TimeStamp=25 Feb2022 12:15:45, Value=1
[12:15:46] - procesing Timer1TimeStamp=25 Feb2022 12:15:46, Value=3
[12:15:47] - procesing Timer1TimeStamp=25 Feb2022 12:15:47, Value=1
[12:15:48] - procesing Timer1TimeStamp=25 Feb2022 12:15:48, Value=1
[12:15:49] - procesing Timer1TimeStamp=25 Feb2022 12:15:49, Value=1
[12:15:50] - procesing Timer1TimeStamp=25 Feb2022 12:15:50, Value=1
[12:15:51] - procesing Timer1TimeStamp=25 Feb2022 12:15:51, Value=1
[12:15:52] - procesing Timer1TimeStamp=25 Feb2022 12:15:52, Value=1
[12:15:53] - procesing Timer1TimeStamp=25 Feb2022 12:15:53, Value=1
[12:15:54] - procesing Timer1TimeStamp=25 Feb2022 12:15:54, Value=1
[12:15:55] - procesing Timer1TimeStamp=25 Feb2022 12:15:55, Value=1
[12:15:56] - procesing Timer1TimeStamp=25 Feb2022 12:15:56, Value=1
[12:15:57] - procesing Timer1TimeStamp=25 Feb2022 12:15:57, Value=1
[12:15:58] - procesing Timer1TimeStamp=25 Feb2022 12:15:58, Value=1
[12:15:59] - procesing Timer1TimeStamp=25 Feb2022 12:15:59, Value=2
[12:16:00] - procesing Timer1TimeStamp=25 Feb2022 12:16:00, Value=2
[12:16:01] - procesing Timer1TimeStamp=25 Feb2022 12:16:01, Value=1
[12:16:02] - procesing Timer1TimeStamp=25 Feb2022 12:16:02, Value=1
[12:16:03] - procesing Timer1TimeStamp=25 Feb2022 12:16:03, Value=1
[12:16:04] - procesing Timer1TimeStamp=25 Feb2022 12:16:04, Value=1
[12:16:05] - procesing Timer1TimeStamp=25 Feb2022 12:16:05, Value=1
[12:16:06] - procesing Timer1TimeStamp=25 Feb2022 12:16:06, Value=2
[12:16:07] - procesing Timer1TimeStamp=25 Feb2022 12:16:07, Value=1
[12:16:08] - procesing Timer1TimeStamp=25 Feb2022 12:16:08, Value=1
[12:16:09] - procesing Timer1TimeStamp=25 Feb2022 12:16:09, Value=1
[12:16:10] - procesing Timer1TimeStamp=25 Feb2022 12:16:10, Value=1
[12:16:11] - procesing Timer1TimeStamp=25 Feb2022 12:16:11, Value=2
[12:16:12] - procesing Timer1TimeStamp=25 Feb2022 12:16:12, Value=2
[12:16:13] - procesing Timer1TimeStamp=25 Feb2022 12:16:13, Value=1
[12:16:14] - Average: 1,6
[12:16:14] - procesing Timer1TimeStamp=25 Feb2022 12:16:14, Value=1
[12:16:15] - procesing Timer1TimeStamp=25 Feb2022 12:16:15, Value=2
[12:16:16] - procesing Timer1TimeStamp=25 Feb2022 12:16:16, Value=1
[12:16:17] - procesing Timer1TimeStamp=25 Feb2022 12:16:17, Value=1
[12:16:18] - procesing Timer1TimeStamp=25 Feb2022 12:16:18, Value=2
[12:16:19] - procesing Timer1TimeStamp=25 Feb2022 12:16:19, Value=1
[12:16:20] - procesing Timer1TimeStamp=25 Feb2022 12:16:20, Value=1
[12:16:21] - procesing Timer1TimeStamp=25 Feb2022 12:16:21, Value=1
[12:16:22] - procesing Timer1TimeStamp=25 Feb2022 12:16:22, Value=1
[12:16:23] - procesing Timer1TimeStamp=25 Feb2022 12:16:23, Value=1
[12:16:24] - procesing Timer1TimeStamp=25 Feb2022 12:16:24, Value=1
[12:16:25] - procesing Timer1TimeStamp=25 Feb2022 12:16:25, Value=1
[12:16:26] - procesing Timer1TimeStamp=25 Feb2022 12:16:26, Value=1
[12:16:27] - procesing Timer1TimeStamp=25 Feb2022 12:16:27, Value=1
[12:16:28] - procesing Timer1TimeStamp=25 Feb2022 12:16:28, Value=1
[12:16:29] - procesing Timer1TimeStamp=25 Feb2022 12:16:29, Value=1
[12:16:30] - procesing Timer1TimeStamp=25 Feb2022 12:16:30, Value=1
[12:16:31] - procesing Timer1TimeStamp=25 Feb2022 12:16:31, Value=1
[12:16:32] - procesing Timer1TimeStamp=25 Feb2022 12:16:32, Value=1
[12:16:33] - procesing Timer1TimeStamp=25 Feb2022 12:16:33, Value=1
[12:16:34] - procesing Timer1TimeStamp=25 Feb2022 12:16:34, Value=1
[12:16:35] - procesing Timer1TimeStamp=25 Feb2022 12:16:35, Value=1
[12:16:36] - procesing Timer1TimeStamp=25 Feb2022 12:16:36, Value=1
```

### Custom EventLog 

* modifying the `app.config` attributes describing the event log

```xml
<add key="EventLogName" value="TestLog"/>
```
in `app.config` without pre-creating the event log leads to the error in the __Application__ event log:

```text
Application: WindowsService.NET.exe
Framework Version: v4.0.30319
Description: The process was terminated due to an unhandled exception.
Exception Info: System.ArgumentException
Stack:
   at System.Diagnostics.EventLog.CreateEventSource(System.Diagnostics.EventSourceCreationData)
   at System.Diagnostics.EventLog.CreateEventSource(System.String, System.String)
   at WindowsService.NET.Service1..ctor()
   at WindowsService.NET.Program.Main()

```
and
```text
Faulting application name: WindowsService.NET.exe, version: 1.0.0.0, time stamp: 0x624f0fa4
Faulting module name: KERNELBASE.dll, version: 6.1.7601.24384, time stamp: 0x5c6e221e
Exception code: 0xe0434352
Fault offset: 0x0000845d
Faulting process id: 0xa60
Faulting application start time: 0x01d84a9ed3b19ae5
Faulting application path: C:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe
Faulting module path: C:\Windows\system32\KERNELBASE.dll
Report Id: 1175fe43-b692-11ec-a583-08002783cd0a
```

and errors in the __System__ event log:
```text
EventId:7009
A timeout was reached (30000 milliseconds) while waiting for the WindowsService.NET service to connect.
```

and
```text
EventId:7000
The WindowsService.NET service failed to start due to the following error: 
The service did not respond to the start or control request in a timely fashion.
```
the workaround is in creating / removing the Windows event log via Powershell cmdlet:
```powershell
$logname = '%EVENTLOGNAME%'
new-eventlog -source $logname -logname $logname -erroraction silentlycontinue
```
No attempt to create a full instrumentation manifest and utilize `wevtutil.exe`

### Vintage Screenshots

Technically,Microsoft introduced __Task Scheduler__ (formerly named __Windows 95 System Agent__ and __Scheduled Tasks__) component in the [Microsoft Plus!](https://en.wikipedia.org/wiki/Microsoft_Plus!) for __Windows 95__

![Windows 95 Example](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/win95_scheduled_task.png)

![Windows 98 Example 1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/win98_scheduled_task1.png)

![Windows 98 Example 2](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/win98_scheduled_task2.png)

![Windows 2k Example 1](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/win2k_scheduled_task1.png)

### See Also

  * [x86 PC emulator and x86-to-wasm JIT, running in the browser](https://copy.sh/v86/) running [windows 98 retail](https://copy.sh/v86/?profile=windows98) and [github project](https://github.com/copy/v86)
  * https://www.sysprobs.com/pre-installed-windows-98-se-virtualbox-image
  * http://virtualdiskimages.weebly.com/vmware.html
  * https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs
  * Alternatives of .NET ServiceInstaller does not provide to modify the  registry entries responsible for Recovery options of Windows C# Windows service [discussion](https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs) and [stackoverflow](https://stackoverflow.com/questions/53009043/how-to-change-windows-service-recovery-option-using-c-sharp)
  * [documentation](https://docs.microsoft.com/en-us/windows/win32/wes/windows-event-log-tools) for `wevtutil.exe` and `mc.exe`
  * [summary](https://www.thewindowsclub.com/what-is-wevtutil-and-how-do-you-use-it) of `wevtutil.exe` commands and aliases
  * another [summary](https://ss64.com/nt/wevtutil.html) of `wevtutil.exe` commands and aliases
  * [documentation](https://docs.microsoft.com/en-us/windows-hardware/test/weg/instrumenting-your-code-with-etw) about __Instrumenting Your Code with ETW__
  * https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wevtutil  * [documentation](https://docs.microsoft.com/en-us/windows/win32/wes/writing-an-instrumentation-manifest) about __Instrumentation Manifest__
  * [documentation](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/remove-eventlog?view=powershell-5.1) of `remove-eventlog`
  * [Task Scheduler Schema Elements](https://learn.microsoft.com/en-us/windows/win32/taskschd/task-scheduler-schema-elements?source=recommendations)
  * [S4U](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-sfu/8ee85a47-7526-4184-a7c5-25a5e4155d7d) - Kerberos Network Authentication Service (V5) Service for User (S4U) Extension provides two extensions to the Kerberos Protocol.
  * KERB_S4U_LOGON structure (ntsecapi.h) [documentation](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/ns-ntsecapi-kerb_s4u_logon)
  * [logonType Simple Type in TaskScheduler XML Schema](https://learn.microsoft.com/en-us/windows/win32/taskschd/taskschedulerschema-logontype-simpletype)
  * standalone class doing elevation check - [Application Startup Permissions Validator](https://www.codeproject.com/Tips/3245/Application-Startup-Permissions-Validator)
  * [requesting Admin Approval at Application Start](https://www.codeproject.com/Articles/66259/Requesting-Admin-Approval-at-Application-Start)
  * `LocalSystem` predefined local account [documentation](https://learn.microsoft.com/en-us/windows/win32/services/localsystem-account). NOTE: the `LocalSystem` user data is stored in `%SYSTEMROOT%\System32\config\systemprofile`. The `takeown.exe` has option to make files and directories owner by the administrators group, and not any user
  * `LocalService` predefined local account [documentation](https://learn.microsoft.com/en-us/windows/win32/services/localservice-account). The files owned by this account [are](https://serverfault.com/questions/9325/where-can-i-find-data-stored-by-a-windows-service-running-as-local-system-accou) under `%SYSTEMROOT%\ServiceProfiles\LocalService`
  * https://medium.com/@tyler_48883/dreaming-about-the-windows-task-scheduler-for-mac-software-3ea76020aac1

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

