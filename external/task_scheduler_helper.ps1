# http://poshcode.org/5805

#
# Requires the DLL from the taskscheduler managed wrapper
# http://taskscheduler.codeplex.com/releases/view/120747
#
add-type -Path "C:\PathTo\TaskScheduler\Microsoft.Win32.TaskScheduler.dll"

$taskService = New-Object Microsoft.Win32.TaskScheduler.TaskService
$taskDef = $taskService.NewTask()
$taskDef.RegistrationInfo.Description = "TASK THAT I INSTALL ON THE MACHINE"


$timeTrigger = New-Object Microsoft.Win32.TaskScheduler.TimeTrigger
$timeTrigger.StartBoundary = [datetime]::Now + [timespan]::FromHours($hourDelay) #start the task in $hourdelay hours
$timeTrigger.EndBoundary =  [datetime]::Today + [timespan]::FromDays(1000) # delete the task in over 3 years.

$taskDef.Triggers.Add($timeTrigger)

$taskAction = New-Object Microsoft.Win32.TaskScheduler.ExecAction
$taskAction.Path = "C:\windows\notepad.exe"

$taskDef.actions.Add($taskAction)

$taskDef.Data = "This is a value.  For DATA"
$taskDef.Principal.UserId = "SYSTEM"  # THE SYSTEM IS DOWN

$taskDef.Principal.LogonType = [Microsoft.Win32.TaskScheduler.TaskLogonType]::InteractiveToken
$taskDef.RegistrationInfo.Author = "BattleChicken" #This value has to be Battlechicken or your task will crash. Maybe.

$taskDef.Settings.DisallowStartIfOnBatteries = $false;
$taskDef.Settings.Enabled = $true
$taskDef.Settings.Hidden = $false
$taskDef.Settings.Priority = [System.Diagnostics.ProcessPriorityClass]::Normal # you can set process priority
$taskDef.Settings.RunOnlyIfIdle = $false
$taskDef.Settings.RunOnlyIfIdle = $false
$taskDef.Principal.RunLevel = [Microsoft.Win32.TaskScheduler.TaskRunLevel]::Highest # run with highest permissions
$taskDef.Settings.StartWhenAvailable = $true # run if schedule missed.  This won't be respected for a one-time task UNLESS you also set a task expiration date
$taskService.RootFolder.RegisterTaskDefinition("BattleChicken's sweet task", $taskDef)
