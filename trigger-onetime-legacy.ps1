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
  [String]$template_filename ='example_task_system.xml',
  [string]$datafile = 'data.txt',
  # [string]$datafile = (resolve-path 'data.txt'),
  [string]$script = 'work_script.ps1',
  # [string]$script = (resolve-path 'work_script.ps1'),
  [string]$task_path = '\Automation',
  [int]$boot_delay = 2,
  [int]$delay = 2,
  [string]$task_name = 'example_task_system',
  [switch]$debug
)

$curdir = (resolve-path '.').Path


$template =  "${curdir}\${template_filename}"
$data = get-content -path (resolve-path $template )
$xml = [xml]($data)

$cnt = 0
$xml.Task.Triggers.BootTrigger |
foreach-object {
  $element = $_
  write-output ( 'item # ' + $cnt ) ;
  write-output $_.'#text'
  write-output $element.'value'
  $cnt ++
}
$start_time_format = 'yyyy-MM-ddTHH:mm:ss'
$start_time = get-date((get-date).AddMinutes($delay)) -format $start_time_format

write-output ('creating task to run on {0}' -f $start_time)

# check
$startup_trigger = new-scheduledtasktrigger -AtStartup
$property_check = [bool](($startup_trigger | get-member -membertype property).name -contains 'Delay')
if ($property_check -eq $true){
  write-host 'Can use Powershell "new-scheduledtasktrigger"'
}
# for compatibility with Windows 7, override the schema version
$xml.Task.version = '1.3'
$xml.Task.Triggers.BootTrigger.Delay = ('PT{0}M' -f $boot_delay)
$command = 'c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe'
write-output $xml.Task.Actions.Exec.Command
write-output $command
$arguments = ('-executionpolicy bypass -noprofile -file "{0}" -send -datafile "{1}"' -f $script, $datafile)
write-output $xml.Task.Actions.Exec.Arguments
write-output $arguments
try { 
  # NOTE: Cannot set "StartBoundary" because only strings can be used as values to setXmlNode properties.
  $xml.Task.Triggers.TimeTrigger.StartBoundary = $start_time.ToString()
} catch [Exception] {
  # ERROR: The task XML contains a value which is incorrectly formatted or out of range.
}
# write-output $xml.Task.Triggers.TimeTrigger.StartBoundary
if ($debug_flag) {
  [System.xml.XmlTextWriter]$console_out = new-object System.xml.XmlTextWriter([Console]::out)
  $xml.WriteTo($console_out) 
}
$file = "${curdir}\a${cnt}.xml"
write-host ('write {0}' -f $file)
[System.xml.XmlTextWriter]$writer = new-object System.xml.XmlTextWriter($file, [System.Text.Encoding]::Ascii)
$xml.WriteTo($writer) 
$writer.flush()
$writer.close()
# NOTE: will not work with Windows 7. The cmdlet did not exist
if([environment]::OSVersion.Version.Major -ne 6){
  unregister-scheduledtask -taskname $task_name -confirm:$false
  # get-scheduledtask : No MSFT_ScheduledTask objects found with property 'TaskName' equal to 'example_task_system'.  Verify the value of the property and retry.
  if (-not (get-scheduledtask -taskname $task_name -erroraction silentlycontinue )) {
    invoke-expression -command "schtasks.exe /create /ru ""NT AUTHORITY\SYSTEM"" /tn ""${task_path}\${task_name}"" /XML ""${file}"""
  
    invoke-expression -command "schtasks.exe /query /tn ""${task_path}\${task_name}"" /FO list"
  }
} else {

  invoke-expression -command "schtasks.exe /query /tn ""${task_path}\${task_name}"" /FO list"
  if ( -not $? ){ 
    invoke-expression -command "schtasks.exe /create /ru ""NT AUTHORITY\SYSTEM"" /tn ""${task_path}\${task_name}"" /XML ""${file}"""
  
    invoke-expression -command "schtasks.exe /query /tn ""${task_path}\${task_name}"" /FO list"
  }
}
