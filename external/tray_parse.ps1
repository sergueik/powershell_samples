$encText = New-Object System.Text.UTF8Encoding
[byte[]] $bytRegKey = @()
$strRegKey = ""
$bytRegKey = $(Get-ItemProperty $(Get-Item 'HKCU:\Software\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\TrayNotify').PSPath).IconStreams
write-host ('convering {0} bytes' -f $bytRegKey.Count)
for($x=0; $x -le $bytRegKey.Count; $x++)
{
write-host ('convering {0} byte' -f $x)
$tempString = [Convert]::ToString($bytRegKey[$x], 16)
switch($tempString.Length)
{
0 {$strRegKey += "00"}
1 {$strRegKey += "0" + $tempString}
2 {$strRegKey += $tempString}
}
}
write-host  $strRegKey