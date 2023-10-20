# https://docs.microsoft.com/en-us/windows/win32/secauthz/well-known-sids
# http://woshub.com/convert-sid-to-username-and-vice-versa/


add-type -assembly 'System.DirectoryServices.AccountManagement'
# https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.accountmanagement.principal.getgroups?view=netframework-4.5#system-directoryservices-accountmanagement-principal-getgroups
# https://stackoverflow.com/questions/8009593/list-group-memberships-for-ad-users
$groups = [System.Security.Principal.WindowsIdentity]::GetCurrent().Groups

$groups | foreach-object { 
$group = $_
# NOTE: localized
write-output ($group.value)
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
$userid = 'S-1-5-32-544'

$userid = 'S-1-5-32-545'
$var = New-ScheduledTaskPrincipal  -userid $userid -logontype S4U -runlevel highest


$task = get-scheduledtask -taskname 'example_task_app' | select-object -first 1
# https://docs.microsoft.com/en-us/powershell/module/scheduledtasks/set-scheduledtask?view=windowsserver2019-ps
Set-ScheduledTask -taskname $task.TaskNAme -taskpath  $task.taskpath  -Principal $var

$name = ( $group.Translate([System.Security.Principal.NTAccount]).value ) -replace '^.*\\', ''
# http://woshub.com/convert-sid-to-username-and-vice-versa/
# NOTE: the following returns nothing for Group accounts
(Get-WmiObject -Class win32_userAccount -Filter "name='$($name)' and domain='$env:computername'").SID

# Inspecting error code: 2147750680
# The task XML contains a value which is incorrectly formatted or out of range. (Exception from HRESULT: 0x80041318)

$var = New-ScheduledTaskPrincipal  -userid 'SYSTEM' -logontype S4U -runlevel highest