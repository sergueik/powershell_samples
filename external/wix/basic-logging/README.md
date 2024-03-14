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

* in regular prompt

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
