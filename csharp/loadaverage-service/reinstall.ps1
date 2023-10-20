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

param(
  [string]$SERVICENAME = 'LoadAverageService',
  [string]$APPNAME = 'LoadAverageService.exe',
  # for Visual Studio artifact use 'WindowsApplication.exe'
  [string]$APPDIR = 'Program\bin\Debug',
  # for Visual Studio artifact use 'bin\Debug'
  [switch]$configure,
  [switch]$info,
  [switch]$uninstall,
  [switch]$force,
  [switch]$xml,
  [switch]$noop,
  [int]$delay = 7,
  [string]$datafile = 'c:\temp\loadaverage.txt',
  [string]$new_config_file = 'app.config',
  [switch]$debug
)
<#
TODO: add build steps

$buildfile = 'loadaverage-service.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
$msbuild = "${framework_path}\MSBuild.exe"
invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" /t:Clean,Build $buildfile"
NOTE: under Windows 7 x86 apparentlly need to also perform equivalent of nuget packages otherwise the console build is failing
with errors like:

  CircularBufferUnitTest.cs(4,7): error CS0246: The type or namespace name 'NUnit' could not be found (are you missing a using directive or an assembly reference?) [C:\developer\sergueik\powershell_ui_samples\csharp\loadaverage-service\Test\Test.csproj]

wihout 'Clean'  shows the warning which is probably the root cause:

  C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets(1605,5): warning MSB3268: The primary reference "nunit.framework" could not be resolved because it has an indirect dependency on the framework assembly "System.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" which could not be resolved in the currently targeted framework. ".NETFramework,Version=v4.5". To resolve this problem, either remove the reference "nunit.framework" or retarget your application to a framework version which contains "System.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a". 
[C:\developer\sergueik\powershell_ui_samples\csharp\loadaverage-service\Test\Test.csproj]

#>
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
$info_flag = [bool]$PSBoundParameters['info'].IsPresent -bor $info.ToBool()

$noop_flag = [bool]$PSBoundParameters['noop'].IsPresent -bor $noop.ToBool()
$force_flag = [bool]$PSBoundParameters['force'].IsPresent -bor $force.ToBool()
$configure_flag = [bool]$PSBoundParameters['configure'].IsPresent -bor $configure.ToBool()
$xml_flag = [bool]$PSBoundParameters['xml'].IsPresent -bor $xml.ToBool()
$uninstall_flag = [bool]$PSBoundParameters['uninstall'].IsPresent -bor $uninstall.ToBool()
check_elevation -debug $debug_flag -message 'administration of windows service applications'

if ( $info_flag) {
  write-verbose 'Reading proxy data from the registry'
  # note the syntax compared to HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\LoadAverageService
  $key = ('HKLM:\SYSTEM\CurrentControlSet\services\{0}' -f $SERVICENAME)


  $r = Get-ItemProperty $key
  write-output ('Service {0} is Log on as: {1}' -f $SERVICENAME, $r.ObjectName)


  # $obj = 'LocalSystem'
  # invoke-expression -command "sc.exe config $SERVICENAME OBJ= $obj"
  # NOTE: A space is required between an option and its value
  # The default setting is LocalSystem
  # see also:
  # https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/sc-config
  # sc.exe sdshow $SERVICENAME
  # see also:
  # https://www.winhelponline.com/blog/view-edit-service-permissions-windows/
  # D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)
  # $sddl = ( invoke-expression -command "sc.exe sdshow ${SERVICENAME}") -join ''
  # convertfrom-sddlstring -sddl $sddl| convertto-json | out-file a.json -encoding ascii
  <#
  {
    "Owner": {},
    "Group": {},
    "DiscretionaryAcl": [
      "NT AUTHORITY\\INTERACTIVE: AccessAllowed (CreateDirectories, GenericExecute, ListDirectory, Read, ReadAttributes, ReadExtendedAttributes, ReadPermissions, WriteAttributes)",
      "NT AUTHORITY\\SERVICE: AccessAllowed (CreateDirectories, GenericExecute, ListDirectory, Read, ReadAttributes, ReadExtendedAttributes, ReadPermissions, WriteAttributes)",
      "NT AUTHORITY\\SYSTEM: AccessAllowed (CreateDirectories, DeleteSubdirectoriesAndFiles, ExecuteKey, GenericExecute, GenericRead, GenericWrite, ListDirectory, Read, ReadAndExecute, ReadAttributes, ReadExtendedAttributes, ReadPermissions, Traverse, WriteAttributes, WriteExtendedAttributes)",
      "BUILTIN\\Administrators: AccessAllowed (ChangePermissions, CreateDirectories, Delete, DeleteSubdirectoriesAndFiles, ExecuteKey, FullControl, GenericAll, GenericExecute, GenericRead, GenericWrite, ListDirectory, Modify, Read, ReadAndExecute, ReadAttributes, ReadExtendedAttributes, ReadPermissions, TakeOwnership, Traverse, Write, WriteAttributes, WriteData, WriteExtendedAttributes, WriteKey)"
    ],
    "SystemAcl": [
      "Everyone: SystemAudit FailedAccess (ChangePermissions, CreateDirectories, Delete, DeleteSubdirectoriesAndFiles, ExecuteKey, FullControl, GenericAll, GenericExecute, GenericRead, GenericWrite, ListDirectory, Modify, Read, ReadAndExecute, ReadAttributes, ReadExtendedAttributes, ReadPermissions, TakeOwnership, Traverse, Write, WriteAttributes, WriteData, WriteExtendedAttributes, WriteKey)"
    ],
    "RawDescriptor": {
      "IsContainer": false,
      "IsDS": false,
      "ControlFlags": 32788,
      "Owner": null,
      "Group": null,
      "SystemAcl": [
        "System.Security.AccessControl.CommonAce"
      ],
      "DiscretionaryAcl": [
        "System.Security.AccessControl.CommonAce",
        "System.Security.AccessControl.CommonAce",
        "System.Security.AccessControl.CommonAce",
        "System.Security.AccessControl.CommonAce"
      ],
      "IsSystemAclCanonical": true,
      "IsDiscretionaryAclCanonical": true,
      "BinaryLength": 140
    }
  }

  #>

  # NOTE: get-service -name $SERVICENAME| select-object -property *
  # will not show the 'Log on as' information
  $startname = get-CIMinstance -class Win32_Service -filter "name ='${SERVICENAME}' " | select-object -expandproperty StartName
  write-output ('Service {0} is Log on as: {1}' -f $SERVICENAME, $startname)
  
  # see also:
  # https://www.commandline.ninja/use-powershell-to-find-windows-svcs-configured-to-run-as-another-user/#findinglogonasorrunasinformation
  # see the discussion https://github.com/PowerShell/PowerShell/issues/2579
  # for modifying the get-service (apparently never become part of code base) 
  exit
}

# e.g. . .\reinstall.ps1 -reconfigure -debug:$false -noop
$env:PATH="${env:PATH};c:\Windows\Microsoft.NET\Framework\v4.0.30319"
$config_file = ".\${APPDIR}\${APPNAME}.config"
$property = 'STATE'
$field = 'P5'
$value = (invoke-expression -command "sc.exe query ${SERVICENAME}" |
select-string -pattern $property |
select-object -first 1|
convertfrom-string)."${field}"
if ($debug_flag){
  write-host ('insected {0}: {1} => {2}' -f $SERVICENAME,$property,$value)
}
$state = $value
if (($state -eq 'running') -or $force_flag ) {
  invoke-expression -command "sc.exe delete ${SERVICENAME}"
  invoke-expression -command "InstallUtil.exe -uninstall ${APPDIR}\${APPNAME}"
}
if ($uninstall_flag) {
  return
}
start-sleep -second 3
if ($configure_flag) {
  if ( -not ( test-path $config_file )) {
    write-host -foreground 'Red' ('The app config file {0} not found, aborting' -f $config_file)
  }
  if ($xml_flag){
    copy-item $new_config_file -destination $config_file -force
  } else {
    # cannot reconfigure
    # NOTE: errors when not doing
    # 1006 The volume for a file has been externally altered so that the opened file is no longer valid.
    # 32   The process cannot access the file because it is being used by another process.
    # 1053 The service did not respond to the start or control request in a timely fashion.
    [xml]$config = [xml]((get-content $config_file) -join '')
    $config.configuration.appSettings.add | foreach-object { if(  $_.Key -eq 'Debug' ) { $_.Value = $debug.ToBool().ToString() }}
    # Cannot set "value" because only strings can be used as values to set XmlNode properties.
    $config.configuration.appSettings.add | foreach-object { if(  $_.Key -eq 'Noop' ) { $_.Value = $noop.ToBool().ToString() }}


    $config.configuration.appSettings.add | foreach-object { if(  $_.Key -eq 'Datafile' ) { $_.Value = $datafile }}

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
}

invoke-expression -command "InstallUtil.exe -install ${APPDIR}\${APPNAME}"
invoke-expression -command "sc.exe start ${SERVICENAME}"

start-sleep -second $delay
.\control.ps1 -datafile $datafile
.\tail_eventlog.ps1
