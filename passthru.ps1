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
# allow passthru exlicit key, value pair
param (
 [string] $line = 'somekey: somevalue',
 [switch] $passthru,
 [switch] $debug
)

function passthru {
  param (
    [string] $line = 'somekey: somevalue',
    [string] $datafile,
    [bool]$debug
  )

  [System.Collections.Hashtable]$y = @{}
  # alternatively reuse the code from updateData itself
  $pattern =  '^ *([^ ]*): *([^ ]*.*)$'

  if ($debug){
    write-host ('examine passhhru line {0}' -f $line )
  }
  if ( $line -match $pattern ) {

    $m = select-string -pattern $pattern -InputObject $line
    $g = $m.Matches.Groups
    $k = $g.Item(1).Value
    $v = $g.Item(2).Value
    # 	if ($debug){
      write-host('key: {0}; value: {1}' -f $k, $v)
    # }
    $y[$k] = $v
  }
}


### Main
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$passthru_flag = [bool]$PSBoundParameters['passthru'].IsPresent -bor $passthru.ToBool()
if ($passthru_flag){
  <#
  . .\pasthru.ps1 -line 'foo: bar2' -debug -passthru
  #>
  passthru -line $line -debug $debug_flag
}
