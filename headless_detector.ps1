#Copyright (c) 2022 Serguei Kouzmine
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

param(
  [bool]$gui_status = $false,
  [switch] $with_elevation,
  [switch] $debug
)

function check_elevation {

  param(
    [string]$message,
    [bool]$debug
  )

  $myWindowsID = [System.Security.Principal.WindowsIdentity]::GetCurrent()
  $myWindowsPrincipal = new-object System.Security.Principal.WindowsPrincipal($myWindowsID)

  $adminRole=[System.Security.Principal.WindowsBuiltInRole]::Administrator
  if ($debug ){
    Write-Host -NoNewLine 'Press any key to continue (checking elevation)...'
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
  }

  # Check to see if we are currently NOT running "as Administrator"
  if ( -not $myWindowsPrincipal.IsInRole($adminRole) ) {
    write-host -foreground 'Red' ('The {0} needs to run in elevated prompt' -f $message)
    exit
  }
}

# see also:
# https://community.idera.com/database-tools/powershell/ask_the_experts/f/powershell_for_windows-12/19845/get-process-user-filtering
function gui_check_wmi {

  param(
    [bool]$debug = $false
  )
  # NOTE: without initializing variable with the value,
  # Powershell will print it initial value
  # (the value will be *False*) to he output
  # [boolean]$status
  [boolean]$status = $false
  $processname = 'explorer'
  $cmdlet_output = Get-Process -Name $processname -IncludeUserName -errorAction SilentlyContinue |where-object { $_.Username -eq "${env:COMPUTERNAME}\${env:USERNAME}" }
  $process_count = $cmdlet_output.Count

  if (($process_count -ne $null) -and ( $process_count -ne 0) ){
    $status = $true
  } else {
    $status = $false
  }
  # NOTE: silent return does not apply

  if ($debug){
    return @($cmdlet_output,
             ('process_count = {0}' -f $process_count),
             ('status = {0}' -f $status))
  } else {
    return $status
  }
}

# see also:
# https://community.idera.com/database-tools/powershell/ask_the_experts/f/powershell_for_windows-12/19845/get-process-user-filtering
function gui_check {
  param(
    [bool]$debug = $false
  )
  # NOTE: without initializing variable with the value,
  # Powershell will print it initial value
  # (the value will be *False*) to he output
  # [boolean]$status
  [boolean]$status = $false
  # cannot distinguish ?
  # $process_count = (get-process -name explorer  -IncludeUserName | Where-object {$_.username -eq $env:USERNAME  }).count
  # NOTE: the -IncludeUserName option of get-process cmdlet requires elevation
  # check if explorer.exe is running for the current user
  $tasklist_output = &tasklist.exe '/FI', "USERNAME eq $env:USERNAME",'/FI', 'IMAGENAME EQ explorer.exe'
  $process_count = ($tasklist_output | select-string 'explorer.exe' ).count
  if (($process_count -ne $null) -and ( $process_count -ne 0) ){
    $status = $true
  } else {
    $status = $false
  }
  # NOTE: silent return does not apply

  if ($debug){
    return @($tasklist_output,
             ('process_count = {0}' -f $process_count),
             ('status = {0}' -f $status))
  } else {
    return $status
  }
}

# origin: https://www.codeproject.com/Articles/14828/How-To-Get-Process-Owner-ID-and-Current-User-SID
# win32 code
Add-Type -ReferencedAssemblies @(
  'System.dll',
  'System.Drawing.dll',
  'System.Windows.Forms.dll'
) -typedefinition @'

using System.Text;
using Microsoft.Win32.SafeHandles;

using System;
using System.Net;
using System.Collections;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security.Principal;


public class ProcessHelper {

	public const int TOKEN_QUERY = 0X00000008;
	const int ERROR_NO_MORE_ITEMS = 259;

	enum TOKEN_INFORMATION_CLASS {
		TokenUser = 1,
		TokenGroups,
		TokenPrivileges,
		TokenOwner,
		TokenPrimaryGroup,
		TokenDefaultDacl,
		TokenSource,
		TokenType,
		TokenImpersonationLevel,
		TokenStatistics,
		TokenRestrictedSids,
		TokenSessionId
	}

	[StructLayout(LayoutKind.Sequential)]
	struct TOKEN_USER {
		public _SID_AND_ATTRIBUTES User;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct _SID_AND_ATTRIBUTES {
		public IntPtr Sid;
		public int Attributes;
	}


	[DllImport("advapi32")]
	static extern bool OpenProcessToken(IntPtr  ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

	[DllImport("kernel32")]
	static extern IntPtr GetCurrentProcess();

	[DllImport("advapi32", CharSet = CharSet.Auto)]
	static extern bool GetTokenInformation(IntPtr  hToken, TOKEN_INFORMATION_CLASS tokenInfoClass, IntPtr TokenInformation, int tokeInfoLength, ref int reqLength);

	[DllImport("kernel32")]
	static extern bool CloseHandle(IntPtr handle);

	[DllImport("advapi32", CharSet = CharSet.Auto)]
	static extern bool ConvertSidToStringSid(IntPtr pSID, [In, Out, MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

	[DllImport("advapi32", CharSet = CharSet.Auto)]
	static extern bool ConvertStringSidToSid([In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid, ref IntPtr pSID);

	public static bool DumpUserInfo(IntPtr pToken, out IntPtr SID) {
		int Access = TOKEN_QUERY;
		IntPtr procToken = IntPtr.Zero;
		bool ret = false;
		SID = IntPtr.Zero;
		try {
			if (OpenProcessToken(pToken, Access, ref procToken)) {
				ret = ProcessTokenToSid(procToken, out SID);
				CloseHandle(procToken);
			}
			return ret;
		} catch {
			// NOTE: Add-Type: Warning as Error: The variable 'err' is declared but never used
			return false;
		}
	}

	private static bool ProcessTokenToSid(IntPtr token, out IntPtr SID) {
		TOKEN_USER tokUser;
		const int bufLength = 256;
		IntPtr tu = Marshal.AllocHGlobal(bufLength);
		bool ret = false;
		SID = IntPtr.Zero;
		try {
			int cb = bufLength;
			ret = GetTokenInformation(token, TOKEN_INFORMATION_CLASS.TokenUser, tu, cb, ref cb);
			if (ret) {
				tokUser = (TOKEN_USER)Marshal.PtrToStructure(tu, typeof(TOKEN_USER));
				SID = tokUser.User.Sid;
			}
			return ret;
		} catch {
			return false;
		} finally {
			Marshal.FreeHGlobal(tu);
		}
	}

	public static string ExGetProcessInfoByPID(int PID, out string SID) {
		IntPtr _SID = IntPtr.Zero;
		SID = String.Empty;
		try {
			Process process = Process.GetProcessById(PID);
			if (DumpUserInfo(process.Handle, out _SID)) {
				ConvertSidToStringSid(_SID, ref SID);
			}
			return process.ProcessName;
		} catch {
			return "Unknown";
		}
	}
}
'@

# http://support.microsoft.com/kb/243330
Add-Type -ReferencedAssemblies @(
  'System.dll',
  'System.Drawing.dll',
  'System.Windows.Forms.dll'
) -typedefinition @'
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

public class SidHelper {

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool LookupAccountSid([In,MarshalAs(UnmanagedType.LPTStr)] string systemName, IntPtr sid, [Out,MarshalAs(UnmanagedType.LPTStr)] StringBuilder name, ref int cbName, StringBuilder referencedDomainName, ref int cbReferencedDomainName, out int use);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool LookupAccountName([In,MarshalAs(UnmanagedType.LPTStr)] string systemName, [In,MarshalAs(UnmanagedType.LPTStr)] string accountName, IntPtr sid, ref int cbSid, StringBuilder referencedDomainName, ref int cbReferencedDomainName, out int use);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool ConvertSidToStringSid(IntPtr sid, [In,Out,MarshalAs(UnmanagedType.LPTStr)] ref string pStringSid);

	[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern bool ConvertStringSidToSid([In, MarshalAs(UnmanagedType.LPTStr)] string pStringSid, ref IntPtr sid);

	public static string GetSid(string name) {
		IntPtr _sid = IntPtr.Zero;	//pointer to binary form of SID string.
		int _sidLength = 0;	//size of SID buffer.
		int _domainLength = 0;//size of domain name buffer.
		int _use;	//type of object.
		StringBuilder _domain = new StringBuilder();	//stringBuilder for domain name.
		int _error = 0;
		string _sidString = "";

		LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
		_error = Marshal.GetLastWin32Error();

		if (_error != 122) {
			throw(new Exception(new Win32Exception(_error).Message));
		} else {
			_domain = new StringBuilder(_domainLength);
			_sid = Marshal.AllocHGlobal(_sidLength);
			bool _rc = LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
			if (_rc == false) {
				_error = Marshal.GetLastWin32Error();
				Marshal.FreeHGlobal(_sid);
				throw(new Exception(new Win32Exception(_error).Message));
			} else {
				_rc = ConvertSidToStringSid(_sid, ref _sidString);
				if (_rc == false) {
					_error = Marshal.GetLastWin32Error();
					Marshal.FreeHGlobal(_sid);
					throw(new Exception(new Win32Exception(_error).Message));
				} else {
					Marshal.FreeHGlobal(_sid);
					return _sidString;
				}
			}
		}

	}

	public static string GetName(string sid) {
		IntPtr _sid = IntPtr.Zero;
		int _nameLength = 0;
		int _domainLength = 0;
		int _use;
		StringBuilder _domain = new StringBuilder();
		int _error = 0;
		StringBuilder _name = new StringBuilder();
		bool _rc0 = ConvertStringSidToSid(sid, ref _sid);
		if (_rc0 == false) {
			_error = Marshal.GetLastWin32Error();
			Marshal.FreeHGlobal(_sid);
			throw(new Exception(new Win32Exception(_error).Message));
		}

		bool _rc = LookupAccountSid(null, _sid, _name, ref _nameLength, _domain, ref _domainLength, out _use);
		_domain = new StringBuilder(_domainLength);
		_name = new StringBuilder(_nameLength);
		_rc = LookupAccountSid(null, _sid, _name, ref _nameLength, _domain, ref _domainLength, out _use);
		if (_rc == false) {
			_error = Marshal.GetLastWin32Error();
			Marshal.FreeHGlobal(_sid);
			throw(new Exception(new Win32Exception(_error).Message));
		} else {
			Marshal.FreeHGlobal(_sid);
			return _domain.ToString() + "\\" + _name.ToString();
		}
	}
}
'@
write-output 'using p/invoke'
$process_id = get-process -name 'explorer' | select-object  -expandproperty id
# https://stackoverflow.com/questions/821744/how-to-call-a-method-with-output-parameters-in-powershell
[string]$data = $null
$current_user_sid = &C:\Windows\System32\whoami.exe "/user"

[ProcessHelper]::ExGetProcessInfoByPID($process_id, [ref]$data )
write-output $data

write-output ([SidHelper]::GetName($data))
$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
# the command line option overrides

if ( -not $gui_status ) {
  $gui_status = gui_check -debug $debug_flag
}

if ([bool]$PSBoundParameters['with_elevation'].IsPresent -bor $with_elevation.ToBool()) {
  check_elevation -debug $debug_flag -message 'running get-process with -IncludeUserName'
  $gui_status = gui_check_wmi -debug $debug_flag
  # NOTE: the following call would require elevation as well
  # to [System.Environment]::SetEnvironmentVariable('name','value',[System.EnvironmentVariableTarget]::Machine)
}

write-output $gui_status