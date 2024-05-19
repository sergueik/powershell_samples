@echo OFF
@REM
set NAME=LoadAverageService
reg query "HKLM\SYSTEM\CurrentControlSet\Services" /k /f "%NAME%" 
reg query "HKLM\SYSTEM\CurrentControlSet\Services" /k /f "%NAME%" | findstr.exe /ic:"%NAME%"
for /f %%. in ('reg query "HKLM\SYSTEM\CurrentControlSet\Services" /k /f "%NAME%" ^| findstr.exe /ic:"%NAME%"') do (reg.exe query "%%." /v "Start")
REM 4 disabled
REM 3 manual
REM 2 Automatic
REM DelayedAutostart 1/0
for /f %%. in ('reg query "HKLM\SYSTEM\CurrentControlSet\Services" /k /f "%NAME%" ^| findstr.exe /ic:"%NAME%"') do (reg.exe add "%%." /v "Start" /t reg_dword /d 3 /f)

