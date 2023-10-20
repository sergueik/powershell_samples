@echo OFF
SETLOCAL ENABLEDELAYEDEXPANSION
REM https://stackoverflow.com/questions/4192971/in-powershell-how-do-i-convert-datetime-to-unix-time

set ARGC=0
for %%. in (%*) do Set /A ARGC+=1

IF "%ARGC%"=="0" goto :NOARG
IF "%ARGC%"=="1" goto :SINGLEARG
set FROM_DATE=%1 %2
GOTO :START
:SINGLEARG
set FROM_DATE=%~1
:START
set F1=%temp%\a.txt
echo FROM_DATE=%FROM_DATE%
if "%FROM_DATE%"==""  GOTO :NOARG
powershell.exe -executionpolicy remotesighed -noprofile -command " & { $input = $args[0] ; $date = Get-Date($input); $seconds = [Math]::Floor([decimal](Get-Date($date).AddDays(1) -uformat '%%s')); write-output $seconds} " "'%FROM_DATE%'" > %F1%
GOTO :RESULT
:NOARG

set F2=%temp%\a.txt
date /t > %F2%
type %F2%
for /F "tokens=*" %%. in ('type %F2%') do (
  powershell.exe -executionpolicy remotesighed -noprofile -command " & { $input = $args[0] ; $date = Get-Date($input); $seconds = [Math]::Floor([decimal](Get-Date($date).AddDays(1) -uformat '%%s')); write-output $seconds } " "'%%.'" > %F1%
)
del /q %F2%
:RESULT

for /F "tokens=*" %%. in ('type "%F1%"') do set RESULT=%%.
del /q %F1%
echo %RESULT%
GOTO :EOF