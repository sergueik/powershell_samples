@echo OFF
REM internet speed and Wi-Fi strength tester with a BAT file and Powershell
REM NOTE the lack of slash between directory and filename
REM NOTE the lack of slash between directory and filename
if /i "%DEBUG%"=="true" @echo powershell.exe -ExecutionPolicy bypass %~dp0speed-test.ps1
call powershell.exe -executionpolicy bypass %~dp0speed_test.ps1
goto :EOF
