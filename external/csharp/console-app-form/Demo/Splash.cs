using System;
using System.Diagnostics;
using System.Threading;
using Program;

namespace Demo {
	class Splash {

		private static Timer _splashTimer = null;

		[STAThread]
		static void Main(string[] args) {
			ConsoleForm splash = ConsoleForm.GetFormInstance(@".\Forms\Splash.xml");
			splash.Render();
			_splashTimer = new Timer(new TimerCallback(_splashTimer_Elapsed), splash, 5000, 5000);
		}

		private static void _splashTimer_Elapsed(object o) {
			_splashTimer.Change(Timeout.Infinite, Timeout.Infinite);
			_splashTimer.Dispose();
			_splashTimer = null;

			// Get the splash screen from the timer state object so it can
			// be disposed.
			var f = (ConsoleForm)o;
			((IDisposable)f).Dispose(); // Kill the keypress loop.
			f = null;
		}

	}
}