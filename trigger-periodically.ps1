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


# see also
# https://learn.microsoft.com/en-us/windows/win32/taskschd/task-scheduler-schema-elements?source=recommendations
# see also:
# Allow Non Admins to Manage Scheduled Tasks
# by including the account into "power users" and "backup operators" groups
# https://social.technet.microsoft.com/Forums/ie/en-US/18af37ed-d18c-477d-9d08-65face75855e/allow-non-admins-to-manage-scheduled-tasks?forum=winserverManagement

# see also: 
# stealth Scheduled Tasks (in Russian)
# https://habr.com/ru/company/rvision/blog/723050
param (

  [String]$datafile = (resolve-path 'data.txt' -erroraction silentlycontinue), # may be missing initially
  # only safe when the script is never run through Scheduled Task
  # specify alternative filename to simplify debugging of the task launch
  [String]$script = (resolve-path 'work_script.ps1'),
  # NOTE:  assuming /cygdriver is mounted ad d: DOS drive d:/home/sergueik/dialog.ps1' for launch from within cygwin
  [int]$delay = 2, # delay in minutes
  [int]$interval_minutes = 0, # repetition interval in minutes, using recurring schedule
  # e.g. PT45M
  [int]$interval_hours = 0, # repetition interval in hours, using recurring schedule
  # PT1H
  # NOTE: cannot combine minutes and hours
  [String]$taskname = 'example_periodic_task',
  [string]$taskpath = 'automation',
  # NOTE: Spaces not handled
  # NOTE: avoid using 'taskname' value for $taskname variable: would conflict with column names
  [string]$url = '',
  # NOTE: optional argument(s)
  [string]$config_dir = '',
  [int]$port = 0,
  # [string]$url = 'http://localhost:8085/basic/upload/',
  # no default value - the default BORG url is hard coded in "update_data.ps1" script
  # when an argument provided, overrides
  [switch]$remove,
  [switch]$currentuser,
  [string]$systemaccount = 'SYSTEM',
  # NOTE: The other two options 'NETWORK SERVICE' and 'LOCAL SERVICE' are failing to collect the data (no meaningful error)
  [switch]$send,
  # TODO: re-implement - see the comment on 0x80070001
  [switch]$noop,
  # NOTE: turning on eventlog flag leads to 1440 eventlog entries per task, which totals to at least 2880
  # No retention policy against custom Event Log is currently implemented
  # Turn on with caution
  [switch]$eventlog,
  [switch]$ssh,
  [string]$plaintext_password = $null,
  [switch]$debug,
  [string]$user = $null,
  [switch]$help,
  [switch]$dev
)

 
function check_elevation {
  param (
    [String]$message,
    [switch]$debug
  )

  # http://blogs.msdn.com/b/virtual_pc_guy/archive/2010/09/23/a-self-elevating-powershell-script.aspx
  #
  # Get the ID and security principal of the current user account
  $myWindowsID = [System.Security.Principal.WindowsIdentity]::GetCurrent()

  $myWindowsPrincipal = New-Object System.Security.Principal.WindowsPrincipal ($myWindowsID)

  # Get the security principal for the Administrator role
  $adminRole = [System.Security.Principal.WindowsBuiltInRole]::Administrator

  # Check to see if we are currently running "as Administrator"
  if (-not $myWindowsPrincipal.IsInRole($adminRole)) {
    write-host -foregroundColor 'Red' ('The {0} is privileged and has to run elevated' -f $message )
    exit
  }
}


function private:ConvertTo-RegularString([Security.SecureString]$securestring) {
  return [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securestring)
  )
}

# TODO: refactor code to do the task name update based on flags before deleting the task -
# alternatively stop updating the variable during the script execution
# based on documentation from
# https://devblogs.microsoft.com/scripting/use-powershell-to-create-scheduled-tasks/
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/?view=windowsserver2019-ps


<#
NOTE: 
Windows 7
get-scheduledtask : The term 'get-scheduledtask' is not recognized as the name of a cmdlet, function, script file, or operable program. Check the spelling of
#>

function check_release {
  param (
    [switch]$debug
  )

  $local:release = $null
  if([environment]::OSVersion.Version.Major -eq 10 ){
    # NOTE: Windows 10 has a few builds, e.g. 19041
    $local:release = 'Windows 2016'
  }

  if([environment]::OSVersion.Version.Major -eq 6){    
    if([environment]::OSVersion.Version.Build -eq 7601){
      $local:release = 'Windows 7'
    } else {
      $local:release = 'Windows 2012'
    }
  }
  if ($debug) {
    write-host ('Release: {0}' -f $local:release )
  }
  return $local:release
} 

[bool]$debug_flag = [bool]$psboundparameters['debug'].ispresent


if ( [bool]$psboundparameters['ssh'].ispresent ) {
  $env_check = $true
} else {
  $env_name = 'SSH_CLIENT'
  $save_pwd = $pwd
  if ($debug_flag) {
    write-host ('checking value of $env:{0}' -f $env_name)
  }
  chdir 'env:'
  $env_check = ((dir $env_name -erroraction silentlycontinue) -ne $null)
  cd $save_pwd
}

if ($debug_flag) {
  write-host (' $env_check=  {0}' -f $env_check )
}


if ([bool]$psboundparameters['help'].ispresent) {
  write-host @'
Example Usage:
  .\trigger-periodically.ps1 -taskname example_collect_app_user -user $env:COMPUTERNAME\app -eventlog
  .\trigger-periodically.ps1 -currentuser -send -taskname example_send_app_user -user $env:COMPUTERNAME\app -eventlog -interval_minutes 2
  .\trigger-periodically.ps1 -script ( resolve-path 'getconfig.ps1').path -taskname 'download_config_system' -config_dir (resolve-path '.').path -interval_hours 1 -port $PORT
'@
  exit
}

check_elevation -debug:$false -message 'Scheduled Task reconfiguration'


if ((check_release -debug $debug_flag ) -eq 'Windows 7') { 
  write-host -foregroundColor 'Red' ('This script relies on cmdlets which do not exist in Windows 7')
  exit

}
# NOTE: stopping task before removing
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/stop-scheduledtask?view=windowsserver2019-ps
if (([bool]$PSBoundParameters['stop'].IsPresent) -or ([bool]$PSBoundParameters['remove'].IsPresent)) {
  # NOTE: Stop-ScheduledTask : A parameter cannot be found that matches parameter name 'confirm'
  write-host ('Stop scheduled task {0}' -f $taskname )
  get-scheduledtask | where-object { $_.taskname -eq $taskname} | stop-scheduledtask
  if ([bool]$PSBoundParameters['stop'].IsPresent ){ exit }
  # NOTE: if creating a new task with same name, skipped
}

write-host ('unregister scheduled task {0}' -f $taskname )
get-scheduledtask | where-object { $_.taskname -eq $taskname } | unregister-scheduledtask -confirm:$false

if ([bool]$PSBoundParameters['remove'].IsPresent) {
  write-host 'done'
  exit
}

# build the command line arguments and task name

if ([bool]$psboundparameters['eventlog'].ispresent) {
  $eventlog_arg = '-eventlog'
} else {
  $eventlog_arg = ''
}

if ([bool]$psboundparameters['send'].ispresent) {
  $send_arg = '-send'
} else {
  $send_arg = ''
}

# make sure the path to datafile is absolute . If no path is provided through argument, assume current directory
[string]$workdir = 'c:\temp'
if ([bool]$psboundparameters['dev'].ispresent) {
  $workdir = (resolve-path '.').path
}
$datafile = $workdir + '\'+ 'data.txt'
# pass down the noop switch as switch
# NOTE:
# when passing in cmd as -noop $false or -noop 0
# the Windows Task Scheduler reports
# return code with return code 2147942401 0x80070001 (incorrect function Exception from HRESULT 0x80070001)
# Cannot process argument transformation on parameter 'noop'.
# Cannot convert value "System.String" to type "System.Boolean".
# Boolean parameters accept only Boolean values $True, $False, 1 or 0.
if ([bool]$psboundparameters['noop'].ispresent) {
  $noop_arg = '-noop'
} else {
  $noop_arg = ''
}

# passing down oarguments to worker script, ignoring ones which have empty values
if ( ( $config_dir -eq $null  ) -or ($config_dir -eq '')) {
  $config_dir_arg = ''
} else {
  $config_dir_arg = ( '-config_dir {0}' -f $config_dir )
}


if ( ( $url -eq $null  ) -or ($url -eq '')) {
  $url_arg = ''
} else {
  $url_arg = ( '-url {0}' -f $url )
}

if ( $port -eq 0 ) {
  $port_arg = ''
} else {
  $port_arg = ( '-port {0}' -f $port )
}


# NOTE: the datafile argment is not optional for historic reasons
# make sure the worker script does not have a different meaning for the datafile argument
# it is fine to simply ignore this parameter

[String]$arguments = ('{0} {1} -datafile "{2}" {3} {4} {5} {6}' -f $send_arg, $noop_arg, $datafile, $url_arg, $eventlog_arg, $config_dir_arg, $port_arg)
# write-host $arguments
$action = new-scheduledtaskaction -execute 'powershell.exe' -argument ('-executionpolicy bypass -noprofile -file {0} {1}' -f $script, $arguments)
$tmpd = (get-date).AddMinutes($delay)
$time = get-date($tmpd) -format 'HH:mm'
write-host ('will do at {0}' -f $time)
# origin: https://stackoverflow.com/questions/20108886/powershell-scheduled-task-with-daily-trigger-and-repetition-interval

if (($interval_hours -eq 0 ) -and ($interval_minutes -eq 0)){
  $interval_minutes = 1
}

# for the script works on both windows server 2012 and 2016
# need to provide different parameters based on the os version
# origin:
# https://docs.microsoft.com/en-us/answers/questions/145419/powershell-new-scheduledtasktrigger-cmdlet-with-in.html
# if([environment]::OSVersion.Version.Major -eq 10){
if ((check_release -debug $debug_flag ) -eq 'Windows 2016') { 
  # Windows 2016
  if ($debug_flag ) {
    write-host ('setting schedule trigger for windows 2016' )
  }
  if ($interval_hours -ne 0) {
    if ($debug_flag ) {
      write-host ('setting repetition interval {0} hours' -f $interval_hours)
    }
    $trigger = new-scheduledtasktrigger -Once -at $time -RepetitionInterval (new-timespan -Hours $interval_hours)
  }
  if ($interval_minutes -ne 0) {
    if ($debug_flag ) {
      write-host ('setting repetition interval {0} minutes' -f $interval_minutes)
    }
    $trigger = new-scheduledtasktrigger -Once -at $time -RepetitionInterval (new-timespan -Minutes $interval_minutes)
  }
}


if ((check_release -debug $debug_flag ) -eq 'Windows 2012') { 
  # if([environment]::OSVersion.Version.Major -eq 6){
  # Windows 2012
  # new-scheduledtasktrigger : The RepetitionInterval and RepetitionDuration Job trigger parameters must be specified together.

  if ($debug_flag ) {
    write-host ('setting schedule trigger for windows 2012' )
  }

  if ($interval_hours -ne 0) {
    if ($debug_flag ) {
      write-host ('setting repetition interval {0} hours' -f $interval_hours)
    }
    $trigger = new-scheduledtasktrigger -Once -at $time -RepetitionInterval (new-timespan -Hours $interval_hours) -RepetitionDuration ([TimeSpan]::MaxValue)
  }
  if ($interval_minutes -ne 0) {
    if ($debug_flag ) {
      write-host ('setting repetition interval {0} minutes' -f $interval_minutes)
    }
    $trigger = new-scheduledtasktrigger -Once -at $time -RepetitionInterval (new-timespan -Minutes $interval_minutes) -RepetitionDuration ([TimeSpan]::MaxValue)
  }

  

  # NOTE:  some forums report
  # register-scheduledtask : The task XML contains a value which is incorrectly formatted or out of range.
  # (10,42):Duration:P99999999DT23H59M59S
}

$settings_set = New-ScheduledTaskSettingsSet
$settings_set.StartWhenAvailable = $true
$settings_set.StopIfGoingOnBatteries = $false
$settings_set.DisallowStartIfOnBatteries = $false
$settings_set.WakeToRun = $true
$settings_set.runonlyifidle = $false

if ([bool]$psboundparameters['currentuser'].ispresent) {
  if ($user -eq $null -or $user -eq '') {
    $user = "${env:USERDOMAIN}\${env:USERNAME}"
  }
  # TODO: prepend domain in all cases

  if ($plaintext_password -ne $null -and $plaintext_password -ne '' )   {
    # verify credentials can be constructed. one actually can pass plain password straight to scheduledtask cmdlets
    $credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $user,(ConvertTo-SecureString $plaintext_password -AsPlainText -Force )
  } else {
    $message = ('Enter password for user {0}' -f $user)
    if ($env_check -eq $true ) {
      $securestring_password = read-host $message -assecurestring
      $credential = new-object -TypeName System.Management.Automation.PSCredential -ArgumentList $User,  $securestring_password
      $plaintext_password = $credential.GetNetworkCredential().Password
    } else {
      $credential = get-credential -username $user -message $message
      $plaintext_password = $credential.GetNetworkCredential().Password
    }
  }

  register-scheduledtask -action $action -trigger $trigger -taskname $taskname -taskpath $taskpath -password $plaintext_password -user $user -description '...' -runlevel Highest  | out-null
  set-scheduledtask -taskName $taskname -taskPath $taskpath -Settings $settings_set -password $plaintext_password -user $user | out-null

  # NOTE: do not clear
  # $plaintext_password = $null
} else {
  $principal = new-scheduledtaskprincipal -userid $systemaccount -logontype S4U -runlevel Highest
  register-scheduledtask -action $action -trigger $trigger -taskname $taskname -taskpath $taskpath -Principal $principal -description '...'| out-null
  set-scheduledtask -taskName $taskname -taskPath $taskpath -Settings $settings_set| out-null
}

$env_check = $false
write-host 'Done'
<#
# origin: "sunset sunrise scheduler"
# https://www.outsidethebox.ms/20351/
# add trigger from Eventlog query
# see also examples in: 
# https://gist.github.com/marckean/fe8fc10bb6ea57b3d0ba166f6c63389c
# https://stackoverflow.com/questions/42801733/creating-a-scheduled-task-which-uses-a-specific-event-log-entry-as-a-trigger
# https://superuser.com/questions/1488344/create-a-scheduled-task-to-trigger-on-a-log-event-with-a-specific-event-id
# https://xplantefeve.io/posts/SchdTskOnEvent
$class = cimclass MSFT_TaskEventTrigger root/Microsoft/Windows/TaskScheduler
$triggerevent = $class | New-CimInstance -ClientOnly
$triggerevent.Enabled = $true
$triggerevent.Subscription = "<QueryList><Query Id='0' Path='System'><Select Path='System'>*[System[Provider[@Name='Microsoft-Windows-Power-Troubleshooter'] and EventID=1]]</Select></Query></QueryList>"
# append to already defined triggers
#
set-scheduledtask -taskname $taskname -trigger $((get-scheduledtask -taskname $taskname).triggers + $triggerevent)


#>
<#
  # NOTE: wher creating the task make sure arguments to match:
  # the name mathes the flags describing the action (-send for send , none for collect)
  # the eventlog flag provided explicitly

  . .\trigger_periodically.ps1 -currentuser -taskname example_send_current_user -send -eventlog
  . .\trigger_periodically.ps1 -currentuser -taskname example_collect_current_user -eventlog

  By default Logged events in "Applications and Services Log","Microsoft","Windows","Task Scheduler","Operational" - maybe too verbosely
  same information is displayed in Task Scheduler (taskschd.msc)  even more labor intnsive to track due to excessive amount of decoration per eventlog
  custom logging added to "Applications and Services Log","winborg"
  Custom log biggest advantage - one can clear it.
#>

<#
  . .\trigger_periodically.ps1 -currentuser -taskname example_collect_current_user -eventlog
  . .\trigger_periodically.ps1 -currentuser -send -taskname example_send_current_user -eventlog

  . .\trigger_periodically.ps1 -taskname example_collect_system -eventlog
  . .\trigger_periodically.ps1 -send -taskname example_send_system -eventlog

  # e.g. for app user

  net.exe user app %APP_PASSWORD% /add
  The password entered is longer than 14 characters.  Computers with Windows prior to Windows 2000 will not be able to use this account. Do you want to continue this operation? (Y/N) [Y]: y
  The command completed successfully.
  net.exe localgroup administrators app /add
  . .\trigger_periodically.ps1 -currentuser -taskname example_collect_app_user -user $env:COMPUTERNAME\app -eventlog
  . .\trigger_periodically.ps1 -currentuser -send -taskname example_send_app_user -user $env:COMPUTERNAME\app -eventlog
  . .\trigger_periodically.ps1 -currentuser -taskname example_collect_app3_user -user $env:COMPUTERNAME\app3 -eventlog
  . .\trigger_periodically.ps1 -currentuser -send -taskname example_send_app3_user -user $env:COMPUTERNAME\app3 -eventlog

  provide credential of the said user in logon dialog

#>

<#

The values reported by SYSTEM and CURRENT USER match well

  . .\trigger_periodically.ps1 -taskname example_collect_system -datafile 'data.system.txt'
  . .\trigger_periodically.ps1 -currentuser -taskname example_collect_current_user -datafile 'data.user.txt'

  # for production run, remove the suffix

  . .\trigger_periodically.ps1 -taskname example_collect_system -datafile 'data.txt'
  . .\trigger_periodically.ps1 -taskname example_send_system -datafile 'data.txt' -send

 # there is currently no argument to specify the built-in account (the default is SYSTEM), one has to modify in code 
#>

