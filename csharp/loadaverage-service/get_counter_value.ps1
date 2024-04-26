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
  [string]$category = $null,
  [string]$counter = $null,
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

RawValue: 3150073856
RawValue: 3149938688 CookedValue: 3149938688


${name} -Category Processor -Counter '% Processor Time' -help

% Processor Time is the percentage of elapsed time that the processor spends to execute a non-Idle thread. It is calculated by measuring the percentage of time that the processor spends executing the idle thread and then subtracting that value from 100%. (Each processor has an idle thread that consumes cycles when no other threads are ready to run). This counter is the primary indicator of processor activity, and displays the average percentage of busy time observed during the sample interval. It should be noted that the accounting calculation of whether the processor is idle is performed at an internal sampling interval of the system clock (10ms). On todays fast processors, % Processor Time can therefore underestimate the processor utilization as the processor may be spending a lot of time servicing threads between the system clock sampling interval. Workload based timer applications are one example  of applications  which are more ikely to be measured inaccurately as timers are signaled just after the sample is taken.

${name} -Category Processor -Counter '% Processor Time' -instance '0' -debug

Category: "Processor" Counter: "% Processor Time" Intance: "0"
RawValue: 65955468750
RawValue: 65955468750 CookedValue: 0
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
    write-output ('Category: "{0}"  Instance: "{1}" Counter: "{1}"' -f $category, $counter, $instance)
  } else {
    write-output ('Category: "{0}" Counter: "{1}"' -f $category, $counter)
  }
}
write-output ('RawValue: {0}' -f $o.RawValue)
# run system cmdlet
if ($Instance -ne '') {
  $p = ( '\{0}({1})\{2}' -f $category, $instance, $counter)
  } else {
  $p = (  '\{0}\{1}' -f $category, $counter)
}
$o = Get-Counter -counter $p
$s = $o.CounterSamples
$r = $s |Get-Member -Name CookedValue
if ($debug_flag) {
  # NOTE: not getting the value
  $r
}
$data = $s | select-object -property CookedValue,TimeBase,RawValue
write-output ('RawValue: {0} CookedValue: {1}' -f $data.RawValue , $data.CookedValue )
 


# write-output $r.CookedValue
<#
# NOTE: need to find information about some counter rawvalue normalization

.\get_counter_value.ps1 -category PhysicalDisk -counter '% Idle Time'
        <add key="CategoryName" value="PhysicalDisk"/>
        <add key="CounterName" value="% Idle Time"/>
        <add key="InstanceName" value="0 C: D:"/>
$category = 'Processor'
$instance =  '0'
$counter =  '% Processor Time'
$o = new-object System.Diagnostics.PerformanceCounter
$categoryName = $category 
$instanceName = $instance
$counterName = $counter
write-output $o
write-output $o.RawValue
# = 1111503437500

CategoryName     : PhysicalDisk
CounterHelp      : % Idle Time reports the percentage of time during the
                   sample interval that the disk was idle.
CounterName      : % Idle Time
CounterType      : 542573824
InstanceLifetime : Global
InstanceName     : 0
ReadOnly         : True
MachineName      : .
RawValue         :
Site             :
Container        :

% Processor Time
$o = Get-Counter -counter  '\Processor(0)\% Processor Time'

$o | select-object -property * | format-list

Readings       : \\lenovo-pc\processor(0)\% processor time :
                 0.0021399542049827


Timestamp      : 4/25/2024 1:15:39 PM
CounterSamples : {0}

$s = $o.CounterSamples

$s| select-object -property *


Path             : \\lenovo-pc\processor(0)\% processor time
InstanceName     : 0
CookedValue      : 0.0021399542049827
RawValue         : 97964843750
SecondValue      : 133585389397873252
MultipleCount    : 1
CounterType      : Timer100NsInverse
Timestamp        : 4/25/2024 1:15:39 PM
Timestamp100NSec : 133585245397870000
Status           : 0
DefaultScale     : 0
TimeBase         : 10000000

$s| get-member

# NOTE: the CookedValue is a property of PerformanceCounterSample object
# it existed in Powershell Version 2.0 already, but not in the .Net
# The PerformanceCounterSample class is not in System.Diagnostics namespace
# it is not available to plain .Net application
# https://learn.microsoft.com/en-us/dotnet/api/microsoft.powershell.commands.getcounter.performancecountersample?view=powershellsdk-1.1.0

   TypeName: Microsoft.PowerShell.Commands.GetCounter.PerformanceCounterSample

Name             MemberType Definition
----             ---------- ----------
Equals           Method     bool Equals(System.Object obj)
GetHashCode      Method     int GetHashCode()
GetType          Method     type GetType()
ToString         Method     string ToString()
CookedValue      Property   double CookedValue {get;set;}
CounterType      Property   System.Diagnostics.PerformanceCounterType Counte...
DefaultScale     Property   uint32 DefaultScale {get;set;}
InstanceName     Property   string InstanceName {get;set;}
MultipleCount    Property   uint32 MultipleCount {get;set;}
Path             Property   string Path {get;set;}
RawValue         Property   uint64 RawValue {get;set;}
SecondValue      Property   uint64 SecondValue {get;set;}
Status           Property   uint32 Status {get;set;}
TimeBase         Property   uint64 TimeBase {get;set;}
Timestamp        Property   datetime Timestamp {get;set;}
Timestamp100NSec Property   uint64 Timestamp100NSec {get;set;}


# NOTE: apparenty the CounterSamples membertype is a Property
# however a plain getter '.' call does not retrieve its Property. 
# It apparently behaves similar to NoteProperty
# see also:
# https://stackoverflow.com/questions/29141914/what-is-a-powershell-noteproperty
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/get-member?view=powershell-5.1
$s.RawValue
# the property assess was problematic in early version of Powershell:
# 2.x
$s.RawValue -eq $null
$True
# 5.x
$s.RawValue -eq $null
False
$s |Get-Member -Name RawValue


   TypeName: Microsoft.PowerShell.Commands.GetCounter.PerformanceCounterSample

Name     MemberType Definition
----     ---------- ----------
RawValue Property   System.UInt64 TimeBase {get;set;}


$r = $s |Get-Member -Name RawValue
$r |select-object -property * |format-list


TypeName   : Microsoft.PowerShell.Commands.GetCounter.PerformanceCounterSample
Name       : RawValue
MemberType : Property
Definition : System.UInt64 RawValue {get;set;}
# this is becoming too complex. The following workaround is possible.
# for general solution, see `ConvertTo-Hashtable` (parse_json_hash.ps1)
# origin: https://4sysops.com/archives/convert-json-to-a-powershell-hash-table/
# see also: https://github.com/sergueik/powershell_ui_samples/blob/master/external/parse_json_hash.ps1
# http://blogs.msdn.com/b/timid/archive/2013/03/05/converting-pscustomobject-to-from-hashtables.aspx
 
$s | select-object -property CookedValue,TimeBase,RawValue

               CookedValue                   TimeBase                  RawValue
               -----------                   --------                  --------
                    1.5625                   10000000                1304531250

$data = $s | select-object -property CookedValue,TimeBase,RawValue


$data.RawValue / $data.TimeBase
130.453125
$data.CookedValue
1.5625
#>

