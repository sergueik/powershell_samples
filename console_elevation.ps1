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
param (
  [string]$message = 'sensitive operation',
  [switch] $debug
  # NOTE: to unset need to pass as -debug:$false
)
function check_elevation {

param(
  [string]$message,
  [bool]$debug
)

  $myWindowsID = [System.Security.Principal.WindowsIdentity]::GetCurrent()
  $myWindowsPrincipal = new-object System.Security.Principal.WindowsPrincipal($myWindowsID)

  $adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator 
  if ($debug ){
    Write-Host -NoNewLine 'Press any key to continue...'
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
  }

  # Check to see if we are currently NOT running "as Administrator"
  if ( -not $myWindowsPrincipal.IsInRole($adminRole) ) {
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
   $password = ConvertTo-SecureString '12345' -AsPlainText -Force
   $credential = New-Object System.Management.Automation.PSCredential ('test1',    $password)
   Enter-PSSession -ComputerName User1 -Credential $credential
#>
# Check to see if we are currently running "as Administrator"
if ($myWindowsPrincipal.IsInRole($adminRole)) {
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
  $newProcess.Arguments = $myInvocation.MyCommand.Definition;
  # Indicate that the process should be elevated
  $newProcess.Verb = 'runas';
  # Start the new process
  [System.Diagnostics.Process]::Start($newProcess);
  # Exit from the current, unelevated, process
  exit
}
