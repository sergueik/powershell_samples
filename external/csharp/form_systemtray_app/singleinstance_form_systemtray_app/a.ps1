# origin:
# https://www.reddit.com/r/PowerShell/comments/mnn62i/get_a_list_of_processes_and_their_cpu_usage/
# ranked as not ideal, 
$TotalCPUTime = (Get-CimInstance Win32_ComputerSystem).NumberOfLogicalProcessors * 100
$Result = (get-counter -Counter '\Process(*)\% Processor Time' -SampleInterval 5 -MaxSamples 12 -ErrorAction SilentlyContinue).CounterSamples.where({$_.InstanceName -notmatch '^(_total)$'}) |
    Group {$_.Instancename} | 
    Select @{Name = 'Computername'; Expression = {$_.group.path.split('\\')[2]} },
           @{Name='Process'; Expression = {$_.Name} },
           @{Name='AvgCPUTime'; Expression = {(($_.group.cookedvalue | measure-object -average).average ).tostring('####.##').PadLeft(5,'0')} },
           @{Name='AvgCPUTime%';Expression = {(($_.group.cookedvalue | measure-object -average).average / $TotalCPUTime * 100).tostring('##.##').PadLeft(5,'0')} } |
    Sort AvgCPUTime -descending
$Result