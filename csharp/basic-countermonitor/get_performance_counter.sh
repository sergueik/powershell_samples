#!/bin/bash

# get_performance_counter.sh
# Linux counterpart to get_performance_counter.ps1
# Collect per-process memory + I/O counters via pidstat
#
# Usage examples:
#   ./get_performance_counter.sh -name java -value example.way2automation.jar
#   ./get_performance_counter.sh -name java -value com.vendor.Main -i 1 -c 300 -o perf.log

set -euo pipefail

# verify pidstat availability early
if ! command -v pidstat >/dev/null 2>&1; then
  echo "[ERROR] pidstat not found. Please install sysstat package."
  exit 3
fi

NAME="java"
VALUE=""
INTERVAL=30
COUNT=20
OUTFILE="pidstat_mem.log"

usage() {
  cat <<'EOF'
Usage:
  get_performance_counter.sh -name <process_name> -value <jar_or_mainclass> [options]

Required:
  -name     Process executable name filter (example: java)
  -value    Main class, jar name, or any unique command-line fragment

Optional:
  -i        Sampling interval in seconds (default: 30)
  -c        Number of samples (default: 20)
  -o        Output log file (default: pidstat_mem.log)
  -h        Show help
EOF
}

while getopts ":n:v:i:c:o:h-:" opt; do
  case "$opt" in
    -)
      case "${OPTARG}" in
        name) NAME="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        value) VALUE="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        *) echo "Unknown option --${OPTARG}"; usage; exit 1 ;;
      esac
      ;;
    n) NAME="$OPTARG" ;;
    v) VALUE="$OPTARG" ;;
    i) INTERVAL="$OPTARG" ;;
    c) COUNT="$OPTARG" ;;
    o) OUTFILE="$OPTARG" ;;
    h) usage; exit 0 ;;
    :) echo "Option -$OPTARG requires an argument"; exit 1 ;;
    \?) echo "Invalid option: -$OPTARG"; exit 1 ;;
  esac
done

if [[ -z "$VALUE" ]]; then
  echo "[ERROR] -value is required"
  usage
  exit 1
fi

echo "[INFO] process name : $NAME"
echo "[INFO] grep value   : $VALUE"

PID=$(
  ps -ef |
  grep "$VALUE" |
  grep "${NAME:0:3}[${NAME:3:1}]${NAME:4}" |
  awk 'NR==1 {print $2}'
)

if [[ -z "$PID" ]]; then
  echo "[ERROR] process not found for value: $VALUE"
  exit 2
fi

echo "[INFO] found PID=$PID"
echo "[INFO] collecting memory and disk I/O counters to file every ${INTERVAL}s max ${COUNT} times"
echo "[INFO] writing pidstat output to $OUTFILE"

pidstat -H -r -d -p "$PID" "$INTERVAL" "$COUNT" | awk 'NR<=3 || /^[0-9]/ { printf( "%s | rss=%7.1f MB | vsz=%8.1f MB | majflt/s=%5s | cmd=%s\n", $1, $7/1024, $6/1024, $5, $9 ); fflush()}' | tee "$OUTFILE"
