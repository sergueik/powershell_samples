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