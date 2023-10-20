param(
  [string]$config_file = ([System.IO.Path]::Combine((resolve-path -path '.').path,'configB.json')),
  [string]$hostname = "${env:COMPUTERNAME}".ToLower(),
  [string]$key = 'PORTS',
  [string]$schema = 'A',
  [switch]$debug
)


function load_configB {
  param(
    [string]$config_file,
    [string]$hostname,
    [bool]$debug
  )
  $data = (Get-Content -Path $config_file) -join "`n"
  if ($debug){
    Write-host ('data: "{0}"' -f $data)
  }

  $json_rows= ConvertFrom-Json -InputObject $data
  $result_rows = @()
  if ($debug) {
    write-host ('Load {0} rows JSON' -f $json_rows.Count)
  }
  $json_rows| foreach-object {
    $json_obj = $_
    $result = @{}
    $json_obj | Get-Member -MemberType NoteProperty | ForEach-Object {
      $hostname_key = $_.Name
      $result[$hostname_key] = @{}
      # NOTE the syntax
      $data = $json_obj."$(${hostname_key})"
      $data_hash_obj = @{}
      $data | Get-Member -MemberType NoteProperty | ForEach-Object {
        $data_key = $_.Name
        $value = $data."$(${data_key})"
        $data_hash_obj[$data_key] = $value
      }
      $result[$hostname_key] = $data_hash_obj
    }
    $result_rows += $result
  }
  if ($debug) {
    write-host ('Inpect {0} rows' -f $result_rows.Count)
  }

  $result = @{ }
  $row_cnt = 0
  $result_rows | foreach-object {
    $row = $_
    if ($debug) {
       write-host ('Row {0} Key(s) {1}' -f $row_cnt, ($row.Keys -join ' '))
    }
    $row_cnt ++
    if ($row.Keys[0] -eq $hostname) {
      if ($debug) {
        write-host ('found data for {0}' -f $hostname )
      }
      $result =  $row[$hostname]
    }
  }
  if ($debug) {
     if ($result.Keys.Count -eq 0) {
        write-host ('nothing found.' )
     }
  }
  return $result
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$config = load_configB -config_file $config_file -hostname $hostname -debug $debug_flag
if ($debug_flag) {
  write-output ('config is {0}' -f $config.GetType())
}
if ($debug_flag) {
  write-host ('{0} entries' -f $config['PORTS'].Count)
}
if (($key -ne $null ) -and ($key -ne '')){
  if ($config.ContainsKey($key)) {
    $data = @()
    $config[$key]| foreach-object {
      $entry = $_
      $data += $entry
    }
  }
  write-output ('Key: {0}' -f $key)
  write-output ('Value(s):' )
  $data | format-list
}
