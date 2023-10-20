using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;

using Utils;

namespace Program {
	class Splash {

		private static Timer splashTimer = null;

		[STAThread]
		static void Main(string[] args) {			
			ConsoleForm splash = ConsoleForm.GetFormInstance(@".\\JSON\\splash.json");
			splash.start();
			splash.Render();
			splashTimer = new Timer(new TimerCallback(formClose), splash, 5000, 5000);
		}

		private static void formClose(object o) {
			splashTimer.Change(Timeout.Infinite, Timeout.Infinite);
			splashTimer.Dispose();
			splashTimer = null;

			// Get the splash screen from the timer state object so it can
			// be disposed.
			var f = (ConsoleForm)o;
			((IDisposable)f).Dispose(); // Kill the keypress loop.
			f = null;
		}
		
		public  static string getFileContents(string path) {

			string contents = null;
			if (path.IndexOf(".\\") == 0) {
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + path.Substring(1);
			}
			using (var streamReader = new StreamReader(path)) {
				contents = streamReader.ReadToEnd();
				// Console.WriteLine(contents);
				
				streamReader.Close();
			}
			return contents;
		}
	}
	

}
