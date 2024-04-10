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
<#

why "Documents and Settings" is the hard link
https://en.wikipedia.org/wiki/Inode

Windows NTFS has a master file table (MFT) storing files in a B-tree. 
Each entry has a "fileID", analogous to the inode number, that uniquely refers to this entry.[25] The three timestamps, a device ID, attributes, reference count, and file sizes are found in the entry, but unlike in POSIX the permissions are expressed through a different API.[26] The on-disk layout is more complex.[27] The earlier FAT file systems did not have such a table and were incapable of making hard links.

http://en.wikipedia.org/wiki/NTFS_junction_point


http://en.wikipedia.org/wiki/Symbolic_link

https://www.2brightsparks.com/resources/articles/NTFS-Hard-Links-Junctions-and-Symbolic-Links.pdf

* Junctions

Sometimes referred to as soft links, the function of a junction is to reference a target directory, unlike a
hard link which points to a file. Junctions can be created to link directories located on different partitions
or volume, but only locally on the same computer. It does this through the implementation of the NTFS
feature called reparse points. Redirected targets in junctions are defined by an absolute path. An
absolute path refers to a path which will contain the root element and the complete directory list that is
required to locate the target. For example, \Main\Folder\report is an absolute path. All of the
information required to locate the target is contained in the path string.

Like hard links, directory junctions do not take up additional space even though they are stored on the
drive partition; their function is to point to the original files in the original directory. Thus, it should be
noted that if the target is deleted, moved or renamed, all junctions which point to the target will break
and continue to point to a non-existing directory. Content changes from any of the junction links or the
target will automatically propagate to the rest.

Junctions are only compatible with Windows 2000 or later. An example in which junctions are often
used is on Windows Vista, where the name `C:\Documents and Settings` is a junction that points to
`C:\Users`. Thus, older programs that reference hard-coded legacy file paths can continue to work in Vista.

* Symbolic Links

Symbolic links were recently introduced in Windows Vista/Windows Server 2008 or later. An NTFS
symbolic link is a file system object that points to another file system object. In simpler terms, it is a
more advanced type of shortcut. Symbolic links can point to any file or folder either on the local
computer or using a SMB path to point at targets over a network (the target machine on the remote end
needs to run Windows Vista or later). They do not use any disk space.

A symbolic link could use either a relative path or an absolute path to point to its target. A relative path
has to be combined with another path in order to properly access the target file. For a detailed
explanation between the difference of absolute and relative paths, please refer to this link:

http://msdn.microsoft.com/en-us/library/aa363878(v=vs.85).aspx

* Note

When a hard link/junction/symbolic link is deleted using
Windows Explorer The original file and other hard links pointing to it remains.
If all associated links are removed, the data is deleted
Windows Vista or later: target is unchanged.
Windows 2000, XP & 2003: target & subfolders are deleted

#>

<#
in addition to
Directory Junction being an older type of symbolic link, 
(e.g. it does not support UNC paths (network paths that begin with \\) and relative paths



the practical difference is this:

creation of Directory Junction does not require admin privileges:
mklink /j  b a
Junction created for b <<===>> a

del b
The directory name is invalid.

rd b

creation of File or Directory symbolic link require elevation:
mklink /d  b a
you do not have sufficient privilege to perform this operation
see also:
https://superuser.com/questions/343074/directory-junction-vs-directory-symbolic-link
#>