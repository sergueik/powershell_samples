using System;
using System.Windows.Forms;

namespace SystemTrayApp {
	static class Program {
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var processIcon = new ProcessIcon()) {
				processIcon.Visible = false;
				processIcon.Display();
				processIcon.DisplayBallonMessage(null, 3000);
				var context = new ApplicationContext();
				Application.Run(context);
			}
		}
	}
}