#!/bin/bash

set -euo pipefail

USERNAME="${1:-vagrant}"
PASSWORD="${2:-vagrant}"
PASSWORD_FILE="${3}"
# NOTE: if no default value provided have to pass argument to prevent
# 3: unbound variable


if [[ ! -z "$PASSWORD_FILE" ]]; then
  if [[ ! -f "$PASSWORD_FILE" ]]; then
    1>&2 echo "[ERROR] Password file not found: $PASSWORD_FILE"
    exit 2
  else
    PASSWORD="$(cat "$PASSWORD_FILE")"
  fi
fi

echo "[INFO] Starting login test for ${USERNAME} ${PASSWORD}"

