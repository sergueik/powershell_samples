\# VM Appliance Docker Login Flow (VirtualBox Guest Script Model)



\## Goal



Provide a deterministic, non-interactive mechanism to perform Docker authentication inside a Linux VM appliance, triggered from a Windows host via:



\- `VBoxManage guestcontrol`

\- a VM-resident shell script

\- optional mocked registry endpoint for testing



This design avoids:

\- interactive login sessions

\- GUI dependencies

\- state inference on the host



\---



\## 1. VM-side script: `/opt/appliance/docker-login.sh`



\### Purpose

Performs a non-interactive Docker login and returns a clear exit code.



\### Script



```bash

\#!/bin/bash

set -euo pipefail



REGISTRY="${1:-registry.mock.local}"

USERNAME="${2:-testuser}"

PASSWORD\_FILE="${3:-/run/secrets/docker\_password}"



echo "\[INFO] Starting Docker login for ${REGISTRY}"



if \[\[ ! -f "$PASSWORD\_FILE" ]]; then

&#x20; echo "\[ERROR] Password file not found: $PASSWORD\_FILE"

&#x20; exit 2

fi



PASSWORD="$(cat "$PASSWORD\_FILE")"



\# Non-interactive login

echo "$PASSWORD" | docker login "$REGISTRY" \\

&#x20; -u "$USERNAME" \\

&#x20; --password-stdin



RC=$?



if \[\[ $RC -eq 0 ]]; then

&#x20; echo "\[INFO] Docker login successful"

&#x20; echo "authenticated" > /tmp/docker\_auth\_state

else

&#x20; echo "\[ERROR] Docker login failed"

&#x20; echo "failed" > /tmp/docker\_auth\_state

fi



exit $RC

