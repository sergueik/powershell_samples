param(
  [string]$httpVerb,
  [string]$body = ''
)

# origin: https://4sysops.com/archives/convert-json-to-a-powershell-hash-table/
# see also: https://github.com/sergueik/powershell_ui_samples/blob/master/external/parse_json_hash.ps1

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
                  $result = ConvertTo-Hashtable -InputObject $object
                  write-output $result
                }
            )
            write-output -NoEnumerate $collection
        } elseif ($InputObject -is [psobject]) { 
            # If the object has properties that need enumeration
            # Convert it to its own dictionary table and return it
            # convertFrom-Json produces System.Management.Automation.PSCustomObject 
            # https://stackoverflow.com/questions/14012773/difference-between-psobject-hashtable-and-pscustomobject
            # which properties enumeration is fastest through its PSObject
            $dictionary = @{}
            foreach ($property in $InputObject.PSObject.Properties) {
                $dictionary[$property.Name] = ConvertTo-Hashtable -InputObject $property.Value
            }
            $dictionary
        } else {
            # the object isn't an array, collection, or other object
            # just return it: it's already a dictionary table
            $InputObject
        }
    }
}


try {
  if (!$env:SCRIPTSERVICES_VERSION) {
    Write-Host 'Running in console'
  }
  write-output ('HTTP "{0}"' -f $httpVerb ) 
  if ($body -ne '') {
    write-output ('body: {0}' -f $body )
    # convert json to PSObjects 
    $body_json = $body | convertfrom-json
    write-output ('body_json: {0}' -f $body_json )
    # convert PSObjects to normal hash/array schema
    $data = $body_json | ConvertTo-HashTable
    write-output ($data.keys -join ',' )
    write-output ('key={0}' -f $data['key'] )
  } else { 
    write-output 'empty body'
  }
} catch [Exception] {
  write-output ( 'Exception: '  +  $_ )
}
