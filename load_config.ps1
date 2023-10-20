param(
  [string]$config_file = ([System.IO.Path]::Combine((resolve-path -path '.').path,'config.json')),
  [string]$hostname = "${env:COMPUTERNAME}".ToLower(),
  [string]$key = 'PORTS',
  [string]$schema = 'A',
  [switch]$debug
)

function load_configA {
  param(
    [string]$config_file,
    [string]$hostname,
    [bool]$debug
  )
  $data = (Get-Content -Path $config_file) -join "`n"
  if ($debug){
    Write-host ('data: "{0}"' -f $data)
  }

  $json_obj = ConvertFrom-Json -InputObject $data
  $result = @{}

  # https://stackoverflow.com/questions/18779762/iterating-through-key-names-from-a-pscustomobject
  # Convert the PSCustomObject back to a HashTable

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

  return $result[$hostname]
}

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
<# 
. .\load_config.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configA.json' ) -schema A
. .\load_config.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configB.json' ) -schema B
. .\load_config.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configC.json' ) -schema C
#>
<#
. .\load_configA.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configA.json' )
. .\load_configB.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configB.json' )
. .\load_configC.ps1 -debug -config_file ((resolve-path '.').path + '\' + 'configC.json' )

#>
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
    switch ($schema) {

      'A' {
          $config = load_configA -config_file $config_file -hostname $hostname -debug $debug_flag
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
	  return
      }

      'B' {
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
	  return
      }

      'C' {
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
          return
      }
      default { Write-host 'Not understood' }
 }

<#
[string]$config_file = ([System.IO.Path]::Combine((resolve-path -path '.').path,'configD.json'))
$data = (Get-Content -Path $config_file) -join "`n"
$json_obj = ConvertFrom-Json -InputObject $data

$json_obj |get-member

   TypeName: System.Management.Automation.PSCustomObject

Name        MemberType   Definition
----        ----------   ----------
Equals      Method       bool Equals(System.Object obj)
GetHashCode Method       int GetHashCode()
GetType     Method       type GetType()
ToString    Method       string ToString()
info        NoteProperty Object[] info=System.Object[]
name        NoteProperty string name=sergueik53
ports       NoteProperty Object[] ports=System.Object[]


Get-Member -MemberType NoteProperty -inputobject $json_obj[0]


   TypeName: System.Management.Automation.PSCustomObject

Name  MemberType   Definition
----  ----------   ----------
info  NoteProperty Object[] info=System.Object[]
name  NoteProperty string name=sergueik53
ports NoteProperty Object[] ports=System.Object[]


$json_obj.GetType().Name
Object[]
$json_obj[0].getType().Name
PSCustomObject
$json_obj[0].'name'.getType().Name
String
$json_obj[0].'info'.getType().Name
Object[]
$json_obj[0].'info'.count
3
$json_obj[0].'info'[0].getType().Name
PSCustomObject


$json_obj[0].'info'[0] | Get-Member -MemberType NoteProperty


   TypeName: System.Management.Automation.PSCustomObject

Name MemberType   Definition
---- ----------   ----------
key1 NoteProperty string key1=value1

Get-Member -MemberType NoteProperty -inputobject $json_obj[0].'info'[0]

   TypeName: System.Management.Automation.PSCustomObject

Name MemberType   Definition
---- ----------   ----------
key1 NoteProperty string key1=value1

$json_obj[0].'info'[0].'Key1'
value1
#>