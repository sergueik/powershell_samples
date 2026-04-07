
# inspired by https://github.com/adnanmasood/ASCII-EBCDIC-Converter o
# which constructs a console Windows app receving its aguments through its ownconfig file
param(
  [string] $key =  'Argument', # $null
  [string] $value =  'aplication-0.1.0-SNAPSHOT.jar', # $null
  [string] $name = 'counter-collector.exe.config'
)

if (( $name -eq $null ) -or ($key -eq $null) -or ($value -eq $null) ){
  write-output 'invalid arguments'
  return
}
# reserved for future use
$key = (get-culture).TextInfo.ToTitleCase($key.ToLower())
# can relatively fast if they are not deep in
$name = get-childitem -path '.' -filter $name -recurse | select-object -expandproperty FullName
if (-not ( test-path -path $name )) {
  return
}
write-output $name
# TODO: use [System.IO.Directory]::GetFiles()
$xml = [xml](get-content -path $name )
$xml.Normalize()

$element = $xml.SelectSingleNode(('/configuration/appSettings/add[@key="{0}"]' -f $key))
if ($element -eq $null ) {
  write-output 'invalid application config'
  return
}
$xml.configuration.appSettings.add | where-object { $_.key -eq $key } | foreach-object { $_.value = $value }
$xml.SelectSingleNode(('/configuration/appSettings/add[@key="{0}"]' -f $key )).value     = $value
$xml.configuration.appSettings.add
# Note: saves into wrong directory
$fullPath = convert-path $name
$xml.Save($fullPath)

<#
Yes, it is possible to use XML text content (inner text) instead of attributes in an app.config file, but the default .NET ConfigurationManager does not support this out of the box. Standard sections like <appSettings> strictly use the key and value attributes
#>
