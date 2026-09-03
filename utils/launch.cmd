@echo OFF
set "DOTNET_HOME=%LOCALAPPDATA%\.dotnet"
if NOT exist "%DOTNET_HOME%" goto :EOF
rem prepend to the PATH to cope with enterprise environment
rem when multiple "DOTNET" are installed on the machine 
rem and the advertised on demand DOTNET 
rem is later in the PATH after the group policy installed one 
rem with PATH in System environment scope
rem presumable the group policy runtime has no SDK included
set PATH=%DOTNET_HOME%;%PATH%
if "%~1" == "-" cmd /K && goto :EOF
rem if any other argument is passed run it otherwise launch VS Code
if "%~1" == "" call code.cmd &&  goto :EOF
%*

