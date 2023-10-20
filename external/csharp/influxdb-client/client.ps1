# Copyright (c) 2022 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the 'Software'), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# based on: documentation of .NET library for efficiently sending points to InfluxDB 1.x
# https://github.com/influxdata/influxdb-csharp
param (
  [string]$assembly_path = '.',
  # NOTE: special variable to avoid: $host
  # Cannot overwrite variable host because it is read-only or constant.
  [string]$influxdbhost = '192.168.0.29',
  [string]$port = '8086',
  [string]$schema = 'http',
  [string]$database = 'example'
)
<#
if ($env:PROCESSOR_ARCHITECTURE -ne 'x86') { 
  # if the dll is compiled in SharpDevelop for x86 (e.g. for debugging)
  # attempt to load in 64 bit Powershell will fail with "BadImageFormatException"
  write-output 'this test needs to be run on c:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe'
  exit 1;
}
#>
$shared_assemblies = @(
  'InfluxDB.Collector.dll',
  'InfluxDB.LineProtocol.dll'
)
<#
  Exception calling "InfluxDB" with "2" argument(s): "Could not load file or assembly 'InfluxDB.LineProtocol, Version=1.0.0.0, Culture=neutral,
#>

$dependent_type = 'InfluxDB.Collector.CollectorConfiguration'

if (-not ($dependent_type -as [type])) {
  pushd $assembly_path
  $shared_assemblies | foreach-object {
    $asssembly = $_
    if (-not (test-path -path ( $assembly_path + '\' + $asssembly ) ) ) { 
      write-host ('Missing dependency {0}' -f ( $assembly_path + '\' + $asssembly ) )
      return
    }
    add-type -path $asssembly
    (get-item -path $asssembly | select-object -expandproperty VersionInfo  | select-object -property Comments,ProductVersion ) | format-list
  }
  popd
}
# Powershell unwinding the builder pattern
[InfluxDB.Collector.CollectorConfiguration]$o = new-object InfluxDB.Collector.CollectorConfiguration
$o.Tag.With('host', $env:COMPUTERNAME) | out-null
$o.Batch.AtInterval([System.TimeSpan]::FromSeconds(2)) | out-null
$o.WriteTo.InfluxDB(('{0}://{1}:{2}' -f $schema,$influxdbhost,$port ), $database) | out-null
$m = $o.CreateCollector()

# NOTE: cannot seeminly declare this one "strongly typed":
# [InfluxDB.Collector.Pipeline.PipelinedMetricsCollector]$m = $o.CreateCollector()
# is leading to exception: Unable to find type [InfluxDB.Collector.Pipeline.PipelinedMetricsCollector]


[System.Collections.Generic.Dictionary[String,Object]]$data = new-object System.Collections.Generic.Dictionary'[String,Object]'
$data.Add('value', 10)
$m.Increment('iterations')
# NOTE: no error checking ? (empty response)?
write-output 'Sending data'
$m.Write('testing', $data)

