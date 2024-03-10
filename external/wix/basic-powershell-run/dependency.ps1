param(
  [string] $logname = 'testlog',
  [int] $eventid = 1,
  [string] $source = 'testlog',
  [string] $message
)
write-eventlog -logname $logname -source $source -eventid $eventid  -entrytype  information -message $message
