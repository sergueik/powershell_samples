### Info

https://stackoverflow.com/questions/27101579/how-to-pass-parameters-to-the-custom-action

### Usage

* compile

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\CustomAction\CustomAction.csproj /t:clean,compile
```
* build

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Setup\Setup.wixproj
```

* in regular prompt (elevated is OK)

```cmd
msiexec.exe /l*v a.log /i Setup\bin\Debug\Setup.msi
```
observe in the MSI log (`a.log`):

```text
MSI (s) (54:4C) [20:31:38:654]: Hello, I'm your 32bit Impersonated custom action server.
SFXCA: Extracting custom action to temporary directory: C:\Windows\Installer\MSI942.tmp-\
SFXCA: Binding to CLR version v4.0.30319
Calling custom action CustomAction!Utils.CustomAction.SessionLog
Custom Action Data Arguments: Arg1=Arg2 Arg2 value2
```

NOTE: the MSI log is in Unicode (UTF16) and grep / find command will not work:
```cmd
findstr.exe -i "Impersonated custom action server" .\a.log
```
the following trick will help:
```powershell
type .\a.log 2>&1|more | findstr  -ic:"Impersonated custom action server"
```
this will show
```text
MSI (s) (04:28) [10:14:04:619]: Hello, I'm your 32bit Impersonated custom action server.
```

repeat the command with `Custom Action Data Arguments:` text to search
```powershell
type .\a.log 2>&1|more | findstr  -ic:"Custom Action Data Arguments:"
```
this is expected to show the text
```text
Custom Action Data Arguments: Arg1=Arg2 Arg2 value2
```

if it does not, the install did not run successfully

![application](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-logging/screenshots/capture-installed-apps.png)

another check is for presence of the custom key in the regisry
```powershell
get-item -path "HKCU:\SOFTWARE\Manufacturer\Session Log Installer"
```
```text

    Hive: HKEY_CURRENT_USER\SOFTWARE\Manufacturer


Name                           Property
----                           --------
Session Log Installer          DummyFile : 1
```

the file is installed into  logged-in user profile, not the sytem profile:
```cmd
dir C:\Users\sergueik\AppData\Local\Session Log Installer\Dummy.txt
```

```text
    Directory: C:\Users\sergueik\AppData\Local\Session Log Installer


Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a----         3/14/2024   9:48 AM             32 Dummy.txt

```
#### Cleanup
```powershell
msiexec.exe /l*v a.log /x Setup\bin\Debug\Setup.msi
```


### See Also

  * https://stackoverflow.com/questions/59046817/how-to-log-custom-messages-in-wix-installer-code
  * https://github.com/kurtanr/WiXInstallerExamples
  * https://stackoverflow.com/questions/26216718/wix-customaction-dll

  * https://wixtoolset.org/docs/v3/
  * https://stackoverflow.com/questions/9556474/automatically-populate-a-timestamp-field-in-postgresql-when-a-new-row-is-inserte
  * https://stackoverflow.com/questions/27101579/how-to-pass-parameters-to-the-custom-action
  * [history](https://en.wikipedia.org/wiki/Windows_Installer)

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
