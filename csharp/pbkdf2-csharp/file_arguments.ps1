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
  $key_content = (get-content -path $k.path)[0]
  $password = $key_content -replace ' *$', ''
  write-host ('password: {0}' -f $password)

  $config_line_regexp  = ('{0} *[:=] * (.*$)' -f $name)
  $matched_line_object = select-string $config_line_regexp $p.Path
  $value_data = $matched_line_object.Matches[0].Captures[0].Groups[1].Value
  write-host ('value_data: {0}' -f $value_data)
  $matched_value_object = select-string -Pattern 'ENC\(([^)]*)\)' -inputobject $value_data
  $value = $matched_value_object.Matches[0].Captures[0].Groups[1].Value
  write-host ('value: {0}' -f $value)
}
# Usage:
<#
. .\file_arguments.ps1 -key 'x\key.txt' -properties 'application.properties' -name 'name'
#>

