param(
  [string]$config_file = ([System.IO.Path]::Combine((resolve-path -path '.').path,'configC.json')),
  [string]$hostname = "${env:COMPUTERNAME}".ToLower(),
  [string]$key = 'PORTS',
  [string]$schema = 'A',
  [switch]$debug
)


function load_configC {
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
  $found = $false
  $json_rows| foreach-object {
    $json_obj = $_
    $result = @{}
    $json_obj | Get-Member -MemberType NoteProperty | ForEach-Object {
      $json_key = $_.Name
      write-host ('hostname_key  = {0}' -f $json_key)
      if ($json_key -eq 'name') {
        if ($result[$json_key] -eq $hostname) {
          $found = $true 
        }
      }
    }
  }
  $json_rows| foreach-object {
    $json_obj = $_
    $result = @{}
    $json_obj | Get-Member -MemberType NoteProperty | ForEach-Object {
        # NOTE the syntax
        $data_key = $_.Name
        $value = $json_obj."$(${data_key})"
        $result[$data_key] = $value
    }
    $result_rows += $result
  }
<#
write-host 'rows: '
$result_rows
#>
  if ($debug) {
    write-host ('Inpect {0} rows' -f $result_rows.Count)
  }

  $result = @{ }
  $row_cnt = 0
  $result_rows | foreach-object {
    $row = $_
    if ($debug) {
       write-host ('Row {0} Key(s) {1}' -f $row_cnt, ($row.Keys -join ','))
        write-host ('Row {0} name {1}' -f $row_cnt, $row['name'])
    }
    $row_cnt ++
    if ($row['name'] -eq $hostname) {
      if ($debug) {
        write-host ('found data for {0}' -f $hostname )
      }
      $result =  $row
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
$config = load_configC -config_file $config_file -hostname $hostname -debug $debug_flag
if ($debug_flag) {
  write-output ('config is {0}' -f $config.GetType())
}
if ($debug_flag) {
  write-host ('{0} entries' -f $config['ports'].Count)
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
