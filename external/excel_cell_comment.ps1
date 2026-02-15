# Path to an existing Excel file
$excelFile = "${env:userprofile}\Desktop\test.xls"
<#
 deal with Excel’s crash/lock recovery mechanism.
 check for owner / lock file - hidden file named ~$test.xls 
 in the same directory as workbook containing
 username
 

machine name


 session info
#>
$lockFile = Join-Path (Split-Path $excelFile) ("~$" + (Split-Path $excelFile -Leaf))

if (test-path $lockFile) {
  Write-Host "Workbook is locked (lock file exists): $lockFile"
  Remove-Item $lockFile -Force
}
else {
  Write-Host "No lock file detected."
}


# detect Excel is still running in memory (orphaned Excel.exe)
if ( Get-Process excel -ErrorAction SilentlyContinue) { 

}

# Sheet and cell to work with
$sheetName = "Test"
$cellAddress = "A2"

# Create Excel COM object
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

# Open workbook
$workbook = $excel.Workbooks.Open($excelFile)

if ($workbook.ReadOnly) {
    Write-Host "Workbook opened in ReadOnly mode (likely locked by another Excel instance)."
    # there is no safe API to unlock this from script.
}

$sheet = $workbook.Worksheets.Item($sheetName)

Write-Host "reading file ${excelFile} sheet ${sheetName}"

# Select range
$range = $sheet.Range($cellAddress)

# NOTE: the $range.Comment -ne $null... does not work

$runCount = 0
$oldText = ""

try {
    $oldText = $range.Comment.Text()
    Write-Host "Existing Comment read from cell $cellAddress :"
    Write-Host $oldText

    if ($oldText -match 'Run\s*#(\d+)') {
        $runCount = [int]$matches[1]
    }
}
catch [Exception] {
    Write-Debug "No comment in cell $cellAddress"
}

$runCount++

try {
    $range.Comment.Delete()
}
catch [Exception]{
}

$commentText = @"
Run #$runCount
"@

$range.AddComment($commentText) | out-null

$readText = $range.Comment.Text()

Write-Host  ('New comment read from cell {0} : {1}' -f $cellAddress, $readText )


Write-Host 'Save and close'
$workbook.Save() | Out-Null
$workbook.Close($true) | Out-Null

if ($excel) {
  $excel.Quit()
}

[System.Runtime.InteropServices.Marshal]::ReleaseComObject($range)     | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($sheet)     | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($workbook)  | Out-Null
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel)     | Out-Null

[GC]::Collect()
[GC]::WaitForPendingFinalizers()

