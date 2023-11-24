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

### NOTE:

the bug -  the subsequent send attempt hang. The pipe appears to still be present after the execution of the `client.ps1`:
```poweshell
[System.IO.Directory]::GetFiles("\\.\\pipe\\") | where-object { $_ -match '.*demo' }
```
```text
\\.\\pipe\\demo
```
and the service is still running:
```cmd
sc.exe query  PipeServer
```
```text
SERVICE_NAME: PipeServer
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, ACCEPTS_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```
but only the first try of exchanging messages works:
```powershell
.\pipe_send.ps1 -message "this is a powershell call 1"
``` 
		
```text
Sending this is a powershell call 1 to pipe.
Send done.
```

the second and subsequent invocation of 
```powershell
.\pipe_send.ps1 -message "this is a powershell call 2"
``` 
are hanging with
```text
Attempting to connect to pipe demo ...
``` 
presumably because the server does not continue listening to the pipe after processing single connection;
```c#
this.pipeServerState = new PipeServerState(this.ServerStream, this.cancellationTokenSource.Token);
this.ServerStream.BeginWaitForConnection(this.ConnectionCallback, this.pipeServerState);
```
or
```c#
this.ServerStream.WaitForConnection();
```
is leading to exception(logged in `Application` log:
```text
Exception Info: System.InvalidOperationException
   at System.IO.Pipes.NamedPipeServerStream.BeginWaitForConnection(System.AsyncCallback, System.Object)
   at Utils.PipeServer.SendCallback(System.IAsyncResult)

```
or 
```text
Exception Info: System.InvalidOperationException
 at System.IO.Pipes.NamedPipeServerStream.BeginWaitForConnection(System.AsyncCallback, System.Object)
   at System.IO.Pipes.NamedPipeServerStream.WaitForConnection()
   at Utils.PipeServer.Send(System.String)
   at Service.PipeServerService.<OnStart>b__0(System.Object, Utils.MessageReceivedEventArgs)

```
and the service is stopped:
```cmd
sc.exe query PipeServer

```
```text
SERVICE_NAME: PipeServer
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 1  STOPPED
        WIN32_EXIT_CODE    : 1067  (0x42b)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```
to continue processing messages, currently requires restart of PipeServer Service. Similar problem observed with Java client.

aftet which operation the  client completes the wait

```powershell
$client = new-object System.IO.Pipes.NamedPipeClientStream('.', 'demo', [System.IO.Pipes.PipeDirection]::InOut, [System.IO.Pipes.PipeOptions]::None,  [System.Security.Principal.TokenImpersonationLevel]::Impersonation)
$client.Connect()
$client.isConnected
```
```text
True
```
```powershell

$writer = new-object System.IO.StreamWriter($client)
$writer.AutoFlush = $true
$writer.WriteLine('this is a test')
$client.Dispose()
```

subsequent `Connect()` calls hang

![eventlog](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/basic-named-pipe/screenshots/capture-pipeserver-eventlog.png)


### NOTE

to create an event log entry on the command line, run the sample command in elevated prompt:
```cmd
EVENTCREATE.exe /T WARNING /ID 491   /L APPLICATION /D "My custom error event for the application log" /SO "Custom Task"
```
this will create it:
```text
SUCCESS: An event of type 'WARNING' was created in the 'APPLICATION' log with 'Custom Task' as the source.
```
works  with custom logs too:
```cmd
EVENTCREATE.exe /T WARNING /ID 10   /L PipeServerLog /D "My custom error event for the application log" /SO "Custom Task"
```
```text
SUCCESS: An event of type 'WARNING' was created in the 'PipeServerLog' log with 'Custom Task' as the source.
```


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
  * [how to create Windows EventLog source from command line](https://stackoverflow.com/questions/446691/how-to-create-windows-eventlog-source-from-command-line)



### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
