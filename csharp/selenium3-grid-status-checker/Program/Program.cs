using System;
using System.Linq;
using System.Windows.Forms;
using Utils;

namespace SeleniumClient {
	public static class Program {
		// The constant 'SeleniumClient.Program.assemblyName' cannot be marked static (CS0504)
		//  If a variable is const, it is also static
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0504
		const String assemblyName = "selgridtray";
		[STAThread]
		public static void Main() {
			Boolean DEBUG = (Environment.GetEnvironmentVariable("DEBUG") != null);
			
			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();

			if (SingleInstanceController.IsSecondInstance(assemblyName)) {
				return;
			}

			using (var processIcon = new ProcessIcon()) {
				// NOTE: ProcessIcon is not a Form, has no Visble property
				// processIcon.Visible = false;
				var context = new ApplicationContext();
				// NOTE: does not show balloon help
				processIcon.DisplayBallonMessage(null, 10000);
				processIcon.Display();
				Application.Run(context);
			}
		}
	}

}
