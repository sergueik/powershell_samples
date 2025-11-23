$U = "elastic"
$P = "changeme"

$body = @{
    timestamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    message   = 'Test probe: HTTP-only telemetry working'
    mem       = 12345
} | ConvertTo-Json

Invoke-RestMethod -Method Post -Uri http://localhost:9200/oom-events/_doc -ContentType 'application/json' -Credential (New-Object System.Management.Automation.PSCredential($U,(ConvertTo-SecureString $P -AsPlainText -Force))) -Body $body

# Query
$query = '{"query":{"match_all":{}},"size":5,"sort":[{"timestamp":{"order":"desc"}}]}'
Invoke-RestMethod -Method Get -Uri 'http://localhost:9200/oom-events/_search?pretty' -ContentType 'application/json' -Credential (New-Object System.Management.Automation.PSCredential($U,(ConvertTo-SecureString $P -AsPlainText -Force))) -Body $query

