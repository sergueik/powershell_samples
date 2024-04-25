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
$name = 'makeconf.ps1'
  write-host @"
Example Usage:
${name} -category CATEGORY -counter COUNTER [-instance INSTANCE]

return the raw value of specified performance counter
"@
  exit
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()

$o = new-object System.Diagnostics.PerformanceCounter
$o.CategoryName = $category
$o.CounterName = $counter
$o.InstanceName = $instance
write-output $o.RawValue

<#
# NOTE: need to find information about some counter rawvalue normalization
$categoryName = 'Processor'
$instanceName = '0'
$counterName = '% Processor Time'
RawValue = 1111503437500
#>


