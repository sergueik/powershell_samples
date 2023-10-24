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
# based on:  https://habr.com/ru/articles/332068/
# NOTE: there appears to be a typo in the snippet -  
# the command is repeated twice:
# New-Object System.Security.AccessControl.FileSystemAccessRule("BUILTIN\Администраторы","ReadAndExecute","Allow")
# see also:
# https://www.bleepingcomputer.com/news/security/vaccine-not-killswitch-found-for-petya-notpetya-ransomware-outbreak/
# https://www.netwrix.com/how_to_get_acl_for_a_folder.html

param(
  [string]$filepath = (resolve-path -path 'Program\bin\Debug\LoadAverageService.exe').path
)


# based on: https://stackoverflow.com/questions/22331668/how-can-i-convert-a-sid-to-an-account-name-in-powershell
function translate_sid {
  param(
   [String]$value
  )
  if ($value -match 'S\-[0-9]+\-[0-9]+\-[0-9]+') {
    # check that the input is valid
  }
  $o = new-object -TypeName System.Security.Principal.SecurityIdentifier -ArgumentList $value;
  $name = $o.Translate([System.Security.Principal.NTAccount]).Value
  return $name
}

### main
$acl = Get-acl $filepath
# https://learn.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.objectsecurity.setaccessruleprotection?view=netframework-4.5

$acl.SetAccessRuleProtection($true,$true)

# https://learn.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.filesystemaccessrule.-ctor?view=netframework-4.5
$s = (translate_sid -value 'S-1-5-18') 
$accrule1 = new-object System.Security.AccessControl.FileSystemAccessRule($s,'FullControl','Deny')
$accrule3 = new-object System.Security.AccessControl.FileSystemAccessRule($s ,'ReadAndExecute','Allow')

$a = (translate_sid -value 'S-1-5-32-544')
$accrule2 = new-object System.Security.AccessControl.FileSystemAccessRule($a ,'FullControl','Deny')
$accrule4 = new-object System.Security.AccessControl.FileSystemAccessRule($a ,'ReadAndExecute','Allow')

# https://learn.microsoft.com/en-us/dotnet/api/system.security.accesscontrol.filesystemsecurity.setaccessrule?view=netframework-4.5

$acl.SetAccessRule($accrule1)
$acl.SetAccessRule($accrule2)
$acl.SetAccessRule($accrule3)
$acl.SetAccessRule($accrule4)
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.security/set-acl?view=powershell-5.1
Set-Acl -AclObject $acl -Path $filepath -erroraction SilentlyContinue



