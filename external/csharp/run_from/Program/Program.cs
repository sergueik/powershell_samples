using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HowToRunPowerShell {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormPowerShellSample());
		}
	}
}