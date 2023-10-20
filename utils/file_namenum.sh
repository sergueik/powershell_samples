LOG_FILE='last_run_report.yaml'
STATEDIR=$(/opt/puppetlabs/puppet/bin/puppet config print statedir)
if [ -d $STATEDIR ] ; then
  pushd $STATEDIR
  if [ -f $LOG_FILE ] ; then
    FILE_COUNT=$(ls -la $LOG_FILE.* 2>/dev/null| wc -l)
    >&2 echo "FILE_COUNT=$FILE_COUNT"
    NEXT_FILE=$(expr $FILE_COUNT + 1)
    >&2 echo "NEXT_FILE=$NEXT_FILE"
    >&2 echo "cp $LOG_FILE $LOG_FILE.$NEXT_FILE"
    cp $LOG_FILE $LOG_FILE.$NEXT_FILE
  fi
  popd
fi
