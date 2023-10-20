@echo OFF

REM assuming en-US locale
REM %DATE% Sat 02/26/2022
REM        12345678901234
REM %TIME% 10:57:29.06

set LOG_SUFFIX=_%DATE:~10,4%%DATE:~4,2%%DATE:~7,2%
set EVENTLOGNAME=TransactionLog
set EVENTLOGNAME=TestLog
set SOURCE=%EVENTLOGNAME%
REM set SOURCE=TestService
REM https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wevtutil
call c:\windows\system32\wevtutil.exe get-log "%EVENTLOGNAME%" 1>nul 2>nul
if errorlevel 15007 echo "No log found" && call :CREATE_LOG


set SERVICENAME=WindowsService.NET
set APPNAME=WindowsService.NET.exe
del Program\bin\Debug\%APPNAME:~1,17%%LOG_SUFFIX%.txt
set PASSWORD=
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
REM TODO: option 
REM "uninstall" to proceed the following two lines: run -uninstall command end exit the script
REM "install" to skip the following two lines 
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\%APPNAME%
sc.exe start %SERVICENAME%
timeout /nobreak /t 3
sc.exe query %SERVICENAME%
goto :EOF

:CREATE_LOG
REM TODO: Install event publisher and log via wevtutil.exe from a manifest (requires manifest)
REM The ampersand (&) character is not allowed. 
REM The & operator is reserved for future use; 
REM wrap an ampersand in double quotation marks ("&") to pass it as part of a string.
REM does now work when called from cmd as "powershell.exe -command
REM works fine when called from Poweshell
REM Failed to read configuration for log TestLog. 
REM The specified channel could not be found. Check channel configuration.
echo creating log %EVENTLOGNAME%
call C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -noprofile -executionpolicy bypass -command "$logname = '%EVENTLOGNAME%'; new-eventlog -source $logname -logname $logname -erroraction silentlycontinue"
timeout /nobreak /t 3
echo getting log %EVENTLOGNAME%

call c:\windows\system32\wevtutil.exe get-log "%EVENTLOGNAME%"
timeout /nobreak /t 3

goto :EOF
REM
REM TODO: remove log publisher and log
wevtutil.exe enum-publishers |  findstr /i "TransactionLog TransactionService"