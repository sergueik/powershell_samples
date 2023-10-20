param(
  [string] $shortcut = 'calc.exe',
  [switch]$debug
)
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent

<#
if (-not ($shortcut -match '.*.lnk')) {
  $shortcut = $shortcut + '.lnk'
}
#>
$target = "${env:userprofile}\Desktop\${shortcut}.lnk"

if ( -not (test-path $target ) ) {
  write-error ('Not a valid path: "{0}"' -f $target )
  return
}

Add-Type -TypeDefinition @'
using System;
using System.Text;

using System.Runtime.InteropServices;

public class Helper {
  [DllImport("user32.dll")]
  public static extern int LoadString(IntPtr h, uint id, System.Text.StringBuilder sb, int maxBuffer);
  [DllImport("kernel32.dll")]
  public static extern IntPtr LoadLibrary(string s);
  public Helper() { }
}
'@
# https://docs.microsoft.com/en-us/windows/win32/shell/fa-verbs

# NOTE: the "PintoTaskbar" verb, may not be available for the desktop shortcut that is already pinned to task bar
$verbs = @{
  'PintoStartMenu' = 5381;
  'UnpinfromStartMenu' = 5382;
  'PintoTaskbar' = 5386;
  'UnpinfromTaskbar' = 5387;
}
$verbid = $verbs['PintoTaskbar']
$shell32_helper = [helper]::LoadLibrary('shell32.dll')
$length = 255
$data = new-object Text.StringBuilder '',$length 
[void][helper]::LoadString($shell32_helper, $verbId, $data, $length)
$verb = $data.ToString()
write-output ('Looking if "{0}" is available' -f $verb)


$o = new-object -ComObject 'Shell.Application'
# https://docs.microsoft.com/en-us/windows/win32/shell/shell-namespace
# https://docs.microsoft.com/en-us/windows/win32/api/shldisp/ne-shldisp-shellspecialfolderconstants
<#
ssfDESKTOP
0x00 (0). Windows desktop—the virtual folder that is the root of the namespace.
ssfBITBUCKET
0x0a (10). Virtual folder that contains the objects in the user's Recycle Bin.
ssfDRIVES
0x11 (17). My Computer—the virtual folder that contains everything on the local computer: storage devices, printers, and Control Panel. This folder can also contain mapped network drives.
#>
$d = $o.Namespace(0x0)
# https://docs.microsoft.com/en-us/windows/win32/shell/folder-parsename
$l = $d.ParseName($target)
# https://docs.microsoft.com/en-us/windows/win32/shell/folderitem-verbs
if ($debug_flag){
  Write-Output 'Verbs:'
  $l.Verbs() | select-object -expandproperty 'Name'
}
# https://msdn.microsoft.com/en-us/library/windows/desktop/bb787850%28v=vs.85%29.aspx
$v = $l.Verbs() | where-object { $_.Name -eq $verb }

if ($v -eq $null) {
  # on Windows 10, with some settings, there may be no 'Pin to Taskbar' Verb shown (anot no such action in context menu).
  write-error ( 'Verb {0} not found for the shortcut {1}. It can be already pinned' -f $verb, $target )
  return
} else {
  $v | select-object -expandproperty 'Name' | write-host -foreground 'yellow'
  # https://msdn.microsoft.com/en-us/library/windows/desktop/bb774170%28v=vs.85%29.aspx
  # https://docs.microsoft.com/en-us/windows/win32/shell/folderitemverb-doit
  $v.DoIt()
}
write-output 'Checking the result in the file system'

get-childitem -path "${env:appdata}\Microsoft\Internet Explorer\Quick Launch\User Pinned\Taskbar" -name "${shortcut}.lnk" | select-object -property *

write-output 'Checking the result in the registry'

$reg_value = (get-itemproperty -path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband' -name 'Favorites').'Favorites'
# note also: 'FavoritesResolve'

$data = [System.Text.Encoding]::Unicode.GetString($reg_value)
if ($data -match ('.%{0}' -f $shortcut)) {
  write-output ('Registry entry: "Favorites": has the string {0}' -f $shortcut)
}
if ($debug_flag){
  write-output 'Registry entry: "Favorites":'
  $data
  # will contain "${shortcut}.lnk"
  # The format is MS proprietary
  # for IE, the link is  'Internet Explorer.lnk' @"C:\Windows\System32\ie4uinit.exe,-731"
}
return
# see also:
# http://forum.oszone.net/thread-349964.html
# e.g. 'All Task' GUID
# explorer.exe shell:::{ED7BA470-8E54-465E-825C-99712043E01C}

# misc. related links
# https://msdn.microsoft.com/en-us/library/windows/desktop/bb787850%28v=vs.85%29.aspx
# "%appdata%\Microsoft\Internet Explorer\Quick Launch\User Pinned\Taskbar"
# http://www.msfn.org/board/topic/154143-command-to-pin-or-unpin-program-from-taskbar/
# http://blogs.technet.com/b/deploymentguys/archive/2009/04/08/pin-items-to-the-start-menu-or-windows-7-taskbar-via-script.aspx
# https://gallery.technet.microsoft.com/scriptcenter/b66434f1-4b3f-4a94-8dc3-e406eb30b750
# http://powershell.com/cs/media/p/15280.aspx
# https://msdn.microsoft.com/en-us/library/windows/desktop/bb787850%28v=vs.85%29.aspx
# http://winaero.com/comment.php?comment.news.108
# https://4sysops.com/archives/configure-pinned-programs-on-the-windows-taskbar-with-group-policy/
