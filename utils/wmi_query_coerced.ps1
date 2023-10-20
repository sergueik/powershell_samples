param (
  [string]$name = 'powershell.exe',
  [switch]$stop,
  [switch]$debug
)
$wmi_results = get-wmiobject -class 'win32_process' -filter "name = '${name}'"
$results_array  = @() ;
$wmi_results | foreach-object { $result = $_; $results_array += $result }
write-host ('coerced results count: {0}' -f $results_array.Count)

if ($wmi_results.Count -ne $null) {
  write-host('number of rows in the original result: {0}' -f $wmi_results.Count)
} else {
  write-host('only one row in the result')
}

0..($results_array.Count - 1) | foreach-object {
  $cnt = $_
  write-host('Processing item # {0}' -f $cnt)
  $result = $results_array.Item($cnt) 
  $tagret_processid = $result.ProcessId
  if ([bool]$PSBoundParameters['stop']) {
    invoke-expression -command "taskkill.exe /F /PID ${tagret_processid}"
  } else {
    @(
      'Name', 
      'ProcessId', 
      'ParentProcessId', 
      'CommandLine') | foreach-object { 
      write-output ('{0}: {1}' -f $_, $result."$_")
    }
  }
}