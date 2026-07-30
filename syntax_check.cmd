@echo OFF
set SCRIPT=%1
if /i "%SCRIPT%" EQU "" >&2 echo Usage: %0 ^<SCRIPT NAME^>  && exit /b 1
if NOT exist "%SCRIPT%" >&2 echo Usage: %0 ^<SCRIPT NAME^> && >&2 echo %SCRIPT% not found  && exit /b 1
@powershell.exe -noprofile -command "&{$p = (resolve-path -path '.').path + '\' + $args[0]; write-host( 'Validating syntax {0} ' -f $p );$e = $null ; [System.Management.Automation.PSParser]::Tokenize((get-content -raw $p ), [ref]$e)|out-null; if ($e.count -gt 0) { $e |format-list } else { write-host 'OK'}}" %SCRIPT%

exit /b
REM  TODO:
REM  deal with


REM Token   : System.Management.Automation.PSToken
REM Message : Unable to find type [System.Windows.Forms.Form].
