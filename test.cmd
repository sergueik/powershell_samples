@echo OFF 
call c:\java\init.cmd
set APPDIR=c:\java\app
set APP=example.upload.jar
cd %APPDIR%
java.exe -jar %APP%