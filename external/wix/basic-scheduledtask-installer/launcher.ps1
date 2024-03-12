param(
  [parameter(position=0)]$logname,
  [parameter(position=1)]$message,
  [parameter(position=2)]$eventid = 2
)
# NOTE: appears to be necessary to pass all requested positonal parametrs
$message = $message.ToUpper()

# NOTE: need to provdide the full path to dependency powershell script:
# when run from MSI through WixQuietExec, the code run directory is:
# c:\Windows\System32

[System.IO.FileInfo] $fileinfo = new-object System.IO.FileInfo($myinvocation.invocationname)
[string] $script_path = $fileinfo.Directory.FullName
# alternatively 
# $script_path = ($MyInvocation.InvocationName) -replace '\\[^\\]*$', ''
write-host ('POWERSHELL SCRIPT RUN: script_path: {0}' -f $script_path)

. ($script_path + '\' + 'dependency.ps1') -message $message -logname $logname -eventid $eventid
