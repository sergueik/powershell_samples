param(
    [string]$filename = 'bootstrap-icons.css'
)
# $debugpreference= 'Continue'

$filepath = (Resolve-Path $filename).Path
write-debug "Processing CSS: $filepath"

$regex = [regex]'\.bi-([a-z0-9\-]+)::before\s*\{\s*content:\s*"\\([0-9a-fA-F]+)";'

$results = @{}

# Stream file line-by-line
get-content -path $filepath |
    foreach-object {
        $line = $_
        $match = $regex.Match($line)
        if ($match.Success) {
            $name = $match.Groups[1].Value
            $hex  = $match.Groups[2].Value.ToLower()

            # Add to hash table
            $results[$hex] = $name
        }
    }
write-debug('Loaded {0} characters' -f $results.keys.cout)
$results
