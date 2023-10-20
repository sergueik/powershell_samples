# see also: https://github.com/sergueik/puppetmaster_vagrant/sidr.cmd 
# for old "advapi.dll" "LookupAccountSid" p/ivoke solution which is not limited to "Active Directory" users
# Get-LocalUser -name 'SYSTEM' | select-object -property sid,name,caption
# origin: https://www.lepide.com/how-to/list-all-user-accounts-on-a-windows-system-using-powershell.html
# NOTE: "get-localuser" seems to not be available prior windows 10
# For Windows 7 and 8 and older use the 
# get-wmiobject -computername '.' -class Win32_UserAccount -filter 'LocalAccount=True'| select-object -property sid,caption
# NOTE: cannot use 'expandproperty' here
# Select-Object : Cannot bind parameter because parameter 'ExpandProperty' is
# specified more than once. To provide multiple values to parameters that can accept multiple values, use the array syntax
# Select-Object : Cannot convert 'System.Object[]' to the type 'System.String' required by parameter 'ExpandProperty'. Specified method is not supported.
# NOTE: the ultra-legacy way works to some extent too
# $adsi = [ADSI]"WinNT://${env:COMPUTERNAME}"
# $Users = $adsi.Children | where {$_.SchemaClassName -eq 'user'}
# $Users
# but is quite slow
