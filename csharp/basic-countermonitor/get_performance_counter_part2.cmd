@echo off
set SCRIPT_NAME=%~dp0get_performance_counter_part2.ps1
powershell.exe -ExecutionPolicy Bypass -File %SCRIPT_NAME% %*
exit /b %errorlevel%
