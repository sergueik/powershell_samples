$clsid = "{52A2AAAE-085D-4187-97EA-8C30DB990436}"
$regKey = "HKCR:\CLSID\$clsid"
if (Test-Path $regKey) {
    Get-ChildItem $regKey -Recurse | Format-List
} else {
    Write-Host "CLSID $clsid not found in registry"
}

# reg query "HKCR\CLSID\{52A2AAAE-085D-4187-97EA-8C30DB990436}" /s
