# Path to an existing Excel file
$excelFile = "${env:userprofile}\Desktop\test.xls"

# Sheet and cell to work with
$sheetName = "Test"
$cellAddress = "A2"

# Create Excel COM object
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

# Open workbook
$workbook = $excel.Workbooks.Open($excelFile)
$sheet = $workbook.Worksheets.Item($sheetName)

Write-Host "reading file ${excelFile} sheet ${sheetName}"

# Select range
$range = $sheet.Range($cellAddress)

# -----------------------------
# OLD BAD ATTEMPT (kept for reference)
# -----------------------------
<#
if ($range.Comment -ne $null) {
    $readText = $range.Comment.Text()

    Write-Host "Existing Comment read from cell $cellAddress :"
    Write-Host $readText
} else {
    Write-Host "No  Comment in cell $cellAddress :"
}
#>

# -----------------------------
# NEW SAFE COMMENT READ LOGIC
# -----------------------------
$runCount = 0
$oldText = ""

try {
    $oldText = $range.Comment.Text()
    Write-Host "Existing Comment read from cell $cellAddress :"
    Write-Host $oldText

    # Extract Run #N if present
    if ($oldText -match 'Run\s*#(\d+)') {
        $runCount = [int]$matches[1]
    }
}
catch {
    Write-Host "No comment in cell $cellAddress"
}

# Increment counter
$runCount++

# Remove existing comment if present
try {
    $range.Comment.Delete()
}
catch {
    # ignore if no comment
}

# New Text for the comment (with counter)
$commentText = @"
Run #$runCount
"@

# Add new comment
$range.AddComment($commentText) | out-null

# Read comment back
$readText = $range.Comment.Text()

Write-Host "New Comment read from cell $cellAddress :"
Write-Host $readText

Write-Host 'Save and close'
$workbook.Save() | Out-Null
$workbook.Close() | Out-Null

# Quit Excel
$excel.Quit()

# Release COM objects (VERY important on Win7/Office 2003)
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($range)     | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($sheet)     | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($workbook)  | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel)     | Out-Null

[GC]::Collect()
[GC]::WaitForPendingFinalizers()

Write-Host "Done."

