using System;

namespace Utils {
	
	public class ConsoleGrep : Grep {
		private bool verbose;
		public bool Verbose {
			get { return verbose; }
			set { verbose = value; }
		}

		protected override  void statusMsg(String message) {
			Console.WriteLine(message);
		}
		protected override void errorMsg(String message) {
			Console.Error.WriteLine(message);
		}
		protected override  void progressMsg(String message) {
			
		}
	}

}
