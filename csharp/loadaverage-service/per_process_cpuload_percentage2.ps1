# based on: https://www.cyberforum.ru/powershell/thread3078719.html#post16771711
# see also:
# real origin:
# https://www.reddit.com/r/PowerShell/comments/mnn62i/get_a_list_of_processes_and_their_cpu_usage/

# NOTE: countersamples names on Windows are localized (!)
# see also: https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference?source=recommendations
$Number = (Get-CimInstance Win32_Processor).NumberOfLogicalProcessors

#region example of % load average per (single) process
$process = '_total' 
$counter = ('\Process({0})\% Processor Time' -f $Process)
$value = (Get-Counter $counter -ea 0).CounterSamples.CookedValue / $Number
if ($value -gt 2) { $color = 'red' } else { $color = 'green' }
write-host -object ('{0}: {1}' -f $ProcessName, $value ) -foregroundcolor $color
#endregion

#region example of % load average per service
$Service = 'winmgmt'
$ProcessId = (Get-CimInstance -ClassName Win32_Service -Filter "Name='$Service'").ProcessId
$counter = '\process(*)\id process'
# finding expression like 'svchost#1'
# another example:'csrss#1'
# see also: https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/debugging-csrss

# $path = (Get-Counter -counter $counter -erroraction SilentlyContinue ).CounterSamples | where-object  -property RawValue -eq $processid |select-object -expandProperty Path
$path = Get-Counter -counter $counter -erroraction SilentlyContinue | foreach-object { write-output $_.CounterSamples }  | where-object  -property RawValue -eq $processid |select-object -expandProperty Path
# NOTE:
# $path -replace '^.*\((.*)\).*', $1
# does not capture 

$hosting_process = $path -replace '.*\((.*)\).*', '$1'
$counter = ('\Process({0})\% Processor Time' -f $hosting_process)
write-host ('Counter: {0}' -f $counter)

$value = (Get-Counter $counter -erroraction SilentlyContinue).CounterSamples.CookedValue/$Number
if ($value -gt 2) { $color = 'red' } else { $color = 'green' }
write-host -object ('{0} hosted in {1}: {2}' -f $Service, $hosting_process,  $value ) -foregroundcolor $color
#endregion

# example of service restart - requires elevation 
<#
if ($value -gt -1) {
    Write-Host -Object ('restart service {0}' -f  $Service )
    Get-Service -Name $Service | Restart-Service -Force -PassThru
}
#>