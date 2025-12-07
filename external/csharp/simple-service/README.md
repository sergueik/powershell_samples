
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

### About Service4User Functionality

#### Kerberos S4U — Beginner’s View

__Kerberos__ is a default security system that lets computers prove who they are without sending passwords over the network. Windows uses it heavily for logging in and letting services talk to each other safely.

__Service for User__ (__S4U__) is a special Microsoft feature added to Kerberos. It’s meant to help in two situations:

__S4U2Self__ – A service can say: “I’m this user” and get a ticket to act as them, even if the user didn’t log in with Kerberos.

__S4U2Proxy__ – Once a service has a ticket for a user, it can get another ticket to talk to another service as that user.

#### Why it’s useful:

Lets you build systems where one program can do work for a user in the background.

Common in web apps and enterprise software that connect to databases or file shares on behalf of a user.

#### Why it’s hard:

It only works if the network and accounts are set up just right in Active Directory. One wrong flag or missing setting can break it.

Updates to Windows may change how it behaves, so automation that once worked can suddenly fail.

Tickets can be too big if a user is in many groups.

Clocks must be in sync, or authentication fails.

#### Why to be cautious:

If set up carelessly, S4U can let a program pretend to be a powerful user. Hackers could abuse this if they get into the wrong account.

Troubleshooting can be slow because many parts of the system (accounts, services, network settings, domain controllers) have to align.

#### Bottom line:
S4U is like giving a trusted helper the keys to run errands for you. It’s powerful and saves time—but if the helper is careless or the locks change, things break or get unsafe.

![Service4Uer flow diagram](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/simple-service/screenshots/s4u_flow.png)


#### S4U Workflow (Beginner-Friendly)
```
Time →
+---------+           +-------------------+           +-------------------+           +-------------+
|  User   |           | Front-end Service |           | Back-end Service  |           |     KDC     |
| (Alice) |           |   (Web App)       |           |   (Database)      |           | (Domain Ctrl)|
+---------+           +-------------------+           +-------------------+           +-------------+
     |                         |                           |                           |
     |  Step 0: Optional login |                           |                           |
     |------------------------>|                           |                           |
     |                         |                           |                           |
     |                         | Step 1: Front-end requests |                           |
     |                         |  to act on behalf of user  |                           |
     |                         |-------------------------->|                           |
     |                         |                           |                           |
     |                         | <------ Step 2: Front-end |                           |
     |                         |  receives confirmation    |                           |
     |                         |                           |                           |
     |                         | Step 3: Front-end requests |                           |
     |                         |  access to back-end       |                           |
     |                         |-------------------------->|                           |
     |                         |                           |                           |
     |                         | <------ Step 4: Back-end  |                           |
     |                         |  confirms access          |                           |
     |                         |                           |                           |
     |                         | Step 5: Front-end uses    |                           |
     |                         |  back-end results         |                           |
     |                         |-------------------------->|                           |

```
### 🛑 Silent Failures Due to S4U Misconfigurations

When implementing Services for User (S4U), certain configurations can lead to silent failures, making troubleshooting challenging. Common pitfalls include:

- **Untrusted Delegation**: If a service isn't marked as "Trusted for Delegation" in Active Directory, it cannot obtain a service ticket on behalf of a user. This results in access being denied without clear errors.

- **Cross-Domain Constraints**: Constrained delegation across different Active Directory domains is not supported in some versions of Windows Server. This limitation can cause delegation attempts to fail silently.

- **Expired or Invalid Tickets**: If a user's ticket-granting ticket (TGT) has expired or is invalid, S4U2Self requests will fail, leading to access denials without explicit error messages.

- **Encryption Mismatches**: Some systems may not support the encryption types required for S4U operations. For instance, certain versions of Windows Server might not accept non-forwardable tickets in S4U2Proxy requests, resulting in errors like `KRB_ERR_BADOPTION`.

For a comprehensive understanding and troubleshooting guidance, refer to Microsoft's documentation on S4U-related errors: [KDC_ERR_C_PRINCIPAL_UNKNOWN](https://learn.microsoft.com/en-us/troubleshoot/windows-server/certificates-and-public-key-infrastructure-pki/kdc-err-c-principal-unknown-s4u2self-request).


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
  * [Kerberos WIKI on Services4User](https://k5wiki.kerberos.org/wiki/Projects/Services4User)
  * [MSDN S4U documentaation](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-sfu/3bff5864-8135-400e-bdd9-33b552051d94?redirectedfrom=MSDN)
  * [Kerberos Explained простыми словами](https://habr.com/ru/articles/803163/)(in Russian)
  * [logonType Simple Type in TaskScheduler XML Schema](https://learn.microsoft.com/en-us/windows/win32/taskschd/taskschedulerschema-logontype-simpletype)
  * standalone class doing elevation check - [Application Startup Permissions Validator](https://www.codeproject.com/Tips/3245/Application-Startup-Permissions-Validator)
  * [requesting Admin Approval at Application Start](https://www.codeproject.com/Articles/66259/Requesting-Admin-Approval-at-Application-Start)
  * `LocalSystem` predefined local account [documentation](https://learn.microsoft.com/en-us/windows/win32/services/localsystem-account). NOTE: the `LocalSystem` user data is stored in `%SYSTEMROOT%\System32\config\systemprofile`. The `takeown.exe` has option to make files and directories owner by the administrators group, and not any user
  * `LocalService` predefined local account [documentation](https://learn.microsoft.com/en-us/windows/win32/services/localservice-account). The files owned by this account [are](https://serverfault.com/questions/9325/where-can-i-find-data-stored-by-a-windows-service-running-as-local-system-accou) under `%SYSTEMROOT%\ServiceProfiles\LocalService`
  * https://medium.com/@tyler_48883/dreaming-about-the-windows-task-scheduler-for-mac-software-3ea76020aac1
  * discussion o/f one of several __SandboxEscaper__ [zero day exploits](https://mspoweruser.com/sandboxescaper-release-privilege-escalation-zero-day-exploit-for-windows-10/?ysclid=lp3dqn1avq960560317) originating on __Task Scheduler__ bugs in legacy job DACL parser support - system allows a regular logged in user to elevate themselves into an admin, which would allow them full control over the server or computer - The hacker, __SandboxEscaper__, has released the exploit on [GitHub](https://web.archive.org/web/20190527072707/https://github.com/SandboxEscaper/polarbearrepo) - it was later purged but mirrors exist  and is known not to warn Microsoft first. Her exploits have been used in malware before, and she says she has found 3 more local privilege escalation exploits which she intends to release later.
  * __SandboxEscaper__/`PoC-LPE` [exploit mechanism explanation](https://habr.com/ru/articles/421593)(in Rusian)
  * [another explanation of the exploit video](https://hunter2.gitbook.io/darthsidious/privilege-escalation/alpc-bug-0day)
  * software Engineering Institute CERT Microsoft Windows task scheduler contains a local privilege escalation vulnerability in the ALPC interface [summary](https://kb.cert.org/vuls/id/906424) and a workaround explanation
  * [collection of projects demonstrating Windows services for powershell scirpt, named pipes event logs etc.](https://github.com/MScholtes/Windows-Service)
 
  * [send a content type “multipart/form-data” request from c#](https://www.codeproject.com/articles/Send-a-content-type-multipart-form-data-request-fr)
  * [creating an Issue with Multiple Attachments in Jira using Rest API](https://www.codeproject.com/articles/Creating-an-Issue-with-Multiple-Attachments-in-Jir) - dealing with `multipart/form-data` parameter in `PostFile` 
  * [sending MIME Messages to a Web Service Endpoint from C#](https://www.codeproject.com/articles/Sending-MIME-Messages-to-a-Web-Service-Endpoint-fr)
  * [send Email with VB.NET windows application](https://www.codeproject.com/articles/Send-Email-with-VB-NET-windows-application) (no source) 
  * [C# File Upload with Form Fields, Cookies and Headers](https://www.codeproject.com/articles/Csharp-File-Upload-with-Form-Fields-Cookies-and-He) (no source) 
  * [Multiple File Upload using AngularJS and ASP.net MVC](https://www.codeproject.com/articles/Multiple-File-Upload-using-AngularJS-and-ASP-net-M)
---

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

