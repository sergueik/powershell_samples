param(
  [string[]]$remote_hosts = @('192.168.99.100'), # Virtual Box Host-only adapter  VM
  [string[]]$remote_ports = @('27017','22'), # mongodb
  [switch]$debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent

if ($debug_flag){
  write-output ('remote_ports(*): {0}' -f ($remote_ports -join ' '))
}
$connections = @{ }
# based on https://qna.habr.com/q/987631
# more verbose but somewhat simpler grep style pipeline and default select-string
if ($debug_flag){
  write-output 'test # 1'
  $output = netstat.exe -ano -p TCP | select-string 'ESTABLISHED'
  $remote_ports | foreach-object {

  $remote_port = $_
  write-output ('Finding {0}' -f $remote_port )
  $filter = ('{0}:{1}' -f '[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}', $remote_port )
    $output | select-string -allmatches $filter | convertfrom-string | format-list
  }
}
$data =  invoke-expression -command 'netstat.exe -ano -p TCP' | select-string  'ESTABLISHED' | select-string -allmatches $filter | convertfrom-string
$totalcnt = 0

$output = invoke-expression -command 'netstat.exe -ano -p TCP' | select-string  'ESTABLISHED'
$remote_ports | foreach-object {

  $remote_port = $_
  if ($debug_flag){
    write-output ('Finding {0}' -f $remote_port )
  }
  $filter = ('{0}:{1}' -f '[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}', $remote_port )
  $data = $output | select-string -allmatches $filter | convertfrom-string
  $origin = ''
  if ($debug_flag){
    write-output ('inspecing {0} candidate rows'-f $data.Count )
  }
  $data | foreach-object {
    $origin = $_.'P4'
    if ($debug_flag){
      write-output( 'Origin: ' +  $origin)
    }

    if (-not $connections.ContainsKey($origin)) {
      $connections[$origin] = 0
    }
    $totalcnt = $totalcnt + 1
    $connections[$origin] = $connections[$origin] + 1
  }

}

if ($totalcnt -gt 0 ) {
  write-output 'Connections: '
  write-output '---'
  $connections.Keys | foreach-object {
    $origin = $_
    $count = $connections[$origin]
    $x = $origin -split ':'
    $RemoteHost = $x[0]
    $RemotePort = $x[1]
    write-output ('{0} {1} {2}' -f $RemoteHost,$RemotePort,$count )
  }
  write-output '---'
  write-output ('Total connections: ' + $totalcnt )
}
exit 0

<#

Testing:
docker build -t $IMAGE -f Dockerfile.$IMAGE .
docker run -d --name $CONTAINER -p 27017:27017 -i $IMAGE
2d791af6d5c7994d31e747670dad0b14c07a865a21946ca273430a102135c091

# may also start more mongodb servers

docker run -d --name ${CONTAINER}2 -p 27117:27017 -i $IMAGE
5410b6eb3ee333a54daa1932faaedd1c3991dc1488c369a5221cbb0434849b13

NOTE: if the MongoDB client app is not Spring boot, the command line option may not work
and will need to update src/test/resources/application.properties instead

.\data_parsing.ps1 -remote_ports 27117,22

to see something like:

Connections:
---
192.168.99.100 27017 2
192.168.0.92 22 1
192.168.0.119 22 1
192.168.0.64 22 3
---
Total connections: 7

#>

<#
with -debug flag also prints

Finding 22
inspecing 5 candidate rows
Origin: 192.168.0.64:22
Origin: 192.168.0.119:22
Origin: 192.168.0.64:22
Origin: 192.168.0.92:22
Origin: 192.168.0.64:22
Finding 27017
inspecing 2 candidate rows
Origin: 192.168.99.100:27017
Origin: 192.168.99.100:27017
Connections:
---
192.168.99.100 27017 2
192.168.0.92 22 1
192.168.0.119 22 1
192.168.0.64 22 3
---
Total connections: 7

and other debugging infomation
#>

