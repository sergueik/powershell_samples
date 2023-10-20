# http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx
# 
# Get the ID and security principal of the current user account
$myWindowsID = [System.Security.Principal.WindowsIdentity]::GetCurrent()
$myWindowsPrincipal = New-Object System.Security.Principal.WindowsPrincipal ($myWindowsID)

# Get the security principal for the Administrator role
$adminRole = [System.Security.Principal.WindowsBuiltInRole]::Administrator

# Check to see if we are currently running "as Administrator"
if ($myWindowsPrincipal.IsInRole($adminRole))
{
  # We are running "as Administrator" - so change the title and background color to indicate this
  $Host.UI.RawUI.WindowTitle = $myInvocation.MyCommand.Definition + "(Elevated)"
  $Host.UI.RawUI.BackgroundColor = "DarkBlue"
  Clear-Host
}
else
{
  # We are not running "as Administrator" - so relaunch as administrator
  # Create a new process object that starts PowerShell
  $newProcess = New-Object System.Diagnostics.ProcessStartInfo "PowerShell";
  # Specify the current script path and name as a parameter
  $newProcess.Arguments = $myInvocation.MyCommand.Definition;
  # Indicate that the process should be elevated
  $newProcess.Verb = "runas";
  # Start the new process
  [System.Diagnostics.Process]::Start($newProcess);
  # Exit from the current, unelevated, process
  exit
}

# http://stackoverflow.com/questions/8343767/how-to-get-the-current-directory-of-the-cmdlet-being-executed
function Get-ScriptDirectory {
  $Invocation = (Get-Variable MyInvocation -Scope 1).Value
  if ($Invocation.PSScriptRoot) {
    $Invocation.PSScriptRoot
  }
  elseif ($Invocation.MyCommand.Path) {
    Split-Path $Invocation.MyCommand.Path
  } else {
    $Invocation.InvocationName.Substring(0,$Invocation.InvocationName.LastIndexOf(""))
  }
}



# Run your code that needs to be elevated here

pushd (Get-ScriptDirectory)
[Environment]::SetEnvironmentVariable('POSHSERVER_HOME',(Get-ScriptDirectory) , 'Machine')
Import-Module ./PoSHServer.psd1
write-host $env:POSHSERVER_HOME
start-PoshServer -pause -DebugMode

start-sleep 120
