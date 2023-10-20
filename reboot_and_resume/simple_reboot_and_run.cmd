@echo off
REM origin: http://forum.oszone.net/thread-332188-2.html
call :isAdmin
REM alternatively
REM call isadmin.cmd
if %errorlevel% == 0 (
  goto :run
) else (
  echo Requesting administrative privileges...
  goto :UACPrompt
)
exit /b

:isAdmin
REM https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/fsutil
REM run privileged operation
fsutil.exe dirty query %systemdrive% >nul
exit /b

:run
REM the installer script

set SCRIPTPATH=install.cmd
if not "%~1" == "" set "SCRIPTPATH=%~1"

set BAT=%~dp0%SCRIPTPATH%

reg.exe ADD HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v EnableLUA /t REG_DWORD /d 0 /f
reg.exe ADD HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce /v Install /t REG_SZ /d "%BAT%" /f

@echo.
@echo Begin process of installation of drivers
@echo.
timeout.exe -t 2
REM shutdown -t 0 -r -f
echo Rebooting the system wait 6 sec ... >con
shutdown.exe -r -t 6 -c "The system will reboot after 6 sec ..."
exit /b

:UACPrompt
echo Set s = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
echo s.ShellExecute "cmd.exe", "/c %~s0 %~1", "", "runas", 1 >> "%temp%\getadmin.vbs"
"%temp%\getadmin.vbs"
del "%temp%\getadmin.vbs"
exit /B
