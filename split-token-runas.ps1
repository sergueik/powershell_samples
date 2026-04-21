# origin: https://github.com/SteveTheKiller/killer-scripts/blob/main/DEPOT.ps1
$Identity  = [Security.Principal.WindowsIdentity]::GetCurrent()
$Principal = [Security.Principal.WindowsPrincipal]$Identity
if (-not $Principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "[!] Not running as Administrator. Relaunching elevated..." -ForegroundColor Yellow
    $scriptPath = $MyInvocation.MyCommand.Path
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`"" -Verb RunAs
    exit
}
# Confirm token is fully elevated (handles domain UAC split-token scenarios)
# The S-1-5-32-544 is the well-known Security Identifier for the 
# built-in Administrators group on Windows systems
# a constant, universally recognized ID across all Windows installations, representing local administrators with full control over the computer. This SID is used to manage high-privileged access and security
$TokenElevation = [System.Security.Principal.WindowsIdentity]::GetCurrent().Claims |
    Where-Object { $_.Type -eq "http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid" -and $_.Value -eq "S-1-5-32-544" }
if (-not $TokenElevation) {
    # Force a new elevated process using explicit RunAs to get a full admin token
    $scriptPath = $MyInvocation.MyCommand.Path
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`"" -Verb RunAs
    exit
}
