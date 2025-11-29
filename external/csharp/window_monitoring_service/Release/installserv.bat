set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v2.0.50727
set PATH=%PATH%;%DOTNETFX2%
InstallUtil /u screenmonitorservice.exe
InstallUtil /i screenmonitorservice.exe
pause