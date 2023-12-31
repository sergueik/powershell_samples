# https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registrykey
# https://www.codeproject.com/Articles/2044/A-C-class-that-deals-with-System-Registry
add-type @'
using System;
using Microsoft.Win32;


namespace RegClassTest
{
	/// <summary>
	/// Summary description for clsRegistry.
	/// </summary>
	public class clsRegistry
	{
		public string strRegError; //this variable contains the error message (null when no error occured)


		public clsRegistry() //class constructor
		{
		}

		/// <summary>
		/// Retrieves the specified String value. Returns a System.String object
		/// </summary>
		public string GetStringValue (RegistryKey hiveKey, string strSubKey, string strValue)
		{
			object objData = null;
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.OpenSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot open the specified sub-key";
					return null;
				}
				objData = subKey.GetValue (strValue);
				if ( objData==null ) 
				{
					strRegError = "Cannot open the specified value";
					return null;
				}
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return null;
			}
			
			strRegError = null;
			return objData.ToString();
		}

		/// <summary>
		/// Retrieves the specified DWORD value. Returns a System.Int32 object
		/// </summary>
		public uint GetDWORDValue (RegistryKey hiveKey, string strSubKey, string dwValue)
		{
			object objData = null;
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.OpenSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot open the specified sub-key";
					return 0;
				}
				objData = subKey.GetValue (dwValue);
				if ( objData==null ) 
				{
					strRegError = "Cannot open the specified value";
					return 0;
				}
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return 0;
			}

			strRegError = null;
			return UInt32.Parse ( objData.ToString() );
		}

		/// <summary>
		/// Retrieves the specified Binary value. Returns a System.Byte[] object
		/// </summary>
		public byte[] GetBinaryValue (RegistryKey hiveKey, string strSubKey, string binValue)
		{
			object objData = null;
			RegistryKey subKey = null;
			Console.Error.WriteLine("0");			
			try
			{
				subKey = hiveKey.OpenSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot open the specified sub-key";
					return null;
				}
				objData = subKey.GetValue (binValue);
				if ( objData==null ) 
				{
					strRegError = "Cannot open the specified value";
					return null;
				}
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return null;
			}

			strRegError = null;
			return (byte[])objData;
		}



		/// <summary>
		/// Sets/creates the specified String value
		/// </summary>
		public void SetStringValue (RegistryKey hiveKey, string strSubKey, string strValue, string strData)
		{
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.CreateSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot create/open the specified sub-key";
					return;
				}
				subKey.SetValue (strValue, strData);
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}

		/// <summary>
		/// Sets/creates the specified DWORD value
		/// </summary>
		public void SetDWORDValue (RegistryKey hiveKey, string strSubKey, string strValue, int dwData)
		{
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.CreateSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot create/open the specified sub-key";
					return;
				}
				subKey.SetValue (strValue, dwData );
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}

		/// <summary>
		/// Sets/creates the specified Binary value
		/// </summary>
		public void SetBinaryValue (RegistryKey hiveKey, string strSubKey, string strValue, byte[] nnData)
		{
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.CreateSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot create/open the specified sub-key";
					return;
				}
				subKey.SetValue (strValue, nnData);
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}



		/// <summary>
		/// Creates a new subkey or opens an existing subkey
		/// </summary>
		public void CreateSubKey (RegistryKey hiveKey, string strSubKey)
		{
			RegistryKey subKey = null;
			
			try
			{
				subKey = hiveKey.CreateSubKey (strSubKey);
				if ( subKey==null ) 
				{
					strRegError = "Cannot create the specified sub-key";
					return;
				}
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}

		/// <summary>
		/// Deletes a subkey and any child subkeys recursively
		/// </summary>
		public void DeleteSubKeyTree (RegistryKey hiveKey, string strSubKey)
		{
			try
			{
				hiveKey.DeleteSubKeyTree (strSubKey);
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}

		/// <summary>
		/// Deletes the specified value from this (current) key
		/// </summary>
		public void DeleteValue (RegistryKey hiveKey, string strSubKey, string strValue)
		{
			RegistryKey subKey = null;
			try
			{
				subKey = hiveKey.OpenSubKey (strSubKey, true);
				if ( subKey==null )
				{
					strRegError = "Cannot open the specified sub-key";
					return;
				}
				subKey.DeleteValue (strValue);
				subKey.Close();
				hiveKey.Close();
			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return;
			}
			
			strRegError = null;
			return;
		}

		/// <summary>
		/// Retrieves the type of the specified Registry value
		/// </summary>
		public Type GetValueType (RegistryKey hiveKey, string strSubKey, string strValue)
		{
			RegistryKey subKey = null;
			object objData = null;

			try
			{
				subKey = hiveKey.OpenSubKey (strSubKey);
				if ( subKey==null )
				{
					strRegError = "Cannot open the specified sub-key";
					return null;
				}
				objData = subKey.GetValue (strValue);
				if (objData==null)
				{
					strRegError = "Cannot retrieve the type of the specified value";
					return null;
				}
				subKey.Close();
				hiveKey.Close();

			} 
			catch (Exception exc)
			{
				strRegError = exc.Message;
				return null;
			}
			
			strRegError = null;
			return objData.GetType();
		}


	}

}

'@

$o = new-object RegClassTest.clsRegistry
$res = $o.GetBinaryValue([Microsoft.Win32.RegistryKey]::LocalMachine,	'SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{8A2CE4D0-352E-43FE-8FB1-05140580EA96}', 'Actions')
write-output ('result: ' + $res)
<#

Add-Type -Language 'VisualBasic' -TypeDefinition @'

Public Class BarChart
  Const HKEY_CURRENT_USER = &H80000001
  Public Sub AllowManualEditBar(ByVal rowText As String) As String
    Set objRegistry = _
       GetObject("Winmgmts:root\default:StdRegProv")

    strPath = "Software\MyKey"
    uBinary = Array(1,2,3,4,5,6,7,8)
    Return = objRegistry.SetBinaryValue(HKEY_CURRENT_USER, _
       strPath, _
       "MyBinaryNamedValue", _
       uBinary)

    If (Return = 0) And (Err.Number = 0) Then
        Wscript.Echo "Binary value added successfully"
    Else
        ' An error occurred
    End If
  end Sub
End Class
'@

$caller = New-Object -TypeName 'BarChart'
#  -ArgumentList ([System.Diagnostics.Process]::GetCurrentProcess().MainWindowHandle)
#>