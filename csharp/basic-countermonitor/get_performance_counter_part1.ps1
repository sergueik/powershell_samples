#Copyright (c) 2026 Serguei Kouzmine
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
  [int] $processid = 42068,
  [string] $value = 'example.Appication',
  [string] $name = 'java.exe',
  [switch]$debug # currently unused
)

[bool]$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'
if ($debug_flag){
  $DebugPreference = 'Continue'
}

function get_process_id_by_commandline_debug {
  param (
    $name = $null,
    $value = $null
 )
  write-host ('name: "{0}" value: "{1}"' -f $name, $value)

  # failing
  # the modern CIM interface (successor to WMI)
  # NOTE uses yet another own version of pseudo-SQL,but simply does not work at all:
  # Get-WmiObject Win32_Process -Filter "Name = '$processName'"
  # NOTE: wmic.exe  and [System.Diagnostics.ManagementObjectSearcher] take advantage of
  # WMI close lookalike pseudo-SQL WQL LIKE with %value% wildcard
  # matching against Win32_Process.CommandLine members
  # to locate the target ProcessId by partial command-line contents.
  # NOTE: preserve WMI vendor class/property mixed camel snake style for readability.
  <#
  Get-CimInstance -InputObject Win32_Process -query "commandline -like '%${name}%'"
  # Get-CimInstance : Cannot bind parameter 'Query' to the target. Exception setting "Query":
  # "Unable to resolve the parameter set name."
    $data = Get-CimInstance -InputObject Win32_Process -query "Name = '$name'" |
      Where-Object { $_.CommandLine -like "*${value}*" } |
      Select-Object Name, ProcessId, CommandLine
  #>
  write-output  'take 1'
  $data = get-WmiObject -Class Win32_Process -filter "Name = 'java.exe'"|
  where-object { $_.commandline -match $value }  |
  Select-Object Name, ProcessId, CommandLine
  write-output ('name: {0} processid: {1}' -f $data.Name, $data.ProcessId )  | format-list
  # [string]$filter=

  # write-output $data   | format-list

  write-output  'take 2'
  [string]$filter="Name = '${name}'"
  write-output ('filter: {0}' -f $filter )
  $data = get-WmiObject -Class Win32_Process -filter $filter |
  #  where-object { $_.CommandLine -match $value } |
  Select-Object Name, ProcessId, CommandLine
  # parsing "*example.Appication*" - Quantifier {x,y} following nothing
  write-output ('{0} rows' -f $data.Count)
  if ($data.Count -gt 0 ) {
    0..($data.Count-1) | foreach-object {
       $cnt = $_
       write-output ('name: {0} processid: {1}' -f $data[$cnt].Name, $data[$cnt].ProcessId )  | format-list
    }
  }

  write-output  'take 3'
  [string]$filter="Name = '${name}'"
  write-output ('filter: {0}' -f $filter )
  $data = get-WmiObject -Class Win32_Process -filter $filter |
  Select-Object Name, ProcessId, CommandLine
  # parsing "*example.Appication*" - Quantifier {x,y} following nothing
  write-output ('{0} rows' -f $data.Count)
  if ($data.Count -gt 0 ) {
    0..($data.Count-1) | foreach-object {
       $cnt = $_
       write-debug $data[$cnt].CommandLine

       if ($data[$cnt].CommandLine -match $value) {
         write-output ('CommanLine matched "{0}"' -f $value )
         write-output ('name: {0} processid: {1}' -f $data[$cnt].Name, $data[$cnt].ProcessId )  | format-list
       }

    }
  }

  write-output  'take 4'
  $data = get-WmiObject -Class Win32_Process -filter "Name = '${name}'" |
  where-object { $_.CommandLine -match $value } |
  Select-Object Name, ProcessId, CommandLine
  if ($data -eq $null ) {
    write-output 'no data'
  } else {
     write-output ('name: {0} processid: {1}' -f $data.Name, $data.ProcessId )
     # | format-list
  }
  write-output  'done'
}

function get_process_id_by_commandline {
  param (
    $name = $null,
    $value = $null
 )
  [string]$result = $null
  write-host ('name: "{0}" value: "{1}"' -f $name, $value)
  [string]$filter="Name = '${name}'"
  write-debug ('filter: {0}' -f $filter )
  $data = get-WmiObject -Class Win32_Process -filter $filter |
  select-object -property Name,ProcessId,CommandLine
  write-debug ('{0} rows' -f $data.Count)
  if ($data.Count -gt 0 ) {
    0..($data.Count-1) | foreach-object {
      $cnt = $_
      write-debug $data[$cnt].CommandLine

      if ($data[$cnt].CommandLine -match $value) {
        $result = $data[$cnt].ProcessId
        write-debug ('CommanLine matched: "{0}"' -f $value )
        write-debug ('name: {0} processid: {1}' -f $data[$cnt].Name, $data[$cnt].ProcessId ) | format-list
      } else {
        write-debug ('CommanLine matched "{0}"' -f $value )
        write-debug $data[$cnt].CommandLine
      }
    }
  }
  $result
}


function get_performance_counter_instance {
  param (
    [int] $processid = -1,
    [string] $name = 'chrome'
 )

  [System.Diagnostics.PerformanceCounterCategory] $performanceCounterCategory = new-object System.Diagnostics.PerformanceCounterCategory -argumentlist ([string]'Process')
  $performanceCounterCategory.GetInstanceNames() | foreach-object {
    $instanceName = $_
    if (-not ($instanceName -match $name)) {
      return
    }
    write-debug ('instanceName: {0}' -f $instanceName)
    $performanceCounter = new-object System.Diagnostics.PerformanceCounter('Process', 'ID Process', $instanceName, $true)
    $rawValue  = $performanceCounter.RawValue
    write-debug ('rawValue: {0}' -f $rawValue)
    if ($rawValue -eq $processid) {
      write-output $instanceName
      # optional - exit for-each
    }
  }
}


$processid = get_process_id_by_commandline -name $name -value $value
write-output ('processid: {0}' -f $processid)
# NOTE:
$name = 'java'
$instance = get_performance_counter_instance -processid $processid -name $name
write-output ('peformance counter instance: {0}' -f $instance)
