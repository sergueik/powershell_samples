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
# 

# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/export-scheduledtask?view=windowsserver2022-ps
# https://petri.com/import-scheduled-tasks-powershell/
# https://docs.microsoft.com/en-us/answers/questions/184857/import-scheduled-task-with-powershell-and-s4u.html
# https://www.get-itsolutions.com/how-to-export-and-import-scheduled-tasks/

# https://docs.microsoft.com/en-us/windows/win32/secauthz/well-known-sids
# http://woshub.com/convert-sid-to-username-and-vice-versa/

# Object picker UI https://docs.microsoft.com/en-us/previous-versions/orphan-topics/ws.11/dn789205(v=ws.11)?redirectedfrom=MSDN
add-type -assembly 'System.DirectoryServices.AccountManagement'
# https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.accountmanagement.principal.getgroups?view=netframework-4.5#system-directoryservices-accountmanagement-principal-getgroups
# https://stackoverflow.com/questions/8009593/list-group-memberships-for-ad-users
$groups = [System.Security.Principal.WindowsIdentity]::GetCurrent().Groups
$groups | foreach-object {  
  $group = $_
  # NOTE: group names are localized
  write-output $group.value
  write-output ($group.Translate([System.Security.Principal.NTAccount]).value)
}

<#
  S-1-5-21-440999728-2294759910-2183037890-513
  sergueik42\None
  S-1-1-0
  Everyone
  S-1-5-21-440999728-2294759910-2183037890-1005
  sergueik42\HelpLibraryUpdaters
  S-1-5-21-440999728-2294759910-2183037890-1010
  sergueik42\HomeUsers
  S-1-5-32-545
  BUILTIN\Users
  S-1-5-4
  NT AUTHORITY\INTERACTIVE
  S-1-2-1
  CONSOLE LOGON
  S-1-5-11
  NT AUTHORITY\Authenticated Users
  S-1-5-15
  NT AUTHORITY\This Organization
  S-1-5-113
  NT AUTHORITY\Local account
  S-1-2-0
  LOCAL
  S-1-5-64-10
  NT AUTHORITY\NTLM Authentication
#>

$sid = 'S-1-5-32-544'
$princpipal = new-scheduledtaskprincipal -userid $sid -logontype S4U -runlevel highest
$taskname = 'example_task_app'
$taskpath = '\automation\'
$task = get-scheduledtask -taskname $taskname -taskpath $taskpath | select-object -first 1

try {
  $task.principal =  $princpipal
  $task | Set-ScheduledTask
} catch [Exception] {
  # HRESULT 0x80041318,The task XML contains a value which is incorrectly formatted or out of range
  # HRESULT 0x8004131a,The task XML is malformed
  # other exceptions
}
export-scheduledtask -taskname $task.TaskName -taskpath  $task.taskpath | out-file .\Desktop\example_task_app_users.xml -encoding ascii -force

copy-item -path .\Desktop\example_task_app_users.xml -destination .\Desktop\example_task_app_administrators.xml


# modify the .\Desktop\example_task_app_administrators.xml changing to
<#
<Principals>
    <Principal id="Author">
      <GroupId>S-1-5-32-544</GroupId>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
#>
# sample file

<#
<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.3" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <RegistrationInfo>
    <Author>DESKTOP-4I0IKQ9\sergueik</Author>
    <Description>...</Description>
    <URI>\test</URI>
  </RegistrationInfo>
  <Principals>
    <Principal id="Author">
      <GroupId>S-1-5-32-544</GroupId>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <DisallowStartIfOnBatteries>true</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <IdleSettings>
      <StopOnIdleEnd>true</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
    <UseUnifiedSchedulingEngine>true</UseUnifiedSchedulingEngine>
  </Settings>
  <Triggers>
    <TimeTrigger>
      <StartBoundary>2022-01-27T09:03:00+03:00</StartBoundary>
    </TimeTrigger>
    <EventTrigger>
      <Subscription>&lt;QueryList&gt;&lt;Query Id="0" Path="Microsoft-Windows-NetworkProfile/Operational"&gt;&lt;Select Path="Microsoft-Windows-NetworkProfile/Operational"&gt;*[System[Provider[@Name="Microsoft-Windows-NetworkProfile"] and EventID=10000]]&lt;/Select&gt;&lt;/Query&gt;&lt;/QueryList&gt;</Subscription>
    </EventTrigger>
  </Triggers>
  <Actions Context="Author">
    <Exec>
      <Command>c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe</Command>
      <Arguments>-executionpolicy bypass -noprofile -file "C:\developer\sergueik\powershell_ui_samples\work_script.ps1" -send -datafile "C:\developer\sergueik\powershell_ui_samples\data.txt"</Arguments>
    </Exec>
  </Actions>
</Task>

#>
$xmlpath = '.\Desktop\example_task_app_system.xml'
# NOTE: need a full path
$new_xmlpath = 'c:\users\sergueik\Desktop\result.xml'
[xml]$task_xml = [xml]((get-content -path $xmlpath) -join '')
$userid_element = $task_xml.Task.Principals.Principal.GetElementsByTagName('UserId').Item(0)
$task_xml.Task.Principals.Principal.RemoveChild($userid_element)
# $task_xml.Task.Principals.Principal.UserId disapppears 
$groupid_element = $task_xml.CreateElement('GroupId', 'http://schemas.microsoft.com/windows/2004/02/mit/task')
$task_xml.Task.Principals.Principal.AppendChild($groupid_element)
$task_xml.Task.Principals.Principal.GroupId ='S-1-5-32-544'
$settings = new-object System.Xml.XmlWriterSettings
$settings.indent = $true
$settings.indentchars = "`t"

$xml_writer = [System.Xml.XmlWriter]::create($new_xmlpath , $settings)

$task_xml.WriteTo($xml_writer)
$xml_writer.Flush()
$xml_writer.Close()
$task_xml.Save($new_xmlpath)
    
$newxmlpath = '.\Desktop\example_task_app_administrators.xml'
$new_taskname = ( '{0}_NEW' -f $task.taskname)
# NOTE: No backslash - there is already a trailing backslash in the taskpath
# to prevent error:
$tn = ( '{0}{1}' -f $task.taskpath, $new_taskname)

schtasks.exe /create /xml $xmlpath /tn $tn
unregister-scheduledtask -taskname $new_taskname -taskpath $task.taskpath -confirm:$false

# alternatively
register-scheduledtask -xml (Get-Content $new_xmlpath| out-string) -taskname $new_taskname -taskpath $task.taskpath

unregister-scheduledtask -taskname $new_taskname -taskpath $task.taskpath -confirm:$false

