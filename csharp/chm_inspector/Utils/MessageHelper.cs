using System;
using System.Runtime.InteropServices;

namespace Utils {
	public static class MessageHelper {
		private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
		private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int FormatMessage( int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, out IntPtr lpBuffer, int nSize, IntPtr Arguments);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LocalFree(IntPtr hMem);
		public static string Msg(HRESULT hresult) { return Msg((int) hresult); }
		public static string Msg(int code) {
			IntPtr lpMsgBuf;
			int ret = FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, IntPtr.Zero, code, 0, out lpMsgBuf, 0, IntPtr.Zero);
			if (ret == 0)
				return String.Format("Unknown HRESULT 0x{0:X8}", code);
			string message = Marshal.PtrToStringAuto(lpMsgBuf);
			LocalFree(lpMsgBuf);
			return message.Trim();
		}
	}
}
