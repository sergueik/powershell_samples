using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTrayApp.Properties;
using System.Threading;
using System.Reflection;

namespace SystemTrayApp {
	// alternatiely may extend System.Windows.Forms.ApplicationContext
	// alternatibely may extend System.Windows.Forms.Form
	class ProcessIcon : IDisposable {
		NotifyIcon notifyIcon;
		// 'Timer' is an ambiguous reference between
		// 'System.Windows.Forms.Timer' and 'System.Threading.Timer'
		System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
		bool is_busy = false;
		int nScanCounter = 1;
		static bool exitFlag = false;
		private Icon idle_icon;
		private Icon busy_icon;

		public ProcessIcon() {
			notifyIcon = new NotifyIcon();
		}

		public void Display() {
			notifyIcon.MouseClick += new MouseEventHandler(ni_MouseClick);
			
			idle_icon = Resources.idle_icon;
			busy_icon = Resources.busy_icon;
			notifyIcon.Icon = idle_icon;
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblydescriptionattribute?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblytitleattribute?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.attribute.getcustomattribute?view=netframework-4.5
			string assemblyTitle = ((AssemblyTitleAttribute) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
			// System.ArgumentOutOfRangeException: Text length must be less than 64 characters long.
		
			notifyIcon.Text = (assemblyTitle.Length > 64) ? (assemblyTitle.Substring(0, 61) + "...") : assemblyTitle;
			notifyIcon.Visible = true;
			notifyIcon.ContextMenuStrip = new ContextMenus().Create();
			myTimer.Tick += new EventHandler(TimerEventProcessor);
			myTimer.Interval = 5000;
			myTimer.Start();
		}

		public void Dispose() {
			notifyIcon.Visible = false;
			notifyIcon.Dispose();
		}

		void ni_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				Process.Start("explorer", null);
			}
		}
	
	
		private void TimerEventProcessor(Object myObject,
			EventArgs myEventArgs) {
			myTimer.Stop();
			nScanCounter++;
			Console.Write("{0}\r", nScanCounter.ToString());
			is_busy = !is_busy;
			notifyIcon.Visible = false;
			if (is_busy)
				notifyIcon.Icon = busy_icon;
			else
				notifyIcon.Icon = idle_icon;
			notifyIcon.Visible = true;

			Thread.Sleep(1000);
			is_busy = !is_busy;
			notifyIcon.Visible = false;
			if (is_busy)
				notifyIcon.Icon = busy_icon;
			else
				notifyIcon.Icon = idle_icon;
			notifyIcon.Visible = true;
			// restart Timer.
			myTimer.Start();
		}
	}
}