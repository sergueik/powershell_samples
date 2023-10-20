@echo OFF
REM see also:
REM https://community.idera.com/database-tools/powershell/ask_the_experts/f/powershell_for_windows-12/19845/get-process-user-filtering
REM assume headless environment
set WINDOWS_NO_DISPLAY=true
REM check if explorer.exe is running for the current user
for /F %%. in ('tasklist.exe /FI "USERNAME eq %USERNAME%" /FI "IMAGENAME EQ explorer.exe"^| findstr.exe /ic:"explorer"') do set WINDOWS_NO_DISPLAY=false&& echo setting WINDOWS_NO_DISPLAY to %WINDOWS_NO_DISPLAY%&& exit /b 0
exit /b 1

REM Usage:
REM
REM call headless_detector.cmd
REM echo %WINDOWS_NO_DISPLAY%
REM true
