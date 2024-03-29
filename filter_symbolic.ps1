#Copyright (c) 2024 Serguei Kouzmine
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
<# 
param (
) 
#>
# origin: https://stackoverflow.com/questions/817794/find-out-whether-a-file-is-a-symbolic-link-in-powershell
# see also:
# NOTE:  5.x get-childitem / Selected.System.IO.DirectoryInfo  has LinkType 

# with 4.x one has to examine Attributes
# get-childitem | where-object { $_.Attributes -match 'ReparsePoint' }
# see also: https://www.cyberforum.ru/powershell/thread3161229.html
# https://en.wikipedia.org/wiki/NTFS_reparse_point
function Test-ReparsePoint([string]$path) {
  $file = Get-Item $path -Force -ea SilentlyContinue
  return [bool]($file.Attributes -band [IO.FileAttributes]::ReparsePoint)
}

<#
 
dir 'c:\Temp' -recurse -force | ?{$_.LinkType} | select FullName,LinkType,Target
# NOTE: linktype needs 5.x
$x = dir 'c:\Temp' -recurse -force | select-object -property PSIsContainer,FullName,LinkType,Attributes 
$x.Count
443

$y = $x | where-object  {$_.PSIsContainer }
$y.Count
71
$y | foreach-object { write-output $_.Attributes  } 
# NOTE vague
$z = $y | Where-Object { $_.Attributes -match 'ReparsePoint' }

 $z |format-list

PSIsContainer : True
FullName      : C:\Temp\b
LinkType      :
Attributes    : Directory, ReparsePoint, NotContentIndexed


$z | foreach-object {  write-output ('"{0}" {1}' -f $_.FullName, $_.Attributes  ) }

"C:\Temp\b" Directory, ReparsePoint, NotContentIndexed

#>
<#
cd d:\temp

dir /x .
dir /x .  |findstr -i syml

08/03/2019  12:39 AM    <SYMLINKD>                  b [a]
08/25/2018  06:45 PM    <SYMLINK>      WEBDRI~1.LIN webdriver.link [webdriver]
#>
<#

PS C:\Users\Serguei> $host.version

Major  Minor  Build  Revision
-----  -----  -----  --------
4      0      -1     -1


# https://petri.com/how-to-check-your-powershell-version/
PS C:\Users\Serguei> $psversiontable

Name                           Value
----                           -----
PSVersion                      4.0
WSManStackVersion              3.0
SerializationVersion           1.1.0.1
CLRVersion                     4.0.30319.36399
BuildVersion                   6.3.9600.20719
PSCompatibleVersions           {1.0, 2.0, 3.0, 4.0}
PSRemotingProtocolVersion      2.2

#>

<#
Test-ReparsePoint -path 'C:\Temp\b'
True
#>
<#
$z2 = $y | Where-Object { $d = $_; Test-ReparsePoint -path $d }
$z2.Count
0
empty

# because of wrong argument type
$y | foreach-Object { $d = $_.FullName; $x = Test-ReparsePoint -path $d ; write-output ('{0} "{1}"' -f $x,$d)}
$z3 = $y | Where-Object { $d = $_.FullName; Test-ReparsePoint -path $d }
non-empty
$z3.Count
1


#>
