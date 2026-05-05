### Info

replica of [JsonnetBinding-Dotnet](https://github.com/JustinPealing/JsonnetBinding-Dotnet) the .NET Bindings for [Jsonnet](https://jsonnet.org/).

### Troubleshooting


NOTE: the legacy __2.6.4__ [Nunit](https://www.nuget.org/packages/NUnit.Runners/2.6.4) is 32 bit only:
```powershell
. .\check.ps1 c:\tools\nunit\nunit-console.exe
32-bit (x86)
```

the __Nunit.Runners__ __3.12.0__ is OK:
>NOTE:  In __Nunit Runners__ __3.x__ the resource linked to the "Download package" url is an archive but is not what is says it is:

```sh
VERSION=VERSION=3.12.0
curl -skLo ~/Downloads/nunit-runners.zip https://www.nuget.org/api/v2/package/NUnit.Runners/$VERSION
```
```command
unzip -ql ~/Downloads/nunit-runners.zip
```
```text
  Length      Date    Time    Name
---------  ---------- -----   ----
      507  2021-01-17 20:35   _rels/.rels
     1965  2021-01-17 20:35   NUnit.Runners.nuspec
     1090  2021-01-17 20:29   LICENSE.txt
    16371  2020-06-21 15:18   images/nunit_256.png
      528  2021-01-17 20:35   [Content_Types].xml
     1344  2021-01-17 20:35   package/services/metadata/core-properties/10d40782ffa34769be43ae9cbcfdc0d1.psmdcp
     9499  2021-01-17 12:55   .signature.p7s
---------                     -------
    31304                     7 files

```
 - it appears in the downloaded archive is just the NuGet package metadata container, not the actual NUnit executable distribution.


For __NUnit__ __3.x__, one wants the official console runner ZIP from GitHub Releases, not the NuGet “tool folder” view.
o
```sh
VERSION=3.16.3
curl -skLo ~/Downloads/nunit-runners.zip https://github.com/nunit/nunit-console/releases/download/$VERSION/NUnit.ConsoleRunner.$VERSION.nupkg
```
> NOTE Ignore  the 
```text
DEPRECATED RELEASE - PLEASE USE EITHER 3.15.5 OR 17.0.0 (OR HIGHER)
```
warning 

```sh
unzip -d /tmp -x ~/Downloads/nunit-runners.zip tools/agents/net462/*
```
will see:
```text

Archive:  /c/Users/kouzm/Downloads/nunit-runners.zip
  inflating: /tmp/tools/agents/net462/nunit-agent.exe
  inflating: /tmp/tools/agents/net462/nunit-agent.exe.config
  inflating: /tmp/tools/agents/net462/nunit-agent-x86.exe
  inflating: /tmp/tools/agents/net462/nunit-agent-x86.exe.config
  inflating: /tmp/tools/agents/net462/nunit.engine.api.dll
  inflating: /tmp/tools/agents/net462/nunit.engine.api.xml
  inflating: /tmp/tools/agents/net462/nunit.engine.core.dll
  inflating: /tmp/tools/agents/net462/testcentric.engine.metadata.dll
  inflating: /tmp/tools/agents/net462/nunit.agent.addins
```
```sh
unzip  -d /tmp -x ~/Downloads/nunit-runners.zip tools/*
```
will see:
```text
Archive:  /c/Users/kouzm/Downloads/nunit-runners.zip
  inflating: /tmp/tools/nunit3-console.exe
  inflating: /tmp/tools/nunit3-console.exe.config
  inflating: /tmp/tools/nunit.engine.api.dll
  inflating: /tmp/tools/nunit.engine.api.xml
  inflating: /tmp/tools/nunit.engine.core.dll
  inflating: /tmp/tools/nunit.engine.dll
  inflating: /tmp/tools/testcentric.engine.metadata.dll
  inflating: /tmp/tools/nunit.console.nuget.addins
```
```sh
cp -rf /tmp/tools/* /c/tools/nunit/
```
```cmd
nunit3-console.exe Tests.dll --labels=All --trace=Debug
```
```
NUnit Console 3.16.3 (Release)
Copyright (c) 2022 Charlie Poole, Rob Prouse
Tuesday, May 5, 2026 6:18:54 PM

labels=All is deprecated and will be removed in a future release. Please use labels=Before instead.

Runtime Environment
   OS Version: Microsoft Windows NT 6.2.9200.0
   Runtime: .NET Framework CLR v4.0.30319.42000

Test Files
    Tests.dll


Errors, Failures and Warnings

1) Invalid : C:\developer\sergueik\powershell_samples\external\csharp\jsonnet-net\JsonnetBinding.Tests\bin\Debug\Tests.dll
No suitable tests found in 'C:\developer\sergueik\powershell_samples\external\csharp\jsonnet-net\JsonnetBinding.Tests\bin\Debug\Tests.dll'.
Either assembly contains no tests or proper test driver has not been found.

Test Run Summary
  Overall result: Failed
  Test Count: 0, Passed: 0, Failed: 0, Warnings: 0, Inconclusive: 0, Skipped: 0
  Start time: 2026-05-05 22:18:54Z
    End time: 2026-05-05 22:18:55Z
    Duration: 0.512 seconds

Results (nunit3) saved as TestResult.xml
```

### See Also
  * https://nunit.org/nunitv2/docs/2.6.4/installation.html
