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
# based on links from https://serverfault.com/questions/834046/remove-a-user-from-acl-completely-using-powershell
# see also: https://blog.netwrix.com/2018/04/18/how-to-manage-file-system-acls-with-powershell-scripts/
# for more low-level modifications 
#
param(
  [string]$filepath = (resolve-path -path 'Program\bin\Debug\LoadAverageService.exe').path
)

# note System.Security.AccessControl.FileSecurity is different and cannot be casted to
# System.Security.AccessControl.DirectorySecurity
[System.Security.AccessControl.FileSecurity]$acl = get-acl -path $filepath 
# TODO: detect dynamically

write-host -ForegroundColor 'DarkRed'  ('Starting configuration')

get-acl -path $filepath | select-object -expandproperty Access



$acl | select-object -expandproperty 'Access' | select-object -property 'IdentityReference'
<# TODO
get-acl -path $filepath | 
select-object -expandproperty Access | filter-object { $_.IdentityReference }

IdentityReference : BUILTIN\Администраторы

IdentityReference : NT AUTHORITY\СИСТЕМА

IdentityReference : BUILTIN\Пользователи

IdentityReference : NT AUTHORITY\Прошедшие проверку

#>


$sids = $acl | select-object -expandproperty 'Access' | select-object -property 'IdentityReference'

# NOTE: after /d every account remains and
# the /remove does not appear to work
# DO NOT UNCOMMENT
# invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /inheritance:d"
#
<#
$x = $sids[0]
$sid =  [System.Security.Principal.NTAccount]$sid = New-Object System.Security.Principal.Ntaccount($x.IdentityReference)
$sid.Translate([System.Security.Principal.SecurityIdentifier])
#>
$sids |  foreach-object { 
$IdentityReference = $_.IdentityReference
$value = $IdentityReference.Translate([System.Security.Principal.SecurityIdentifier]).Value
write-output $IdentityReference
write-output $value
}
$value = $sid.Translate([System.Security.Principal.SecurityIdentifier]).Value
<#
# https://learn.microsoft.com/en-us/windows/win32/secauthz/well-known-sids
$all_values = @{ 
  'S-1-3-0' = 'CREATOR OWNER';
  'S-1-5-18' = 'NT AUTHORITY\SYSTEM';
  # Value : NT AUTHORITY\СИСТЕМА
  'S-1-5-32-544' = 'BUILTIN\Administrators';
  # Value : BUILTIN\Администраторы
  'S-1-5-21-440999728-2294759910-2183037890-1000' = 'sergueik42\sergueik';
  # Value : BUILTIN\Пользователи
  # S-1-5-32-545

  # Value : NT AUTHORITY\Прошедшие проверку
  # S-1-5-11
 }



#>

# https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/icacls
$keep_values = @( 'S-1-5-18', 'S-1-5-32-544' )
# optional try to deal with perms ahead of removing the recursion 
$sids |  foreach-object { 
  $IdentityReference = $_.IdentityReference
  $value = $IdentityReference.Translate([System.Security.Principal.SecurityIdentifier]).Value
  if (-not ($value -in $keep_values)) {
    write-output $IdentityReference
    write-output $value
	    # note argument order
    write-host "c:\Windows\System32\icacls.exe ${filepath} /remove:g $value"
    invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /remove:g $value"  
    # NOTE: the command does not return error but nothing changes
    $acl.PurgeAccessRules($IdentityReference)

  }
  if ( ($value -in $keep_values)) {
    write-output $IdentityReference
    write-output $value
    # note {} around value. without those interpolates to nothing
    write-host "c:\Windows\System32\icacls.exe ${filepath} /grant *${value}:F"
    invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /grant *${value}:F"  
  }
}
<#
c:\Windows\System32\icacls.exe C:\developer\sergueik\powershell_ui_samples\csharp\loadaverage-service\Program\bin\Debug\LoadAverageService.exe /grant S-1-5-18:F
Недопустимый параметр: "/grant"
#>
# https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/icacls
# remove  all inheritance
invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /inheritance:r"
# repeat granting and removing the rights

$sids |  foreach-object { 
  $IdentityReference = $_.IdentityReference
  $value = $IdentityReference.Translate([System.Security.Principal.SecurityIdentifier]).Value
  if (-not ($value -in $keep_values)) {
    write-output $IdentityReference
    write-output $value
    # note argument order
    write-host -ForegroundColor 'Yellow' ( "c:\Windows\System32\icacls.exe ${filepath} /remove:g $value")
    invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /remove:g $value"  
    # NOTE: the command does not return error but nothing changes
    $acl.PurgeAccessRules($IdentityReference)

  }
  if ( ($value -in $keep_values)) {
    write-output $IdentityReference
    write-output $value
    # note {} around value. without those interpolates to nothing
    write-host -ForegroundColor 'Blue' ("c:\Windows\System32\icacls.exe ${filepath} /grant *${value}:F")
    invoke-expression -command "c:\Windows\System32\icacls.exe ${filepath} /grant *${value}:F"  
  }
}

write-host -ForegroundColor 'Green'  ('display the results')

get-acl -path $filepath | select-object -expandproperty Access



<# NOTE:
$acl.PurgeAccessRules($value)
Не удается преобразовать аргумент "identity", со значением: "S-1-5-32-544", для "PurgeAccessRules" в тип "System.Security.Principal.IdentityReference": "Не удается преобразовать значение "S-1-5-32-544" типа "System.String" в тип "System.Security.Principal.IdentityReference"."
#>


# https://learn.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.filesystemaccessrule.-ctor?view=netframework-4.5#system-security-accesscontrol-filesystemaccessrule-ctor(system-security-principal-identityreference-system-security-accesscontrol-filesystemrights-system-security-accesscontrol-inheritanceflags-system-security-accesscontrol-propagationflags-system-security-accesscontrol-accesscontroltype)
$rule1 = New-Object System.Security.AccessControl.FileSystemAccessRule('BUILTIN\Administrators', 'FullControl', 'ContainerInherit, ObjectInherit', 'None', 'Allow')
$rule2 = New-Object System.Security.AccessControl.FileSystemAccessRule('NT AUTHORITY\SYSTEM', 'FullControl', 'ContainerInherit, ObjectInherit', 'None', 'Allow')
$rule3 = New-Object  system.security.accesscontrol.filesystemaccessrule('CREATOR OWNER','FullControl','ContainerInherit, ObjectInherit','InheritOnly','Allow')
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.security/set-acl?view=powershell-5.1
# https://ss64.com/ps/set-acl.html
# $using:folder is probably simply a vairable name
$acl = Get-Acl $directory_path
# https://learn.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.objectsecurity.setaccessruleprotection?view=netframework-4.5
$acl.SetAccessRuleProtection($true,$false)
# protect the access rules associated with this  object from inheritance
# remove inherited access rules
#
# https://serverfault.com/questions/834046/remove-a-user-from-acl-completely-using-powershell
# https://www.codeproject.com/Questions/5310029/How-to-remove-special-NTFS-permission-from-folder
# not really answering the specific queston but suggests workaround
# replace access rule


$acl.SetAccessRule($rule2)
$acl.AddAccessRule($rule1)
$acl.RemoveAccessRuleAll($rule3)
$acl | Set-Acl $directory_path
# NOTE: occasionally see
# Set-Acl : Attempted to perform an unauthorized operation.

$value = 'S-1-5-32-544'; invoke-expression -command "c:\Windows\System32\icacls.exe ${directory_path} /grant *${value}:F"
$value = 'S-1-5-18'; invoke-expression -command "c:\Windows\System32\icacls.exe ${directory_path} /grant *${value}:F"

$acl | Set-Acl $directory_path 

# when not elevated:
# Set-Acl : The process does not possess the 'SeSecurityPrivilege' privilege which is required for this operation.

