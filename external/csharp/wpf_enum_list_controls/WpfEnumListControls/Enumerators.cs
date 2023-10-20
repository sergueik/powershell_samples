using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfEnumListControls
{
	public enum EnumTest1 { One=1, Two, Three, Four, Fifty=50, FiftyOne, FiftyTwo }
	[Flags]
	public enum EnumFlagTest { None=0, One=1, Two=2, Four=4, Eight=8, Sixteen=16, ThirtyTwo=32, All=63 }
}
