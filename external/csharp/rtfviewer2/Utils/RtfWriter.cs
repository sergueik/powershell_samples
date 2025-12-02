using System;
using System.Text;
using System.Linq;
using System.Drawing;

namespace Utils {
	public class RtfWriter {
		private StringBuilder sb1 = new StringBuilder() ;
		private StringBuilder sb2 = new StringBuilder();
		public StringBuilder AppendLine(string value) {
			sb1.AppendLine(value);
			sb2.AppendLine(value);
			return sb1;
		}
		public string ToString() {
			return sb1.ToString();
		}	
	}
}
