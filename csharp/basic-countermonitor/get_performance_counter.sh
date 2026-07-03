#!/bin/bash

# get_performance_counter.sh
# Linux counterpart to get_performance_counter.ps1
# Collect per-process memory + I/O counters via pidstat. Optionally post to Prometheus Pushgateway
#
# Usage examples:
#   ./get_performance_counter.sh --name java --jar example.way2automation.jar
#   ./get_performance_counter.sh --name java --main com.vendor.Main -i 1 -c 300 -o perf.log
#   ./get_performance_counter.sh --name java --jar example.way2automation.jar -i 1 -c 300 --url http://localhost:9091/metrics/job/pidstat

set -euo pipefail

# confirm pidstat availability early
if ! command -v pidstat >/dev/null 2>&1; then
  echo '[ERROR] pidstat not found. Please install sysstat package'
  exit 3
fi

NAME='java'
VALUE=''
JAR=''
URL=''
MAIN_CLASS=''
INTERVAL=30
COUNT=20
OUTFILE='performance_counter.log'
FORMAT='pretty'

usage() {
  cat <<'EOF'
Usage:
  get_performance_counter.sh -name <process_name> -jar <jar> -main <mainclass> [options]

Required:
  -name     Process executable name filter (example: java)
  -main     Main class or any unique command-line fragment
  -jar      Jar name

Optional:
  -f        Output format: pretty|csv (default: pretty)
  -i        Sampling interval in seconds (default: 30)
  -c        Number of samples (default: 20)
  -o        Output log file (default: performance_counter.log)
  -h        Show help
  -url      Prometheus pushgateway url to post the metrics (e.g. http://localhost:9091/metrics/job/pidstat )
EOF
}

while getopts ':n:j:m:u:f:i:c:o:h-:' opt; do
  case "$opt" in
    -)
      case "${OPTARG}" in
        name) NAME="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        jar) JAR="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        main) MAIN_CLASS="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        url) URL="${!OPTIND}"; OPTIND=$((OPTIND + 1)) ;;
        *) echo "Unknown option --${OPTARG}"; usage; exit 1 ;;
      esac
      ;;
    n) NAME="$OPTARG" ;;
    j) JAR="$OPTARG" ;;
    m) MAIN_CLASS="$OPTARG" ;;
    f) FORMAT="$OPTARG" ;;
    i) INTERVAL="$OPTARG" ;;
    c) COUNT="$OPTARG" ;;
    o) OUTFILE="$OPTARG" ;;
    u) URL="$OPTARG" ;;
    h) usage; exit 0 ;;
    :) echo "Option -$OPTARG requires an argument"; exit 1 ;;
    \?) echo "Invalid option: -$OPTARG"; exit 1 ;;
  esac
done

# use valid bash syntax
if [[ -z "$JAR" ]] && [[ -z "$MAIN_CLASS" ]]; then
  echo '[ERROR] -jar or -main argument is required (usually just one)'
  usage
  exit 1
fi

VALUE=$(echo "${JAR} ${MAIN_CLASS}" | sed 's/[[:space:]]*$//')

echo "[INFO] process name : ${NAME}"
echo "[INFO] grep values  : ${VALUE}"
if [[ ! -z "$URL" ]]; then
  echo "[INFO] url          : ${URL}"
fi
echo  "ps -ef | grep \"$VALUE\" | grep \"${NAME:0:3}[${NAME:3:1}]${NAME:4}\" | awk 'NR==1 {print \$2}'"

PID=$(ps -ef | grep "$VALUE" | grep "${NAME:0:3}[${NAME:3:1}]${NAME:4}" | awk 'NR==1 {print $2}' )

if [[ -z "$PID" ]]; then
  echo "[ERROR] process not found for name: ${NAME} value: ${VALUE}"
  exit 2
fi

echo "[INFO] found PID=$PID"
ps -fp "$PID"
pidstat -H -r -p "$PID" 1 1
echo "[INFO] collecting process memory counters every ${INTERVAL} sec max ${COUNT} times"
if [[ ! -z "$URL" ]]; then
  echo "[INFO] uploading metrics to ${URL}"
  # convert pidstat fields into prometheus gauges and post to prometheus
  # NOTE: se parameter expansion with a defaul
  if [[ ! -z "${TMP:-}" ]] ; then
    TMP_LOGFILE=$TMP/log.$$.txt
  else 
    TMP_LOGFILE=/tmp/log.$$.txt
  fi
  # NOTE: With -H, the first column in pidstat is the epoch time
  # NOTE: -v creates an awk variable, not a shell variable therefore inside the awk program
  # one reference it as PID, not $PID
  # NOTE: one cannot and do not need to pass timestamps explicitly to the Prometheus Pushgateway. 
  SAMPLE_COUNT=1
  while [ "$SAMPLE_COUNT" -le "$COUNT" ]; do
    pidstat -H -r -p "$PID" 1 1 |
    awk -v PID=$PID '/UID/{next} /^[0-9]/{pid=PID;rss=$7/1024;vsz=$6/1024;maj=$5;cmd=$NF;gsub(/"/,"\\\"",cmd);label="pid=\""pid"\",cmd=\""cmd"\"";printf "process_rss_mb{%s} %.1f\nprocess_vsz_mb{%s} %.1f\nprocess_majflt{%s} %s\n",label,rss,label,vsz,label,maj;fflush()}' | tee "$TMP_LOGFILE" /dev/stderr | curl --data-binary @- $URL
	if [ "$SAMPLE_COUNT" -lt "$COUNT" ]; then
		sleep "$INTERVAL"
	fi
	SAMPLE_COUNT=$((SAMPLE_COUNT + 1))
   done
else
  echo "[INFO] writing pidstat output to $OUTFILE"
  if [[ "$FORMAT" == "csv" ]]; then
    echo 'time,rss_mb,vsz_mb' | tee "$OUTFILE" /dev/stderr > /dev/null
    pidstat -H -r -p "$PID" "$INTERVAL" "$COUNT" | awk '/^[0-9]/ { printf "%s,%.1f,%.1f\n", $1, $7/1024, $6/1024; fflush() }' | tee -a "$OUTFILE" /dev/stderr > /dev/null
  else
    pidstat -H -r -p "$PID" "$INTERVAL" "$COUNT" | awk ' /UID/ { next } /^[0-9]/ {printf( "%s | rss=%7.1f MB | vsz=%8.1f MB | majflt/s=%5s | cmd=%s\n", $1, $7/1024, $6/1024, $5, $9 ); fflush();}' | tee "$OUTFILE"
  fi
fi


# unused
declare -A TAG_MAP

# truncate and normalize the jar name stripping the version and extension and replacing dots and dashes
SAMPLE_JAR=$JAR
SAMPLE_JAR=foobar-server-1.0.0-SNAPSHOT.jar
TAG_KEY=$(echo "$SAMPLE_JAR" | sed 's/\.jar$//' | sed 's/SNAPSHOT//' | sed 's/-[0-9].*$//' | tr '.' '_' | tr '-' '_' | cut -c1-40)
TAG_MAP['com.enterprise.payment.service']='payment'
TAG_MAP['com.enterprise.auth.service']='auth'
TAG_MAP['foobar_server']='server_foobar'
TAG="${TAG_MAP[$TAG_KEY]:-$TAG_KEY}"

echo "tag: ${TAG}"

