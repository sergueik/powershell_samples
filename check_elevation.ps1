#Copyright (c) 2022,2023 Serguei Kouzmine
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
  # Alternative(?) is (https://www.cyberforum.ru/powershell/thread3136876.html#post17094408)
  #  'S-1-5-32-544' = 'BUILTIN\Administrators'
  # if (-not $myWindowsPrincipal.Groups -contains 'S-1-5-32-544')) {

  if ( -not $myWindowsPrincipal.IsInRole($adminRole) ) {
    write-host -foreground 'Red' ('The {0} needs to run in elevated prompt' -f $message) 
    exit
  }
}

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()

check_elevation -debug $debug_flag -message $message
