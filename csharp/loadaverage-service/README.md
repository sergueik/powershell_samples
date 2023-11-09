### Info


https://www.codeproject.com/Articles/13655/Mastering-Windows-Services

this directory contains application collecting the `\\System\Processor Queue Length` performance counter to generate load average - like metrics.
windows Service applications are long-running applications that do not have a user interface it is a good container to run low level metrics

![perfmon](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture_perfmon.png)

![event log](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture_eventlog.png)

![command line](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture_commandline.png)

```text
LOAD15MIN: 5,43 from 465 samples
LOAD5MIN: 1,31 from 297 samples
LOAD1MIN: 1,27 from 60 samples
```
Since load averages aren't natively supported by Windows, the values  provided  by this app are only compared with themselves, not with Linux / Unix Load average which meaning it quite well known:

The __definition__ is:
System load averages is the average number of processes that are either in a runnable or uninterruptable state. A process in a runnable state is either using the CPU or waiting to use the CPU. A process in uninterruptable state is waiting for some I/O access, eg waiting for disk. The averages are taken over the three time intervals. Load averages are not normalized for the number of CPUs in a system, so a load average of 1 means a single CPU system is loaded all the time while on a 4 CPU system it means it was idle 75% of the time.

Currenty aplication simpy collects the performance counter raw data than calculates its average using [LINQ](https://docs.microsoft.com/en-us/dotnet/standard/linq/) and [Extension Methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods):

```csharp
float interval = 1F * minutes;
var now = DateTime.Now;
var values = (from row in rows
	where ((now - row.TimeStamp).TotalMinutes) <= interval
	select row.Value);
var average  = values.Average();

```
### Configuration Options

entry | description | defaultvalue
 --- | --- | ---
Autorun | Perform periodic Average calculation | `False`
Noop | Use a random number generator instead of reading the performance counter `\\System\Processor Queue Length` | `False`
Debug | Log debugging information to `EventLog`| `False`
CollectInterval | Interval (in mullisecond) between reading performance counter info| `1000`
AutoAverageInterval | Interval (in mullisecond) between calculating the averages when `Autorun` is enabled | `120000`
Datafile | Filename to save the "Load Average" results| `c:\temp\loadaverage.txt`	
Eventlog | Application Eventlog parameter<br/><i>unused</i>| `Application`
EventlogSource | Application Eventlog parameter<br/><i>unused</i>| `Custom`


* to apply configuration options, perform reinstall e.g.:
```powershell
.\reinstall.ps1 -config -datafile 'c:\temp\datafile.txt' -autorun
```

### Calculating the Averages
```csharp
private void AverageData() {
	var rows = buffer.ToList();
	const float interval = 1F ;
	var now = DateTime.Now;
	var values = (from row in rows
		where ((now - row.TimeStamp).TotalMinutes) <= interval
		select row.Value);
	result = String.Format("{1, 1:f0} minute Average: {0, 4:f2}" , values.Average(), interval);

```

### Usage
* delete and uninstall service if running
```powershell
.\reinstall.ps1 -uninstall
```

* rebuild the complex project in the IDE or commandline

```powershell
remove-item -recurse -force Utils/obj,Utils/bin/,Program/bin/,Program/obj/,Test/bin/,Test/obj/ -erroraction SilentlyContinue
```
```powershell
$buildfile = 'loadaverage-service.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$env:path="${env:path};${framework_path}"
msbuild.exe -p:FrameworkPathOverride="${framework_path}" $buildfile /p:Configuration=Release /p:Platform=x86 /t:"Clean,Build"
```
See [stackoverflow](https://stackoverflow.com/questions/3155492/how-do-i-specify-the-platform-for-msbuild) discussion
alternatively
```powershell
$buildfile = 'loadaverage-service.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$msbuild = "${framework_path}\MSBuild.exe"
invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" $buildfile  /p:Configuration=Release /p:Platform=x86 /t:Clean,Build"
```
```powershell
cmd %%-/c tree.com
```
```text
C:.
├───Installer
├───Program
│   ├───bin
│   │   └───Release
│   ├───obj
│   │   └───x86
│   │       └───Release
│   └───Properties
├───screenshots
├───Setup
│   └───images
├───Test
│   ├───bin
│   │   └───Release
│   ├───obj
│   │   └───Release
│   └───Properties
└───Utils
    ├───bin
    │   └───Release
    └───obj
        └───x86
            └───Release
```
- the exact  path to `msbuild.exe` may vary with Windows release. To find, inspect the output of


```powershell
get-childitem -path 'C:\Windows\Microsoft.NET' -name 'msbuild.exe' -recurse
```

will get something like

```text

assembly\GAC_32\MSBuild\v4.0_4.0.0.0__b03f5f7f11d50a3a\MSBuild.exe
assembly\GAC_64\MSBuild\v4.0_4.0.0.0__b03f5f7f11d50a3a\MSBuild.exe
Framework\v2.0.50727\MSBuild.exe
Framework\v3.5\MSBuild.exe
Framework\v4.0.30319\MSBuild.exe
Framework64\v2.0.50727\MSBuild.exe
Framework64\v3.5\MSBuild.exe
Framework64\v4.0.30319\MSBuild.exe
```
* install the service
```powershell
.\reinstall.ps1
```
#### NOTE

the `Test` project seems to sometimes fail in console possibly because of the relative path in the nunit.framework.dll reference:

```xml

  <ItemGroup>
    <Reference Include="nunit.framework">
      <HintPath>..\packages\NUnit.2.6.4\lib\nunit.framework.dll</HintPath>
    </Reference>
    <Reference Include="System" />
  </ItemGroup>
```
### More Information

* The "utility" dlls and app config will be placed in the same folder with the main program executable
```cmd
Directory of Program\bin\Release
03/04/2022  09:50 PM    <DIR>          .
03/04/2022  09:50 PM    <DIR>          ..
03/04/2022  09:18 PM               631 app.config
03/04/2022  09:49 PM            16,896 LoadAverageService.exe
03/04/2022  09:18 PM               631 LoadAverageService.exe.config
03/05/2022  01:05 PM            69,739 LoadAverageService.InstallLog
03/04/2022  09:50 PM             9,290 LoadAverageService.InstallState
03/04/2022  09:49 PM            22,016 LoadAverageService.pdb
03/04/2022  09:49 PM            10,752 Utils.dll
03/04/2022  09:49 PM            32,256 Utils.pdb
```
to make service run under interactive user authority add the following - see also
[article](https://www.aspsnippets.com/Articles/Install-Windows-Service-silently-without-entering-Username-and-Password-using-InstallUtilexe-from-Command-Prompt-Line.aspx)
explaining steps to install a managed app Windows Service silently without prompting for Username and Password using `InstallUtil.exe` utility (bay not work as described):


```cmd
set PASSWORD=....
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe  -uninstall Program\bin\Release\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Release\WindowsService.NET.exe
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% /unattend Program\bin\Release\WindowsService.NET.exe
```
this does not appear to work. When credentials are incorrect the process is aborted however when the password is valid, it is logged as if it was successful:

but the service remains assigned to Local System account
```powershell
get-service -name 'WindowsService.NET'|select-object -property *
```
```text
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
```powershell
$servicename = 'WindowsService.NET'
get-wmiobject win32_service -filter "name='$($servicename)'" | format-list *

get-wmiobject win32_service -filter "name='$($servicename)'" |select-object -property startname

startname
---------
LocalSystem
```
- see [also](https://devblogs.microsoft.com/scripting/the-scripting-wife-uses-powershell-to-find-service-accounts/)


the `reinstall.ps1` will collect sample data and also show the logs.

```text
The Commit phase completed successfully.

The transacted install has completed.

SERVICE_NAME: DBWriter
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 2  START_PENDING
                                (NOT_STOPPABLE, NOT_PAUSABLE, IGNORES_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x7d0
        PID                : 1408
        FLAGS              :

```
```text
Sending 200 to DBWriter
testing path: C:\temp\loadaverage.txt
executing select-string:

LOAD1MIN: 0.14
LOAD5MIN: 0.14
LOAD15MIN: 0.14
Loaded entries:
{
    "LOAD1MIN":  "0.14",
    "LOAD5MIN":  "0.14",
    "LOAD15MIN":  "0.14"
}


TimeGenerated : 3/5/2022 3:55:09 PM
Message       : LOAD5MIN: 0.14 from 7 samples
InstanceId    : 3

TimeGenerated : 3/5/2022 3:55:09 PM
Message       : LOAD1MIN: 0.14 from 7 samples
InstanceId    : 3

TimeGenerated : 3/5/2022 3:55:09 PM
Message       : DBWriter FiileHelper c:\temp\loadaverage.txt
InstanceId    : 4

TimeGenerated : 3/5/2022 3:55:09 PM
Message       : LOAD15MIN: 0.14 from 7 samples
InstanceId    : 3



TimeGenerated : 3/5/2022 3:48:17 PM
Message       : DBWriter Serice Started Successfully on 3/5/2022 3:48:17 PM.
                Collection Inteval: 1000. AutoAverage Interval: 120000
                DEBUG: False
                NOOP: False
                AUTORUN: False
InstanceId    : 0



```
to do the same check later use the scrpit
```powershell
.\control.ps1 -eventlog
```

if you run it after 5 minutes from launching the service you will notice the different timeseries sample count for each load average:

```text
Sending 200 to DBWriter
testing path: C:\temp\loadaverage.txt
executing select-string:

LOAD1MIN: 0.00
LOAD5MIN: 0.04
LOAD15MIN: 0.04
Loaded entries:
{
    "LOAD1MIN":  "0.00",
    "LOAD5MIN":  "0.04",
    "LOAD15MIN":  "0.04"
}

```

```text
TimeGenerated : 3/5/2022 4:02:11 PM
Message       : LOAD5MIN: 0.04 from 299 samples
InstanceId    : 3

TimeGenerated : 3/5/2022 4:02:11 PM
Message       : LOAD1MIN: 0.00 from 60 samples
InstanceId    : 3

TimeGenerated : 3/5/2022 4:02:11 PM
Message       : DBWriter FiileHelper c:\temp\loadaverage.txt
InstanceId    : 4

TimeGenerated : 3/5/2022 4:02:11 PM
Message       : LOAD15MIN: 0.04 from 427 samples
InstanceId    : 3

```
```text
Creating EventLog source WindowsService.NET in log Application...

The Install phase completed successfully, and the Commit phase is beginning.
See the contents of the log file for the c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Release\WindowsService.NET.exe assembly's progress.
The file is located at c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Release\WindowsService.NET.InstallLog.
Committing assembly 'c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Release\WindowsService.NET.exe'.
Affected parameters are:
   logfile = c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Release\WindowsService.NET.InstallLog
   username = sergueik42\sergueik
   assemblypath = c:\developer\sergueik\powershell_samples\external\csharp\simple-service\Program\bin\Release\WindowsService.NET.exe
   install =
   logtoconsole =
   password = ********
```

### Troubleshooting
Service uses file to save the averages and does it automativally when configured with `Autorun` set to `True`. This is intended for debugging. Then
the external too tries to access the same file, it may see the following error
```powershell
.\control.ps1 -eventlog
```
will print
```text
Sending 200 to DBWriter
testing path: C:\temp\loadaverage.txt
Got Exception during Read:
The process cannot access the file 'C:\temp\loadaverage.txt' because it is being used by another process..
Wait 0.00 sec and retry
executing select-string:
```

```text
LOAD1MIN: 0.02
LOAD5MIN: 0.12
LOAD15MIN: 0.09
Loaded entries:
{
    "LOAD1MIN":  "0.02",
    "LOAD5MIN":  "0.12",
    "LOAD15MIN":  "0.09"
}
```

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

### Testing Service after the Reboot

* [configure the service](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/sc-config)

```cmd
sc.exe  config DBWriter start= auto
```
```text
[SC] ChangeServiceConfig SUCCESS
```
```cmd
sc.exe  config DBWriter start= auto obj= Localsystem
```
```text
[SC] ChangeServiceConfig SUCCESS
```
```cmd
sc.exe start DBWriter
```
```cmd
shutdown.exe /g /t 0
```

* run the locad average collection with `clean` flag:

```powershell
.\control.ps1  -eventlog -clean
Removing file: C:\temp\loadaverage.txt
Sending 200 to DBWriter
testing path: C:\temp\loadaverage.txt
executing select-string:
```
```text
LOAD1MIN: 0.02
LOAD5MIN: 0.01
LOAD15MIN: 0.05
```
```text


Loaded entries:
{
    "LOAD1MIN":  "0.02",
    "LOAD5MIN":  "0.01",
    "LOAD15MIN":  "0.05"
}

```
```text
TimeGenerated : 3/8/2022 6:57:19 AM
Message       : LOAD5MIN: 0.01 from 300 samples
InstanceId    : 3

TimeGenerated : 3/8/2022 6:57:19 AM
Message       : LOAD1MIN: 0.02 from 60 samples
InstanceId    : 3

TimeGenerated : 3/8/2022 6:57:19 AM
Message       : DBWriter FiileHelper c:\temp\loadaverage.txt
InstanceId    : 4

TimeGenerated : 3/8/2022 6:57:19 AM
Message       : LOAD15MIN: 0.05 from 530 samples
InstanceId    : 3
```

if observing the error

```powershell
 .\control.ps1  -eventlog -clean
```
```text
Sending 200 to DBWriter
testing path: C:\temp\loadaverage.txt
file not found: C:\temp\loadaverage.txt
```
restart the service and retry (currently no better troubleshooting steps available).

To change the service configuration
```cmd
cd C:\developer\sergueik\powershell_samples\external\csharp\loadaverage-service\Program\bin\Release>
```
modify the `LoadAverageService.exe.config`
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

NOTE: these cmdlets are Windows-verson specific.


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
Program\bin\Release\WindowsService.NET_20220122.txt
```
You may have to close `services.msc` which is displaying the service if reports failure to uninstall or install. Such errors are observed occasionally

### Environment Setup

As covered in __about deprecating TLS 1.0 and 1.1 on NuGet.org__ [article](https://devblogs.microsoft.com/nuget/deprecating-tls-1-0-and-1-1-on-nuget-org/#ensuring-your-system-uses-tls-1-2) for Visual Studio the following commans needed to be run
```cmd
reg.exe add HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.30319 /v SystemDefaultTlsVersions /t REG_DWORD /d 1 /f /reg:64
reg.exe add HKLM\SOFTWARE\Microsoft\.NETFramework\v4.0.30319 /v SystemDefaultTlsVersions /t REG_DWORD /d 1 /f /reg:32
```
a similar workaround `reg` file is saved in [this project](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/nuget_tls_fix.reg)

### Performace Counters

* list avalable counters
```powershell
Get-Counter -ListSet * | where-object { $_.countersetname -eq  'Processor Information' } | select-object -expandproperty paths
```
```text
\Processor Information(*)\Performance Limit Flags
\Processor Information(*)\% Performance Limit
\Processor Information(*)\% Privileged Utility
\Processor Information(*)\% Processor Utility
\Processor Information(*)\% Processor Performance
\Processor Information(*)\Idle Break Events/sec
\Processor Information(*)\Average Idle Time
\Processor Information(*)\Clock Interrupts/sec
\Processor Information(*)\Processor State Flags
\Processor Information(*)\% of Maximum Frequency
\Processor Information(*)\Processor Frequency
\Processor Information(*)\Parking Status
\Processor Information(*)\% Priority Time
\Processor Information(*)\C3 Transitions/sec
\Processor Information(*)\C2 Transitions/sec
\Processor Information(*)\C1 Transitions/sec
\Processor Information(*)\% C3 Time
\Processor Information(*)\% C2 Time
\Processor Information(*)\% C1 Time
\Processor Information(*)\% Idle Time
\Processor Information(*)\DPC Rate
\Processor Information(*)\DPCs Queued/sec
\Processor Information(*)\% Interrupt Time
\Processor Information(*)\% DPC Time
\Processor Information(*)\Interrupts/sec
\Processor Information(*)\% Privileged Time
\Processor Information(*)\% User Time
\Processor Information(*)\% Processor Time
```
and
```powershell
Get-Counter -ListSet * | where-object { $_.countersetname -eq  'Processor' } | select-object -expandproperty paths
```
```text
\Processor(*)\% Processor Time
\Processor(*)\% User Time
\Processor(*)\% Privileged Time
\Processor(*)\Interrupts/sec
\Processor(*)\% DPC Time
\Processor(*)\% Interrupt Time
\Processor(*)\DPCs Queued/sec
\Processor(*)\DPC Rate
\Processor(*)\% Idle Time
\Processor(*)\% C1 Time
\Processor(*)\% C2 Time
\Processor(*)\% C3 Time
\Processor(*)\C1 Transitions/sec
\Processor(*)\C2 Transitions/sec
\Processor(*)\C3 Transitions/sec
```
that can get the instant values via

```powershell
(Get-Counter '\Processor(*)\% Processor Time').CounterSamples| format-list
```
this will list allavailable CPU
```text

Path         : \\sergueik53\processor(0)\% processor time
InstanceName : 0
CookedValue  : 0.825095535185472

Path         : \\sergueik53\processor(1)\% processor time
InstanceName : 1
CookedValue  : 5.47391918197365

Path         : \\sergueik53\processor(2)\% processor time
InstanceName : 2
CookedValue  : 5.47391918197365

Path         : \\sergueik53\processor(3)\% processor time
InstanceName : 3
CookedValue  : 5.47391918197365

Path         : \\sergueik53\processor(_total)\% processor time
InstanceName : _total
CookedValue  : 4.31170831153138
```

### Installer


* build package
```powershell
cd Setup
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\Setup.wixproj
```
the `Setup.msi` will be in `Setup\bin\Release`.


### Install

in elevated prompt

* install
```cmd
msiexec.exe /quiet /i bin\Release\Setup.msi
```
* confirm
```cmd
sc.exe query "LoadAverageService"

```
```text
SERVICE_NAME: LoadAverageService
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```
```powershell
get-service -name LoadAverageService
```
```text
Status   Name               DisplayName
------   ----               -----------
Running  LoadAverageService Time Service
```
(the names are slightly incorrect)
* uninstall

```cmd
msiexec /quiet /x bin\Release\Setup.msi
```

*  confirm service is removed
```cmd
sc.exe query "LoadAverageService"
```

```text
[SC] EnumQueryServicesStatus:OpenService FAILED 1060:

The specified service does not exist as an installed service.

```
* run with logs 

```cmd
cd Setup\bin\Release
msiexec /l*v a.log /quiet /i Setup.msi
```
the log shows an issue

```text
MSI (s) (14:DC) [18:12:26:104]: Note: 1: 2203 2: ...Setup.msi 3: -2147287038 
```

this likely means
```
.\message_from_hresult.ps1  0x80030002
Inspecting error code: 2147680258
 could not be found. (Exception from HRESULT: 0x80030002 (STG_E_FILENOTFOUND))
```
and is fixed by running the install in elevated prompt

### Predefined SIDs

[Some](https://learn.microsoft.com/en-us/windows/win32/secauthz/well-known-sids) predefined SIDs:

--- | --- | ---
`S-1-1-0` | Everyone | 
`S-1-5`   | SECURITY_NT_AUTHORITY | SIDs that are not universal but are meaningful only on Windows installations
`S-1-5-18`  | SECURITY_LOCAL_SYSTEM_RID | A special account used by the operating system a.k.a. "SYSTEM" a.k.a. "LOCAL SYSTEM"
`S-1-5-19` | |  A special account `NT AUTHORITY\LOCAL SERVICE` used by the operating system for some services
`S-1-5-21` | SECURITY_NT_NON_UNIQUE |  SIDS are not unique

`S-1-5-21-3828517485-1467542541-3583938822-1001` |  | a typical local Windows user

### See Also

  * https://blog.sflow.com/2011/02/windows-load-average.html
  * `pdh.h` [information](https://docs.microsoft.com/en-us/windows/win32/api/pdh/)
  * [p/invoke PDH](https://github.com/dahall/Vanara/blob/master/PInvoke/Pdh/Pdh.cs)
  * [read performance data from a log file with C# pinvoke](http://adamserrata.blogspot.com/2009/01/how-to-use-c-to-read-performance-data.html)
  * java JNA [adapter](https://java-native-access.github.io/jna/4.2.0/com/sun/jna/platform/win32/Pdh.html) of Windows Performance Data Helper (a.k.a. PDH).
  * Java PDH raw counters [example](https://github.com/java-native-access/jna/blob/master/contrib/platform/test/com/sun/jna/platform/win32/PdhTest.java)

  * Similar [native app](https://github.com/sflow/host-sflow) (NOTE: not updatedin the Windows part since 2012).
  * [hint](https://www.urtech.ca/2015/09/solved-cannot-load-counter-name-data-because-an-invalid-index-was-read-from-the-registry/) for fixing the `Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.` error
  * https://copy.sh/v86/?profile=windows98
  * https://www.sysprobs.com/pre-installed-windows-98-se-virtualbox-image
  * http://virtualdiskimages.weebly.com/vmware.html
  * https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs
  * Alternatives of .NET ServiceInstaller does not provide to modify the  registry entries responsible for Recovery options of Windows C# Windows service [discussion](https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs) and [stackoverflow](https://stackoverflow.com/questions/53009043/how-to-change-windows-service-recovery-option-using-c-sharp)
  * https://github.com/joaoportela/CircularBuffer-CSharp
  * http://www.java2s.com/Tutorial/CSharp/0450__LINQ/Catalog0450__LINQ.htm
  * http://www.java2s.com/Tutorial/CSharp/0450__LINQ/UseLINQwithDictionary.htm
  * a simpler [circular buffer](http://www.java2s.com/Open-Source/CSharp_Free_Code/DotNet/Download_Circular_Buffer_for_NET.htm)
  * [blog](https://csharp.christiannagel.com/2022/03/22/windowsservice-2/) on nuget package `Microsoft.Extensions.Hosting.WindowsServices` adapting the Windows Service specifics to the .Net 6.x runtime thus creating "latest greatest" version of Windows Service
  * Service Install [overview](https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-install-and-uninstall-services) (now ranked "legacy" by Microsoft)
  * [creating](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service)  a "new" Windows Service through subclassing a `BackgroundService`
  * [worker Services in .net](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers)
  * `new-service` cmdlet [documentation](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/new-service?view=powershell-5.1)
  * [blog](https://www.yaplex.com/blog/create-a-windows-service-using-powershell/) on installing Windows Service using several cmdlets
  * Win32 PerfMon monitoring [service](https://github.com/Iristyle/PerfTap) that publishes Windows performance data for usage in Graphite
  * an [old article](https://www.codeproject.com/Articles/3990/Simple-Windows-Service-Sample) on __Simple Windows Service Sample__
  * [hosting](https://www.codeproject.com/Articles/38160/WCF-Service-Library-with-Windows-Service-Hosting) an WCF (SOAP) web app within a Windows Service
  * [Windows Service Applications Introduction](https://learn.microsoft.com/en-us/dotnet/framework/windows-services/introduction-to-windows-service-applications)
  * [stackoverflow link](https://stackoverflow.com/questions/1140002/where-can-i-find-a-detailed-view-of-the-lifecycle-of-a-windows-service-as-develo) for lifecycle of a Windows Service
  * [developing a Windows SERVICE Application using .NET Framework with C#](https://www.codeproject.com/Articles/6106/Developing-a-Windows-SERVICE-Application-using-NET)
  * __what are Windows Services?__ [blog](https://stackify.com/what-are-windows-services)
  * [minimum user permissions required to install a Windows service](https://superuser.com/questions/91908/what-are-the-minimum-user-permissions-required-to-install-a-windows-service) - a 13 year old answer which is still valid. The TLDR anser is: Only processes with Administrative privileges are able to operate Service Control Manager
  * [service security and access rights](https://learn.microsoft.com/en-us/windows/win32/services/service-security-and-access-rights) - very detailed list of rights mapped to accounts
  * [stackoverflow](https://stackoverflow.com/questions/9021075/how-to-create-an-installer-for-a-net-windows-service-using-visual-studio)
  * [stackoverlow](https://stackoverflow.com/questions/1942039/how-to-install-and-start-a-windows-service-using-wix) on How to install and start a Windows Service using WiX
  * [creating a Windows Service and Installer using Visual Studio and WiX](https://cmartcoding.com/creating-a-windows-service-and-installer-using-net-and-wix/) - no details about the resulting XML, only covered steps to do in UI
  * [how To Add Windows Services with WiX Installer](https://www.advancedinstaller.com/versus/wix-toolset/wix-installer-add-windows-services.html) - explained the critical XML parts, has a full WiX source file example
  * [stackoverflow](https://serverfault.com/questions/328260/what-is-the-closest-equivalent-of-load-average-in-windows-available-via-wmi) on What is the closest equivalent of __load average__ in Windows available via __WMI__?
  * [discussion](https://www.cyberforum.ru/csharp-net/thread106428-page2.html#post8424851) of implementing single instance forms (in Russian)
  * `PerformanceCounter` class [reference](https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference)
  * `PerformanceCounter` [functins](https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-functions)
  * [capturing Performance Counter Data for a Process by Process Id](https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id)
  * [monitoring Windows resources with Performance Counters](https://wazuh.com/blog/monitoring-windows-resources-with-performance-counters/)
  * [working with](https://stackoverflow.com/questions/38370012/win32-processor-only-shows-cpu0) `LoadPercentage` property of `Win32_processor` WMI object
   * find out how processor load using WMI `Win32_PerfFormattedData_PerfOS_Processor` in C# [article](https://www.codeproject.com/Articles/42580/find-out-how-processor-load-using-wmi)
   * basic Processor metric inventory via WMI `Win32_Processor` [article](https://www.codeproject.com/Articles/26310/Using-WMI-to-retrieve-processor-information-in-C)
   * https://www.codeproject.com/Articles/9462/Counting-Physical-and-Logical-Processors
   * introduction To Performance Counters [article](https://www.codeproject.com/Articles/8590/An-Introduction-To-Performance-Counters)
   * performance Counters Enumerator [article](https://www.codeproject.com/Articles/58240/Performance-Counters-Enumerator)
   * Performance Counters Helper Class [article](https://www.codeproject.com/Articles/315576/A-Performance-Counters-Helper-Class)
   * links collection performance counter reviews [article](https://www.codeproject.com/Articles/42001/NET-Best-Practice-No-Using-performance-counters)
   * Perf Counter web control [article](https://www.codeproject.com/Articles/17861/AJAX-enabled-Performance-Counter-Web-Control)
   * `Perfon.Net` -  alternative metric collection helper class [article](https://www.codeproject.com/Articles/1170712/Asp-Net-Monitor-performance-without-using-windows)
   * simple custom performance counter app in c# [article](https://www.codeproject.com/Articles/1170712/Asp-Net-Monitor-performance-without-using-windows)
   * MsiExec.exe and InstMsi.exe Error Messages (for Developers) [document](https://learn.microsoft.com/en-us/windows/win32/msi/error-codes)	
### Vintage Screenshots

![ActiveState Perl PPM chimera script Example](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture-ppm_bat.jpg)

### Visual Studio Note

The projects and solutions are not compatible with Visual Studio 2019 or later:

![Visual Studio 2019 refusing to open solution file](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture_visualstudio_2019_solution_failure.png)

![Visual Studio 2019 reporting crash when loading project file](https://github.com/sergueik/powershell_samples/blob/master/csharp/loadaverage-service/screenshots/capture_visualstudio_project_failure.png)
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

