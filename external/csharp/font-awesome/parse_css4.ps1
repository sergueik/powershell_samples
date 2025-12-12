param(
    [string]$filename = 'bootstrap-icons.css',
    [string]$enumName = 'BootstrapIcons',
    [string]$namespace = 'IconFonts',
    [switch]$Sort
)

$filepath = (Resolve-Path $filename).Path

$regex = [regex]'\.bi-([a-z0-9\-]+)::before\s*\{\s*content:\s*"\\([0-9a-fA-F]+)";'

# convert the Unicode glyph name to common C# PascalCase naming convention for enum members
function toPascal{
  param (
    [string]$name
  ) 
  $name = ($name -split '-' | 
  foreach-object { $_.Substring(0,1).ToUpper() + $_.Substring(1) }) -join ''

  # If the leading character is a digit, prefix with _
  if ($name -match '^[0-9]') { 
	  $name = '_' + $name }

  return $name
}

# Stream parse into PSCustomObject list
$items =
    Get-Content $filepath |
    ForEach-Object {
        $m = $regex.Match($_)
        if ($m.Success) {
            [pscustomobject]@{
                RawName = $m.Groups[1].Value
                Hex     = $m.Groups[2].Value.ToLower()
                CsName  = toPascal -name $m.Groups[1].Value
            }
        }
    }

# Optional sort by hex value
if ($Sort) {
    $items = $items | Sort-Object { [int]("0x" + $_.Hex) }
}

# Emit header
write-output "namespace $namespace {"
write-output "    public enum $enumName {"

# Emit each line like: PersonFillDash = 0xf89f,
foreach ($item in $items) {
    write-output ('        {0} = 0x{1},' -f $item.CsName, $item.Hex )
}

write-output '    }'
write-output '}'

