### Info 


[influxdb-csharp](https://github.com/cbovar/influxdb-csharp)

### Usage
===============


C# client for Influxdb

Only very basic features are implemented so far.

```cs
var client = new InfluxDBClient("192.168.1.100", 8086, "root", "root", "MyDataBase");

// Create series
var serie = new Serie { Name = "foo", ColumnNames = new List<string> { "value", "value_str" } };
serie.Points.Add(new List<object> { 1.0, "first" });
var series = new List<Serie> { serie };

// Insert series
client.Insert(series);

// Query database
var result = client.Query("select * from foo");
```

### Powershell Module

For one metric posts one can use [markwragg/PowerShell-Influx](https://github.com/markwragg/PowerShell-Influx)

The two featuring files in the repo `ConvertTo-InfluxLineString.ps1` and `Write-Influx.ps1` need to be placed in certain directory, e.g. current. Note the  original project contains some 100 extra files for helping installing the module.
The small fix may need to be applied to the file to prevent Powershell croak:
```text
The parameter alias cannot be specified because an alias with the name 'db' was already defined multiple times for the command.
```
comment the line annitating the input  parameter with the alias
```powershell
        [Parameter(Mandatory=$true)]
        # [Alias('DB')]
        [string]
        $Database,
```
run it
```powershell
. .\ConvertTo-InfluxLineString.ps1
. .\Write-Influx.ps1
Write-Influx -Measure testing -Tags @{Server=$env:COMPUTERNAME} -Metrics @{CPU=100; Memory=50} -Database example -Server http://192.168.0.29:8086
```
```text
TagData: ,Server=SERGUEIK53
Body: testing,Server=SERGUEIK53 CPU=100
URI: http://192.168.0.29:8086/write?&db=example
```
there is often a timeout seen when the target series does not yet exist:
```text
Invoke-RestMethod : {"error":"timeout"}
At write-influx.ps1:206 char:25        
```
### NOTE

the [original project](https://github.com/cbovar/influxdb-csharp) appears to use outdated endpoints

### See Also

  * [aternative .NET Library for Data Access to InfluxDB](https://github.com/marcolew/InfluxDataAccess)	
  * [async C# client for InfluxDB](https://github.com/mikkelfish/influx-c--client) (requires C# 6+)
  * another [.NET client](https://github.com/mhowlett/LightweightInfluxDb) for reading data from and writing data to InfluxDb (also requires C# 6+)
  * https://github.com/catwithboots/LibInfluxDB.NET (really async, implements a number of `Task<Response>` in code, is .Net 4.0 compatible but also requires C# 6+
  * C# [client](https://github.com/influxdata/influxdb-csharp)  implementation of the InfluxDB ingestion 'Line Protocol'. - still requires Visual Studio 2017 or later (due to project file format) - quite a project
  * the `InfluxDB.Collector` [nuget package](https://www.nuget.org/packages/InfluxDB.Collector/1.0.0) - version __1.0.0__ supports .Net Framework 4.5
  * the `InfluxDB.LineProtocol` [nuget package](https://www.nuget.org/packages/InfluxDB.LineProtocol/1.0.0) - version __1.0.0__ supports .Net Framework 4.5
  * influx - InfluxDB command line interface [docuentation](https://docs.influxdata.com/influxdb/v1.8/tools/shell/) - for __1.8__ release
https://github.com/Tom-shushu/InfluxDB1.xAnd2.x-SpringBoot


