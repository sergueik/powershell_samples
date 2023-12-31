# origin: https://www.cyberforum.ru/powershell/thread3078719.html#post16748460
param (
    # Количество считываний показаний для вычисления из них среднего
    [ValidateRange(1, 900)] [int] $Count = 60,
    # Порог средней загруженности для срабатывания скриптблока Action
    [ValidateRange(0, 100)] [int] $Threshold = 50,
    # Скриптблок будет выполнен, если Threshold больше или равен средней загрузке
    [scriptblock] $Action,
    # Не будут выводится показания, только в конце
    [switch] $Silent
)
 
begin { $CpuLoadPercentage = @() }
 
process {
    foreach ($i in @(1..$Count)) {
        $ProcessorLoadPercentage = Get-CimInstance CIM_Processor |
            Select-Object -Property DeviceID, LoadPercentage
 
        if (!$Silent) { $ProcessorLoadPercentage }
 
        $CpuLoadPercentage += $ProcessorLoadPercentage.LoadPercentage
    }
}
 
end {
    $Average = ($CpuLoadPercentage | Measure-Object -Average).Average
 
    $Out = [PSCustomObject]@{
        Average   = $Average
        Count     = $Count
        Threshold = $Threshold
    }
 
    $Out
    
    if ($Average -ge $Threshold -and $Action) { & $Action $Out }
}
 
<#
.SYNOPSIS
    Средние проценты загруженности процессора
.INPUTS
    Count
        Количество считываний показаний для вычисления из них среднего
.INPUTS
    Threshold
        Порог средней загруженности для срабатывания скриптблока Action
.INPUTS
    Action
        Скриптблок будет выполнен если Threshold больше или равен средней загрузке
        В скриптблок, параметром, передаётся [PSCustomObject]
        с полями: Average, Count, Threshold
        см. примеры
.INPUTS
    Silent
        Не будут выводится все показания, только в конце, среднее
.EXAMPLE

    .\Get-AverageCpuLoadPercentage.ps1 -Count 5 -Threshold 11 -Action { param ($x) write-host "average cpu load percentage: $($x.Average)`n" } -silent:$false

   PS C:\> 1..12 | % {
        .\Get-CpuLoadPercentage -Count 5 -Threshold 11 -Action {
                param ($x) "ВЫСОКАЯ СРЕДНЯЯ НАГРУЗКА: $($x.Average)`n"
            }
        }
    Считывание показаний 5 раз и вычисление из них среднего и так 12 раз
    Если средняя нагрузка больше или равна Threshold, то сработает скриптблок
.NOTES
    Автор: iNNOKENTIY21
#>
