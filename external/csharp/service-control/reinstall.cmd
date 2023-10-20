@echo OFF
set APPNAME=TransactionService.exe
del Program\bin\Debug\WindowsService.NET_20220224.txt
set PASSWORD=
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe  -uninstall Program\bin\Debug\%APPNAME%
InstallUtil.exe /username=%USERDOMAIN%\%USERNAME% /password=%PASSWORD% -install Program\bin\Debug\%APPNAME%
