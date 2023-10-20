
$uri = new-object Uri('http://192.168.99.100:9090/cgi-bin/config.cgi')
Invoke-RestMethod -uri $uri -method GET -contentType 'application/json'
sergueik119 sergueik53               sergueik71
----------- ----------               ----------
            @{PORTS=System.Object[]} @{PORTS=System.Object[]}

# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/invoke-restmethod?view=powershell-5.1

$response =  Invoke-RestMethod -uri $uri -method GET -timeoutsec 3 -ContentType 'application/json' -Headers @{ Accept = "application/json" } 
# Powershell immediately attempts to deserialize JSON and even XML (when remote end is WCF)
$response |format-list

<#

sergueik71               sergueik53               sergueik119
----------               ----------               -----------
@{PORTS=System.Object[]} @{PORTS=System.Object[]}


$response.'sergueik53'

PORTS
-----
{22, 443, 3306}


$response.'sergueik53'.'PORTS'
22
443
3306

$response | convertto-json -depth 99
{
  "sergueik53":  {
    "PORTS":  [
      22,
      443,
      3306
    ]
 },
  "sergueik119":  {
  },
  "sergueik71":  {
    "PORTS":  [
       5432
    ]
 }
}

# NOTE: the formatting will be worse. There is no way to specify desired whitespace with convertto-json

$response | get-member


   TypeName: System.Management.Automation.PSCustomObject

Name        MemberType   Definition
----        ----------   ----------
Equals      Method       bool Equals(System.Object obj)
GetHashCode Method       int GetHashCode()
GetType     Method       type GetType()
ToString    Method       string ToString()
sergueik119 NoteProperty System.Management.Automation.PSCustomObject serguei...
sergueik53  NoteProperty System.Management.Automation.PSCustomObject serguei...

# alternative

$response = (Invoke-WebRequest -uri $uri -method GET -timeoutsec 3 -ContentType 'application/json' -Headers @{ Accept = "application/json" } ).Content | ConvertFrom-Json

sergueik119 sergueik53               sergueik71
----------- ----------               ----------
            @{PORTS=System.Object[]} @{PORTS=System.Object[]}

# NOTE: the -SessionVariable VARIABLE_NAME feature / WebSession feature of Invoke-WebRequest
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/invoke-webrequest?view=powershell-5.1
# https://stackoverflow.com/questions/44695645/sessionvariable-parametrised-in-invoke-webrequest
- aboud naming the sessionvariable variable arbitrarily
# https://davidhamann.de/2019/04/12/powershell-invoke-webrequest-by-example/
# https://adamtheautomator.com/invoke-webrequest/#Submitting_a_Form_and_Working_with_Sessions (NOTE: poor formattting, lots of ads)
#>
