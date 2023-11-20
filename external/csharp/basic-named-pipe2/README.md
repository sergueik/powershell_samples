### Info

This directory contains code from the __Inter-Process Communication in .NET Using Named Pipes__
[codeproject article](https://www.codeproject.com/Articles/7176/Inter-Process-Communication-in-NET-Using-Named-Pip).
The project is updated to .Net Framework __4.5__

![eventlog](https://github.com/sergueik/powershell_samples/blob/master/external/csharp/basic-named-pipe2/screenshots/capture-test.png)

### Powershell test

```powershell
[System.IO.Directory]::GetFiles("\\.\\pipe\\") | where-object { $_ -match '.*myPipe' }
```
```text
\\.\\pipe\\MyPipe
```
```powershell
$client = new-object System.IO.Pipes.NamedPipeClientStream('.', 'MyPipe', [System.IO.Pipes.PipeDirection]::InOut, [System.IO.Pipes.PipeOptions]::None,  [System.Security.Principal.TokenImpersonationLevel]::Impersonation)
$client.Connect()
$writer = new-object System.IO.StreamWriter($client)
$writer.AutoFlush = $true
$writer.WriteLine('this is a test')
```

### Note


  * not very stable:
    + if the "server" started in Debuger, the excepion received in the "client" is: `NamedPipeIOException: Pipe \\.\pipe\MyPipe is too busy. Internal error: 231` and `NamedPipeIOException: Error reading from pipe. Internal error: 109` 
    + the exception received in Powershell client is: `Pipe is broken.`
  * needs moderate modifications to be used as Windows Service

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
