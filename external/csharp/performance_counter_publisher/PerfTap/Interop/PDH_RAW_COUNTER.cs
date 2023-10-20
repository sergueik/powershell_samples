using System;
using System.Runtime.InteropServices;

namespace PerfTap.Interop{

	[StructLayout(LayoutKind.Sequential)]
	public struct PDH_RAW_COUNTER {
		public uint CStatus;
		public System.Runtime.InteropServices.ComTypes.FILETIME TimeStamp;
		public long FirstValue;
		public long SecondValue;
		public uint MultiCount;
	}
}