param(
  [String]$logname = 'LoadAverageCounterServiceLog',
  [String]$source = 'LoadAverageCounterService',
  [int]$interval = 2,
  [int]$count = 4,
  [switch]$now = $true,
  [switch]$debug
)


$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$event_date = (get-date).AddMinutes(-1 * $interval )
# NOTE: c:\windows\system32\wevutil.exe did not exist on Windows 7
<#
  if ($debug_flag) {
    invoke-expression -command "c:\windows\system32\wevutil.exe gl ${logname}"
  }
#>
# TODO: better handle get-eventlog : No matches found
# error
if ([System.Diagnostics.Eventlog]::Exists($logname)) {
  get-eventlog -logname $logname -newest $count -after $event_date -source $source -erroraction stop|
  sort-object -descending -property date |
  select-object -property TimeGenerated,Message,InstanceID |
  format-list
} else {
  write-host ('The log {0} is not found' -f $logname)
}