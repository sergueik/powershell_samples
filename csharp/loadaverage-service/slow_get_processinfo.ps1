# origin: https://qna.habr.com/q/1348406
# see also:
# https://www.red-gate.com/simple-talk/sysadmin/powershell/powershell-day-to-day-admin-tasks-monitoring-performance/
# https://www.msxfaq.de/code/powershell/psperfcounter.htm
# custom counters:
# https://ilovepowershell.com/powershell-for-windows-server/monitoring-processes-with-custom-performance-counters/
# see also:
# https://github.com/PowerShell/PowerShell/blob/master/src/Microsoft.PowerShell.Commands.Diagnostics/GetCounterCommand.cs
# https://github.com/PowerShell/PowerShell/blob/master/src/Microsoft.PowerShell.Commands.Diagnostics/CounterSet.cs
# https://github.com/PowerShell/PowerShell/blob/master/src/Microsoft.PowerShell.Commands.Diagnostics/PdhHelper.cs
# https://github.com/PowerShell/PowerShell/blob/master/src/Microsoft.PowerShell.Commands.Diagnostics/CounterSample.cs#L48

# https://github.com/PowerShell/PowerShell/blob/master/test/powershell/Modules/Microsoft.PowerShell.Diagnostics/Get-Counter.Tests.ps1
# https://github.com/MicrosoftDocs/azure-docs/issues/48447
# suggested to monitor:
# New-AzOperationalInsightsWindowsPerformanceCounterDataSource -ResourceGroupName rgname -WorkspaceName loganalyticsworkspacename -ObjectName "Memory" -InstanceName "*" -CounterName "Available MBytes" -IntervalSeconds 60 -Name "Memory-Available MBytes"
function GetProcessInfo {
    $lang = ([CultureInfo]::InstalleduICulture).Name
    if ($lang -match "ru-") { $cpucounter = '\процесс(*)\% загруженности процессора' }
    else { $cpucounter = '\process(*)\% processor time' }
    # https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.diagnostics/get-counter?view=powershell-5.1
    $allProcesses = (Get-Counter $cpucounter -ErrorAction SilentlyContinue).CounterSamples | Sort CookedValue -Descending                                     

    $results = @()

    foreach ($process in $allProcesses) {
        $processName = $process.InstanceName
        $pidi = (Get-Process -Name $processName).Id
        $username = (Get-Process -Name $processName -IncludeUserName).UserName
        $p = Get-Process -id $pidi
        $temp = 0
        $gpu = ((Get-Counter "\GPU Engine(pid_$($p.id)*engtype_3D)\Utilization Percentage").CounterSamples | where CookedValue).CookedValue
        foreach ($p in $gpu) {
            $temp = [math]::Round($p, 2)
        }
        [pscustomobject]@{
            ProcessName = $processName
            PID = $pidi
            CPU = [math]::Round(($process.CookedValue / ($allProcesses.CookedValue)[0] * 100), 2)
            User = $username
            GPU = [math]::Round($temp, 2)
        }
    }
       $results
}
GetProcessInfo
