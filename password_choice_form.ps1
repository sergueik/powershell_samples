
Add-Type -TypeDefinition @"
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace  Custom {
public static class CredentialsHelper {
    public static Credential ReadCredential(string applicationName) {
			IntPtr nCredPtr;
			bool read = CredRead(applicationName, CredentialType.Generic, 0, out nCredPtr);
			if (read) {
				using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr)) {
					CREDENTIAL cred = critCred.GetCredential();
					return ReadCredential(cred);
				}
			}

			return null;
		}

		private static Credential ReadCredential(CREDENTIAL credential) {
			string applicationName = Marshal.PtrToStringUni(credential.TargetName);
			string userName = Marshal.PtrToStringUni(credential.UserName);
			string secret = null;
			if (credential.CredentialBlob != IntPtr.Zero) {
				secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);
			}

			return new Credential(credential.Type, applicationName, userName, secret);
		}

		public static void WriteCredential(string applicationName, string userName, string secret) {
			byte[] byteArray = secret == null ? null : Encoding.Unicode.GetBytes(secret);
			// XP and Vista: 512;
			// 7 and above: 5*512
			if (Environment.OSVersion.Version < new Version(6, 1) /* Windows 7 */) {
				if (byteArray != null && byteArray.Length > 512)
					throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 512 bytes.");
			} else {
				if (byteArray != null && byteArray.Length > 512 * 5)
					throw new ArgumentOutOfRangeException("secret", "The secret message has exceeded 2560 bytes.");
			}

			CREDENTIAL credential = new CREDENTIAL();
			credential.AttributeCount = 0;
			credential.Attributes = IntPtr.Zero;
			credential.Comment = IntPtr.Zero;
			credential.TargetAlias = IntPtr.Zero;
			credential.Type = CredentialType.Generic;
			credential.Persist = (uint)CredentialPersistence.LocalMachine;
			credential.CredentialBlobSize = (uint)(byteArray == null ? 0 : byteArray.Length);
			credential.TargetName = Marshal.StringToCoTaskMemUni(applicationName);
			credential.CredentialBlob = Marshal.StringToCoTaskMemUni(secret);
			credential.UserName = Marshal.StringToCoTaskMemUni(userName ?? Environment.UserName);

			bool written = CredWrite(ref credential, 0);
			Marshal.FreeCoTaskMem(credential.TargetName);
			Marshal.FreeCoTaskMem(credential.CredentialBlob);
			Marshal.FreeCoTaskMem(credential.UserName);

			if (!written) {
				int lastError = Marshal.GetLastWin32Error();
				throw new Exception(string.Format("CredWrite failed with the error code {0}.", lastError));
			}
		}

		public static IReadOnlyList<Credential> EnumerateCrendentials() {
			List<Credential> result = new List<Credential>();

			int count;
			IntPtr pCredentials;
			bool ret = CredEnumerate(null, 0, out count, out pCredentials);
			if (ret) {
				for (int n = 0; n < count; n++) {
					IntPtr credential = Marshal.ReadIntPtr(pCredentials, n * Marshal.SizeOf(typeof(IntPtr)));
					result.Add(ReadCredential((CREDENTIAL)Marshal.PtrToStructure(credential, typeof(CREDENTIAL))));
				}
			} else {
				int lastError = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastError);
			}

			return result;
		}

		[DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

		[DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
		static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] UInt32 flags);

		[DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern bool CredEnumerate(string filter, int flag, out int count, out IntPtr pCredentials);

		[DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
		static extern bool CredFree([In] IntPtr cred);

		private enum CredentialPersistence : uint {
			Session = 1,
			LocalMachine,
			Enterprise
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CREDENTIAL {
			public uint Flags;
			public CredentialType Type;
			public IntPtr TargetName;
			public IntPtr Comment;
			public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
			public uint CredentialBlobSize;
			public IntPtr CredentialBlob;
			public uint Persist;
			public uint AttributeCount;
			public IntPtr Attributes;
			public IntPtr TargetAlias;
			public IntPtr UserName;
		}

		sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid {
			public CriticalCredentialHandle(IntPtr preexistingHandle) {
				SetHandle(preexistingHandle);
			}

			public CREDENTIAL GetCredential() {
				if (!IsInvalid) {
					CREDENTIAL credential = (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL));
					return credential;
				}

				throw new InvalidOperationException("Invalid CriticalHandle!");
			}

			protected override bool ReleaseHandle() {
				if (!IsInvalid) {
					CredFree(handle);
					SetHandleAsInvalid();
					return true;
				}

				return false;
			}
		}
	}

	public enum CredentialType {
		Generic = 1,
		DomainPassword,
		DomainCertificate,
		DomainVisiblePassword,
		GenericCertificate,
		DomainExtended,
		Maximum,
		MaximumEx = Maximum + 1000,
	}

	public class Credential {
		private readonly string _applicationName;
		private readonly string _userName;
		private readonly string _password;
		private readonly CredentialType _credentialType;

		public CredentialType CredentialType {
			get { return _credentialType; }
		}

		public string ApplicationName {
			get { return _applicationName; }
		}

		public string UserName {
			get { return _userName; }
		}

		public string Password {
			get { return _password; }
		}

		public Credential(CredentialType credentialType, string applicationName, string userName, string password) {
			_applicationName = applicationName;
			_userName = userName;
			_password = password;
			_credentialType = credentialType;
		}

		public override string ToString() {
			return string.Format("CredentialType: {0}, ApplicationName: {1}, UserName: {2}, Password: {3}", CredentialType, ApplicationName, UserName, Password);
		}
	}
}


"@ -ReferencedAssemblies 'System.dll'

function promptForCredentalAuto ($title,$message) {


  [void][System.Reflection.Assembly]::LoadWithPartialName('System.Windows.Forms')
  [void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')

  $f = New-Object System.Windows.Forms.Form

  $f.Text = $title

  $f.Size = New-Object System.Drawing.Size (640,430)

  $l1 = New-Object System.Windows.Forms.label
  $l1.Location = New-Object System.Drawing.Size (30,30)
  $l1.Size = New-Object System.Drawing.Size (400,20)
  $l1.Text = 'password'
  $f.Controls.Add($l1)

  $t1 = New-Object System.Windows.Forms.Textbox
  $t1.Location = New-Object System.Drawing.Size (30,50)
  $t1.Size = New-Object System.Drawing.Size (280,20)
  $t1.Text = ''
  $t1.Name = 't1'
  $t1.PasswordChar = '*'
  $f.Controls.Add($t1)

  $l2 = New-Object System.Windows.Forms.label
  $l2.Location = New-Object System.Drawing.Size (93,92)
  $l2.Size = New-Object System.Drawing.Size (300,20)
  $l2.Text = 'Credential Manager'
  $f.Controls.Add($l2)

  $c = New-Object System.Windows.Forms.checkbox
  $c.Location = New-Object System.Drawing.Size (75,90)
  $c.Size = New-Object System.Drawing.Size (20,20)
  $f.Controls.Add($c)

  $t2 = New-Object System.Windows.Forms.Textbox
  $t2.Location = New-Object System.Drawing.Size (93,92)
  $t2.Name = 't2'
  $t2.Size = New-Object System.Drawing.Size (280,20)
  $t2.Text = ''

  $t2.Visible = $false
  $f.Controls.Add($t2)
  $t2.Add_Leave({
    if ($t2.Text -ne $null) {
      $password =  $t1.text
      $t1.text = ''
      $data = [Custom.CredentialsHelper]::ReadCredential($t2.Text)
      if ($data.Password -ne $null) {
        $password = $data.Password
        $t1.text = $password
        $t2.Enabled = $false
        $password = $data.Password -replace '.', '?'
        write-host ("Type: {0}" -f $data.CredentialType)
        write-host ("ApplicationName: {0}" -f $data.ApplicationName)
        write-host ("UserName: {0}" -f $data.UserName)
        write-host ("Password (masked): {0}" -f $password )
        $password = ''
      } else {
        $t1.text = $password
        $password= ''
        $t2.Enabled = $true
      }
    }
  })

  # The System.Windows.Forms.Label does not receive click events so the below code will have no effect
  # https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.label.tabstopchanged?view=netframework-4.0
  $l2.add_click({
    # https://stackoverflow.com/questions/1764198/programmatically-click-on-a-checkbox
    $checkbox_click
  })

  $checkbox_click = {
    if ($c.Checked -eq $true) {
      $t1.Enabled = $false
      $t2.Visible = $true
      $l2.Visible = $false
    } else {
      $t1.Enabled = $true;
      $t2.Visible = $false
      $l2.Visible = $true
    }
  }

  $c.add_click($checkbox_click)


  $l3 = New-Object System.Windows.Forms.label
  $l3.Location = New-Object System.Drawing.Size (93,130)
  $l3.Size = New-Object System.Drawing.Size (300,20)
  $l3.Text = 'Some other input to Enter data'
  $f.Controls.Add($l3)

  $t3 = New-Object System.Windows.Forms.Textbox
  $t3.Location = New-Object System.Drawing.Size (30,150)
  $t3.Size = New-Object System.Drawing.Size (280,20)
  $t3.Text = ''
  $f.Controls.Add($t3)


  $f.StartPosition = 'CenterScreen'

  $b1 = New-Object System.Windows.Forms.Button
  $b1.Location = New-Object System.Drawing.Size (75,350)
  $b1.Size = New-Object System.Drawing.Size (75,23)
  $b1.Text = 'OK'

  $b1.add_click({
    $f.close()
  })

  $f.Controls.Add($b1)

  $b2 = New-Object System.Windows.Forms.Button
  $b2.Location = New-Object System.Drawing.Size (260,350)
  $b2.Size = New-Object System.Drawing.Size (75,23)
  $b2.Text = 'Cancel'
  $b2.add_click({
   $f.close()
  <#
    stop-Process -processname powershell
    #>
  })

  $f.Controls.Add($b2)

  $f.Topmost = $True
  $f.Add_Shown({ $f.Activate() })
  [void]$f.ShowDialog()
  $password = $t1.Text -replace '.', '?'

  write-host ('Password (masked): {0}' -f $password )
  # https://stackoverflow.com/questions/4483912/find-a-control-in-windows-forms-by-name
  [System.Windows.Forms.Textbox]$t = $f.Controls.Find($t1.Name, $true)[0]
  $password = $t.Text -replace '.', '?'
  write-host ('Password (masked): {0}' -f $password )
  # write-host ('Password (masked): {0}' -f $t1.Controls.Items.indexOf($t1.Name).text )

  switch ($result)
  {
    0 {
        write-host ("Password (masked): {0}" -f $password )
return $false
 }
    1 {
return $true
}
    2 { Write-Host `n"Process Halted At Step: " $title`n
      break }
  }

}
promptForCredentalAuto ('test','')

