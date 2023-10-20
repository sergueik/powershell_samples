# origin: https://pscustomobject.github.io/powershell/functions/PowerShell-SecureString-To-String/
param(
  [string]$example = 'lorem ipsum',
  [switch]$credentials,
  [string]$user = $null

)
[securestring]$securedata | out-null
if ([bool]$psboundparameters['credentials'].ispresent) {
  if ($user -eq $null -or $user -eq '') {
    $user = "${env:USERDOMAIN}\${env:USERNAME}"
	}
  # step 1. Conllect credential as SecureString
	$credential = get-credential -username $user -message 'enter credential'
  [System.Net.NetworkCredential] $networkCredential = $credential.GetNetworkCredential()
  write-output ('plain text: {0}' -f $networkCredential.Password )
  # usually simply as
  $data = $credential.GetNetworkCredential().password
  # note that the plain text password is returned by calling to '.GetNetworkCredential()'
  # see also
  # https://www.py4u.net/discuss/1760454
  # https://docs.microsoft.com/en-us/dotnet/api/system.net.networkcredential.-ctor?view=netframework-4.5
  write-output write-output ('plain text: {0}' -f $data)
	$securedata = $credential.password
} else {
  # step 1. Convert plain text to SecureString
  $securedata = ConvertTo-SecureString $example -AsPlainText -Force
}

$result = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securedata))

# NOTE: using ConvertTo-SecureString with the above snippet will lead to an error
# it is for storing in files
# https://stackoverflow.com/questions/28352141/convert-a-secure-string-to-plain-text
# see also vendor documentation:
# https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.securestringtobstr?view=netframework-4.8

write-output $result
