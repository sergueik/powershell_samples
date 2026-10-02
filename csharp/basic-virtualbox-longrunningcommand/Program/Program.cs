using System;
using System.Windows.Forms;

namespace Program {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var processIcon = new ProcessIcon()) {
				processIcon.Display();
				Application.Run();
			}
		}
	}
}