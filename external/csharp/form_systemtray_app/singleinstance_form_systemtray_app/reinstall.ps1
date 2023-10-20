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

param(
  [string]$APPDIR = 'Program\bin\Debug',
  [string]$APPNAME = 'SystemTrayApp.exe',
  # NOTE: Visual Studio will place artifact to 'bin\Debug'
  [switch]$configure,
  [switch]$build,
  [switch]$uninstall,
  [switch]$debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
$build_flag = [bool]$PSBoundParameters['build'].IsPresent -bor $build.ToBool()

$configure_flag = [bool]$PSBoundParameters['configure'].IsPresent -bor $configure.ToBool()
$uninstall_flag = [bool]$PSBoundParameters['uninstall'].IsPresent -bor $uninstall.ToBool()
if ($build_flag){
  $uninstall_flag = $true
}

# e.g. . .\reinstall.ps1 -reconfigure -debug:$false -noop
$env:PATH="${env:PATH};c:\Windows\Microsoft.NET\Framework\v4.0.30319"
$config_file = ".\${APPDIR}\${APPNAME}.config"
$ini_file = ".\${APPDIR}\config.in"
if ($uninstall_flag) {
  $name = get-item -path ('{0}\{1}' -f $APPDIR, $APPNAME ) | select-object -expandproperty BaseName
  if ($name -ne $null){
    # find process by name and terminate without using Win32 API or NAMED PIPE
    if ((get-process | where-object { $_.Name -eq $name }) -ne $null) {
      # NOTE: CloseWindow() will not work with system tray apps
      get-process | where-object { $_.Name -eq $name }| foreach-object { $_.Kill()}
      write-host 'Done'
    }
  }
  if ( -not $build_flag ){
    exit
  }
}
$buildfile = 'form_systemtray_app.sln'
$framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
if ( $build_flag ){
  $env:path="${env:path};${framework_path}"
  msbuild.exe -p:FrameworkPathOverride="${framework_path}" $buldfile
  # alternatively
  $framework_path = 'c:\Windows\Microsoft.NET\Framework\v4.0.30319'
  $msbuild = "${framework_path}\MSBuild.exe"
  invoke-expression -command "$msbuild -p:FrameworkPathOverride=""${framework_path}"" /t:Clean,Build ${buildfile}"
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

