
### Info

This directory contains the code from the [article](https://www.codeproject.com/Articles/13655/Mastering-Windows-Services)
illustrating interacting with service via `OnCustomCommand` event handler and passing the arguments via file on disk: 	

There are two project: `service.sln` for the service and `client.sln` for controlling Windows form
There is no inter-project dependency -  the receiver of the call is constructed based on registry scan by the name

```c#
String
ServiceController controller = new System.ServiceProcess.ServiceController.ServiceController("DBWriter");
controller.ExecuteCommand(200);
```
thearguments are passed  through file:
```c#
  StringBuilder record = new StringBuilder();
  record.Append("CustomerID :" + this.txtCustID.Text);
  record.Append(" | First Name : " + this.txtFirstName.Text );
  record.Append(" | House No : " + this.txtHouseNum.Text);

  StreamWriter swRecord = new StreamWriter("c:\\Transaction.tmp",true);
  swRecord.WriteLine(record);
  swRecord.Flush();
  swRecord.Close();
```

### Usage
* build the complex project, all the"utility" dlls will be in the same folder with the main program executable:
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -uninstall Program\bin\Debug\WindowsService.NET.exe
```
```cmd
c:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -install Program\bin\Debug\WindowsService.NET.exe
```
to use interactive user credentials add the following (see [also](https://www.aspsnippets.com/Articles/Install-Windows-Service-silently-without-entering-Username-and-Password-using-InstallUtilexe-from-Command-Prompt-Line.aspx)):


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
See the contents of the log file for the c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe assembly's progress.
The file is located at c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.InstallLog.
Committing assembly 'c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe'.
Affected parameters are:
   logfile = c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.InstallLog
   username = sergueik42\sergueik
   assemblypath = c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service\Program\bin\Debug\WindowsService.NET.exe
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
```
REM An exception occurred during the Install phase.
REM System.InvalidOperationException: Cannot load Counter Name data because an inval
REM id index '♀?♦♂ ' was read from the registry.
```

to fix run `counter_fix.cmd` script from [blog](https://www.urtech.ca/2015/09/solved-cannot-load-counter-name-data-because-an-invalid-index-was-read-from-the-registry/)
calling `C:\Windows\System32\lodctr.exe`
```cmd
counter_fix.cmd
```
https://github.com/Darkseal/WindowsService.NET
### Note
Check the home-brewed plaintext log written by service e.g.

```text
Program\bin\Debug\WindowsService.NET_20220122.txt
```
You may have to close `services.msc` which is displaying the service if reports failure to uninstall or install. Such errors are observed occasionally

### See Also

  * https://copy.sh/v86/?profile=windows98
  * https://www.sysprobs.com/pre-installed-windows-98-se-virtualbox-image
  * http://virtualdiskimages.weebly.com/vmware.html
  * https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs
  * Alternatives of .NET ServiceInstaller does not provide to modify the  registry entries responsible for Recovery options of Windows C# Windows service [discussion](https://social.msdn.microsoft.com/Forums/en-US/217f4163-45a8-4352-aed8-d6581ff13ce8/set-recovery-options-in-properties-of-windows-service?forum=Vsexpressvcs) and [stackoverflow](https://stackoverflow.com/questions/53009043/how-to-change-windows-service-recovery-option-using-c-sharp)

### Vintage Screenshots


![Win98 Example 1](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/simple-service/screenshots/win98_scheduled_task1.png)

![Win98 Example 2](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/simple-service/screenshots/win98_scheduled_task2.png)

![Win2k Example 1](https://github.com/sergueik/powershell_ui_samples/blob/master/external/csharp/simple-service/screenshots/win2k_scheduled_task1.png)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
