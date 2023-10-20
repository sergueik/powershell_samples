param (
  [String]$name = 'DBWriter',
  [int]$command = 200
)
[System.Reflection.Assembly]::LoadWithPartialName('System.ServiceProcess') | Out-Null
try {
  [System.ServiceProcess.ServiceController] $controller = new-object System.ServiceProcess.ServiceController($name)
}  catch [Exception] {
  $message = $_.Exception.Message
  write-host -foregroundcolor 'red' ('Exception: ' +  $message)
  # WindowsService.NET was not found on computer '.'
  # Cannot control WindowsService.NET service on computer '.'
}

$execute_ok = $false
if ($controller -ne $null){
  if ( $controller.Status -eq 'Running') {
    $execute_ok = $true
  } else { 
    write-host -foregroundcolor 'red' ('{0} not running' -f $name) 
  }
} else { 
  write-host -foregroundcolor 'red' ('{0} not found' -f $name) 
}
if ($execute_ok) {
  try {
    write-host -foregroundcolor 'Green' ('Sending {0} to {1}' -f $command, $name)
    $controller.ExecuteCommand($command)
  } catch [Exception] {
    # WindowsService.NET was not found on computer '.'
    # Cannot control WindowsService.NET service on computer '.'
    $message = $_.Exception.Message
    write-host -foregroundcolor 'red' ('Exception: ' +  $message)
  }
}

$filepath = 'c:\customers.db'
if (test-path -path $filepath) {
  get-content -path $filepath
}