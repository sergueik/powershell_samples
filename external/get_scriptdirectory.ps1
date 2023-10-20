# http://poshcode.org/2887
# http://stackoverflow.com/questions/8343767/how-to-get-the-current-directory-of-the-cmdlet-being-executed
# https://msdn.microsoft.com/en-us/library/system.management.automation.invocationinfo.pscommandpath%28v=vs.85%29.aspx
function Get-ScriptDirectory
{
  [string]$MyDir = $null

  if ($host.Version.Major -gt 2) {
    $MyDir = (Get-Variable PSScriptRoot).Value
    Write-Debug ('$PSScriptRoot: {0}' -f $MyDir)
    if ($Mydir -ne $null) {
      return $MyDir;
    }
    $MyDir = [System.IO.Path]::GetDirectoryName($MyInvocation.PSCommandPath)
    Write-Debug ('$MyInvocation.PSCommandPath: {0}' -f $MyDir)
    if ($Mydir -ne $null) {
      return $MyDir;
    }

    $MyDir = Split-Path -Parent $PSCommandPath
    Write-Debug ('$PSCommandPath: {0}' -f $MyDir)
    if ($Mydir -ne $null) {
      return $MyDir;
    }
  } else {
    $MyDir = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)
    if ($Mydir -ne $null) {
      return $MyDir;
    }
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value
    if ($Invocation.PSScriptRoot) {
      $MyDir = $Invocation.PSScriptRoot
    } elseif ($Invocation.MyCommand.Path) {
      $MyDir = Split-Path $Invocation.MyCommand.Path
    } else {
      $MyDir = $Invocation.InvocationName.Substring(0,$Invocation.InvocationName.LastIndexOf('\'))
    }
    return $MyDir
  }
}


Get-ScriptDirectory
