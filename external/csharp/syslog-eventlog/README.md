### Info

Code from the __Syslog daemon for Windows Eventlog__ [codeproject article](https://www.codeproject.com/articles/Syslog-daemon-for-Windows-Eventlog)

on [SysLog](https://www.ietf.org/rfc/rfc3164.txt) implemented in managed code backed by Windows Event Log


### NOTE

The CLSID `58FBCF7C-E7A9-467C-80B3-FC65E8FCCA08` is COM Server implementing the `INetFwAuthorizedApplications` [see](https://docs.microsoft.com/en-us/windows/win32/api/netfw/nn-netfw-inetfwauthorizedapplications)

the build error
```text
Task failed because "AxImp.exe" was not found, or the correct Microsoft Windows SDK is not installed. The task is looking for "AxImp.exe" in the "bin" subdirectory beneath the location specified in the InstallationFolder value of the registry key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools-x86. You may be able to solve the problem by doing one of the following:  1) Install the Microsoft Windows SDK.  2) Install Visual Studio 2010.  3) Manually set the above registry key to the correct location.  4) Pass the correct location into the "ToolPath" parameter of the task. (MSB3091)

```
is caused by a missing Windows SDK tool 

```text
c:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\aximp.exe
```
```text
c:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\aximp.exe
```

### See Also

  * project [github repository](https://github.com/alphons/Syslogd) - currently targeting __.Net core 8__
  * the [Microsoft Windows SDK 7.0a](https://www.microsoft.com/en-us/download/details.aspx?id=8442) is a specific version of the Windows Software Development Kit (SDK) that was bundled with [Visual Studio 2010](https://visualstudio.microsoft.com/vs/older-downloads/)  with the necessary components to build software for __Windows 7__, __Windows__ __Server 2008 R2__, __Windows Vista__, and __Windows__ __XP SP3__.
  * the [Microsoft Windows SDK 8.0a](https://support.microsoft.com/en-us/topic/an-update-is-available-for-windows-sdk-for-windows-8-fa4b10b5-3209-5d85-b08a-0436135a8948) was a specific version of the Windows Software Development Kit, released around late 2012/early 2013 (after the main 8.0), providing tools (compilers, headers, libraries) for building Windows applications for __Windows 8__/__Server 2012__ and older systems, often used with [Visual Studio 2012](https://visualstudio.microsoft.com/vs/older-downloads/), but noted for requiring external Perl/MinGW for command-line builds, unlike earlier SDKs

---

### Author
[Serguei Kouzmine](mailto:kouzmine_serguei@yahoo.com)
