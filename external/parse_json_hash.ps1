# origin: https://4sysops.com/archives/convert-json-to-a-powershell-hash-table/
# NOTE: original post comments claim cases exist where this does not work


# System.Management.Automation.PSCustomObject is a default type when creating a custom object using the New-Object

# [PSCustomObject]  isn't an object type but is a type accelerator.
# It constructs a PSObject, but does so in a way that results in hash table keys becoming properties

# What is the difference between Hashtable and PSCustomObject?
# PSCustomObject is constructed of properties and values.
# PSCustomObjects can be stored in arrays and hashtables, 
# but ultimately they are intended to be a collection of properties

# it has 5 hidden members
# PSTypeNames
# PSAdapted
# PSBase
# PSExtended
# PSObject
#  PSCustomObject directly derives from System.Object, does not implement any interface and does not have any fields or properties except the internal field SelfInstance
#
# see also:
# https://learn-powershell.net/2012/03/02/working-with-custom-types-of-custom-objects-in-powershell/
# https://renenyffenegger.ch/notes/Microsoft/dot-net/namespaces-classes/System/Management/Automation/PSCustomObject

function ConvertTo-Hashtable {
    [CmdletBinding()]
    [OutputType('hashtable')]
    param (
        [Parameter(ValueFromPipeline)]
        $InputObject
    )

    process {
        if ($null -eq $InputObject) {
            return $null
        }

        if ($InputObject -is [System.Collections.IEnumerable] -and $InputObject -isnot [string]) {
            $collection = @(
                foreach ($object in $InputObject) {
                    $object = $_ 
                    ConvertTo-Hashtable -InputObject $object
                }
            )

            Write-Output -NoEnumerate $collection
        } elseif ($InputObject -is [psobject]) { 
            # convertFrom-Json produces System.Management.Automation.PSCustomObject 
            # https://stackoverflow.com/questions/14012773/difference-between-psobject-hashtable-and-pscustomobject#:~:text=%5BPSCustomObject%5D%20is%20a%20type%20accelerator,called%20with%20no%20constructor%20parameters.
            # which properties enumeration is fastest through its PSObject
            $dictionary = @{}
            foreach ($property in $InputObject.PSObject.Properties) {
                $dictionary[$property.Name] = ConvertTo-Hashtable -InputObject $property.Value
            }
            $dictionary
        } else {
            # the object is likely a hash table
            $InputObject
        }
    }
}

$data_json = @'
{
  "Apples": {
     "red": [10, 20, 30],
     "green":1
   },
  "Pears": {
    "yellow": { "something else": [{ "one": 1}, {"two": 2 }, {"tree": {} } ]}
   }
}
'@

$data = $data_json | ConvertFrom-Json | ConvertTo-HashTable

$data['Pears']['yellow']['something else']
$data['Pears']['yellow']['something else'][1]['two']
