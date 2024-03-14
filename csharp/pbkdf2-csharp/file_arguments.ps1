param (
  [string]$properties = '',
  [string]$name = '',
  [string]$keyfile = '',
  [switch]$strong = $true,
  [switch]$debug
)

$strong_flag = [bool]$PSBoundParameters['all'].IsPresent -bor $strong.ToBool()
if ($debug) {
  write-host ('Strong : {0}' -f $strong_flag) 
  exit
}
# NOTE: do not get-content from the specicied text file raw
# trim all trailing whitespace
#  NOTE: may seem counterintuitive but it works:
<#
. .\file_arguments.ps1 -debug
Strong : 1
 . .\file_arguments.ps1 -strong -debug
Strong : 1
 . .\file_arguments.ps1 -strong:$false -debug
Strong : 0
#>
[boolean] $file_args = $false
[boolean] $file_args_valid = $false
if ($name -ne '') {
  $file_args = $true
  if ($keyfile -ne '') { 
    $k = resolve-path $keyfile -erroraction silentlycontinue
    if ( -not ($k -eq $null)){
      if ($properties -ne '') { 
        $p = resolve-path $properties -erroraction silentlycontinue
        if ( -not ($p -eq $null)){
          $file_args_valid = $true
        }
      }
    }
  }
}
if ($file_args) {
  if (-not ($file_args_valid)) {
    write-error 'invalid args'
    exit 
  }
  $e  = ('{0} *[:=] * (.*$)' -f $name)
  $x = select-string $e $p.Path
  $p = $x.Matches[0].Captures[0].Groups[1].Value
  $k = (get-content -path $k.path)[0]
  $key = $k -replace ' *$', ''
  write-output ('key: {0}' -f $key)
  write-output ('p: {0}' -f $p)
  $v = select-string -Pattern 'ENC\(([^)]*)\)' -inputobject $p
  $value = $v.Matches[0].Captures[0].Groups[1].Value
  write-output ('value: {0}' -f $value)
}
# Usage:
<#
. .\file_arguments.ps1 -key 'x\key.txt' -properties 'application.properties' -name 'name'
#>
