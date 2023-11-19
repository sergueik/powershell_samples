#Copyright (c) 2022 Serguei Kouzmine
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

param(
  [string]$SERVICENAME = 'PipeServer' ,
  [string]$APPNAME = 'PipeServerService.exe',
  [switch]$configure,
  [switch]$uninstall,
  [switch]$noop,
  [int]$delay = 7,
  [switch]$debug
)

# based on:
# http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx
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
$noop_flag = [bool]$PSBoundParameters['noop'].IsPresent -bor $noop.ToBool()
$configure_flag = [bool]$PSBoundParameters['configure'].IsPresent -bor $configure.ToBool()
$uninstall_flag = [bool]$PSBoundParameters['uninstall'].IsPresent -bor $uninstall.ToBool()

check_elevation -debug $debug_flag -message 'administration of windows service applications'

# e.g. . .\reinstall.ps1 -reconfigure -debug:$false -noop
$env:PATH="${env:PATH};c:\Windows\Microsoft.NET\Framework\v4.0.30319"
$config_file = ".\Service\bin\Debug\${APPNAME}.config"

invoke-expression -command "sc.exe delete ${SERVICENAME}"
invoke-expression -command "InstallUtil.exe -uninstall Service\bin\Debug\${APPNAME}"
if ($uninstall_flag) {
  return
}
start-sleep -second 3
if ($configure_flag) {
  # cannot reconfigure
  # NOTE: errors when not doing
  # 1006 The volume for a file has been externally altered so that the opened file is no longer valid.
  # 32   The process cannot access the file because it is being used by another process.
  # 1053 The service did not respond to the start or control request in a timely fashion.
  [xml]$config = [xml]((get-content $config_file) -join '')
  $config.configuration.appSettings.add | foreach-object { if(  $_.Key -eq 'Debug' ) { $_.Value = $debug.ToBool().ToString() }}
  # Cannot set "value" because only strings can be used as values to set XmlNode properties.
  $config.configuration.appSettings.add | foreach-object { if(  $_.Key -eq 'Noop' ) { $_.Value = $noop.ToBool().ToString() }}
  $settings = new-object System.Xml.XmlWriterSettings
  $settings.indent = $true
  $settings.indentchars = "`t"
  $scriptDirectory = (resolve-path '.').Path
  $xml_writer = [System.Xml.XmlWriter]::create([System.IO.Path]::Combine($scriptDirectory, $config_file) , $settings)

  $config.WriteTo($xml_writer)
  $xml_writer.Flush()
  $xml_writer.Close()
  $config.Save($scriptDirectory + '\' + $config_file)
}

invoke-expression -command "InstallUtil.exe -install Service\bin\Debug\${APPNAME}"
invoke-expression -command "sc.exe start ${SERVICENAME}"

start-sleep -second $delay

invoke-expression -command "sc.exe query ${SERVICENAME}"

$name = 'demo'

[System.IO.Directory]::GetFiles('\\.\pipe\') | where-object {$_ -match $name} | select-object -first 1
# see also: https://github.com/sergueik/serverspec_custom_types/blob/master/windows/named_pipe_spec.rb
