#!/bin/bash

# Performs a non-interactive Docker login and returns a clear exit code
set -euo pipefail

REGISTRY="${1:-registry.mock.local}"
USERNAME="${2:-testuser}"
PASSWORD_FILE="${3:-/run/secrets/docker_password}"

echo "[INFO] Starting Docker login for ${REGISTRY}"

if [[ ! -f "$PASSWORD_FILE" ]]; then
  echo "[ERROR] Password file not found: $PASSWORD_FILE"
  exit 2
fi

PASSWORD="$(cat "$PASSWORD_FILE")"

# Non-interactive login
echo "$PASSWORD" | docker login "$REGISTRY" \
  -u "$USERNAME" \
  --password-stdin

RC=$?

if [[ $RC -eq 0 ]]; then
  echo "[INFO] Docker login successful"
  echo "authenticated" > /tmp/docker_auth_state
else
  echo "[ERROR] Docker login failed"
  echo "failed" > /tmp/docker_auth_state
fi

exit $RC


