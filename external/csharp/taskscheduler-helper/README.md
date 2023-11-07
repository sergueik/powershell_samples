### Info

Code from the article by Dennis Austin __New Task Scheduler Class Library for .NET__ covering  [.NET Interop](https://www.codeproject.com/Articles/2407/A-New-Task-Scheduler-Class-Library-for-NET) around __COM__ server `148BD527-A2AB-11CE-B11F-00AA00530503`, which is the __Task Scheduler Service__ [interface](https://github.com/tpn/winsdk-10/blob/master/Include/10.0.16299.0/um/MSTask.idl)
with a  lot of COM interop C# code around __Task Scheduler v 1.0__ API which is technically largely compatible with later versions while available on Windows 7 or earler.
### Note

The original build included Manifest Options compiler instruction
```XML
    <PostBuildEvent>mt.exe -manifest "$(ProjectDir)app.manifest" -outputresource:"$(TargetDir)$(TargetName).exe;#1"
</PostBuildEvent>
```
which is not available in __Sharp Develop__
### See Also

  * https://stackoverflow.com/questions/63864131/why-does-schtasks-not-recognize-xml-syntax
  * MS Powershell-centric redesigned [ScheduledTasks Module](https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/?view=windowsserver2019-ps) applicatble to __Task Scheduler v. 2.?__ that lists some 20 cmdlets:
     + `Disable-ScheduledTask`
     + `Enable-ScheduledTask`
     + `Export-ScheduledTask`
     + `Get-ClusteredScheduledTask`
     + `Get-ScheduledTask`
     + `Get-ScheduledTaskInfo`
     + `New-ScheduledTask`
     + `New-ScheduledTaskAction`
     + `New-ScheduledTaskPrincipal`
     + `New-ScheduledTaskSettingsSet`
     + `New-ScheduledTaskTrigger`
     + `Register-ClusteredScheduledTask`
     + `Register-ScheduledTask`
     + `Set-ClusteredScheduledTask`
     + `Set-ScheduledTask`
     + `Start-ScheduledTask`
     + `Stop-ScheduledTask`
     + `Unregister-ClusteredScheduledTask`
     + `Unregister-ScheduledTask`
    
   * https://forum.powerbasic.com/forum/user-to-user-discussions/programming/21836-using-task-scheduler-via-com
   * https://stackoverflow.com/questions/44078143/itaskschedulerdelete-fails
   * https://doxygen.reactos.org/dd/d19/mstask_8idl_source.html
   * https://learn.microsoft.com/en-us/windows/win32/taskschd/using-the-task-scheduler
   * [discussion on running the Scheduled task with under system account of on downlevel systems](https://stackoverflow.com/questions/22151631/powershellpack-taskscheduler-module-run-task-with-no-logged-user)
   *  "", "NT AUTHORITY\SYSTEM", or "SYSTEM" account arguments of `schtasks.exe` [documentation](https://learn.microsoft.com/en-us/windows/win32/taskschd/schtasks)
  * [How Can I Best Work with Task Scheduler?](https://devblogs.microsoft.com/scripting/hey-scripting-guy-how-can-i-best-work-with-task-scheduler/)
  * Run whether user is logged on or not" in Powershell?  [stackoverflow dscussion] (https://stackoverflow.com/questions/12770293/how-to-set-schedule-service-run-whether-user-is-logged-on-or-not-in-powershell)
  * https://learn.microsoft.com/en-us/windows/win32/taskschd/taskfolder-registertaskdefinition
   
### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
