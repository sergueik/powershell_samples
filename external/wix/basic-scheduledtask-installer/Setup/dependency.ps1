param(
  [string] $logname = 'testlog',
  [int] $eventid = 1,
  [string] $source = 'testlog',
  [int]$interval_minutes = 1, # repetition interval in minutes, using recurring schedule
  [int]$delay = 2, # delay in minutes
  [string] $message
)

# NOTE: the "get-scheduledtask" cmdlet will not work on Windows 7
# upgrading Powershell will not help
# https://learn.microsoft.com/en-us/powershell/module/scheduledtasks/get-scheduledtask?view=windowsserver2019-ps
$taskname = 'ATASK' #  'A1' #
$tmpd = (get-date).AddMinutes($delay)
$time = get-date($tmpd) -format 'HH:mm'

$trigger = new-scheduledtasktrigger -Once -at $time -RepetitionInterval (new-timespan -Minutes $interval_minutes)
$settings_set = New-ScheduledTaskSettingsSet
$settings_set.StartWhenAvailable = $true
$settings_set.StopIfGoingOnBatteries = $false
$settings_set.DisallowStartIfOnBatteries = $false
$settings_set.WakeToRun = $true
$settings_set.runonlyifidle = $false


# NOTE:  taskpath argument cannot be empty

$data = get-scheduledtask | where-object { $_.taskname -eq $taskname}
write-eventlog -logname $logname -source $source -eventid $eventid  -entrytype  information -message ('Configuring the task: {0}' -f ($data -join ' ') )
[string]$taskpath = $data.TaskPath
# NOTE: HRESULT 0x80041319,Set-ScheduledTask
# $taskpath = '\'
# ERROR: The creation of the scheduled task failed. Reason: The Task Name may not contain the characters: < > : / \ |
# possibly due to taskpath
#  
set-scheduledtask -taskName $taskname -taskPath $taskpath -Settings $settings_set  -trigger $trigger
# $data = schtasks.exe /query /TN $taskname

write-eventlog -logname $logname -source $source -eventid $eventid  -entrytype  information -message ('Configured the task: {0}' -f ($data -join ' ') )
