@echo OFF
REM based on:
REM origin https://superuser.com/questions/1051217/passing-command-line-parameters-through-schtasks

set d=%DATE%
set t=%TIME%
REM TODO password prompt
set PASSWORD=<place password here for testing>
REM need to explicitly pass the path to powershell.exe
schtasks.exe /create /TN AUTOMATION\ATASK /tr "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy bypass -noprofile -f '%1'" /rl HIGHEST /ru "NT AUTHORITY\SYSTEM" /sc once /st %t:~0,8% /sd %d:~4,10%
rem  /v1 /z
REM need to provide with full path to the target Powershell script otherwise will see the logged message:
REM Task Scheduler successfully completed task "\RunCMD", instance "{3c3cf19b-9163-4fbb-a9e5-b97381652a94}" , action "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" with return code 4294770688.
REM 0xFFFD0000 (4294770688), which stands for "The field "Add arguments (optional)" contains an invalid file name".
REM see also: https://troubleshootingsql.com/2014/11/17/gotcha-executing-powershell-scripts-using-scheduled-tasks/
REM show the task XML
schtasks.exe /query /TN RunCMD /XML
REM C:\Windows\System32\taskschd.msc
schtasks.exe /query /TN RunCMD
REM execute the task, this also deletes it
schtasks.exe /run /tn RunCMD
