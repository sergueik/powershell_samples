cd c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service

$env:PATH="$env:PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319"
cd c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service

write-host 'querying service'

sc.exe query WindowsService.NET

write-host 'stopping service'
sc.exe stop WindowsService.NET

write-host 'querying service'
sc.exe query WindowsService.NET


write-host 'uninstalling service'

InstallUtil.exe  -uninstall .\Program\bin\Debug\WindowsService.NET.exe
write-host 'removing eventlog'

remove-eventlog -logname TransactionLog
write-host 'checking if eventlog still present (likely say it is not)'
invoke-expression -command "c:\windows\system32\wevtutil.exe get-log TransactionLog"