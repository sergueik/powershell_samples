### Info

```text
type %temp%\loadaverage.csv
```
```csv
TimeStamp,Value
2026-03-31 20:42:11,2128171008.00
2026-03-31 20:44:30,2316846592.00
2026-03-31 20:47:08,2336112640.00
2026-03-31 20:47:18,2316656640.00
2026-03-31 20:47:28,2159769600.00
2026-03-31 20:47:38,2071382272.00
```
![task manager](screenshots/capture-taskmanager.png)

![resource monitor](screenshots/capture-resourcemonitor.png)

![appender](screenshots/capture-append.png)

![chart](screenshots/capture-chart.png)

the curve proves

From the timestamps:

21:22:51 → 21:27:53
Available memory falls from 7.67 GB → 647 MB
21:27:53 → 21:29:03
Rapid recovery back to ~1.97 GB
21:29:03 → 21:31:04
Stable plateau around 1.9–2.0 GB
21:31:14 onward
Strong recovery to 8.77 GB

This is a textbook workload signature

this took only 58 samples (technically more, because some averaging performed but still ):

```cmd
type %temp%\loadaverage.csv | c:\Windows\system32\find.exe /v /c ""
```
```text
58
```
### Background Info
Background Info

Given that Microsoft Windows continuously performs the heavy lifting every minute of every hour, collecting an log structurd set of performance counters — both global system metrics and per-instance process data — the telemetry foundation is already present before a single line of application code is written.

#### Available Counters 

`Process.Explorer`   |
---------------------|
`% Processor Time` |
`% User Time`|
`% Privileged Time`|
`Virtual Bytes Peak`|
`Virtual Bytes`|
`Page Faults/sec`|
`Working Set Peak`|
`Working Set`|
`Page File Bytes Peak`|
`Page File Bytes`|
`Private Bytes`|
`Thread Count`|
`Priority Base`|
`Elapsed Time`|
`ID Process`|
`Creating Process ID`|
`Pool Paged Bytes`|
`Pool Nonpaged Bytes`|
`Handle Count`|
`IO Read Operations/sec`|
`IO Write Operations/sec`|
`IO Data Operations/sec`|
`IO Other Operations/sec`|
`IO Read Bytes/sec`|
`IO Write Bytes/sec`|
`IO Data Bytes/sec`|
`IO Other Bytes/sec`|
`Working Set - Private`|


Counter Category `Process` |
-------------------------- |
`% Processor Time`         |
`% User Time`              |
`% Privileged Time`|
`Virtual Bytes Peak`|
`Virtual Bytes`|
`Page Faults/sec`|
`Working Set Peak`|
`Working Set`|
`Page File Bytes Peak`|
`Page File Bytes`|
`Private Bytes`|
`Thread Count`|
`Priority Base`|
`Elapsed Time`|
`ID Process`|
`Creating Process ID`|
`Pool Paged Bytes`|
`Pool Nonpaged Bytes`|
`Handle Count`|
`IO Read Operations/sec`|
`IO Write Operations/sec`|
`IO Data Operations/sec`|
`IO Other Operations/sec`|
`IO Read Bytes/sec`|
`IO Write Bytes/sec`|
`IO Data Bytes/sec`|
`IO Other Bytes/sec`|
`Working Set - Private`|



Coupled with Microsoft’s own architectural decision to ship .NET Framework 4.x as a Windows component for as long as that Windows version is supported — a very strong promise by Microsoft standards — and to include a trusted, fully-featured assembly repository under the Global Assembly Cache (GAC) on every installation -  something that shiped is effectively immortal.

One can rhetorically ask: can we not take advantage of an operating system that already offers mature instrumentation primitives and a pre-established trusted deployment substrate

Borrowing from a certain unforgettable cinematic farewell: it is important to always try new things.

__Fusion__ (managed and unmanaged) isn’t just assembly binding  which it is on the surface — in the managed/unmanaged context of .NET Framework, it also serves as a gatekeeper: it enforces code trust and licensing, and ensures that assemblies (and the OS/runtime) won’t function if licensing checks fail.
__WPA__ (__Windows Product Activation__) is part of the same concept at the OS level: making the system refuse to run if it wasn’t properly licensed.

This move is contrary to almost every other decision Microsoft made, before or after:
Historically, they monetized and controlled access via licensing, Fusion, WPA, IE bundling, etc.
Suddenly, the GAC becomes free infrastructure — no gate, no enforcement, no direct profit.

So what you’re highlighting is a truly exceptional paradox in Microsoft history: they accidentally (or perhaps naively) created something “as open as Unix” inside Windows, but without any financial benefit — a rare crack in the otherwise highly controlled ecosystem.
### Usage

* rebuild the complex project in the IDE or commandline
> NOTE - is *script execution policy* is tight, still can paste individual command in the console


```powershell
remove-item -recurse -force Utils/obj,Utils/bin/,Program/bin/,Program/obj/,Test/bin/,Test/obj/ -erroraction SilentlyContinue
```
```powershell
$buildfile = 'basic-countermonitor.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$env:path="${env:path};${framework_path}"
msbuild.exe -p:FrameworkPathOverride="${framework_path}" $buildfile /p:Configuration=Release /p:Platform="Any CPU" /t:"Clean,Build"
```
> NOTE without the `/v:diag` flag __MSbuild__ may silently abort executions when there are problems creaing the illusion it has run but nothing is produced

See [stackoverflow](https://stackoverflow.com/questions/3155492/how-do-i-specify-the-platform-for-msbuild) discussion
alternatively
```powershell
$buildfile = 'basic-countermonitor.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$msbuild = "${framework_path}\MSBuild.exe"
invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" $buildfile  /p:Configuration=Release /p:Platform=""Any CPU"" /t:Clean,Build"
```
```powershell
cmd %%-/c tree.com
```
```text
├───packages
│   └───NUnit.2.6.4
│       └───lib
├───Program
├───screenshots
├───Test
├───TestUtils
└───Utils
```
- the exact  path to `msbuild.exe` may vary with Windows release. To find, inspect the output of


```powershell
get-childitem -path 'C:\Windows\Microsoft.NET' -name 'msbuild.exe' -recurse
```

will get something like

```text

assembly\GAC_32\MSBuild\v4.0_4.0.0.0__b03f5f7f11d50a3a\MSBuild.exe
assembly\GAC_64\MSBuild\v4.0_4.0.0.0__b03f5f7f11d50a3a\MSBuild.exe
Framework\v2.0.50727\MSBuild.exe
Framework\v3.5\MSBuild.exe
Framework\v4.0.30319\MSBuild.exe
Framework64\v2.0.50727\MSBuild.exe
Framework64\v3.5\MSBuild.exe
Framework64\v4.0.30319\MSBuild.exe
```

### Client


when subject application maven is used it becomes a little messy:
```cmd
mvn spring-boot:run
```
runs `java.exa` somewhat differently - with a very long `classpath` argument including the project `target\classes` path, and all dependencies from `$HOME\.m2\repository` and the jar main class invoked explicilty:


to find out explore it

```cmd
set LOG=%TEMP%\output.txt
wmic.exe /output:%LOG% path win32_process where (commandline like "%java.exe%" and name != "wmic.exe") get processid,name,commandlinewmic.exe /output:%LOG% path win32_process where (commandline like "%java.exe%" and name != "wmic.exe") get processid,name,commandline
```
> NOTE: there isn't native `/noheaders` or `/output:noheaders` switch `wmic.exe` would recognize. The stabdard hack is to use 
```cmd
more.com +1 %LOG%
```
To inspect the `java` command line it is key to know the subject application Spring Application class (for __Spring Framework__ apps)
or the project directory

```cmd
set MAIN_CLASS=example.Application
more.com +1 %LOG% | findstr -i %MAIN_CLASS%
```

```cmd
set PROJECT_DIR=basic-way2automation
more.com +1 %LOG% | findstr -i %PROJECT_DIR%\target
```
when __Spring Boot__ applcation is run via `java.exe -jar`, naturally to filter by the application jar name:

```cmd
set APPLICATION_JAR=example.way2automation.jar
more.com +1 %LOG% | findstr -i %APPLICATION_JAR%
```

> NOTE:  the `wmic.exe` does not honor the column order requsted

![wmic](screenshots/capture-wmic.png)
### See Also:
   * example code from [Updating Your Form from Another Thread without Creating Delegates for Every Type of Update](https://www.codeproject.com/Articles/52752/Updating-Your-Form-from-Another-Thread-without-Cre)

  * https://stackoverflow.com/questions/661561/how-do-i-update-the-gui-from-another-thread
  * [effective way to wrire](https://www.codeproject.com/Articles/37642/Avoiding-InvokeRequired) `InvokeRequired` delegates
  * [how to use InvokeRequired](https://stackoverflow.com/questions/15580494)
  * https://learn.microsoft.com/en-us/dotnet/desktop/winforms/how-to-change-the-borders-of-windows-forms?view=netframework-4.5
  * https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.textbox?view=netframework-4.5
 
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
