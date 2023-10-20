<# :
  @echo off
    powershell /noprofile /executionpolicy bypass /command ^
    "&{[ScriptBlock]::Create((Get-Content \"%~f0\") -join [Char]10).Invoke()}"
  exit /b
REM origin: http://www.cyberforum.ru/blogs/579090/blog3770.html
#>
