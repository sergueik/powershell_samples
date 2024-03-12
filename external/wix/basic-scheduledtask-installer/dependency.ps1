param(
  [string] $logname = 'testlog',
  [int] $eventid = 1,
  [string] $source = 'testlog',
  [string] $message
)
# NOTE: the "get-scheduledtask" cmdlet will not work on Windows 7
# upgrading Powershell will not help
# https://learn.microsoft.com/en-us/powershell/module/scheduledtasks/get-scheduledtask?view=windowsserver2019-ps
$taskname = 'ATASK' #  'A1' #
# $data = get-scheduledtask | where-object { $_.taskname -eq $taskname}
$data = schtasks.exe /query /TN $taskname
write-eventlog -logname $logname -source $source -eventid $eventid  -entrytype  information -message ('task: {0}' -f ($data -join ' ') )
