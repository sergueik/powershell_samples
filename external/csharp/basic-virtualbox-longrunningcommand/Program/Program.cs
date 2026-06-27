using System;
using System.Windows.Forms;

namespace SystemTrayApp {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (ProcessIcon pi = new ProcessIcon()) {
				pi.Display();

				Application.Run();
			}
		}
	}
}