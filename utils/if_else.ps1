# ./return_true.ps1 2>&1>$null 
./return_true.ps1 |out-null
echo $?
# ./return_true.ps1 |out-null && Write-Host "result: OK: $?" || Write-Host "result: not OK: $?"
# The token '&&' is not a valid statement separator in this version.
# ./return_false.ps1 |out-null && Write-Host "result: OK ($?)" || Write-Host "result: KO ($?)"
