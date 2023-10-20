#Copyright (c) 2023 Serguei Kouzmine
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
  [string] $marker_filename = 'firstrun.json',
  [string] $script_dir = (resolve-path -path '.').path,
  [switch] $remove = $true,
  [switch] $debug

)
# NOTE: following the exit code 0 for success 1 for failure
[int]$status = 1
[string]$marker_filepath = ([System.IO.Path]::Combine($script_dir,$marker_filename))
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent
$remove_flag = ([bool]$PSBoundParameters['remove'].IsPresent) -bor $remove.toBool()

if ($remove_flag){
  if (test-path -pathtype leaf -literalpath $marker_filepath) {
    $status = 0
    if ($debug_flag){
      write-host ('Remove marker file from {0}' -f $script_dir )
    }
    remove-item -literalpath $marker_filepath -erroraction silentlycontinue
  } else {
    $status = 1
  }

} else {
  if ($debug_flag){
    write-host ('Create marker file from {0}' -f $script_dir )
  }

 ( new-item -ItemType "file" -Value "{}" -name $marker_filename -path $script_dir -erroraction silentlycontinue) | out-null
  $status = 0
}

if ($debug_flag){
  write-host ('exit {0}' -f $status )
}

exit $status
