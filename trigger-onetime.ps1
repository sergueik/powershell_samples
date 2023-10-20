#Copyright (c) 2020,2022,2023 Serguei Kouzmine
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
# Note this script is behind on other configuration defails of the task:
# * MultipleInstancesPolicy (default is IgnoreNew, we like StopExisting)
# * ExecutionTimeLimit TBD
# * RestartOnFailure
# * Interval PT1M
# * Count 3
# see also
# https://learn.microsoft.com/en-us/windows/win32/taskschd/task-scheduler-schema-elements?source=recommendations
# see also:
# Allow Non Admins to Manage Scheduled Tasks
# by including the account into "power users" and "backup operators" groups
# https://social.technet.microsoft.com/Forums/ie/en-US/18af37ed-d18c-477d-9d08-65face75855e/allow-non-admins-to-manage-scheduled-tasks?forum=winserverManagement
# see also:
# stealth Scheduled Tasks (in Russian)
# https://habr.com/ru/company/rvision/blog/723050
param(
  [string]$datafile = 'data.txt',
  # [string]$datafile = (resolve-path 'data.txt'),
  [string]$script = 'work_script.ps1',
  # [string]$script = (resolve-path 'work_script.ps1'),
  [switch]$powershell = $true,
  # TODO: append based on login choice
  [string]$taskname = 'example_task_app',
  [string]$taskpath = 'automation',
  [int]$delay = 2,
  [switch]$remove,
  [switch]$asuser,
  [switch]$with_startup_trigger,
  [string]$user = $null
)
# origin: http://www.cyberforum.ru/powershell/thread1719005.html
# see also: https://pscustomobject.github.io/powershell/functions/PowerShell-SecureString-To-String/
# with cygwin there is a custom implementation of an NT/W2K service starter, similar to Microsoft's INSTSRV and SRVANY programs named cygrunsrv:
# https://github.com/jeromeetienne/neoip/blob/master/mingw_cfg/cygrunsrv-1.17-1/cygrunsrv.README
# the cygrunsrv is written in c++ 14 years ago
# operates at very low level e.g.
# regSetValueEx (srv_key, PARAM_PATH, 0, REG_SZ, (const BYTE *) path, strlen (path) + 1)
# https://github.com/jeromeetienne/neoip/blob/master/mingw_cfg/cygrunsrv-1.17-1/cygrunsrv.cc#L209
function private:ConvertTo-RegularString([Security.SecureString]$s) {
  return [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($s)
  )
}

$script = ( resolve-path '.').path + '\' + $script
$datafile = ( resolve-path '.').path + '\' + $datafile
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/new-scheduledtaskaction?view=windowsserver2019-ps
# TODO: convert from pipeline to arg collecting, passing
# get-scheduledtast -taskname $taskname
get-scheduledtask | where-object { $_.taskname -eq $taskname} | unregister-scheduledtask -confirm:$false
if ([bool]$psboundparameters['remove'].ispresent) {
  write-host 'Done.'
  exit
}

# $action = new-scheduledtaskaction -execute 'c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe' -argument ('-executionpolicy bypass -noprofile -file "{0}" -send -noop $false -datafile "{1}"' -f $script, $datafile)
# NOTE: there is a problem with passing boolean arguments, on some Windows releases:
# it manifests itself through error reported by Task Scheduler into EventLog
# return code 2147942401 (which is 0x80070001
# and then running the command reveals
# Cannot process argument transformation on parameter 'noop'.
# Cannot convert value "System.String" to type "System.Boolean"
# Boolean parameters accept only Boolean values $True, $False, 1 or 0
if (-not (test-path -path $script ) ) {
  write-host -foreground Red ( 'Script {0} is not found' -f $script )
  exit
  # if the path to the script is incorrect of invalid the error will be
  # 4294770688 (0xFFFD0000) which is not a COM error:
  # the given file format is not supported. Specify a valid path for the -file parameter.
}
if ($powershell) {
  $action = new-scheduledtaskaction -execute 'c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe' -argument ('-executionpolicy bypass -noprofile -file "{0}" -send -datafile "{1}"' -f $script, $datafile)
} else {
  $action = new-scheduledtaskaction -execute $script -argument ('-datafile "{0}"' -f $datafile)
}

$time = get-date((get-date).AddMinutes($delay))	 -format 'HH:mm'
write-output ('creating task to run on {0}' -f $time)
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/new-scheduledtasktrigger?view=windowsserver2019-ps
$trigger = new-scheduledtasktrigger -once -at $time
$startup_trigger = new-scheduledtasktrigger -AtStartup
# NOTE: Windows 8.1/Windows Server 2012 R2 and older
# The property 'Delay' cannot be found on this object. Verify that the property exists and can be set.
# NOTE: some the approaches
# https://java2blog.com/check-if-object-has-property-powershell
# does not appear to work
# see also:
# https://stackoverflow.com/questions/26997511/how-can-you-test-if-an-object-has-a-specific-property
# NOTE packing the one-liner if statement was found to accidentally not work correctly
# if (($startup_trigger | get-member -membertype property).name -contains 'Delay') { 
#   write-output '*** passed'
# }
$property_check = [bool](($startup_trigger | get-member -membertype property).name -contains 'Delay')
if ($property_check -eq $true){
  $startup_trigger.Delay = (new-timespan -Minutes 1)

  # NOTE: when configured interactively ends up  having it like:
  # get-scheduledtask -taskname 'example_task_app' | select-object -first 1 | select-object -expandproperty Triggers | where-object { $_.ToString() -match 'MSFT_TaskBootTrigger'} | select-object -expandproperty Delay
  # PT1M 
  # reported empty on Windows 8.1 - panel clearly shows 1 minute
  # $taskname =  'example_task_current_user' 
  # get-scheduledtask -taskname $taskname | select-object -first 1 | 
  # select-object -expandproperty Triggers| 
  # where-object { $_.ToString() -match 'MSFT_TaskBootTrigger'} | 
  # select-object -property Delay | format-list
  # Delay :
  # on Windows 10
  # get-scheduledtask -taskname 'example_task_current_user' | select-object -first 1 | select-object -expandproperty Triggers| where-object { $startup_trigger.Delay = 'PT1M' }

  # register-scheduledtask : XML-код задачи содержит значение в неправильном формате или за пределами допустимого диапазона. (13,25):Delay:00:01:00
  
  # $taskname = 'example_task_current_user'
  # $taskpath = '\automation' 
  # schtasks.exe /query /tn ('{0}\{1}' -f $taskpath, $taskname) /xml |out-file "${taskname}-1.xml"
  # no Delay in: 
  #    <BootTrigger>
  #      <Repetition>
  #        <Interval>PT1M</Interval>
  #        <StopAtDurationEnd>false</StopAtDurationEnd>
  #      </Repetition>
  #      <Enabled>true</Enabled>
  #    </BootTrigger>


  # Manually configure startup delay
  # now will have
  #   <BootTrigger>
  #      <Enabled>true</Enabled>
  #      <Delay>PT1M</Delay>
  #    </BootTrigger>
  #
  # aparently can /query saving XML and /delete and finally /create
  # schtasks.exe  /ru %computername%\%username% /rp "%  PASSWORD%"  /create /tn example_task_current_user2 /XML example_task_current_user-2.xml
  # SUCCESS: The scheduled task "example_task_current_user2" has successfully been created.
}
<#

get-scheduledtask -taskname $taskname |
select-object -first 1 |
select-object -expandproperty Triggers |
foreach-object {
  write-output $_.ToString()
}
# see trigger types 
# https://docs.microsoft.com/en-us/windows/win32/taskschd/trigger-types
# MSFT_TaskTimeTrigger
# MSFT_TaskBootTrigger
# the other types are not used:
# MSFT_TaskLogonTrigger
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/new-scheduledtasktrigger?view=windowsserver2019-ps
# MSFT_TaskLogonTrigger, ROOT\Microsoft\Windows\TaskScheduler
# VB script example
# https://docs.microsoft.com/en-us/windows/win32/taskschd/time-trigger-example--scripting-
# See also
# https://docs.microsoft.com/en-us/windows/win32/api/mstask/ne-mstask-task_trigger_type
# https://docs.microsoft.com/en-us/windows/win32/api/mstask/ns-mstask-task_trigger
#>
if ([bool]$psboundparameters['asuser'].ispresent) {
    # NOTE: one seems to need to provide the 'USERDOMAIN'
    # in credentials  dialog at all times
    # DESKTOP-P976KI9\app random
    #  . .\trigger-onetime.ps1 -user "${env:USERDOMAIN}\app" -asuser
    if ($user -eq $null -or $user -eq '') {
  	  $user = "${env:USERDOMAIN}\${env:USERNAME}"
	}
	$credential = get-credential -username $user -message 'credentials for the task'
	$password = convertTo-RegularString $credential.password
        # $password = $credential.GetNetworkCredential().Password
	register-scheduledtask -action $action -trigger $trigger,$startup_trigger -taskname $taskname -taskpath $taskpath -password $password -user $user -description '...'
 <#
Creating Task With a Batch File - run task as soon as possible if missed -
There is no command line option for this -  available in scheduled task module
https://superuser.com/questions/651235/creating-task-with-a-batch-file-run-task-as-soon-as-possible-if-missed
#>

# set-scheduledtask -taskName $taskname -taskPath $taskpath -StartWhenAvailable -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
# NOTE: apparently -StartWhenAvailable is available in Windows 10 only. Noted in several occasions that these cmdlets are somewht flaky with regards to argument processing

# NOTE: the type: Microsoft.Management.Infrastructure.CimInstance#Root/Microsoft/Windows/TaskScheduler/MSFT_TaskSettings3
$settings_set = New-ScheduledTaskSettingsSet
# Run task as soon as possible after a scheuled start is missed
$settings_set.StartWhenAvailable = $true
# TODO: if the task fails restart every 1 minute
# TODO: stop the task if it runs longer than: 3 days
# TODO: if the runningtask does not end when requested, force it to stop
# TODO: If the task is not scheuled to run again delete it after: 30 days
# TODO: Conditions/Idle
# Start the test only if the computer is idle for: 10 min
# Stop if the comuter ceases to be idle
# Restart if the idle state resumes

# Conditions/Power: "Stop if the computer switches to battery power"
$settings_set.StopIfGoingOnBatteries = $false
# Start task only if the computer is on AC power
$settings_set.DisallowStartIfOnBatteries = $false
# Wake the computer to run this task
$settings_set.WakeToRun = $true
$settings_set.runonlyifidle = $false
<#
# new argument synax
$task_argument_literal = @{
  TaskName = $taskname
  TaskPath = $taskpath
  Trigger  = new-scheduledtasktrigger -AtLogon
  User     = "$env:USERDOMAIN\$env:USERNAME"
  Action   = new-scheduledtaskaction -Execute "PowerShell.exe"
  Settings = new-scheduledTasksettingsset -AllowStartIfOnBatteries
}
# NOTE Perl-style sigil https://www.perl.com/article/on-sigils/ 
register-ScheduledTask @task_argument_literal
Get-ScheduledTask -TaskName -taskName $taskname -taskPath $taskpath | select -ExpandProperty Settings | select DisallowStartIfOnBatteries

#>
$settings_set.AllowHardTerminate = $true
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/new-scheduledtasksettingsset?view=windowsserver2019-ps
# constructor cmdlet accepts some but not allpossible values of MultipleInstancesEnum
# -MultipleInstances Parallel| Queue| IgnoreNew
# https://stackoverflow.com/questions/59113643/stop-existing-instance-option-when-creating-windows-scheduled-task-using-powersh
# The StopInstance enum value currently is not supported by ScheduledTasks module.
# one has to modify directly via CIM property value setter instead:
$settings_set.CimInstanceProperties.Item('MultipleInstances').Value = 8
# NOTE: unreliable: appears not cause effect on Windows 10 / RU. Is this property localized ?
# 1 corresponds to 'Do not start new instance'
# 2 corresponds to 'Stop the existing instance'  
# MultipleInstancesPolicy.IgnoreNew
# 3 corresponds to 'Stop the existing instance'
# MultipleInstancesPolicy.Queue
# MultipleInstancesPolicy.Parallel
# 4 corresponds to 'Stop the existing instance'
# MultipleInstancesPolicy.StopExisting
# To see the effect needs to restart taskschd.msc
# NOTE: the propety value in new instance:
# $settings_set.Compatibility 'Win7'
set-scheduledtask -taskName $taskname -taskPath $taskpath -Settings $settings_set -password $password -user $user
  # alternative is to not convert
  # see https://nointerrupts.com/2018/10/18/update-scheduled-task-password-with-powershell/
# See Also:
# Job file internal format
# [FIXDLEN_DATA](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-tsch/b6425baf-7eb1-46c9-be5f-b13649004d0c)
# [Variable length section](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-tsch/1e7c0d2f-d126-4bf0-b080-c8cd46730d46)
# [SASec RPC interface is used to securely set or get account information associated with tasks](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-tsch/7849c5ca-a8df-4c1d-8565-41a9b979a63d)
#  See Also:
#  [Windows Job File Format](https://forensicswiki.xyz/wiki/index.php?title=Windows_Job_File_Format)
  <#

    get-ScheduledTask |
    where-object {
      $_.name -eq $taskname
    } |
    set-scheduledtask -user $credential.UserName -password $credential.GetNetworkCredential().Password
  #>

  # note the call to '.GetNetworkCredential()' in the middle - creates an [System.Net.NetworkCredential] instance
  # can also mass-update
  # https://www.cyberforum.ru/powershell/thread3077026.html
  # NOTE: setting task principal to interacive account may conflict with the setting: "run whether user logged on or not" 
  # NOTE: localized Windows 10 shows inaccurate tranlation "выполнять для всех пользователей" *English version is OK)
  # Windows 7 had a better label "выполнять вне зависимости от регистрации пользователя")
  # TODO: automate the setting: 
  # "Configure For:"
  # Windows 10
  # Windows 7,Windows Server 2008 R2
  # Windows Vista, Windows Serevr 2008
  # Windows Server 2003, Windows XP or Windows 2000
  # which corresponds to the version of the schema:
  # <Task version="1.4" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  # <Task version="1.3" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  # <Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  # <Task version="1.1" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  # with aditional differences across XML, like e.g.
  #  <DisallowStartOnRemoteAppSession>false</DisallowStartOnRemoteAppSession>
  #  <UseUnifiedSchedulingEngine>true</UseUnifiedSchedulingEngine>

  # and what effect does the
  # "Hidden Task" setting
  # have
  #  <Settings>
  #    <Hidden>true</Hidden>

  <#
    set-scheduledtask |
    where-object {
      $_.Principal.UserId -eq $credential.UserName
    } |
    set-scheduledtask -user $credential.UserName -password $credential.GetNetworkCredential().Password
  #>
} else {
  # register-scheduledtask : Access is denied
  # solved by 'runas' elevation
  # even on a vanilla Windows 10 machine and Windows 8.1
  # 'S4U' logon type for execution regardless of user logged in or not
  # see also:
  # https://learn.microsoft.com/en-us/windows/win32/taskschd/taskschedulerschema-logontype-simpletype
  # about S4U - Kerberos Network Authentication Service (V5) Service for User (S4U) Extension provides two extensions to the Kerberos Protocol.
  # https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/ns-ntsecapi-kerb_s4u_logon
  # https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-sfu/8ee85a47-7526-4184-a7c5-25a5e4155d7d

  # see also: https://www.cyberforum.ru/powershell/thread3062695.html#post16663321
  $principal = new-scheduledtaskprincipal -userid 'NETWORKSERVICE' -logontype S4U -runlevel Highest
  # 
  # drop the $startup_trigger - works
  if ($with_startup_trigger) {
    # Windows 10: 
    # register-scheduledtask : XML-код задачи содержит значение в неправильном формате или за пределами допустимого диапазона.
    # FullyQualifiedErrorId : HRESULT 0x80041318,Register-ScheduledTask
    # The task XML contains a value which is incorrectly formatted or out of range.
    register-scheduledtask -action $action -trigger $trigger,$startup_trigger -taskname $taskname -taskpath $taskpath -Principal $principal -description '...'
  } else {
    register-scheduledtask -action $action -trigger $trigger -taskname $taskname -taskpath $taskpath -Principal $principal -description '...'
  }
}

# if ($debug){

  get-scheduledtask -taskname $taskname |
  select-object -first 1 |
  select-object -expandproperty Triggers |
  foreach-object {
    write-output $_.ToString()
  }


# }
<#
inspect with
& taskschd.msc
#>
# Example XML can be found in
# https://docs.microsoft.com/en-us/windows/win32/taskschd/logon-trigger-example--xml-
# XML file is saved into e.g.
# C:\Windows\System32\Tasks\automation\example_task_app
# The registy key of the job is
# [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tree\automation\example_task_app]
# and 
# HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{D294533F-D96D-4365-B6E7-43E9A548F3DE}
# with the unique guid in place of the
# D294533F-D96D-4365-B6E7-43E9A548F3DE
# the Trigers, Actions are in both resources
# NOTE: reported windows version inconsistent processing of `RepetitionDuration`# property specifically when it comes to configure to "run forever"
# https://docs.microsoft.com/en-us/answers/questions/145419/powershell-new-scheduledtasktrigger-cmdlet-with-in.html

<#
# https://docs.microsoft.com/en-us/answers/questions/145419/powershell-new-scheduledtasktrigger-cmdlet-with-in.html
$repetition_interval = (new-timespan -Minutes 3)
$starttime = (get-date)
if([environment]::OSVersion.Version.Major -eq 10){
  # Windows 16
  write-host 'Setting Schedule Trigger for Widows 2016'
  # the RepetitionDuration may be omitted
  $params =  @{
    'Once' = $true
    'At' = $startTime
    'RepetitionInterval' = $repetition_interval
  }
}
# the next param option also works for Windows 8
if([environment]::OSVersion.Version.Major -eq 6){
  # Windows 12
  write-host 'Setting Schedule Trigger for Widows 2012 and below'
  # the RepetitionInterval and RepetitionDuration must be both provided
  $params =  @{
    'Once' = $true
    'At' = $startTime
    'RepetitionInterval' = $repetition_interval
    'RepetitionDuration' = ([TimeSpan]::MaxValue)
  }
}
# NOTE: Powershell 5.x *syntax sugar* -
$trigger = new-scheduledtasktrigger @params
# of couse the regular advise of very long time span for repetitio duration
# new-timespan -Days (365*500)
# leaves unanswered if the schedule resumes after the reboot
#>
# alternatively
# $trigger = new-scheduledtasktrigger -once -at $startTime -repetitionInterval (new-timespan -Minutes $interval) -repetinionDuration (new-timespan -Minutes $duration)
<#
The application-specific permission settings do not grant
Local Activation permission for the COM Server application with CLSID
{6B3B8D23-FA8D-40B9-8DBD-B950333E2C52}
 and APPID
{4839DDB7-58C2-48F5-8283-E1D1807D0D7D}
 to the user NT AUTHORITY\LOCAL SERVICE SID (S-1-5-19)
 from address LocalHost (Using LRPC) running in the application container
 Unavailable SID (Unavailable).
 This security permission can be modified using the
 Component Services administrative tool.

#>

<#
The application-specific permission settings do not grant
Local Activation permission for the
COM Server application with CLSID
{D63B10C5-BB46-4990-A94F-E40B9D520160}
 and APPID
{9CA88EE3-ACB7-47C8-AFC4-AB702511C276}
 to the user DESKTOP-P976KI9\sergueik SID (S-1-5-21-2643251824-300502830-132655133-1001)
 from address LocalHost (Using LRPC) running in the application container
 Unavailable SID (Unavailable).
 This security permission can be modified using the
 Component Services administrative tool.

#>
<#
https://www.cyberforum.ru/powershell/thread2903854.html
#>

$task_user = New-ScheduledTaskPrincipal -UserID "NT AUTHORITY\SYSTEM" -LogonType ServiceAccount -RunLevel Highest


<#

# NOTE concerning triggers:
# see also:
# https://xplantefeve.io/posts/SchdTskOnEvent

$taskname = 'example_task_app'
$subscription = get-scheduledtask | 
where-object { 
  $_.taskname -match $taskname  
} | 
select-object -expandproperty triggers | 
where-object {
  $_.subscription -ne $null 
} | 
select-object -expandproperty Subscription
# example Subscription:
# <QueryList><Query Id="0" Path="Application"><Select Path="Application">*[System[Provider[@Name='Microsoft-Windows-RestartManager'] and EventID=100000]]</Select></Query></QueryList>
# job 'example_task_app' subscribes to Application event of source: RestartManager of EventId 10000
# that says "starting session"
MSFT_TaskRepetitionPattern

$helperclass = cimclass MSFT_TaskEventTrigger root/Microsoft/Windows/TaskScheduler
$trigger = New-CimInstance -cimclass $helperclass -ClientOnly
$trigger.Enabled = $true

# note: some challenge with single quotes within single quotes HERE-Document 
# '' is not interpolated
$subscription = @'
<QueryList><Query Id="0" Path="Microsoft-Windows-NetworkProfile/Operational">
<Select Path="Microsoft-Windows-NetworkProfile/Operational">*
[System[Provider[@Name="Microsoft-Windows-NetworkProfile"] and EventID=10000]]</Select></Query></QueryList>
'@

$trigger.Subscription = ( $subscription -replace '\n', '' )-join ''
# NOTE: cannot operate 
$task = get-scheduledtask |
  where-object {
  $_.taskname -match $taskname 
}
$task.triggers.add($trigger)
# collection of the fixed size
$taskname = $task.taskname 
$taskpath = $task.taskpath
 [Microsoft.Management.Infrastructure.CimInstance[]]$current_triggers = $task.triggers
set-scheduledtask -taskName $taskname -taskPath $taskpath -triggers $current_triggers,$trigger
# cannot convert Microsoft.Management.Infrastructure.CimInstance[] type to Microsoft.Management.Infrastructure.CimInstance type
 [Microsoft.Management.Infrastructure.CimInstance[]]$new_triggers = @()
[Microsoft.Management.Infrastructure.CimInstance[]]$new_triggers = @()
 $new_triggers.add($current_triggers.Item(0))
 # collection of the fixed size
 # set-scheduledtask -taskName $taskname -taskPath $taskpath -trigger $current_triggers.item(0),$trigger

# the argument  type mismatch
 set-scheduledtask -taskName $taskname -taskPath $taskpath -trigger $current_triggers.item(0),$current_triggers.item(1),$current_triggers.item(2)
# works

# inspect and compare $trigger with $current_triggers.item(2)
# filling missing 'Repetition' property
$helperclass = cimclass MSFT_TaskRepetitionPattern root/Microsoft/Windows/TaskScheduler
$repetition = New-CimInstance -cimclass $helperclass -ClientOnly
$trigger.repetition = $repetinion
#>
