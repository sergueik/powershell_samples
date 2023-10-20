param(
  [String]$logname = 'TransactionLog',
  [String]$source = 'TransactionService',
  [switch]$remove,
  [switch]$debug
)



$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()

$remove_flag = [bool]$PSBoundParameters['remove'].IsPresent -bor $remove.ToBool()

# NOTE: there is no "SubtractMinutes" method in System.DateTime
# the "Subtract" is unrelated
$event_date = (get-date).AddMinutes(-1 * $interval )
[int]$count = 5

if ($debug_flag) {
  invoke-expression -command "c:\windows\system32\wevutil.exe get-log `"${logname}`""
}

if ([System.Diagnostics.Eventlog]::Exists($logname)) {
  get-eventlog -logname $logname -newest $count -after $event_date -source $source -erroraction stop|
  sort-object -descending -property date |
  select-object -property TimeGenerated,Message,InstanceID |
  format-list
} else {
  write-host ('The log {0} is not found' -f $logname)
}

if ($remove_flag) { 
  if ([System.Diagnostics.Eventlog]::Exists($logname)) {
    new-eventlog -logname $logname -erroraction stop
  } else {
    write-host ('Event log {0}  not found' -f $logname)
  }
} else { 
  if ([System.Diagnostics.Eventlog]::Exists($logname)) {
    write-host ('Event log {0} already exists' -f $logname)
  } else {
    try{ 
      [string]$source = $logname
      new-eventlog -logname $logname -source $source -erroraction stop
    } catch [Exception] { 
      write-output ('Exeption(ignored:{0}{1}' -f "`n", $_.Exception.Message )
    }
  }
}
# TODO: c:\Windows\System32\wevtutil.exe im | install-manifest 
# Install event publishers and logs from manifest.

