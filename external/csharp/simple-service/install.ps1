$env:PATH="$env:PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319"
cd c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service

new-eventlog -logname TestLog -source TestLog
write-host 'checking if eventlog is present'
invoke-expression -command "c:\windows\system32\wevtutil.exe get-log TestLog"

installUtil.exe  -install .\Program\bin\Debug\WindowsService.NET.exe
write-host 'querying service'
sc.exe query WindowsService.NET
write-host 'starting service'
sc.exe start  WindowsService.NET

write-host 'querying service'
sc.exe query WindowsService.NET






