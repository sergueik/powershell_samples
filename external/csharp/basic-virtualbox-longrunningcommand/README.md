### VM Appliance Docker Login Flow (VirtualBox Guest Script Model)



#### Goal

```cmd
VBoxManage.exe list runningvms
```
```text

"XPSP3" {91047a20-5df0-4b68-b11d-1abd36738105}
"Xubuntu 22.04" {7e261a39-d356-4eb1-a8ed-75675b149241}
"default" {59c3df8a-e359-4211-8e7c-74ec5dd3e51d}
"Windows 7" {55d01a4a-4656-480f-bccb-e6838f5df285}
"Windows 10 x64 ru" {184f37d0-8529-474c-962d-6fd6781d9757}
"Xubuntu VS Code" {0b64d785-4228-4357-83bc-2b6a436f81bf}

```

Provide a deterministic, non-interactive mechanism to perform Docker authentication inside a Linux VM appliance, triggered from a Windows host via:



- `VBoxManage guestcontrol`
- a VM-resident shell script
- optional mocked registry endpoint for testing



This design avoids:

- interactive login sessions
- GUI dependencies
- state inference on the host



---



#### 1. VM-side script: `/opt/appliance/docker-login.sh`



### Purpose

Performs a non-interactive Docker login and returns a clear exit code.



##### Script



```bash

#!/bin/bash

set -euo pipefail



REGISTRY="${1:-registry.mock.local}"

USERNAME="${2:-testuser}"

PASSWORD\_FILE="${3:-/run/secrets/docker\_password}"



echo "[INFO] Starting Docker login for ${REGISTRY}"



if [[ ! -f "$PASSWORD\_FILE" ]]; then

&#x20; echo "[ERROR] Password file not found: $PASSWORD\_FILE"

&#x20; exit 2

fi



PASSWORD="$(cat "$PASSWORD\_FILE")"



# Non-interactive login

echo "$PASSWORD" | docker login "$REGISTRY" \\

&#x20; -u "$USERNAME" \\

&#x20; --password-stdin



RC=$?



if [[ $RC -eq 0 ]]; then

&#x20; echo "[INFO] Docker login successful"

&#x20; echo "authenticated" > /tmp/docker\_auth\_state

else

&#x20; echo "[ERROR] Docker login failed"

&#x20; echo "failed" > /tmp/docker\_auth\_state

fi



exit $RC



```



### Execution Checkpoints



- Accept parameters (future: credentials)

- Simulate Docker login execution

- Optionally call real `docker login`

- Sleep for controlled delay

- Exit with provided status code





### Troubleshooting



```text

---------------------------

SharpDevelop

---------------------------

Can not start process. The application has failed to start because its side-by-side configuration is incorrect. 

Please see the application event log or use the command-line sxstrace.exe tool for more detail.  

(Exception from HRESULT: 0x800736B1)



---------------------------

OK   

---------------------------



```

```text

Activation context generation failed for "C:\\developer\\sergueik\\powershell\_samples\\external\\csharp\\basic-virtualbox-longrunningcommand\\Program\\bin\\Debug\\VboxManageSystemTrayApp.exe".Error in manifest or policy file "C:\\developer\\sergueik\\powershell\_samples\\external\\csharp\\basic-virtualbox-longrunningcommand\\Program\\bin\\Debug\\VboxManageSystemTrayApp.exe.Config" on line 9. Invalid Xml syntax.

```



```

&#x20;xml fo app.config

app.config:10.27: Entity 'qquot' not defined

&#x20;   <add key="VM" value="\&qquot;Xubuntu 22.04\&qquot; {7e261a39-d356-4eb1-a8ed-75

&#x20;                               ^

app.config:10.47: Entity 'qquot' not defined

&#x20;   <add key="VM" value="\&qquot;Xubuntu 22.04\&qquot; {7e261a39-d356-4eb1-a8ed-75

&#x20;                                                   ^

app.config:15.102: Entity 'qquot' not defined

stcontrol %VM% run --username root --password secret --exe /bin/sh -- -c \&qquot;

&#x20;                                                                              ^

app.config:15.117: Entity 'qquot' not defined

run --username root --password secret --exe /bin/sh -- -c \&qquot;uname -a\&qquot;

&#x20;                     

```



HTML historically accumulated hundreds and eventually thousands of named entities:



* `&copy;`
* `&nbsp;`
* `&eacute;`
* `&rdquo;`
* `&ldquo;`
* `&hellip;`


but XML 1.0 the only allowed are:

|entity  |symbol  |
|--------|--------|
|`&amp;` |  &amp; |
|`&lt;`  |&lt;    |
|`&gt;`  |&gt;    |
|`&quot;`|"       |
|`&apos;`|'       |


### Building and Running in Console

```powershell
$env:PATH="${env:PATH};c:\windows\microsoft.net\Framework\v4.0.30319"
msbuild.exe .\basic-virtualbox-longrunningcommand.sln "/p:Platform=Any CPU"
```

```powershell
.\Program\bin\Debug\VboxManageSystemTrayApp.exe
```

```powershell
type $env:temp\v*txt
```

```text
STDERR: "Exception: The system cannot find the file specified"
```
```
[System.Reflection.AssemblyName]::GetAssemblyName('VboxManageSystemTrayApp.exe')
```
```txt
Exception calling "GetAssemblyName" with "1" argument(s): "Could not load file or assembly 'VboxManageSystemTrayApp.exe' or one of its dependencies. The system cannot find the file specified."
```

```powershell
..\..\..\binary_check.ps1 -filename VboxManageSystemTrayApp.exe
```
```
x86 (32-bit)
```



```powershell
$env:PATH="${env:PATH};C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
```
```powershell
cd C:\developer\sergueik\powershell_samples\external\csharp\basic-virtualbox-longrunningcommand
msbuild.exe .\basic-virtualbox-longrunningcommand.sln "/p:Platform=x64" /detailedsummary /t:clean,build
```
```powershell
..\..\..\binary_check.ps1 -filename VboxManageSystemTrayApp.exe
```
```
x86 (32-bit)
```
```
 .\binary_check.ps1 .\Program\bin\x64\Debug\VboxManageSystemTrayApp.exe
```
```
Unknown machine type: -31132
```



### Script Execution

```cmd
pushd "c:\Program Files\Oracle\VirtualBox"
set PASSWORD=...
set VM={7e261a39-d356-4eb1-a8ed-75675b149241}
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /bin/sh -- -c "uname -a"
```
- trouble composing command to test:
```text
/bin/sh: 0: cannot open uname -a: No such file
```
```cmd
set VM={7e261a39-d356-4eb1-a8ed-75675b149241}
set PASSWORD=...
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /bin/sh -- -c ""uname -a""
```
```text
/bin/sh: 0: cannot open uname: No such file
```
```powershell
.\VBoxManage.exe guestcontrol $env:VM run --username sergueik --password $env:PASSWORD --exe /bin/whoami
```
```text
sergueik
```
```powershell
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /bin/w
```
```text
 18:14:04 up  4:12,  1 user,  load average: 0.02, 0.02, 0.00
USER     TTY      FROM             LOGIN@   IDLE   JCPU   PCPU WHAT
sergueik tty7     :0               14:02    4:12m 14.32s  0.27s xfce4-session
```
```cmd
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe whoami
```
```text
VBoxManage.exe: error: No such file or directory on guest
VBoxManage.exe: error: Details: code VBOX_E_IPRT_ERROR (0x80bb0005), component GuestProcessWrap, interface IGuestProcess, callee IUnknown
VBoxManage.exe: error: Context: "WaitForArray(ComSafeArrayAsInParam(aWaitStartFlags), gctlRunGetRemainingTime(msStart, cMsTimeout), &waitResult)" at line 1529 of file VBoxManageGuestCtrl.cpp
```

```cmd
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /tmp/a.sh
```
```
this is a test
```

```cmd
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /tmp/a.sh sample
```
```text
this is a test with argument: none received
```

```sh
#!/bin/sh
ARG=${1:-'none received'}
echo -n 'this is a test with argument: ' 
echo $ARG
```

```powershell
pushd "c:\Program Files\Oracle\VirtualBox"
$env:PASSWORD=
$env:VM='{7e261a39-d356-4eb1-a8ed-75675b149241}'
.\VBoxManage.exe guestcontrol $env:VM run --username sergueik --password $env:PASSWORD --exe /bin/sh -- -c "uname -a"
```
```powershell
.\VBoxManage.exe guestcontrol $env:VM run --username sergueik --password $env:PASSWORD --exe "/bin/sh -- -c 'uname -a'"
```
```text
VBoxManage.exe: error: No such file or directory on guest
VBoxManage.exe: error: Details: code VBOX_E_IPRT_ERROR (0x80bb0005), component GuestProcessWrap, interface IGuestProcess, callee IUnknown
VBoxManage.exe: error: Context: "WaitForArray(ComSafeArrayAsInParam(aWaitStartFlags), gctlRunGetRemainingTime(msStart, cMsTimeout), &waitResult)" at line 1529 of file VBoxManageGuestCtrl.cpp
```

```cmd
VBoxManage.exe guestcontrol %VM% run --username sergueik --password %PASSWORD% --exe /bin/sh -- /bin/sh -c "/tmp/a.sh sample"
```
```text
this is a test with argument: sample
```

```powershell
.\VBoxManage.exe guestcontrol $env:VM run --username sergueik --password $env:PASSWORD  --exe /bin/sh -- /bin/sh -c "/tmp/a.sh 'sample aergument with spaces'"
```
```text
this is a test with argument: sample aergument with spaces
```

#### How this Version Works


* Layer 1: VBoxManage
```
exe = /bin/sh
argv = ["/bin/sh", "-c", "/tmp/a.sh sample"]
```
* Layer 2: Linux shell (/bin/sh)
```
-c "/tmp/a.sh sample"
```
* Layer 3: your script
```
/tmp/a.sh sample
```
So the command only works because:

/bin/sh becomes the single deterministic interpreter boundary

2. Why the “extra /bin/sh” looks redundant but is required

This part:
```cmd
--exe /bin/sh -- /bin/sh -c ...
```

is what fixes VBoxManage’s strict argument model.

__VBoxManage__ does NOT reliably infer:

* `PATH` resolution
* shell interpretation
* command concatenation

So you explicitly anchor it twice:

|Part    |	Purpose |
|--------|----------|
|--exe /bin/sh	| actual process launched in guest|
|-- /bin/sh -c ...	|argv passed to that process |

This is redundant only syntactically — not semantically.

__VBoxManage__ `guestcontrol` is not a command executor — it is a process spawner with strict argv semantics.


### NOTE

By default, Ubuntu does not set a password for the root user: root account is effectively locked to prevent direct logins
