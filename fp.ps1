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


# "Object-oriented"-style data processing converted from Perl to Powershell

$funcs = @{
  'x' = { $key = $args[0]; $value = $args[1] ; $value};
  'y' = { ("args[0]='{0}' args[1]='{1}'" -f $args[0], $args[1]) };
  'z' = { param($key, $value) ('key="{0}" value="{1}"' -f $key, $value )};
}
function default_convertor {
  param (
    $key,
    $value
  )
  return ( 'default_convertor: key="{0}" value="{1}"' -f $key, $value )
}
$data = @{ 'x' = 10; 'y' = 20; 'z' = 30; 't' = 101; }

$data.keys |
foreach-object {
  $k = $_
  $res = $null
  $v = $data[$k]
  if ($funcs.containskey($k)) {
    $func = $funcs[$k]
    $res = $func.invoke($k,$v)
  } else  { 
    $res = default_convertor -key $k -value $v
  }
  # NOTE: print will produce something useless when composed this way
  # write-output ('{0}: {1}' -f $k, $res) 
  # x System.Collections.ObjectModel.Collection`1[System.Management.Automation.PSObject]
  # Workaround is
  write-output (('{0}: ' -f $k ) + $res) 
}
