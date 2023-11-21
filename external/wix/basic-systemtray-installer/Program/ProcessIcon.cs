using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTrayApp.Properties;
using System.IO;
using System.Threading;
using System.Reflection;

namespace SystemTrayApp {
	// alternatively may  
	// implement System.IDisposable
	// extend System.Windows.Forms.ApplicationContext
	public partial class ProcessIcon : Form {
		public ProcessIcon() {
			notifyIcon = new NotifyIcon();
		}

		public void Display() {
			notifyIcon.MouseClick += new MouseEventHandler(ni_MouseClick);
            notifyIcon.Icon = idle_icon;
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblydescriptionattribute?view=netframework-4.5
			string assemblyDescription = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute( Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false)).Description;
			notifyIcon.Text = assemblyDescription;

			notifyIcon.Visible = true;
			
			notifyIcon.BalloonTipText = "polls status of Selenium Grid";
            notifyIcon.BalloonTipTitle = "System Tray Selenium Grid Status Checker";

			notifyIcon.ContextMenuStrip = new ContextMenus().Create();
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = 5000;
            myTimer.Start();
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

            notifyIcon.Icon = (is_busy) ? busy_icon :  idle_icon;
            notifyIcon.Visible = true;

            Thread.Sleep(1000);
            is_busy = !is_busy;
            notifyIcon.Visible = false;
            notifyIcon.Icon = (is_busy) ? busy_icon :  idle_icon;
            notifyIcon.Visible = true;
            // restart Timer.
            myTimer.Start();
        }


       public void DisplayBallonMessage(string message, int delay) {
            if (!string.IsNullOrEmpty(message)) {
                notifyIcon.BalloonTipText = message;
			} else {
				notifyIcon.BalloonTipText = "Balloon Tip Text";
            	notifyIcon.BalloonTipTitle = "Balloon Tip Title";
			}
            notifyIcon.ShowBalloonTip(delay);
		}
		
		// NOTE: A field initializer cannot reference the non-static field, method, or property 'SystemTrayApp.ProcessIcon.drawIcon(string)' (CS0236) -
		public static Icon drawIcon(string iconBase64) {
				var iconBytes = Convert.FromBase64String(iconBase64);
				var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
				iconStream.Write(iconBytes, 0, iconBytes.Length);
				var iconImage = Image.FromStream(iconStream, true);
				var iconBitmap = new Bitmap(iconStream);			
				IntPtr hicon = iconBitmap.GetHicon();
				Icon icon = Icon.FromHandle(hicon);
				return icon;
		}

		// NOTE: 'SystemTrayApp.ProcessIcon.Dispose()' hides inherited member 'System.ComponentModel.Component.Dispose()'.
		// Use the new keyword if hiding was intended.
		// TODO: the icon does not disappear instantly on exit, only after mouse hover
		// method protection level prevents from calling
		// notifyIcon.Dispose( disposing )
		/*
		public void Dispose() {
			// NOTE: not reached
			notifyIcon.Dispose();
		}
		
		*/
	}

}
