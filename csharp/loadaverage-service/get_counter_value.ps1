#Copyright (c) 2024 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

param (
  [string]$instance = $null,
  [string]$category = 'System',
  [string]$counter = 'Processor Queue Length',
  [switch]$help,
  [switch]$debug

)
# https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics?view=netframework-4.5

if ([bool]$psboundparameters['help'].ispresent) {
  if ($category -ne '') {
    if ($counter -ne '') {
      $o = new-object System.Diagnostics.PerformanceCounter
      $o.CategoryName = $category
      $o.CounterName = $counter
      if ($Instance -ne '') {
        $o.InstanceName = $instance
      }
      write-output $o.CounterHelp
      exit
    }
  }
  $name = 'get_counter_value.ps1'
  write-host @"
Usage:
${name} -category CATEGORY -counter COUNTER [-instance INSTANCE]

return the raw value of specified performance counter
Example usage:

${name} -Category Memory -Counter 'Available bytes' -help

Available Bytes is the amount of physical memory, in bytes, immediately availab
le for allocation to a process or for system use. It is equal to the sum of mem
ory assigned to the standby (cached), free and zero page lists.

${name} -Category Memory -Counter 'Available bytes'

13079433216


${name} -Category Processor -Counter '% Processor Time' -help

% Processor Time is the percentage of elapsed time that the processor spends to execute a non-Idle thread. It is calculated by measuring the percentage of time that the processor spends executing the idle thread and then subtracting that value from 100%. (Each processor has an idle thread that consumes cycles when no other threads are ready to run). This counter is the primary indicator of processor activity, and displays the average percentage of busy time observed during the sample interval. It should be noted that the accounting calculation of whether the processor is idle is performed at an internal sampling interval of the system clock (10ms). On todays fast processors, % Processor Time can therefore underestimate the processor utilization as the processor may be spending a lot of time servicing threads between the system clock sampling interval. Workload based timer applications are one example  of applications  which are more ikely to be measured inaccurately as timers are signaled just after the sample is taken.

${name} -Category Processor -Counter '% Processor Time' -instance '0' -debug

Category: "Processor" Counter: "% Processor Time" Intance: "0"
65955468750
"@
  exit
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()

$o = new-object System.Diagnostics.PerformanceCounter
$o.CategoryName = $category
$o.CounterName = $counter
$o.InstanceName = $instance
if ($debug_flag) {
  if ($Instance -ne '') {
    write-output ('Category: "{0}" Counter: "{1}" Instance: "{2}"' -f $category, $counter, $instance)
  } else {
    write-output ('Category: "{0}" Counter: "{1}"' -f $category, $counter)
  }
}
write-output $o.RawValue



