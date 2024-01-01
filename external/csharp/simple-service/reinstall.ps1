# NOTE: not tested much
remove-eventlog -logname TransactionLog
new-eventlog -logname TransactionLog -source TransactionLog
$env:PATH="$env:PATH%;c:\Windows\Microsoft.NET\Framework\v4.0.30319"
cd c:\developer\sergueik\powershell_ui_samples\external\csharp\simple-service
nstallUtil.exe  -install .\Program\bin\Debug\WindowsService.NET.exe
InstallUtil.exe  -uninstall .\Program\bin\Debug\WindowsService.NET.exe


InstallUtil.exe  -install .\Program\bin\Debug\WindowsService.NET.exe
sc.exe query WindowsService.NET

# NOTE: alternatively
# $SERVICENAME = 'WindowsService.NET.exe'
# & sc.exe create ${SERVICENAME} binpath= "Program\bin\Debug\${SERVICENAME}" start= auto Displayname= "${SERVICENAME}"
# $Description = 'C# demo Windows Service.' 
# & sc.exe description ${SERVICENAME} "${Description}"
# see also: https://github.com/MScholtes/Windows-Service/blob/master/NamedPipesService/Install.bat#L13

sc.exe start  WindowsService.NET


sc.exe query WindowsService.NET






