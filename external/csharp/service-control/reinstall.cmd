@echo OFF
set APPNAME=TransactionService.exe
del Program\bin\Debug\WindowsService.NET_20220224.txt
set PASSWORD=
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
installutil.exe -uninstall Program\bin\Debug\%APPNAME%
installutil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\%APPNAME%

REM NOTE: alternatively
REM sc.exe create %APPNAME% binpath= "Program\bin\Debug\%APPNAME%" start= auto Displayname= "%APPNAME%"
REM set Description=C# Demo Service
REM sc.exe description %APPNAME% "%Description%"
REM see also: https://github.com/MScholtes/Windows-Service/blob/master/NamedPipesService/Install.bat#L13
