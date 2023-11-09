### Info

This directory contains code from the __IPC with Named Pipes__
[codeproject article](https://www.codeproject.com/Articles/810030/IPC-with-Named-Pipes).
The project is updated to .Net Framework __4.5__, converted to Windows Service listening to the pipe `\\.\pipe\demo`

and a console Client.

### Usage

  * build and install service
  * run the client app (embedded in Powershell script):
```powerhsell
 .\pipe_send.ps1 -message "this is a powershell call"
```
```text
Attempting to connect to pipe demo ...Connected to pipe.
There are currently 1 pipe server instances open.
Sending this is a powershell call to pipe.
Send done.
```
  * observe the text entered in console be sent, received, optionally echoed back in the sender console and logged into Event Log named `PipeServerLog`, and to filesyste log file (`c:\temp\service.log` or as configured   )

![eventlog](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/basic-named-pipe/screenshots/capture-pipeserver-eventlog.png)

### See Also

  * __Anonymous Pipes Made Easy__ [codeproject article](https://www.codeproject.com/Articles/1087779/Anonymous-Pipes-Made-Easy)
  * https://www.codeproject.com/Articles/7176/Inter-Process-Communication-in-NET-Using-Named-Pip
  * https://www.codeproject.com/Tips/492231/Csharp-Async-Named-Pipes
  * https://www.codeproject.com/Articles/50603/Simple-Windows-Service-in-NET-with-Console-Mode
  * https://rkeithhill.wordpress.com/2014/11/01/windows-powershell-and-named-pipes/
  * https://stackoverflow.com/questions/24096969/powershell-named-pipe-no-connection
  * [Named Pipe Security and Access Rights](https://learn.microsoft.com/en-us/windows/win32/ipc/named-pipe-security-and-access-rights)
  * `System.IO.Pipes.NamedPipeServerStream` [class](https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.namedpipeserverstream?view=netframework-4.5)
  * `System.IO.Pipes.PipeAccessRule` [class](https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes.pipeaccessrule.-ctor?view=netframework-4.5#system-io-pipes-pipeaccessrule-ctor(system-string-system-io-pipes-pipeaccessrights-system-security-accesscontrol-accesscontroltype)
  * [blog](https://decoder.cloud/2019/03/06/windows-named-pipes-impersonation/) on usage of Windows pipes to impersonate the client process
  * [fundamentals of Windows Named Pipes](https://versprite.com/blog/security-research/microsoft-windows-pipes-intro/)
  * https://www.codeproject.com/Articles/810030/IPC-with-Named-Pipes 
  * [interprocess communication using Publisher-Subscriber pattern and Named Pipes as a transport](https://www.codeproject.com/Articles/5282791/Interprocess-Communication-using-Publisher-Subscri)
  * [Full Duplex Asynchronous Read/Write with Named Pipes](https://www.codeproject.com/Articles/1179195/Full-Duplex-Asynchronous-Read-Write-with-Named-Pip)
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
