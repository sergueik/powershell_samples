### Info 

this directory contains a minimally refactored replica of

[ScriptServices](https://github.com/perceptile/ScriptServices)
 
exposing PowerShell scripts as RESTful endpoints, using 
  * [NancyFx](https://www.nuget.org/packages/Nancy/1.1.0) web framework
  * [Nancy.Hosting.Wcf](https://www.nuget.org/packages/Nancy.Hosting.Wcf) WCF bridge
  * [Topshelf](https://www.nuget.org/packages/Topshelf/3.3.0) services hosting framework


### Usage
```powershell
 .\Program\bin\Debug\ScriptServices.exe
```
```text
Configuration Result:
[Success] Name ScriptServices
[Success] Description Exposes powershell scripts as REST-based micro services
[Success] ServiceName ScriptServices
Topshelf v3.3.152.0, .NET Framework v4.0.30319.42000
The ScriptServices service is now running, press Control+C to exit.

```

```sh
curl -s http://localhost:8001/script/hello-world?param1=value
```
this prints
```text
Hello World - value\r\n"

```

after invoking the Powershell script `hello-world.ps1` from the directory `Program\bin\Debug`
```sh
curl -X POST -d '{"key": "value"}' -s http://localhost:8001/script/sample
```

the server will log

```text
Body: {"key": "value"}
Calling: script=.\sample.ps1 args=[body={"key": "value"}]
```

the `ScriptServices.exe` writes temporary launcher script `SSLaunch-${guid}.ps1`:
```powershell

$env:SCRIPTSERVICES_VERSION = '1.0.0.0'
$env:PSModulePath='C:\Users\sergueik\Documents\WindowsPowerShell\Modules;C:\Prog
ram Files\AutoIt3\AutoItX;C:\Program Files\WindowsPowerShell\Modules';
function Write-Host {}
function Write-Verbose {}
function Write-Debug {}
. '.\sample.ps1' -httpVerb "POST" -body "{`"key`": `"value`"}"

```

The curl command prints the response:
```text
"HTTP \"POST\"\r\nBody \"{\"key\": \"value\"}\"\r\nkey=value\r\n"
```
### NOTE

* need to un server from elevated prompt:
```text

Topshelf.Hosts.ConsoleRunHost Error: 0 : An exception occurred, System.ServiceModel.AddressAccessDeniedException: HTTP could not register URL http://+:8001/. 
Your process does not have access rights to this namespace (see http://go.microsoft.com/fwlink/?LinkId=70353 for details). ---> System.Net.HttpListenerException:
Access is denied
```

* Occasionally fails to close after being terminated:
```powershell
dir .\Program\bin\Debug\ScriptServices.exe
```

```text
PermissionDenied: (C:\developer\se...iptServices.exe:String) 
[Get-ChildItem], UnauthorizedAccessException
    + FullyQualifiedErrorId : ItemExistsUnauthorizedAccessError,Microsoft.PowerShell.Commands.GetChildItemCommand

dir : Cannot find path 'Program\bin\Debug\ScriptServices.exe' because it does not exist.
```

```
```powershell
dir .\Program\bin\Debug
```
```text

    Directory: Program\bin\Debug


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----       12/29/2023   7:17 AM             74 error-test.ps1
-a----       12/29/2023   7:17 AM             64 hello-world.ps1
-a----       12/29/2023   7:22 AM         304128 log4net.dll
-a----       12/29/2023   7:22 AM        1531139 log4net.xml
-a----       12/29/2023   7:22 AM         899072 Nancy.dll
-a----       12/29/2023   7:22 AM          11264 Nancy.Hosting.Wcf.dll
-a----       12/29/2023   7:22 AM           1503 Nancy.Hosting.Wcf.xml
-a----       12/29/2023   7:22 AM         765218 Nancy.xml
-a----       12/29/2023   7:30 AM            431 sample.ps1
-a----       12/29/2023   7:17 AM            315 ScriptServices.exe.config
-a----       12/29/2023   7:22 AM         282112 Topshelf.dll

```