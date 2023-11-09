### Info

This directory contains an enhanced version of the Powershell driven (multi) reboot-resume installer skeleton framework project from
[Reboot and Resume PowerShell Script](https://www.codeproject.com/Articles/223002/Reboot-and-Resume-PowerShell-Script)
merged with [self-elevating script sample](http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx) to deal with UAC and a better argument passing. The scipt allows passing the single `Step` parameter across the reboot and processes 'First', 'Second' and 'Third' steps separately, relaucnching itself elevated. The next (if any) step to run is written into the `HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` key `Restart-And-Resume` value.

#### Usage

To demo the script run  the command
```powershell
. .\testscript.ps1 -step "First"
```
The `executionPolicy` is expected to be set to `RemoteSigned` or lower
The script will prompt for UAC access if system is configured to enforce one
and display the message indicating the step is about to run at the moment:

![Script run Example](https://github.com/sergueik/powershell_samples/blob/master/reboot_and_resume/screenshots/capture.png)


### See also

  * [forum](http://forum.oszone.net/thread-332188.html)(in Russian) on CMD/VBscript implementation of a similar flow with *driver pack install scenario* when several reboots are needed after installing the drivers 

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

