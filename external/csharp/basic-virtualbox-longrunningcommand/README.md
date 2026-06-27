### VM Appliance Docker Login Flow (VirtualBox Guest Script Model)



#### Goal



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