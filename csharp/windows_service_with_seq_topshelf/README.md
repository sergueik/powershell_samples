### Info

This is a replica of the [friction-free Windows Service with Topshelf, Serilog, Seq, and Octopus Deploy](https://github.com/wshirey/WindowsServiceTemplate) project downgraded to an older version of MSBuild for compatibility with classic .NET Framework.

### Background

There are several overloaded ways of installing a Windows Service. Each has different tooling requirements.

#### Manual — no installer, no Visual Studio, no InstallUtil

```cmd
sc.exe create MyService binPath= "C:\path\MyService.exe" start= auto
sc.exe start MyService
```
To uninstall:
```
sc.exe stop MyService
sc.exe delete MyService
```

Note: The space after the equal sign in `binPath= "C:\path\MyService.exe"` is required. Without it, `sc.exe` silently creates a corrupt service entry.

#### Tool-chain integrated — using `InstallUtil.exe`
```powershell
$env:PATH=${env:PATH};C:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe MyService.exe
```

To uninstall:
```powershell
InstallUtil.exe /u MyService.exe
```

Notes: Relative paths to the binary are allowed. Requires boilerplate `ProjectInstaller` code (see below).

#### Visual Studio “all-in-one” solution
```c#
[RunInstaller(true)]
public class ProjectInstaller : Installer {
    public ProjectInstaller() {
        var processInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
        var serviceInstaller = new ServiceInstaller {
            StartType = ServiceStartMode.Automatic,
            ServiceName = "MyService"
        };

        Installers.Add(processInstaller);
        Installers.Add(serviceInstaller);
    }
}

```
This is typically found when the project was created in Visual Studio using the designer as a Service Installer.

### Custom Commands / ExecuteCommand

Windows Services support custom control codes (`128`–`255`) delivered through the Service Control Manager (__SCM__) via __RPC__.

one can use
```powershell
(Get-Service MyService).ExecuteCommand(128)
```

to send a command to the running service.

Topshelf support: Topshelf exposes the same mechanism through `WhenCustomCommandReceived` event:
```c#
s.WhenCustomCommandReceived((hostControl, command) =>
{
    switch (command) {
        case 128:
            // handle command
            break;
    }
});

```
__Topshelf__ does not mock custom commands; they are only fired by the *real* __SCM__.

Console mode does not trigger them automatically.

This mechanism is identical to the `ServiceBase` class `OnCustomCommand(int command)` override.

Note: Custom commands are not delivered via *named pipes* — they use local *RPC* via __SCM__. __Wine__ partially implements service control but may not fully support user-defined codes.

### See Also 

* [C# template for MicroService console application using TopShelf and Serilog](https://github.com/wshirey/MicroServiceProjectTemplate), on .Net Framework, by same author


### Author

[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)