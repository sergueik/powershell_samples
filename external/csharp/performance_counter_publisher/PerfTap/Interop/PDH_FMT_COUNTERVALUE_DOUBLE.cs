using System;
using System.Runtime.InteropServices;

namespace PerfTap.Interop {
	[StructLayout(LayoutKind.Sequential)]
	public struct PDH_FMT_COUNTERVALUE_DOUBLE {
		public uint CStatus;
		public double doubleValue;
	}
}