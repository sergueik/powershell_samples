@echo off
@REM lighthouse wrapper script
@REM lighthouse https://developers.google.com/web/tools/lighthouse/ 
@REM itself is a nodejs app, most likely "run" as batch itself thus could ruin the setlocal
@REM https://nodejs.org/download/release/latest-v11.x/node-v11.10.1-x86.msi
setLocal EnableDelayedExpansion

for /f "delims=" %%a in (%USERPROFILE%\Documents\NTQ\lighthouse\urlsall.txt) DO (

set "urlName=%%~NXa"
REM it is very likely that lighthouse is a cmd script, like other npm "apps"
REM C:\Windows\system32>where lighthouse.*
REM C:\Users\sergueik\AppData\Roaming\npm\lighthouse
REM C:\Users\sergueik\AppData\Roaming\npm\lighthouse.cmd
call lighthouse --quiet --output=json > !urlName!.json --chrome-flags="-headless" %%a
)
where the urlsall file has urls are like this (but they have the http part which I cannot put in here)