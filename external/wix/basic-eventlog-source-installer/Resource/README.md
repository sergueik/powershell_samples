### Info

this directory contains a replica of sample resource-only DLL buildproject from __how to create and integrate customized resource-only DLLs into your .NET application for Windows event logging__
[codeproject article](https://www.codeproject.com/Articles/9889/EventLog-and-resource-only-DLL-orchestration)
converted to Visual Studio 2012

### Message DLL

When the project is open in Visual Studio 2019, it will suggest to install a massive number of additional packages of total size 5+GB, download size alone 1.3GB- to enable for __Desktop Developmentwith C++__- appears no way to refuse and get a minimum just to compile the resource-only dll.

Alternative aproach is to install one of Winows SDK.

  * [Windows 7 SDK](https://e www.microsoft.com/en-us/download/details.aspx?id=8279)
  * [Windows 7 SDK and .NET Framework 4 offline installer (ISO)](https://www.microsoft.com/en-us/download/details.aspx?id=8442) (3 iso 500 MB each)

  + `GRMSDK_EN_DVD.iso` (for x86)
  + `GRMSDKIAI_EN_DVD.iso` (for x64)
  + `GRMSDKX_EN_DVD.iso` (for Itanium)

For  installing message compiler and linker it is sufficient to
download the GRMSDK_EN_DVD.iso and run `Setup.exe` from there.
only select __Tools__. This installs `mc.exe` and `rc.exe` into `c:\Program Files\Microsoft SDKs\Windows\v7.1\Bin` but will not install `link.exe`.

![Windows SDK Installer](https://github.com/sergueik/powershell_samples/blob/master/external/wix/basic-eventlog-source-installer/screenshots/capture-win7-sdk-installer.png)

It prompts for missing __.Net 4.0 RTM__ as grounds for not installing the linker.
it may be necessary to remove the later __.Net 4.x__ before installing the legacy out of support [.Net Framework 4.0](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net40)


Will need to install from [.Net 4.0 Framework Standalone (offline) installer](https://www.microsoft.com/en-us/download/details.aspx?id=17718)
- the `dotNetFx40_Full_setup.exe` online installer fails to download some
Windows 7 update package possibly due to end of life of OS and packages.

This unblocks the Windows SDK Installer compilers section. Reinstaling the Windows SDK after this brings the platform-specific linkers:

```text
c:\Program Files\Microsoft Visual Studio 10.0\VC\bin\link.exe
c:\Program Files\Microsoft Visual Studio 10.0\VC\bin\x86_amd64\link.exe
c:\Program Files\Microsoft Visual Studio 10.0\VC\bin\x86_ia64\link.exe
```
the link.exe still unable to find one other dependency: `mspdb100.dll` (one will need to start Administrator console to run run the batch file to see the error message about the missing dll)

Copying the runtime dll "c:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\mspdb100.dll"
to "c:\Program Files\Microsoft Visual Studio 10.0\VC\bin"
fixes this issue

After SDK linker and message commpiler part is successfully installed, it is recommended to remove .Net 4.0 RTM  Extended and Client Profile via Add Remove Programs and install [.NET Framework 4.8 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer) on the machine

Surprisingly however, after removal of __.Net 4.0 Client Profile__ the linker command fails with

```text
LINK : fatal error LNK1123: failure during conversion to COFF: file invalid or c
orrupt
```
Installing __.Net Framework 4.8__ does not fix this. The attemt to keep __.Net 4.0 Client Profile__ installed an install the
__.NET Framework 4.8__ results in the same error, and is time consuming - takes a few reboots

#### Usage

* note - will also produce

```text
MSG00407.bin
MSG00409.bin
```
(407 is for EN, 409 is for DE)

### NOTE  

Migration Report:

```
MakeCustomsource.vcproj: Due to the requirement that Visual C++ projects produce an embedded (by default) Windows SxS manifest, manifest files in the project are now automatically built with the Manifest Tool. You may need to change your build in order for it to work correctly. For instance, it is recommended that the dependency information contained in any manifest files be converted to "#pragma comment(linker,"<insert dependency here>")" in a header file that is included from your source code. If your project already embeds a manifest in the RT_MANIFEST resource section through a resource (.rc) file, the line may need to be commented out before the project will build correctly.
MakeCustomsource.vcproj: Due to a conformance change in the C++ compiler, code change may be required before your project will build without errors. Previous versions of the C++ compiler allowed specification of member function pointers by member function name (e.g. MemberFunctionName). The C++ standard requires a fully qualified name with the use of the address-of operator (e.g. &ClassName::MemberFunctionName). If your project contains forms or controls used in the Windows Forms Designer, you may have to change code in InitializeComponent because the designer generated code used the non-conformant syntax in delegate construction (used in event handlers).
MakeCustomsource.vcproj: This application has been updated to include settings related to the User Account Control (UAC) feature of Windows Vista. By default, when run on Windows Vista with UAC enabled, this application is marked to run with the same privileges as the process that launched it. This marking also disables the application from running with virtualization. You can change UAC related settings from the Property Pages of the project.
```
### Generating Resource-Only Message .dll in CSC

```sh
 curl -O https://raw.githubusercontent.com/palantir/windows-event-forwarding/master/windows-event-channels/CustomEventChannels.man
```
```cmd
"c:\Program Files (x86)\Windows Kits\8.1\bin\x64\mc.exe" CustomEventChannels.man
```
this producuces
```text
CustomEventChannelsTEMP.BIN
CustomEventChannels.h
CustomEventChannels.rc
MSG00001.bin
```

```cmd
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\mc.exe" -css CustomEventChannels.DummyEvent C:\ECMan\CustomEventChannels.man
```
this produces
```text
CustomEventChannels.cs
CustomEventChannels.rc
```
```cmd
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\rc.exe" CustomEventChannels.rc
```

this produces

```text
CustomEventChannels.res
```
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /win32res:CustomEventChannels.res /unsafe /target:library /out:.\CustomEventChannels.dll CustomEventChannels.cs
```

this produces

```text
CustomEventChannels.dll
```


### See Also

  * https://stackoverflow.com/questions/4501040/mc-microsofts-message-compiler-replacement-for-linux-gcc
   * [using MC.exe, message resources and the NT event log in your own projects](https://www.codeproject.com/Articles/4166/Using-MC-exe-message-resources-and-the-NT-event-lo)
  * [resource Editor in .Net](https://www.codeproject.com/Articles/3208/Resource-Editor-NET) - only supports image resources
  * [embed Win32 resources in C# programs](https://www.codeproject.com/Articles/3348/Embed-Win32-resources-in-C-programs) 
  * [creating event message DLL with Visual Studio Express and Microsoft Platform SDK](https://www.eventsentry.com/blog/2010/11/creating-your-very-own-event-m.html)
  * [EventLog and resource-only DLL orchestration](https://www.codeproject.com/Articles/9889/EventLog-and-resource-only-DLL-orchestration), using `mc.exe`,`rc.exe`, `link.exe`
  * [creating Custom Windows Event Forwarding Logs](https://learn.microsoft.com/en-us/archive/blogs/russellt/creating-custom-windows-event-forwarding-logs) using `mc.exe`,`rc.exe`, `csc.exe` (Note: some linked resources cannot be found) and the [stackoverflow discussion](https://stackoverflow.com/questions/58682189/using-the-c-sharp-compler-to-compile-custom-channels-dll-for-event-viewer-rc-fil) and [reposiory](https://github.com/palantir/windows-event-forwarding/tree/master/windows-event-channels) where one can find the `CustomEventChannels.man` resource
  * [EventSourceCreationData.MessageResourceFile Property](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.eventsourcecreationdata.messageresourcefile?view=windowsdesktop-7.0)	
  * [win32 resource Types](https://learn.microsoft.com/en-us/windows/win32/menurc/resource-types)
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
