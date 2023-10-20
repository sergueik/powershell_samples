#Copyright (c) 2021,2022,2023 Serguei Kouzmine
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

param(
  [bool]$gui_status = $false,
  [bool]$submit_file = $true
)


# origin: https://stackoverflow.com/questions/36268925/powershell-invoke-restmethod-multipart-form-data
# see also: https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html

# origin: https://stackoverflow.com/questions/36268925/powershell-invoke-restmethod-multipart-form-data
# see also: https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html

function submit_file {
  param(
    [string]$filePath = (resolve-path 'data.txt'),
    [string]$boundary = [System.Guid]::NewGuid().ToString(),
    [string]$url = 'http://localhost:8085/basic/upload',
    [bool]$debug = $false,
    [System.Collections.Hashtable]$params = @{
      'operation' = 'send';
      'param'   = 'data';
    }
  )
  $date = get-date -format 'yyyy-MM-dd HH:mm'
  $filename = (($filePath -replace '/', '\\' ) -replace '^.*\\', '') + '_' + ($date -replace '[\-: ]', '_')
  write-host ('reading {0}' -f $filepath)
  # TODO: it appears that [System.IO.File]::ReadAllBytes assumes that relative filePath is referring to users home directory
  $payload = [System.Text.Encoding]::GetEncoding('UTF-8').GetString([System.IO.File]::ReadAllBytes($filePath))

  $LF = "`r`n";
  $B = '--' + $boundary
  $body_lines = @()
  $body_lines += $B
  $params.keys | foreach-object {
    $key = $_
    $val = $params.Item($key)
    $body_lines += ('Content-Disposition: form-data; name="{0}"' -f $key)
    $body_lines += ''
    $body_lines += $val
    $body_lines += $B
  }
  $body_lines += ('Content-Disposition: form-data; name="file"; filename="{0}"' -f $filename)
  $body_lines += 'Content-Type: application/octet-stream'
  $body_lines += ''
  $body_lines += $payload
  $body_lines += $B + '--'
  $body_lines += ''
  $body = $body_lines -join $LF

  # NOTE: Powershell does not allow dash in variables names
  $content_type = ('multipart/form-payload; boundary="{0}"' -f $boundary)
  if ($debug)  {
    write-host ('invoke-restmethod -uri {0} -method Post -contenttype "{1}" -body {2}' -f $uri, $content_type, [char]10 + $body)
  }
  # quotes aroung content_type arguments are optional

  $result = invoke-restmethod -uri $URL -method Post -contenttype "$content_type" -body $body
  return $result
}

# see also:
# https://community.idera.com/database-tools/powershell/ask_the_experts/f/powershell_for_windows-12/19845/get-process-user-filtering
function gui_check {
  param(
    [bool]$debug = $false
  )
  # NOTE: without initializing variable with the value,
  # Powershell will print it initial value
  # (the  value will be *False*) to he output
  # [boolean]$status
  [boolean]$status = $false
  # cannot distinguish ?
  # $process_count = (get-process -name explorer  -IncludeUserName | Where-object {$_.username -eq $env:USERNAME  }).count
  # NOTE: the -IncludeUserName option of get-process cmdlet requires elevation
  # check if explorer.exe is running for the current user
  $tasklist_output = &tasklist.exe '/FI', "USERNAME eq $env:USERNAME",'/FI', 'IMAGENAME EQ explorer.exe'
  $process_count = $tasklist_output | select-string 'explorer.exe'
  if (($process_count -ne $null) -and ( $process_count -ne 0) ){
    $status = $true
  } else {
    $status = $false
  }
  # NOTE: silent return does not apply

  if ($debug){
    return @($tasklist_output,
             ('process_count = {0}' -f $process_count),
             ('status = {0}' -f $status))
  } else {
    return $status
  }
}



$processid = $pid
# NOTE: somewhat heavy interpolation. Cannot use ${...} in this command
# $wmiresult = get-wmiobject win32_process -filter "processid='${processid}'" | select-object -property parentprocessid,name

$wmiresult = get-wmiobject win32_process -filter "processid='$($processid)'" | select-object -property parentprocessid
$parentprocessid = $wmiresult.parentprocessid

$wmiresult = get-wmiobject win32_process -filter "processid='$($parentprocessid)'" | select-object -property name
$parentprocessname = $wmiresult.name
$date = get-date -format 'yyyy-MM-dd HH:mm'

# the command line option overrides
if ( -not $gui_status ) {
  $gui_status = gui_check
}

$data = @{
  'username' = $env:USERNAME;
  'pid' = $processid;
  'parent' = $parentprocessname;
  'message' = 'test';
  'invoked' = $date;
  'gui'     = $gui_status;
}


# write identifying information to the text file
# NOTE: avoid using resolve-path cmdlet in script  run by Windows Task Scheduler
$result_filepath = ('{0}\result.txt' -f $env:temp )

# specify directory directly
$app_dir = 'c:\app'
$result_filepath = ('{0}\result.txt' -f $app_dir)
out-file -encoding ascii -filepath $result_filepath -append -InputObject $data


# this will produce unique message like

<#
Name                           Value
----                           -----
username                       sergueik
parent                         cmd.exe
pid                            9920
message                        test
invoked                        2021-10-14 16:35

#>
# from interactive runs and
<#

Name                           Value
----                           -----
username                       sergueik
parent                         svchost.exe
pid                            9796
message                        test
invoked                        2021-10-14 16:36
#>
# from scheduled task runs (the user will likely be different)


if ($submit_file) {
  #  need full path otherwise  is it looked up in user home directory 
  
  $datatxt = ('{0}/data.txt' -f $app_dir)
# NOTE: this will not work (no get method in System.Collections.Hashtable
# pid: ${data.get('pid')}
# nor will this
# pid: $data['pid']
# nor will this
# pid: ${data['pid']}
  @"
PID: $($data.Item('pid'))
USERNAME: $($data['username'] -replace "${env:COMPUTERNAME}\$", 'SYSTEM')
TIME: $(get-date($data['invoked']) -format {yyyy-MM-dd hh:mm:ss})
PARENT: $($data.Item('parent'))
MESSAGE: $($data['message'])
"@ | out-file -encoding ascii -filepath $datatxt

<#
NOTE: when there is a syntax error in the script expect the COM error 2147942401 in otherwise counted a successful run EventLog entry
e.g. by adding a space in front of replace in the inline document above
Inspecting error code: 2147942401
Incorrect function. (Exception from HRESULT: 0x80070001)
#>  
  $url = 'http://192.168.0.25:8085/basic/upload'
  $submit_file_result = submit_file -filePath $datatxt -url $url
  write-output $submit_file_result
  out-file -encoding ascii -filepath $result_filepath -append -InputObject $submit_file_result
}
