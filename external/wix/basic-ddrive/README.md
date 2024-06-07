###  Info

example based on [recommendation](https://stackoverflow.com/questions/42838287/wix-installation-move-installed-files-to-another-drive)

### Usage

```powershell
msbuild .\Setup.wixprojo

msiexec.exe /l*v a.log /qn /i bin\Debug\Setup.msi

```
### NOTE:

Appliction successfully installs but
fails to uninstall from `f:` drive when `f:` drive itself is a logical disk substituted, not a real drive:

```powershell
cd $env:USERPROFILE
subst F: $env:USERPROFILE
```
```powershell
msiexec.exe /l*v a.log /qn /x bin\Debug\Setup.msi

```
```text
MSI (s) (84:34) [12:12:03:391]: Executing op: ProgressTotal(Total=1,Type=1,ByteEquivalent=175000)
MSI (s) (84:34) [12:12:03:391]: Executing op: SetTargetFolder(Folder=F:\AppName\Dummy Application Installer\)
MSI (s) (84:34) [12:12:03:391]: Executing op: FileRemove(,FileName=dummy.txt,,ComponentId={0F4F25E3-8F46-4068-86DD-29935E5B57DD})
MSI (s) (84:34) [12:12:03:406]: Note: 1: 1926 2: 3 3: F:\Config.Msi\ 
MSI (s) (84:34) [12:12:03:406]: Note: 1: 2205 2:  3: Error 
MSI (s) (84:34) [12:12:03:406]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1926 
MSI (s) (84:34) [12:12:03:406]: Note: 1: 2205 2:  3: Error 
MSI (s) (84:34) [12:12:03:406]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1709 
MSI (s) (84:34) [12:12:03:406]: Product: Dummy Application Installer -- Error 1926. Could not set file security for file 'F:\Config.Msi\'. Error: 3.  Verify that you have sufficient privileges to modify the security permissions for this file.

Error 1926. Could not set file security for file 'F:\Config.Msi\'. Error: 3.  Verify that you have sufficient privileges to modify the security permissions for this file.
MSI (s) (84:34) [12:12:03:406]: User policy value 'DisableRollback' is 0


MSI (s) (84:34) [12:12:03:625]: Note: 1: 1725 
MSI (s) (84:34) [12:12:03:625]: Note: 1: 2205 2:  3: Error 
MSI (s) (84:34) [12:12:03:625]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1725 
MSI (s) (84:34) [12:12:03:625]: Note: 1: 2205 2:  3: Error 
MSI (s) (84:34) [12:12:03:625]: Note: 1: 2228 2:  3: Error 4: SELECT `Message` FROM `Error` WHERE `Error` = 1709 
MSI (s) (84:34) [12:12:03:625]: Product: Dummy Application Installer -- Removal failed.

MSI (s) (84:34) [12:12:03:625]: Windows Installer removed the product. Product Name: Dummy Application Installer. Product Version: 1.0.0.0. Product Language: 1033. Manufacturer: My Company. Removal success or error status: 1603.

MSI (s) (84:34) [12:12:03:625]: Deferring clean up of packages/files, if any exist
MSI (s) (84:34) [12:12:03:625]: MainEngineThread is returning 1603
MSI (s) (84:64) [12:12:03:625]: RESTART MANAGER: Session closed.
MSI (s) (84:64) [12:12:03:625]: No System Restore sequence number for this installation.
=== Logging stopped: 3/8/2024  12:12:03 ===
MSI (s) (84:64) [12:12:03:625]: User policy value 'DisableRollback' is 0
MSI (s) (84:64) [12:12:03:625]: Machine policy value 'DisableRollback' is 0
MSI (s) (84:64) [12:12:03:625]: Incrementing counter to disable shutdown. Counter after increment: 0
MSI (s) (84:64) [12:12:03:625]: Note: 1: 1402 2: HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Installer\Rollback\Scripts 3: 2 
MSI (s) (84:64) [12:12:03:625]: Note: 1: 1402 2: HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Installer\Rollback\Scripts 3: 2 
MSI (s) (84:64) [12:12:03:625]: Decrementing counter to disable shutdown. If counter >= 0, shutdown will be denied.  Counter after decrement: -1
MSI (s) (84:64) [12:12:03:625]: Restoring environment variables
MSI (c) (94:E0) [12:12:03:625]: Decrementing counter to disable shutdown. If counter >= 0, shutdown will be denied.  Counter after decrement: -1
MSI (c) (94:E0) [12:12:03:625]: MainEngineThread is returning 1603
=== Verbose logging stopped: 3/8/2024  12:12:03 ===
```

interactie uninstall leads to  the following dialog:

![uninstall failure](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-ddrive/screenshots/capture-failure-uninstall.png)

and the product directory remains on the machine until the reboot of subst removal of the vierual drive letter is performed:

```text

 Directory of f:\AppName\Dummy Application Installer

03/08/2024  12:09 PM                 0 dummy.txt
               1 File(s)              0 bytes
               0 Dir(s)     504,037,376 bytes free
```

when the drive letter used to install the application is a real volume


![volume](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-ddrive/screenshots/capture-volume.png)

no error is observed with (un)installing the application to alternate drive letter
### See Also

  * [history](https://en.wikipedia.org/wiki/Windows_Installer)
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)

