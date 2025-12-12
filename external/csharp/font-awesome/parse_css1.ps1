param(
  [string]$filename = 'bootstrap-icons.css'
)
# $debugpreference= 'Continue'

$filepath = (resolve-path -path $filename ).path 
write-debug ('Processing CSS {0}' -f $filepath )

$payload = Get-Content -path $filepath -raw
write-debug ('Processing {0} lines' -f  (($payload -split [char]10).count) )  

# regex matches:
# .bi-person-fill-dash::before { content:"\f89f"; }
$pattern = '\.bi-([a-z0-9\-]+)::before\s*\{\s*content:\s*"\\([0-9a-fA-F]+)"?;'

$matches = [regex]::Matches($css, $pattern)
write-debug ('Processing {0} characters' -f $matches.Count)
$results = foreach ($m in $matches) {
    [PSCustomObject]@{
        Name      = $m.Groups[1].Value
        Hex       = $m.Groups[2].Value.ToLower()
        Codepoint = [Convert]::ToInt32($m.Groups[2].Value,16)
        Char      = [char]([Convert]::ToInt32($m.Groups[2].Value,16))
    }
}

$results

