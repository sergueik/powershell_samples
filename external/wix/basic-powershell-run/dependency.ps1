param(
  [string] $logname = 'testlog',
  [string] $source = 'testlog',
  [string] $message
)
write-eventlog -logname $logname -source $source -eventid 1 -entrytype  information -message $message
