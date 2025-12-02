using System;
using System.Linq;
using System.Drawing;

namespace Utils {
	public class RtfColorInfo {
		public Color color;
		public int colorTableNumber;

		public RtfColorInfo(Color color, int colorTableNumber) {
			this.color = color;
			this.colorTableNumber = colorTableNumber;
		}

		public string asFontColor() {
			return String.Format("\\cf{0} ", colorTableNumber);
		}

		public string asBackgroundColor()  {
			return String.Format("\\highlight{0} ", colorTableNumber);
		}
	}
}
