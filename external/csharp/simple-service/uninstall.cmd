@echo OFF

REM assuming en-US locale
REM %DATE% Sat 02/26/2022
REM        12345678901234
REM %TIME% 10:57:29.06

set LOG_SUFFIX=_%DATE:~10,4%%DATE:~4,2%%DATE:~7,2%
set EVENTLOGNAME=TestLog
set SOURCE=%EVENTLOGNAME%
REM set SOURCE=TestService
set SERVICENAME=WindowsService.NET
set APPNAME=WindowsService.NET.exe
del Program\bin\Debug\%APPNAME:~1,17%%LOG_SUFFIX%.txt
set PASSWORD=
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
REM TODO: option 
REM "uninstall" to proceed the following two lines: run -uninstall command end exit the script
REM "install" to skip the following two lines 
InstallUtil.exe -uninstall Program\bin\Debug\%APPNAME%

call c:\windows\system32\wevtutil.exe get-log "%EVENTLOGNAME%" 1>nul 2>nul
if errorlevel 15007 echo "No log found" && goto :EOF

REM https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wevtutil
call :REMOVE_LOG

goto :EOF

:REMOVE_LOG
echo removing log %EVENTLOGNAME%

C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -noprofile -executionpolicy bypass -command "$logname = '%EVENTLOGNAME%'; remove-eventlog -logname $logname -erroraction silentlycontinue"
timeout /nobreak /t 3
echo getting log %EVENTLOGNAME%
call c:\windows\system32\wevtutil.exe get-log "%EVENTLOGNAME%"
timeout /nobreak /t 3

goto :EOF

REM
REM TODO: remove log publisher and log
wevtutil.exe enum-publishers |  findstr /i "TransactionLog TransactionService"