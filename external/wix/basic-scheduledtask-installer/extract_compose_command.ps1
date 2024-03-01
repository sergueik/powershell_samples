param(
  [switch]$asuser,
  [string]$user = $null,
  [string] $password = ''
)
$name = 'Product.wxs' 
$xml = [xml](get-content -path $name )
$xml.Normalize()
# $guid = [guid]::NewGuid()
$xml.Wix.Product.CustomAction
# = $guid.ToString()
# $xml.Save($name)
# list
$Product = $xml.'Wix'.'Product'
$Product.CustomAction | foreach-object { if  ( $_.Id -eq 'CreateScheduledTask') { write-output $_.ExeCommand}}
$Product.'CustomAction' | where-object { $_.Id -eq 'CreateScheduledTask' } | select-object -first 1 | foreach-object { write-output $_.ExeCommand }
# update

$ExeCommand = $Product.'CustomAction' | where-object { $_.Id -eq 'CreateScheduledTask' } | select-object -first 1 | select-object -expandproperty ExeCommand 
# write-output $ExeCommand
$ExeCommand = $ExeCommand -replace '\s+', ' '
# write-output $ExeCommand

$Product.CustomAction | foreach-object { if  ( $_.Id -eq 'CreateScheduledTask') { $_.ExeCommand = $ExeCommand }}
if ([bool]$psboundparameters['asuser'].ispresent) {
  # NOTE: one seems to need to provide the 'USERDOMAIN'
  # in credentials  dialog at all times
  if ($user -eq $null -or $user -eq '') {
    $user = "${env:USERDOMAIN}\${env:USERNAME}"
  }
  $credential = get-credential -username $user -message 'credentials for the task'
  $password = $credential.GetNetworkCredential().Password
  # TODO: handle 'ssh' console run
}
if ($password -ne ''){
  $Product.Property | foreach-object { if  ( $_.Id -eq 'PASSWORD') { $_.Value = $password }}
}
$xml.Save($name)