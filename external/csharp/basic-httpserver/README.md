### Info 
replica of [httpServer](https://github.com/qinyuanpei/HttpServer)

### Testing
```cmd
set PORT=4050
netsh.exe http delete urlacl url=http://+:%PORT%/
netsh.exe http add urlacl url=http://+:%PORT%/ user=%COMPUTERNAME%\%USERNAME%
```
this will respond with
```sh
URL reservation successfully added
```
this will need to be done again if a different HTTP port used

Also,configure the "advanced firewall rule"
```cmd
set NAME=HttpServer
set PROGRAM="%CD%\Server\bin\Debug\HttpServer.exe"

netsh.exe advfirewall firewall Delete rule name="%NAME%"
netsh advfirewall firewall add rule name="%Name%" dir=in action=allow program=%PROGRAM% enable=yes profile=domain,private,public

set PROGRAM="%programfiles%\SharpDevelop\5.1\bin\SharpDevelop.exe"

netsh.exe advfirewall firewall add rule name="%NAME%" dir=in action=allow program=%PROGRAM% enable=yes
netsh.exe advfirewall firewall add rule name="%NAME%" dir=in action=allow program="C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" enable=yes
```
```cmd
netsh.exe advfirewall firewall show rule name="%NAME%"
```
```text
Rule Name:                            MiniHttpdConsole
----------------------------------------------------------------------
Enabled:                              Yes
Direction:                            In
Profiles:                             Domain,Private,Public
Grouping:
LocalIP:                              Any
RemoteIP:                             Any
Protocol:                             Any
Edge traversal:                       No
Action:                               Allow
Ok.
```

launch the server
```cmd
Server\bin\Debug\HttpServer.exe
```
examine ports
```cmd
netstat.exe -ant | findstr.exe -i :4050 | findstr.exe LISTEN
```
```text
  TCP    0.0.0.0:4050           0.0.0.0:0              LISTENING       InHost
```

```
curl -sv http://localhost:4050/
```
NOTE: if receiving

```text
* Received HTTP/0.9 when not allowed
```
This error means curl received an extremely old or malformed HTTP/0.9 response (no headers) from a server, which modern curl (v7.66.0+) disables by default for security. It usually indicates you are connecting to the wrong port, a non-HTTP service, or a misconfigured server that is rejecting your reques

if seeing the server exception
```text
Unhandled Exception: System.NullReferenceException: Object reference not set to an instance of an object.
   at Utils.BaseHeader.GetHeaderByKey(Enum header) in c:\developer\sergueik\powershell_samples\external\csharp\basic-httpserver\Utils\BaseHeader.cs:line 20
   at Utils.HttpRequest.GetHeader(RequestHeaders header) in c:\developer\sergueik\powershell_samples\external\csharp\basic-httpserver\Utils\HttpRequest.cs:line 77
   at Utils.HttpRequest..ctor(Stream stream) in c:\developer\sergueik\powershell_samples\external\csharp\basic-httpserver\Utils\HttpRequest.cs:line 47
   at Utils.HttpServer.ProcessRequest(TcpClient handler) in c:\developer\sergueik\powershell_samples\external\csharp\basic-httpserver\Utils\HttpServer.cs:line 222
   at Utils.HttpServer.<>c__DisplayClass1.<Start>b__0() in c:\developer\sergueik\powershell_samples\external\csharp\basic-httpserver\Utils\HttpServer.cs:line 141
   at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.ThreadHelper.ThreadStart()

```

it will mean that `Headers` is not initialized.



### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
