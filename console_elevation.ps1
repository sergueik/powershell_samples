#Copyright (c) 2023 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# based on:
# http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx
# see also:
# https://stackoverflow.com/questions/7985755/how-to-detect-if-cmd-is-running-as-administrator-has-elevated-privileges
# https://stackoverflow.com/questions/1894967/how-to-request-administrator-access-inside-a-batch-file

param (
  [string]$message = 'sensitive operation',
  [switch] $elevated,
  [switch] $debug
  # NOTE: to unset need to pass as -debug:$false
)
function check_elevation {

param(
  [string]$message,
  [bool]$debug
)

  $windowsidentity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
  $windowsprincipal = new-object System.Security.Principal.WindowsPrincipal($windowsidentity)

  $adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator 
  if ($debug ){
    Write-Host -NoNewLine 'Press any key to continue...'
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
  }

  # Check to see if we are currently NOT running "as Administrator"
  # Alternative(?) is (https://www.cyberforum.ru/powershell/thread3136876.html#post17094408)
  #  'S-1-5-32-544' = 'BUILTIN\Administrators'
  # if (-not $windowsprincipal.Groups -contains 'S-1-5-32-544')) {
  if ( -not $windowsprincipal.IsInRole($adminRole) ) {
    write-host -foreground 'Red' ('The {0} needs to run in elevated prompt' -f $message) 
    exit
  }
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
# discussions:
# https://www.cyberforum.ru/powershell/thread3132437.html (with PS Session)
# https://qna.habr.com/q/1310770 (in Russian)
check_elevation -debug $debug_flag -message $message
# pass certs to session
# https://stackoverflow.com/questions/22764288/windows-powershell-give-password-in-command-enter-pssession

<#
   $username = 'user1'
   $comoutername = 'computer1'
   $plaintext_password = '...'
   [System.Security.SecureString]$securestring = convertto-securestring $plaintext_password -asplaintext -force
   [System.Management.Automation.PSCredential]$credential = new-object System.Management.Automation.PSCredential($username, $securestring)
   enter-pssession -computerName $comutername -credential $credential
#>
# Check to see if we are currently running "as Administrator"
if ($windowsprincipal.IsInRole($adminRole)) {
  # We are running "as Administrator" - so change the title and background color to indicate this
  $Host.UI.RawUI.WindowTitle = $myInvocation.MyCommand.Definition + '(Elevated)'
  $Host.UI.RawUI.BackgroundColor = 'DarkBlue'
  Clear-Host
} else {
  # the script was not lauched elevated
  # relaunch "as administrator"
  # Create a new process object that starts PowerShell
  $newProcess = New-Object System.Diagnostics.ProcessStartInfo 'PowerShell';
  # Specify the current script path and name as a parameter
  $newProcess.Arguments = $myInvocation.MyCommand.Definition
  # Indicate that the process should be elevated
  $newProcess.Verb = 'runas'
  # Start the new process
  [System.Diagnostics.Process]::Start($newProcess)
  # alternatively simply run a long commandline:
  # start-process powershell.exe -verb RunAs -argumentlist ('-noprofile -noexit -file "{0}" -elevated' -f ( $myinvocation.MyCommand.Definition ))
  # may also format the arguments array for compact notation:
  $arguments = @(
    '-NoLogo'
    '-NoExit'
    '-NoProfile'
    '-ExecutionPolicy bypass'
    '-File'
     $MyInvocation.MyCommand.Definition
  )
 
  # start-process -filepath PowerShell.exe -argumentlist $arguments -verb RunAs
  # Exit from the current, unelevated, process
  #
  exit
}
