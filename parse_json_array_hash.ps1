# origin: https://4sysops.com/archives/convert-json-to-a-powershell-hash-table/
# does not work

function ConvertTo-Hashtable {
    [CmdletBinding()]
    [OutputType('hashtable')]
    param (
        [Parameter(ValueFromPipeline)]
        $InputObject
    )

    process {
        ## Return null if the input is null. This can happen when calling the function
        ## recursively and a property is null
        if ($null -eq $InputObject) {
            return $null
        }

        ## Check if the input is an array or collection. If so, we also need to convert
        ## those types into hash tables as well. This function will convert all child
        ## objects into hash tables (if applicable)
        if ($InputObject -is [System.Collections.IEnumerable] -and $InputObject -isnot [string]) {
            $collection = @(
                foreach ($object in $InputObject) {
                    ConvertTo-Hashtable -InputObject $object
                }
            )

            ## Return the array but don't enumerate it because the object may be pretty complex
            Write-Output -NoEnumerate $collection
        } elseif ($InputObject -is [psobject]) { ## If the object has properties that need enumeration
            ## Convert it to its own hash table and return it
            $hash = @{}
            foreach ($property in $InputObject.PSObject.Properties) {
                $hash[$property.Name] = ConvertTo-Hashtable -InputObject $property.Value
            }
            $hash
        } else {
            ## If the object isn't an array, collection, or other object, it's already a hash table
            ## So just return it.
            $InputObject
        }
    }
}

$data_json = @'
{
  "status": "OK",
  "result": [
    "configuration_file1",
    "configuration_file2",
    "configuration_file4"
  ]
}
'@

# This does not work with nested JSON

  $json_obj = $data_json | convertfrom-json
  $response = $json_obj | ConvertTo-HashTable
  if ( -not ($response['status'] -eq 'OK' ) ) {
     write-host ('Exit response status: {0}' -f $response['status'])
     exit
  }
  write-host 'Response:'
  write-host $response
  $result = $response['result']

  $count = $result.Count
  write-host 'Result:'
  write-host $result

  write-host ('downloading {0} files' -f $count)

  0..($count - 1)| foreach-object {
    $i = $_
    $filename = $result[$i]
    write-host ('downloading config[{0}]: {1}' -f $i, ($result[$i]))
  }
