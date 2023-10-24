# Converting from JSON without Creating PSCustomObject
# based on https://stackoverflow.com/questions/40495248/create-hashtable-from-json
# see also: https://stackoverflow.com/questions/3740128/pscustomobject-to-hashtable
# NOTE: In Powershell 7.x the `convertfrom-json` cmdlet starts to support the `-AsHashtable` option
# https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/convertfrom-json?view=powershell-7.2
param(
  [string] $file
)

$h = @{}  
# (get-content $json_data | convertfrom-json).psobject.properties.foreach{ $h[$_.Name] = $_.Value }
(get-content $json_data | convertfrom-json).psobject.properties | foreach-object { 
  $h[$_.Name] = $_.Value
}
$h | format-list
