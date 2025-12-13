param(
    [string]$filename = 'straight.css',
    [string]$enumName = 'FlatIcons',
    [string]$namespace = 'Utils',
    [switch]$sort
)

# $debugpreference= 'Continue'
$filepath = (resolve-path -path $filename ).path 
write-debug ('Processing CSS {0}' -f $filepath )

$payload = Get-Content -path $filepath -raw
write-debug ('Processing {0} lines' -f  (($payload -split [char]10).count) )  

# Converts kebab-case -> PascalCase compliant with C# identifiers
function Convert-ToPascal([string]$name) {
    $parts = $name -split '-'
    ($parts | ForEach-Object { $_.Substring(0,1).ToUpper() + $_.Substring(1) }) -join ''
}

# regex matches:
# .bi-person-fill-dash::before { content:"\f89f"; }
$pattern = '\.bi-([a-z0-9\-]+)::before\s*\{\s*content:\s*"\\([0-9a-fA-F]+)"?;'
$pattern = '\.([a-z0-9\-]+):{1,2}before\s*\{[^}]*?content\s*:\s*"\\([0-9a-fA-F]+)"\s*;'

$matches = [regex]::Matches($payload, $pattern)
write-debug ('Processing {0} characters' -f $matches.Count)
$results = @( )
$matches | foreach-object {
  $m = $_
  $results += [pscustomobject]@{
                RawName = $m.Groups[1].Value
                Hex     = $m.Groups[2].Value.ToLower()
                CsName  = Convert-ToPascal $m.Groups[1].Value
            }
}

# Optional sort by hex value
if ($sort) {
    $results = $results | Sort-Object { [int]("0x" + $_.Hex) }
}

# Emit header
"namespace $namespace {"
"    public enum $enumName {"

# Emit each line like: PersonFillDash = 0xf89f,
foreach ($result in $results) {
    "        {0} = 0x{1}," -f $result.CsName, $result.Hex
}

"    }"
"}"

