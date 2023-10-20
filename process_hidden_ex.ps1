# http://www.java2s.com/Tutorial/CSharp/0280__Development/CreateProcessStartInfo.htm
# https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.windowstyle?view=netframework-4.5
# see also forum question: http://forum.oszone.net/thread-353733.html
[System.Diagnostics.ProcessStartInfo] $si = new-object System.Diagnostics.ProcessStartInfo
$process_workdir = 'c:\temp'
$logfile = 'a.log'
$si.FileName = 'cmd.exe'
$si.Arguments = ('/c C:\Windows\System32\PING.EXE -n 3 www.google.com >> {0}' -f $logfile )
$si.UseShellExecute = $true
# NOTE: The Process object must have the UseShellExecute property set to false in order to redirect IO streams.
$si.RedirectStandardOutput = $false
$si.WorkingDirectory = $process_workdir
$si.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
$si.ErrorDialog = $true
$x = [System.Diagnostics.Process]::Start($si)
# TODO: improve wait until exited
while (-not $x.HasExited) {
  start-sleep -seconds 1
}

get-content -path "${process_workdir}\${logfile}"
# Alternative to 
invoke-expression ('cmd %%- /c C:\Windows\System32\PING.EXE -n 3 www.google.com  >> {0}\{1}' -f $process_workdir,$logfile )
# and another alternative is
start-process -WindowStyle hidden -filepath "C:\Windows\System32\PING.EXE" -argumentlist @('-n', '3' ,'www.google.com') -passthru |out-file ('{0}\{1}' -f $process_workdir,$logfile )
# but it will *not* collect the process output but rather the useless Powershell output:
# Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id  SI ProcessName
# -------  ------    -----      ----- -----   ------     --  -- -----------
#      4       4      260       1180 ...59     0.00   7128   1 PING
<#
# with redirection
# # reading the output only possible until
# $process.HasExited
while (-not $x.StandardOutput.EndOfStream) {
    [string]$line = $x.StandardOutput.ReadLine()
    write-host $line
}

#>
<#
Start-Process 'C:\Program Files (x86)\Nmap\nmap.exe' '-v', '-iR', 10000, '-Pn', '-p', 80 -RedirectStandardOutput 'C:\Users\Administrator\Documents\perm.txt' -WindowStyle 'hidden'
#>

# see also [dealing with hidden scheduled tasks powershell jobs etc](https://www.outsidethebox.ms/21628/)(in Russian)
