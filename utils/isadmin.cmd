@echo OFF 
REM see: http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx

powershell.exe -noprofile -executionpolicy bypass ^
"$status = [boolean]((new-object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)); write-output $status ;exit($status) "
REM NOTE: inverted errorlevel check due to Powershell
REM using 1 for True and 0 for False
if %errorlevel% == 1 (
  goto :EOF
) else (
  echo Will need to Request administrative privileges.
  goto :EOF
)
