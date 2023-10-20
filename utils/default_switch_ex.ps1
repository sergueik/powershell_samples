# see also:
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_parameters_default_values?view=powershell-5.1

param(
  # switch_parameter System.Management.Automation.SwitchParameter
  [switch] $switch_parameter = $true,
  [string] $switch_string = 'true',
  [switch] $debug
)

# NOTE: the$ PSDefaultParameterValues apparently does not work as documented. 
# NOTICE  the semantics
$PSDefaultParameterValues = @{
  "*:switch_parameter" = $true;
}

write-host 'Usage: ./check_switch_parameter.ps1 [-switch_parameter]'
# To set switch to false, have to use -switch_parameter:$false semantics
# invocation with -flag2:false argument leads to
# Cannot process argument transformation on parameter 'flag2'.
# Cannot convert value "System.String" to type "System.Management.Automation.SwitchParameter".

$incorrect_switch_flag = [bool]$PSBoundParameters['switch_parameter'].IsPresent
$switch_flag = [bool]$PSBoundParameters['switch_parameter'].IsPresent -bor $switch_parameter.ToBool()
# the workaround is to convert the parametr from [switch] type to [String] type
# then parse it via static class method
write-host ('switch_string value is "{0}"' -f  $switch_string )
$switch_flag = [Boolean]::Parse($switch_string)

write-host ('incorrect_switch_flag value is {0}' -f  $incorrect_switch_flag )
write-host ('switch_flag value is {0}' -f  $switch_flag )

<#
.\default_switch_ex.ps1 -switch_parameter:$false
incorrect_switch_flag value is False
switch_flag value is False

.\default_switch_ex.ps1 -switch_parameter
incorrect_switch_flag value is True
switch_flag value is True

.\default_switch_ex.ps1

incorrect_switch_flag value is False
switch_flag value is True
#>
