function Get-TableIndexByHeadersGeneric {
    param(
        [Utils.HtmlDocument]$doc,
        [string[]]$expectedHeaders,
        [int]$minColumns = 1
    )

    $tables = $doc.GetElementsByTagName('table')
    for ($i = 0; $i -lt $tables.Count; $i++) {
        $table = $tables[$i]

        # Prefer thead > tr, else first tr
        $headerRow = $table.QuerySelectorAll('thead > tr') | Select-Object -First 1
        if (-not $headerRow) {
            $headerRow = $table.QuerySelectorAll('tr') | Select-Object -First 1
        }
        if (-not $headerRow) { continue }

        # Get all th, fallback to td
        $cells = $headerRow.QuerySelectorAll('th')
        if ($cells.Count -eq 0) { $cells = $headerRow.QuerySelectorAll('td') }

        if ($cells.Count -lt $minColumns) { continue }

        # Collect normalized text
        $cellTexts = $cells | ForEach-Object { ($_ .InnerText -replace '\s+','').ToLower() }

        # Check all expected headers are present somewhere
        $match = $true
        foreach ($h in $expectedHeaders) {
            if (-not ($cellTexts -contains ($h -replace '\s+','').ToLower())) {
                $match = $false
                break
            }
        }

        if ($match) { return $i }
    }

    return -1
}

# Usage example:
$expected = @('Name','Size','Description')
$minCols = 3
$index = Get-TableIndexByHeadersGeneric -doc $h -expectedHeaders $expected -minColumns $minCols
Write-Host "Matching table index: $index"

