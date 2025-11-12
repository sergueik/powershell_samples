<#

New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My -Subject "CN=Local Script Signing"
Set-AuthenticodeSignature .\Decompile-Chm.ps1 -Certificate (Get-ChildItem Cert:\CurrentUser\My\ -CodeSigningCert)

#>
<#
.SYNOPSIS
    Silently decompiles a .CHM file using Windows HTML Help COM interfaces.

.DESCRIPTION
    This script lets you pick a .CHM file via GUI and extracts its contents
    to a chosen output folder using the "ITStorage" COM object.

.NOTES
    Author: Serguei Kouzmine
    Date: $(Get-Date -Format 'yyyy-MM-dd')

.LINK
    Code signing overview:
    https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_signing

    Signing scripts with your own certificate:
    https://learn.microsoft.com/en-us/powershell/scripting/security/secure-scripts#signing-scripts-with-your-own-certificate

    Execution Policy reference:
    https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies
#>

Add-Type -AssemblyName System.Windows.Forms

function Select-File {
    $dialog = New-Object System.Windows.Forms.OpenFileDialog
    $dialog.Filter = "Compiled HTML Help (*.chm)|*.chm"
    $dialog.Title = "Select CHM File"
    if ($dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
        return $dialog.FileName
    }
    return $null
}

function Select-Folder {
    $dialog = New-Object System.Windows.Forms.FolderBrowserDialog
    $dialog.Description = "Select output folder for decompiled content"
    if ($dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK) {
        return $dialog.SelectedPath
    }
    return $null
}

# Add COM interop to ITStorage from hhctrl.ocx
# NOTE: hhctrl.ocx as part of Windows Optional Features (the HTML Help subsystem).
# It’s often present in WinSxS (side-oby-side) component storage — not directly registered globally.
<#
c:\windows\System32\hhctrl.ocx
c:\windows\SysWOW64\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.5074_none_6449c421349cd574\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.5074_none_6449c421349cd574\r\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.6725_none_63f8fe5934da3120\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.6725_none_63f8fe5934da3120\r\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.7019_none_6443544934a2b7dd\hhctrl.ocx
c:\windows\WinSxS\amd64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.7019_none_6443544934a2b7dd\r\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.5074_none_6e9e6e7368fd976f\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.5074_none_6e9e6e7368fd976f\r\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.6725_none_6e4da8ab693af31b\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.6725_none_6e4da8ab693af31b\r\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.7019_none_6e97fe9b690379d8\hhctrl.ocx
c:\windows\WinSxS\wow64_microsoft-windows-htmlhelp_31bf3856ad364e35_10.0.26100.7019_none_6e97fe9b690379d8\r\hhctrl.ocx

#>
# but no COM registration in the registry for the HTML Help COM class (HTMLHelp.HTMLHelp / {52A2AAAE-085D-4187-97EA-8C30DB990436}).
# Effect:
<#
New-Object -ComObject "HTMLHelp.HTMLHelp" fails.

[DllImport("hhctrl.ocx")] also fails if the class factory is queried
#>
# Per-user COM registration is possible, but requires either:

# regsvr32 /n /i:user hhctrl.ocx (some OCXs allow per-user registration), or

# Writing the necessary CLSID/ProgID keys to HKCU\Software\Classes manually (rare and tricky).
# # Check if CLSID exists under HKCR
# $clsid = "{52A2AAAE-085D-4187-97EA-8C30DB990436}"
# $regKey = "HKCR:\CLSID\$clsid"

# This will enumerate all subkeys/values under that CLSID if present.

<#
regsvr32.exe c:\windows\system32\hhctrl.ocx
---------------------------
RegSvr32
---------------------------
DllRegisterServer in c:\windows\system32\hhctrl.ocx succeeded.
---------------------------
OK   
---------------------------
#>


Add-Type -TypeDefinition @"

using System;
using System.Runtime.InteropServices;

[ComImport, Guid("41B23C28-488E-4E5C-ACE2-BB0BBABE99E8")]
public class ITStorageClass { }

[ComImport, Guid("B8D1455C-CCD3-11D0-882D-00A0C903B83C"),
 InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IStorage
{
    void CreateStream(string pwcsName, int grfMode, int reserved1, int reserved2, out IntPtr ppstm);
    void OpenStream(string pwcsName, IntPtr reserved1, int grfMode, int reserved2, out IntPtr ppstm);
    void CreateStorage(string pwcsName, int grfMode, int reserved1, int reserved2, out IntPtr ppstg);
    void OpenStorage(string pwcsName, IntPtr pstgPriority, int grfMode, IntPtr snbExclude, int reserved, out IntPtr ppstg);
    void CopyTo(int ciidExclude, IntPtr rgiidExclude, IntPtr snbExclude, IntPtr pstgDest);
    void MoveElementTo(string pwcsName, IntPtr pstgDest, string pwcsNewName, int grfFlags);
    void Commit(int grfCommitFlags);
    void Revert();
    void EnumElements(int reserved1, IntPtr reserved2, int reserved3, out IntPtr ppenum);
    void DestroyElement(string pwcsName);
    void RenameElement(string pwcsOldName, string pwcsNewName);
    void SetElementTimes(string pwcsName, ref System.Runtime.InteropServices.ComTypes.FILETIME pctime, ref System.Runtime.InteropServices.ComTypes.FILETIME patime, ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
    void SetClass(ref Guid clsid);
    void SetStateBits(int grfStateBits, int grfMask);
    void Stat(out IntPtr pstatstg, int grfStatFlag);
}
"@

# Select CHM file and output directory
$chmFile = Select-File
if (-not $chmFile) { Write-Host "No CHM selected. Exiting."; exit }

$outDir = Select-Folder
if (-not $outDir) { Write-Host "No output folder selected. Exiting."; exit }

try {
    Write-Host "Decompiling '$chmFile' to '$outDir'..."
    $hh = New-Object -ComObject "HTMLFile"
    $result = $hh.TextPopup("","",0,0,0) # Dummy to ensure COM init
    $hh.TextPopup("Decompiling, please wait...","Arial,10",100,100,0)

    $hh.ExecWB(0,0,$chmFile,$outDir)
    Write-Host "✅ Done."
} catch {
    Write-Warning "Decompilation failed: $_"
}

