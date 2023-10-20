# https://www.cyberforum.ru/powershell/thread2847764.html
   Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName PresentationFramework
[System.Windows.Forms.Application]::EnableVisualStyles()
$Form                            = New-Object system.Windows.Forms.Form
$Form.ClientSize                 = '480,400'
$Form.Text                       = "Test"
$Form.TopMost                    = $false
$Form.FormBorderStyle            = 'Fixed3D'
$Form.Autosize                   = $true
$Form.minimumSize                = New-Object System.Drawing.Size(470,400) 
 
 
#Place Holder Form Junk Above
$button = New-Object System.Windows.Forms.Button
$button.Text = 'add'
$button.Font = New-Object System.Drawing.Font('Franklin Gothic',8)
$button.FlatStyle = [System.Windows.Forms.FlatStyle]::Flat
$button.FlatAppearance.BorderSize = 0
$button.ForeColor = [System.Drawing.ColorTranslator]::FromHtml("#ffffff")
$button.BackColor = [System.Drawing.ColorTranslator]::FromHtml("#515582")
$button.Size = New-Object System.Drawing.Size(90,90)
$button.Location = New-Object System.Drawing.Point(10,10)
$button.Add_Click({
 
#Reset these on Run
$Global:x             = 10 #Reset up down
$Global:z             = 10 #Reset left right
 
Function Make-Button([string] $ToolName, [string] $ToolPath, [int] $SetZ, [int] $SetX){
$Button                         = New-Object system.Windows.Forms.Button
$Button.text                    = $ToolName
$Button.Size = New-Object System.Drawing.Size(90,90)
$Button.location                = New-Object System.Drawing.Point($SetZ,$SetX)
$Button.Font                    = New-Object System.Drawing.Font('Franklin Gothic',10)
$Button.FlatStyle           = [System.Windows.Forms.FlatStyle]::Flat
$Button.FlatAppearance.BorderSize  = 0
$Button.ForeColor           = [System.Drawing.ColorTranslator]::FromHtml("#ffffff")
$Button.BackColor           = [System.Drawing.ColorTranslator]::FromHtml("#515582")
 
$Button.tag = $Tool
$Button.Add_Click{start $this.tag}
 
$Form.Controls.Add($Button)
 
Write-Host "$ToolName"
Write-Host "$Tool"
Write-Host "z: $SetZ"
Write-Host "x: $SetX"
 
}
 
function Get-Position{
switch ($Global:ObjectNumber) {
-1
{
$Global:ObjectNumber += 1
Write-Host "Object:" $Global:ObjectNumber 
 
$Global:x = 10
$Global:z += 0}
 
 
Default{
$Global:ObjectNumber += 1
Write-Host "Object:" $Global:ObjectNumber 
 
$Global:x += 0
$Global:z += 110
}
}#end switch
 
if($Global:z -eq 340){
$Global:x = 110
$Global:z = 10
Write-Host "New Row"
While($Global:z -eq 340){
$Global:x += 220
$Global:z += 10
Write-Host "New Row"
}
}
Write-Host "get-position z: $global:Z"
Write-Host "get-position x: $global:X"
}
 
 
$Tools = New-Object system.Windows.Forms.Button
$Count = ( $Tools | Measure-Object ).Count;
Write-Host "Entries:" $Count
 
 
$Names = @($Tools) #Put Tools in Array
$Names | ForEach-Object{
 
  Get-Position
  Make-Button ($_.Name).replace(".exe","") ($_.FullName) $Global:z $Global:x
}
})
 
$Form.Controls.Add($button)
[void]$Form.ShowDialog()
