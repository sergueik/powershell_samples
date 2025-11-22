try {
    $resp = Invoke-RestMethod -Method Post -Uri "http://localhost:9200/oom-events/_doc" -ContentType "application/json" -Credential (New-Object System.Management.Automation.PSCredential($U,(ConvertTo-SecureString $P -AsPlainText -Force))) -Body '{"message":"test"}'
    Write-Host 'Telemetry OK'
} catch {
    Write-Host ( 'Telemetry FAILED: ' + $_.Exception.Message )
}
