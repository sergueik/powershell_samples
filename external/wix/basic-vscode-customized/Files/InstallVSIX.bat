@echo off
setlocal

set CODECMD="%LocalAppData%\Programs\Microsoft VS Code\bin\code.cmd"
set VSIXFILE="%~dp0\vscode-extension-for-zowe.vsix"

:wait
if not exist %CODECMD% (
    echo waiting for %CODECMD%
    timeout /t 2 >nul
    goto wait
)

%CODECMD% --install-extension %VSIXFILE% --force

