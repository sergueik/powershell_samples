using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

// based on: https://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C
// NOTE: not uniform: several pinvoke difffer in return string argument
namespace Utils {
	public class IniFileNative {
		public string path;

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
		[DllImport("kernel32")]
		private static extern uint GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, uint size, string filePath);

		public IniFileNative(string path)
		{
			this.path = path;
		}

		public void WriteValue(string Section, string Key, string Value)
		{
			WritePrivateProfileString(Section, Key, Value, this.path);
		}

		// see also: https://www.pinvoke.net/default.aspx/kernel32.getprivateprofilestring
		public string ReadValue(string Section, string Key) {
			var sb = new StringBuilder(500);
			uint i = GetPrivateProfileString(Section, Key, "", sb, (uint)sb.Capacity, this.path);
			String result = sb.ToString();
			return result;
		}

		// https://www.pinvoke.net/default.aspx/kernel32.getprivateprofilesectionnames
		[DllImport("kernel32.dll")]
		static extern uint GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
			uint nSize, string lpFileName);

		// http://pinvoke.net/default.aspx/kernel32/GetPrivateProfileSection.html?diff=y
		[DllImport("kernel32.dll")]
		static extern uint GetPrivateProfileSection(string lpAppName, IntPtr lpszReturnBuffer,
			uint nSize, string lpFileName);

		public string[] SectionNames() {
			uint MAX_BUFFER = 32767;
			IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
			uint bytesReturned = GetPrivateProfileSectionNames(pReturnedString, MAX_BUFFER, this.path);
			if (bytesReturned == 0)
				return null;
			string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
			Marshal.FreeCoTaskMem(pReturnedString);
			//use of Substring below removes terminating null for split
			return local.Substring(0, local.Length - 1).Split('\0');
		}
		// based on https://stackoverflow.com/questions/7090053/read-all-ini-file-values-with-getprivateprofilestring
		public List<string> GetKeys(string category) {

			uint MAX_BUFFER = 32767;
			IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER);
			uint bytesReturned = GetPrivateProfileSection(category, pReturnedString, MAX_BUFFER, this.path);
			if (bytesReturned == 0)
				return null;

			string local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned).ToString();
			Marshal.FreeCoTaskMem(pReturnedString);
			//use of Substring below removes terminating null for split
			String[] tmp = local.Substring(0, local.Length - 1).Split('\0');
			List<string> result = new List<string>();

			foreach (String entry in tmp) {
				result.Add(entry.Substring(0, entry.IndexOf("=")));
			}
			return result;
		}
	}
}