using System;
using System.Text;
using System.Linq;
using System.Drawing;

namespace Utils {
	public class RtfWriter {
		private bool toggle;
		private bool Toggle {get; set;}
		public RtfWriter(bool toggle = true) {
			this.toggle = toggle;
		}
		private StringBuilder sb1 = new StringBuilder() ;
		private StringBuilder sb2 = new StringBuilder();
		public RtfWriter Append(string value) {
			sb1.Append(value);
			sb2.Append(value);
			return this;
		}
		public RtfWriter AppendLine(string value) {
			sb1.AppendLine(value);
			sb2.AppendLine(value);
			return this;
		}
		public override string ToString() {
			return toggle? sb2.ToString(): sb1.ToString();
		}
	}
}
