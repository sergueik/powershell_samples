@echo OFF

REM assuming en-US locale
REM %DATE% Sat 02/26/2022
REM        12345678901234
REM %TIME% 10:57:29.06

set LOG_SUFFIX=_%DATE:~10,4%%DATE:~4,2%%DATE:~7,2%REM https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/wevtutil
call :REMOVE_LOG
if errorlevel 15007 echo "No log found" && goto :CREATE_LOG


set SERVICENAME=WindowsService.NET
set APPNAME=WindowsService.NET.exe
del Program\bin\Debug\%APPNAME:~1,17%%LOG_SUFFIX%.txt
set PASSWORD=
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
REM TODO: option 
REM "uninstall" to proceed the following two lines: run -uninstall command end exit the script
REM "install" to skip the following two lines 
InstallUtil.exe -uninstall Program\bin\Debug\%APPNAME%
REM goto :EOF
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\%APPNAME%
REM NOTE: alternatively
REM sc.exe create %APPNAME% binpath= "Program\bin\Debug\%APPNAME%" start= auto Displayname= "%APPNAME%"
REM set Description=C# Demo Service
REM sc.exe description %APPNAME% "%Description%"
REM see also: https://github.com/MScholtes/Windows-Service/blob/master/NamedPipesService/Install.bat#L13
sc.exe start %SERVICENAME%
timeout 3
goto :EOF

REM
REM TODO: remove log publisher and log
wevtutil.exe enum-publishers |  findstr /i "TransactionLog TransactionService"
