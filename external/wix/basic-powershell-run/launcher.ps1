param(
  [parameter(position=0)]$logname,
  [parameter(position=1)]$message
)
$message = $message.ToUpper()

# NOTE: need to provdide the full path  when run from MSI through WixQuietExec

[System.IO.FileInfo] $fileinfo = new-object System.IO.FileInfo($MyInvocation.InvocationName)
[string] $script_path = $fileinfo.Directory.FullName
# alternatively 
# $script_path = ($MyInvocation.InvocationName) -replace '\\[^\\]*$', ''
write-host ('script_path: {0}' -f $script_path)

. ($script_path + '\' + 'dependency.ps1') -message $message -logname $logname
