@echo off
REM  origin: http://www.cyberforum.ru/powershell/thread2124072.html
REM  see also:
REM  https://stackoverflow.com/questions/7985755/how-to-detect-if-cmd-is-running-as-administrator-has-elevated-privileges
REM  https://stackoverflow.com/questions/1894967/how-to-request-administrator-access-inside-a-batch-file

REM check if elevation is necessary
net session >nul 2>&1 && goto :run
if /i "%~1"=="yes" goto :run
>"%temp%\uac.vbs" echo set objShell = CreateObject^("Shell.Application"^)
>> "%temp%\uac.vbs" echo objShell.ShellExecute "%~0", "yes", , "runas", 1
cscript /nologo /e:vbscript "%temp%\uac.vbs"
exit /b 0
 
:run
del "%temp%\uac.vbs"
cd /d "%~dp0"
title %cd%
powershell.exe -executionpolicy bypass -command "&{cd '%~dp0';.\script.ps1}"
echo Done.
pause
