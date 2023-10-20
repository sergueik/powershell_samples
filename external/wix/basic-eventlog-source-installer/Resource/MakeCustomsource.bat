@echo OFF
set TARGET=messages
del %TARGET%.dll %TARGET%.rc %TARGET%.res
REM call "c:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\vcvars32.bat"
call "c:\Program Files\Microsoft Visual Studio 10.0\VC\bin\vcvars32.bat"
REM ignore the ERROR: Cannot determine the location of the VS Common Tools folder. 
set PATH=c:\Program Files\Microsoft Visual Studio 10.0\VC\bin;c:\Program Files\Microsoft SDKs\Windows\v7.1\Bin;%PATH%
mc.exe -u %TARGET%.mc
rc.exe -r -fo %TARGET%.res %TARGET%.rc
link.exe -dll -noentry -out:%TARGET%.dll %TARGET%.res /MACHINE:X86
REM Alternatively
c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /win32res:%TARGET%.res /unsafe /target:library /out:.\%TARGET%_csc.dll
xcopy.exe /Y %TARGET%.dll ..\Setup
