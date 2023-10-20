@echo OFF
set APPNAME=TransactionService.exe
set PATH=%PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319
InstallUtil.exe  -uninstall Program\bin\Debug\%APPNAME%

