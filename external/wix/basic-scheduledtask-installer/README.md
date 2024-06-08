### Info

this project contains
example from [Using wix to schedule a task as the local system](https://kamivaniea.com/?p=632)
merged with [chapter 13 recipe 1](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_13/code/recipe_1/scheduledtaskinstaller/ScheduledTaskInstaller) and [cahapter 6 recipe 2](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_6/code/recipe_2/passingpropertyinstaller/PassingPropertyInstaller) (showing passing properties to a deferred custom action) and [chapter 6 recipe4](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_6/code/recipe_4/quietcustomactioninstaller/QuietCustomActionInstaller) (showing running a batch file "quietly" during install)
from [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book

### Usage

* set sensitive vars

```powershell
$filename = '.\Setup\Product.wxs'
[xml]$product = [XML](get-content -path $filename)
$product

xml                            Wix
---                            ---
version="1.0" encoding="UTF-8" Wix

$product.Wix.Product.Property

Id       Value
--       -----
PASSWORD place the valid password here


$product.Wix.Product.Property | foreach-object { if ($_.Id -eq 'password') { $_.Value = 'xxx' }
$product.Wix.Product.Property

Id       Value
--       -----
PASSWORD xxx
$product.Save($filename)
```
use the real password instead of the `xxx` . The dialog prompt is Work in Progress
alternatively run the script:

```powershell
. .\extract_compose_command.ps1 -password xxx
```
or
```powershell
. .\extract_compose_command.ps1 -asuser
```
* build custom action
```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\CustomAction\CustomAction.csproj
```
* package
```powershell

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\Setup\Setup.wixproj
```
ignore the warning
```text
Product.wxs(20): warning LGHT1076: ICE68: Even though custom action 'InvokeTestPS1WixCA' is marked to be elevated (with attribute msidbCustomActionTypeNoImpersonate), it will not be run with elevated privileges because it's not deferred (with attribute msidbCustomActionTypeInScript). [C:\developer\sergueik\powershell_samples\external\wix\basic-scheduledtask-installer\Setup.wixproj]

```
and
```
```text
(target AddSolutionDefineConstants) ->
  C:\Program Files (x86)\MSBuild\Microsoft\WiX\v3.x\wix2010.targets(1199,5): warning : 
Solution properties are only available during IDE builds or when building the solution file from the command line. 
To turn off this warning set <DefineSolutionProperties>false</DefineSolutionProperties> 
in your .wixproj file.
```
the `Setup.msi` will be in `Setup\bin\Debug`

* install
in elevated prompt

```cmd
msiexec.exe /l*v a.log /qn /i Setup\bin\Debug\Setup.msi
```
it will still flash few console windows while installing, presumably this still qualifies as no ui.

At the end of `a.log` will see the success message
```text
MSI (c) (70:C0) [21:43:02:610]: MainEngineThread is returning 0
```
To debug replace 
```XML
    <CustomAction Id="CreateScheduledTask" Directory="SystemFolder" ExeCommand="&quot;[SystemFolder]schtasks.exe&quot; /Create /v1 /z /rl HIGHEST /TN [TASKNAME] /SC ONCE /ST 03:55 /RU &quot;NT AUTHORITY\SYSTEM&quot; /RI 1 /TR &quot;c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f '[#ApplicationScript]'&quot;" Execute="deferred" Impersonate="no" />
```
with
```XML
    <CustomAction Id="CreateScheduledTask" Directory="SystemFolder" ExeCommand="cmd.exe /k echo  &quot;[SystemFolder]schtasks.exe&quot; /Create /v1 /z /rl HIGHEST /TN [TASKNAME] /SC ONCE /ST 03:55 /RU &quot;NT AUTHORITY\SYSTEM&quot; /RI 1 /TR &quot;c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f '[#ApplicationScript]'&quot;" Execute="deferred" Impersonate="no" />
```

and observe if theres is no synatax error in the command
```text
 "C:\Windows\system32\schtasks.exe" /Create /v1 /z /rl HIGHEST /TN ATASK /SC ONCE /ST 16:55 /RU "NT AUTHORITY\SYSTEM" /RI 1 /TR "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f 'C:\Program Files\ScheduledTaskInstaller\dialog.ps1'"
```

check the task
```text
 Directory of c:\Windows\system32\tasks

02/29/2024  04:33 PM             3,488 ATASK
               1 File(s)          3,488 bytes
```
If there is no task, the scheduled time must be in the past (work in progress)
```powershell
cmd %% - /c time /t
```
```text
21:49
```
check the log
```text
MSI (s) (18:2C) [06:24:49:068]: Note: 1: 2203 2: C:\developer\sergueik\powershell_samples\external\wix\basic-scheduledtask-installer\Setup.msi 3: -2147287038 
```

if seeing that error code `-2147287038 `, it is likely to be indicating
```text
Exception from HRESULT: 0x80030002 (STG_E_FILENOTFOUND) - could not be found.
```

After the successfull run  the Application directory `ScheduledTaskInstaller` is created under `Program Files` on 32 bit Windows and under `c:\Program Files (x86)` on a 64 bit system:

```text
 Directory of c:\Program Files (x86)\ScheduledTaskInstaller

12.03.2024  20:18    <DIR>          .
12.03.2024  20:18    <DIR>          ..
12.03.2024  18:00               626 dependency.ps1
12.03.2024  18:00               809 launcher.ps1
12.03.2024  18:00             3 224 system_log_eventlog.ps1
```
the Application script `dialog.ps1` is deployed to Appication directory

and a Scheduled Task with the name `ATASK` is created

```cmd
schtasks /query /tn ATASK
```
```text
Folder: \
TaskName                                 Next Run Time          Status
======================================== ====================== ===============
Atask                                    N/A                    Ready
```


```cmd
schtasks.exe /query /tn "\ATASK" /v /fo list
```
this will output
```text
Folder: \
HostName:                             SERGUEIK42
TaskName:                             \ATASK
Next Run Time:                        4/8/2023 5:00:00 PM
Status:                               Ready
Logon Mode:                           Interactive/Background
Last Run Time:                        4/6/2023 8:01:40 PM
Last Result:                          0
Author:                               SYSTEM
Task To Run:                          c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f "C:\Program Files\ScheduledTaskInstaller\dialog.ps1"
Start In:                             c:\Windows\System32\WindowsPowerShell\v1.0

Comment:                              N/A
Scheduled Task State:                 Enabled
Idle Time:                            Disabled
Power Management:                     Stop On Battery Mode, No Start On Batteries
Run As User:                          SYSTEM
Delete Task If Not Rescheduled:       PT0S
Stop Task If Runs X Hours and X Mins: 72:00:00
Schedule:                             Scheduling data is not available in this format.
Schedule Type:                        Daily
Start Time:                           5:00:00 PM
Start Date:                           4/6/2023
End Date:                             N/A
Days:                                 Every 1 day(s)
Months:                               N/A
Repeat: Every:                        Disabled
Repeat: Until: Time:                  Disabled
Repeat: Until: Duration:              Disabled
Repeat: Stop If Still Running:        Disabled
```

The event log `testlog` will have the listing of the same task:

```powerhsell
get-eventlog -logname testlog -newest 1 | select-object -property *
```
```text

EventID            : 2
MachineName        : DESKTOP-82V9KDO
Data               : {}
Index              : 6
Category           : (1)
CategoryNumber     : 1
EntryType          : Information
Message            : task:  Папка: \ Имя задачи                               В
                     ремя следующего запус Состояние       ======================================== ====================== ==============
                     = ATASK                                    12.03.2024 20:35:00    Готово
Source             : testlog
ReplacementStrings : {task:  Папка: \ Имя задачи
                     Время следующего запус Состояние       ======================================== ====================== =============
                     == ATASK                                    12.03.2024 20:
                     35:00    Готово         }
InstanceId         : 2
TimeGenerated      : 12.03.2024 20:18:08
TimeWritten        : 12.03.2024 20:18:08
UserName           :
Site               :
Container          :

```
* NOTE: multiple settings will need some tuning

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture_added_task.png)

The second execution of msi performs uninstall. If it does not, use
```cmd
msiexec.exe -x Setup\bin\Debug\Setup.msi
```
command
### TODO

* Wix Variable
* added the `PASSWORD` variable
```xml
 <WixVariable Id="PASSWORD" Value="does not get passwed"/>
```
```XML
ExeCommand="&quot;[SystemFolder]schtasks&quot; /Create /v1 /z  /rl HIGHEST /TN ATASK /SC DAILY /ST 17:00 /RU &quot;sergueik&quot; /RP  &quot;[PASSWORD]&quot; /TR &quot;c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f 'c:\temp\dialog.ps1'&quot;" 
```
but it was not taken into account: installer stops and 

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-progress.png)

prompts to enter the blank value

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-blank-password.png)

* the test

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-test.png)

### TODO

The failing runs lead to mutliple entries with the same name:

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect.png)

under the hood the installer msi are cached under

```text
c:\Windows\Installer
```
![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect2.png)

and failing installers will continue to fail no matter how many retries attempted

![task](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect3.png)


### NOTE

the `schtasks.exe` does not appear to support configuring the following features of Windows Task:

  * Start Task only if the computer is on AC power
  * Stop if the computer switches to battery power

which leads to the following subtle errors in Task Scheduler log indicating failure to run the said task:
```text
Task Scheduler did not launch task "\ATASK"  because computer is running on batteries. User Action: If launching the task on batteries is required, change the respective flag in the task configuration.
```
and
```text
Task Scheduler failed to start "\ATASK" task for user "NT AUTHORITY\System". Additional Data: Error Value: 2147750692.
```
The error code `2147750692` meaning (Exception from HRESULT: 0x80041324) is

```text
The Task Scheduler service attempted to run the task, but the task did not run due to one of the constraints in the task definition.
```
There is also an obscure setting which one may like to set:
 * if the task is not scheduled to run again delete it after: "immediately"

When active (it is active in the configuration produced by `schtasks.exe`) it is preventing from saving the changes interactively

### After the task runs the following custom event log messages start to appear every minute:
```text
The description for Event ID 1001 from source Microsoft-Windows-EventSystem cannot be found. Either the component that raises this event is not installed on your local computer or the installation is corrupted. You can install or repair the component on the local computer.


If the event originated on another computer, the display information had to be saved with the event.

The following information was included with the event: 

{     "username":  "SERGUEIK42$",     "parent":  "taskeng.exe",     "pid":  3848,     "message":  "test",     "invoked":  "2024-03-02 14:30" }

```
```text

The description for Event ID 1001 from source Microsoft-Windows-EventSystem cannot be found. Either the component that raises this event is not installed on your local computer or the installation is corrupted. You can install or repair the component on the local computer.


If the event originated on another computer, the display information had to be saved with the event.

The following information was included with the event: 

{     "username":  "SERGUEIK42$",     "parent":  "taskeng.exe",     "pid":  3124,     "message":  "test",     "invoked":  "2024-03-02 14:31" }

```

### NOTE

new-eventlog -source testlog -logname testlog

NOTE:
the Powershell fails to run with the default executionpolicy settings 
```powershell
get-executionpolicy  -list
```
```
        Scope ExecutionPolicy
        ----- ---------------
MachinePolicy       Undefined
   UserPolicy       Undefined
      Process       Undefined
  CurrentUser       Undefined
 LocalMachine    Unrestricted
```

```cmd
schtasks.exe /create /TN AUTOMATION\ATASK /tr "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f '%1'" /rl HIGHEST /ru "NT AUTHORITY\SYSTEM" /sc once /st 23:30
```
NOTE:  willneed dto remove the /v1 /Z options otherwise will be getting the error
```text
ERROR: The creation of the scheduled task failed. Reason: The Task Name may not contain the characters: < > : / \ |
```
the command below is just for reference
```powershell
"C:\Windows\system32\schtasks.exe" /v1 /Z /Create /RL HIGHEST /TN "\Automation\ATASK" /SC ONCE /ST 22:55 /RU "NT AUTHORITY\SYSTEM" /RI 1 /TR "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f 'C:\Program Files\ScheduledTaskInstaller\dialog.ps1'"

```
### See Also

  * https://wixtoolset.org/docs/v3/xsd/wix/customaction/
  * creating a Scheduled Task __Wix Cookbook__ [book extract](https://subscription.packtpub.com/book/web-development/9781784393212/13/ch13lvl1sec82/creating-a-scheduled-task)
  * create Scheduled task to run action in future [script](https://garytown.com/create-scheduled-task-to-run-action-in-future-nowxx-time)
  * create task to run every N minutes and start within N minute from the moment the command is run [documentation](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/schtiasks-create#to-schedule-a-task-to-run-every-n-minutes)
  * [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book
  * [stealth Scheduled Tasks](https://habr.com/ru/company/rvision/blog/723050/) (in Russian)
  * Misc
    + https://forums.ironmansoftware.com/t/passing-command-line-parameters-during-msi-installation/2082
    + https://stackoverflow.com/questions/49012022/wix-installer-execute-a-cmd-with-parameters
    + https://davton.com/blog/how-to-pass-custom-actions-to-a-wix-installer-using-command-line-arguments/

  * https://stackoverflow.com/questions/9075564/change-settings-for-power-for-windows-scheduled-task
  * [history](https://en.wikipedia.org/wiki/Windows_Installer)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
