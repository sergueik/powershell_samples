### Info

this project contains
example from [Using wix to schedule a task as the local system](https://kamivaniea.com/?p=632)
merged with [chapter 13 recipe 1](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_13/code/recipe_1/scheduledtaskinstaller/ScheduledTaskInstaller) and [cahapter 6 recipe 2](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_6/code/recipe_2/passingpropertyinstaller/PassingPropertyInstaller) (showing passing properties to a deferred custom action) and [chapter 6 recipe4](https://resources.oreilly.com/examples/9781784393212/-/tree/master/chapter_6/code/recipe_4/quietcustomactioninstaller/QuietCustomActionInstaller) (showing running a batch file "quietly" during install)
from [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book

### Usage

* set sensitive vars

```powershell
$filename = '.\Product.wxs'
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

* package
```powershell

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe .\Setup.wixproj
```
the `Setup.msi` will be in `Setup\bin\Debug`.
* install
in elevated prompt

```cmd
cd bin\Debug
msiexec.exe /l*v a.log /i Setup.msi
```

check the log
```text
MSI (s) (18:2C) [06:24:49:068]: Note: 1: 2203 2: C:\developer\sergueik\powershell_ui_samples\external\wix\basic-scheduledtask-installer\Setup.msi 3: -2147287038 
```

if seeing that error code `-2147287038 `, it is likely to be indicating
```text
Exception from HRESULT: 0x80030002 (STG_E_FILENOTFOUND) - could not be found.
```

After the successfull run  the Application directory `ScheduledTaskInstaller` is created under `Program Files`:

```text
 Directory of c:\Program Files\ScheduledTaskInstaller

03/08/2023  05:51 AM                 0 Sample.txt
03/18/2023  02:58 PM               716 dialog.ps1
03/08/2023  06:33 AM    <DIR>          ..
03/08/2023  06:33 AM    <DIR>          .
```
the Application script `dialog.ps1` is deployed to Appication directory

and a Scheduled Task with the name `MyTaskName` is created

```cmd
schtasks.exe /query /tn "\MyTaskName" /v /fo list
```
this will output
```text
Folder: \
HostName:                             SERGUEIK42
TaskName:                             \MyTaskName
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
Run As User:                          sergueik
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

* NOTE: multiple settings will need some tuning

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture_added_task.png)

The second execution of msi performs uninstall. If it does not, use
```cmd
msiexec.exe -x Setup.msi
```
command
### TODO

* Wix Variable
* added the `PASSWORD` variable
```xml
 <WixVariable Id="PASSWORD" Value="does not get passwed"/>
```
```XML
ExeCommand="&quot;[SystemFolder]schtasks&quot; /Create /v1 /z  /rl HIGHEST /TN MyTaskName /SC DAILY /ST 17:00 /RU &quot;sergueik&quot; /RP  &quot;[PASSWORD]&quot; /TR &quot;c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f 'c:\temp\dialog.ps1'&quot;" 
```
but it was not taken into account: installer stops and 

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-progress.png)

prompts to enter the blank value

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-blank-password.png)

* the test

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-test.png)

### TODO

The failing runs lead to mutliple entries with the same name:

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect.png)

under the hood the installer msi are cached under

```text
c:\Windows\Installer
```
![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect2.png)

and failing installers will continue to fail no matter how many retries attempted

![task](https://github.com/sergueik/powershell_ui_samples/blob/master/external/wix/basic-scheduledtask-installer/screenshots/capture-installer-defect3.png)

### See Also

  * https://wixtoolset.org/docs/v3/xsd/wix/customaction/
  * creating a Scheduled Task __Wix Cookbook__ [book extract](https://subscription.packtpub.com/book/web-development/9781784393212/13/ch13lvl1sec82/creating-a-scheduled-task)
  * [source code](https://resources.oreilly.com/examples/9781784393212) of __Wix Cookbook__ book
  * [stealth Scheduled Tasks](https://habr.com/ru/company/rvision/blog/723050/) (in Russian)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
