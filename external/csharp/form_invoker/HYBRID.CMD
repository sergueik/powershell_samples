

/* 2>nul
@ECHO OFF
echo %ERRORLEVEL%
REM 9009
SETLOCAL enabledelayedexpansion
echo %ERRORLEVEL%
set "key=HKLM\SOFTWARE\Microsoft\.NETFramework"
    for /f "tokens=3" %%i in (
       
      '2^>nul reg query %key% /v InstallRoot'

    ) do set "root=%%i"
    if /i "!root!" equ "" echo:Could not find .NET root.
    
    for /f "delims=" %%i in (
      'dir /ad /b "!root!" ^| findstr /irc:"v[0-9.].*"'
    ) do (
echo i=%%i
      set "csc=%root%%%i\csc.exe"
      if exist "!csc!" set "csc=!csc!"
echo csc=!csc!
    )

goto :EOF
*/