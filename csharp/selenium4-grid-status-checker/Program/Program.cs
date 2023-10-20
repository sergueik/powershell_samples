using System;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using Utils;

namespace SeleniumClient {
	public static class Program {
		[STAThread]
		public static void Main()
		{
			Boolean DEBUG = (Environment.GetEnvironmentVariable("DEBUG") != null);

			// NOTE: parseArgs is unused
			// var parseArgs = new ParseArgs(Environment.CommandLine);

			// use GDI
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();
			// https://stackoverflow.com/questions/3127288/how-can-i-retrieve-the-assemblycompany-setting-in-assemblyinfo-cs
			// http://www.java2s.com/Code/CSharp/Reflection/AssemblyTitle.htm
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblytitleattribute?view=netframework-4.5
			// see also:
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblyname.getassemblyname?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly?view=netframework-4.5
			string assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute( Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
			if (SingleInstanceController.IsSecondInstance(assemblyTitle)) {
				return;
			}

			using (var processIcon = new ProcessIcon()) {
				processIcon.Visible = false;
				var context = new ApplicationContext();
				processIcon.Display();
				// show balloon help
				processIcon.DisplayBallonMessage(null, 1000);
				Application.Run(context);
			}
		}
	}

}
