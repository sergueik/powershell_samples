# see also: https://blog.netnerds.net/2016/12/runspaces-simplified/
# https://docs.microsoft.com/en-us/powershell/scripting/developer/hosting/creating-multiple-runspaces?view=powershell-7.1
$RunspacePool = [runspacefactory]::CreateRunspacePool(1,8)
$RunspacePool.ApartmentState = "MTA"
$RunspacePool.Open()
$scriptblock = {
invoke-expression -commamd 'C:\windows\system32\ping.exe -n 10 www.google.com'
}
$Work = [powershell]::Create().AddScript($scriptblock)

$Work.RunspacePool = $RunspacePool
$gpcAsyncResult = $Work.BeginInvoke()
[PSObject[]]$gpcOutput = $Work.EndInvoke($gpcAsyncResult)
write-output $gpcOutput
$RunspacePool.Close() | Out-Null
$RunspacePool.Dispose() | Out-Null
