param(
  [string]$config_file = ([System.IO.Path]::Combine((resolve-path -path '.').path,'config.json')),
  [string]$hostname = "${env:COMPUTERNAME}".ToLower(),
  [string]$key = 'PORTS',
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
  <#
  NOTE:
   TODO:handle exceptions
  convertFrom-Json : Invalid JSON primitive:
  # e.g. unbalance parenthesis in the JSON paayload
  #>
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

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
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

