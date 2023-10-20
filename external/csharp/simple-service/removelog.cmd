@echo OFF
set EVENTLOGNAME=TestLog

call C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -noprofile -executionpolicy bypass -command "$logname = '%EVENTLOGNAME%'; remove-eventlog -logname $logname -erroraction silentlycontinue"
timeout /nobreak /t 3
echo getting log %EVENTLOGNAME%
call c:\windows\system32\wevtutil.exe get-log "%EVENTLOGNAME%"
timeout /nobreak /t 3


goto :EOF