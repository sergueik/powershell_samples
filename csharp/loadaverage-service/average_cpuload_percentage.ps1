# based on: https://www.cyberforum.ru/powershell/thread3078719.html#post16748460
# TODO: convert from cmdlet to regular function
# see also: https://4sysops.com/archives/understanding-powershell-begin-process-and-end-blocks/
function get_average_cpuload_percentage {
  param (
    # average capacity
    [ValidateRange(1, 900)] [int]$count = 60,
    # action threshold
    [ValidateRange(0, 100)] [int]$threshold = 50,
    [scriptblock] $action = $null,
    [switch] $silent
  )
 
  begin { $values = @() }
 
  process {
  @(1..$count) | foreach-object {
    # alternatively collect from Win32_PerfFormattedData_PerfOS_Processor the PercentProcessorTime
    $value = get-ciminstance CIM_Processor | Select-Object -Property DeviceID, LoadPercentage
    # NOTE: no delay between probes
    if ($silent -eq $false) { write-host $value }
    # NOTE: with WMI need to extract the property from the query result
    $values += $value.LoadPercentage
  }
  }
  
  end {
    $average = ($values | Measure-Object -Average).Average
 
    $result = [PSCustomObject]@{
      Average = $average.tostring('##.##')
      Count = $count
      Threshold = $threshold
    }
 
    $result
  
    if ($average -ge $threshold -and $action) { & $action $result }
  }
}
<#
  Usage:
   # source the function
   . .\average_cpuload_percentage.ps1
   # define the callback
   [scriptblock]$action = { param ($x) write-host ('average value: {0}{1}' -f $x.Average, [char]10) }
average_cpuload_percentage -count 10 -threshold 1 -action $action -silent:$false

   # call the function in a regular way
   get_average_cpuload_percentage -count 10 -threshold 1 -action $action -silent:$false
  will print
  @{DeviceID=CPU0; LoadPercentage=3}
  @{DeviceID=CPU0; LoadPercentage=1}
  @{DeviceID=CPU0; LoadPercentage=9}
  @{DeviceID=CPU0; LoadPercentage=13}
  @{DeviceID=CPU0; LoadPercentage=6}
  @{DeviceID=CPU0; LoadPercentage=4}
  @{DeviceID=CPU0; LoadPercentage=3}
  @{DeviceID=CPU0; LoadPercentage=1}
  @{DeviceID=CPU0; LoadPercentage=4}
  @{DeviceID=CPU0; LoadPercentage=2}
  
  average value: 4.6
  
  Average Count Threshold
  ------- ----- ---------
  4.6        10         1


  # Original Usage sample:
  PS C:\> 1..12 | % { .\Get-CpuLoadPercentage -Count 5 -Threshold 11 -Action { param ($x) "ВЫСОКАЯ СРЕДНЯЯ НАГРУЗКА: $($x.Average)`n" } }
  Считывание показаний 5 раз и вычисление из них среднего и так 12 раз
  Если средняя нагрузка больше или равна Threshold, то сработает скриптблок
#> 
# see also: https://stackoverflow.com/questions/39943928/listing-processes-by-cpu-usage-percentage-in-powershell
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.management/get-process?view=powershell-7.3#outputs
# CPU(s): The amount of processor time that the process has used on all processors, in seconds.
# TODO: mormalization ?!
# see also:
# https://techcommunity.microsoft.com/t5/windows-powershell/fetch-top-10-processes-utilizing-high-cpu-as-shown-in-task/m-p/1239627
# Misses it: sums the time, not the percentage
# get-process -name vivaldi | select-object -expand CPU | measure-object -sum | select-object -expand Sum

<#
get-process -name vivaldi | select-object -property name,CPU
Name           CPU
----           ---
vivaldi    3.21875
vivaldi    3.65625
vivaldi   6.234375
vivaldi 209.078125
...
#>

# see also:
# https://www.pdq.com/blog/powershell-get-cpu-usage-for-a-process-using-get-counter/
# https://www.pdq.com/powershell/get-counter/

# see also:
# https://www.techtarget.com/searchenterprisedesktop/tip/How-IT-admins-can-use-PowerShell-to-monitor-CPU-usage
# https://www.reddit.com/r/PowerShell/comments/mnn62i/get_a_list_of_processes_and_their_cpu_usage/ - negative but clarifying
