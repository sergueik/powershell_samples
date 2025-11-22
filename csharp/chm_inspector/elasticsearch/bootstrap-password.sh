#!/bin/bash
# NOTE: requires installing elasticsearch with
# xpack.security.enabled: true
set -e
while ! curl -sf http://localhost:9200 >/dev/null; do
  sleep 2
done

PW=$(bin/elasticsearch-reset-password -u elastic -a | awk '{print $NF}')
echo "$PW" > /run/secrets/elastic-password

