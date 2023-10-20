# origin:
# https://www.reddit.com/r/PowerShell/comments/mnn62i/get_a_list_of_processes_and_their_cpu_usage/
# NOTE: ranked as not ideal
<#
Get-Counter | select-object -expandproperty CounterSamples| foreach-object { write-output ( $_.Path -replace "\\\\${env:COMPUTERNAME}", '' )} | format-list

Path
----
\network interface(intel[r] pro_1000 mt desktop adapter)\bytes total/sec
\network interface(isatap.{1d79c73f-a1cf-461e-8e3c-7d2285d89c9d})\bytes total/sec
\network interface(local area connection* 9)\bytes total/sec
\processor(_total)\% processor time
\memory\% committed bytes in use
\memory\cache faults/sec
\physicaldisk(_total)\% disk time
\physicaldisk(_total)\current disk queue length
#>
<#
$counter = '\process(*)\id process'
# $path = (Get-Counter -counter $counter -erroraction SilentlyContinue ).CounterSamples | where-object  -property RawValue -eq $processid |select-object -expandProperty Path
$path = Get-Counter -counter $counter -erroraction SilentlyContinue | foreach-object { write-output $_.CounterSamples }  | where-object  -property RawValue -eq $processid |select-object -expandProperty Path
# NOTE:
# $path -replace '^.*\((.*)\).*', $1
# does not capture 
$path -replace -replace '.*\((.*)\).*', '$1'

$processname = 'idle'
$counter = ('\process({0})\*' -f $processname)

# NOTE: bad defalt property ??
# get-counter -counter $counter | foreach-object {write-output $_.CounterSamples } | foreach-object { $line = $_ -replace "\\${env:COMPUTERNAME}", '';write-output $line }
# rows of
# Microsoft.PowerShell.Commands.GetCounter.PerformanceCounterSample

get-counter -counter $counter | foreach-object {write-output $_.CounterSamples } | foreach-object { write-output ( $_.Path -replace "\\\\${env:COMPUTERNAME}", '' )}

\process(idle)\% processor time
\process(idle)\% user time
\process(idle)\% privileged time
\process(idle)\virtual bytes peak
\process(idle)\virtual bytes
\process(idle)\page faults/sec
\process(idle)\working set peak
\process(idle)\working set
\process(idle)\page file bytes peak
\process(idle)\page file bytes
\process(idle)\private bytes
\process(idle)\thread count
\process(idle)\priority base
\process(idle)\elapsed time
\process(idle)\id process
\process(idle)\creating process id
\process(idle)\pool paged bytes
\process(idle)\pool nonpaged bytes
\process(idle)\handle count
\process(idle)\io read operations/sec
\process(idle)\io write operations/sec
\process(idle)\io data operations/sec
\process(idle)\io other operations/sec
\process(idle)\io read bytes/sec
\process(idle)\io write bytes/sec
\process(idle)\io data bytes/sec
\process(idle)\io other bytes/sec
\process(idle)\working set - private

#>
# see also:
# https://www.pdq.com/blog/powershell-get-cpu-usage-for-a-process-using-get-counter/
# https://www.pdq.com/powershell/get-counter/
$TotalCPUTime = (Get-CimInstance Win32_ComputerSystem).NumberOfLogicalProcessors * 100
$counter = '\Process(*)\% Processor Time'
$Result = (get-counter -counter $counter -sampleinterval 5 -maxsamples 12 -erroraction SilentlyContinue).CounterSamples.where({$_.InstanceName -notmatch '^(_total)$'}) |
    Group {$_.Instancename} | 
    Select @{Name = 'Computername'; Expression = {$_.group.path.split('\\')[2]} },
           @{Name='Process'; Expression = {$_.Name} },
           @{Name='AvgCPUTime';  Expression = {(($_.group.cookedvalue | measure-object -average).average ).tostring('####.##').PadLeft(5,'0')} },
           @{Name='AvgCPUTime%'; Expression = {(($_.group.cookedvalue | measure-object -average).average / $TotalCPUTime * 100).tostring('##.##').PadLeft(5,'0')} } |
    Sort AvgCPUTime -descending
$Result
