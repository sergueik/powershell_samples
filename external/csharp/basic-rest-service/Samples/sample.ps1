param(
  [string]$httpVerb,
  [string]$body = ''
)

try {
  if (!$env:SCRIPTSERVICES_VERSION) {
    Write-Host "Running natively"
  }
  Write-Output ('HTTP "{0}"' -f $httpVerb ) 
  if ($body -ne '') {
       $o = $body | convertfrom-json
    Write-Output ('key={0}' -f $o.key )
   # TODO: convert to hash
   # Write-Output ($o.keys -join ',' )
    } else { 
    Write-Output 'empty body'
    }

} catch {
	
}
