using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Program.Properties;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Timers;
using System.Linq;

using Utils;

namespace Program
{

	class ProcessIcon : IDisposable
	{
		private NameValueCollection appSettings;
		private Boolean debug;
		static int retries = 2;
		private int collectInterval = 1000;
		private int waitInterval = 120000;
		private string vmName = "";
		private string toolPath = null;
		private string logFile = null;
		private string fileName = null;
		private string arguments1 = null;
		private string arguments2 = null;
		private string runCommand = null;
		private string userName = null;
		private string password = null;
		private string script = null;
		private string scriptArguments = null;
	
		NotifyIcon notifyIcon;
		// NOTE: Timer is an ambiguous reference between
		// System.Windows.Forms.Timer and System.Threading.Timer
		System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
		bool is_busy = false;
		int nScanCounter = 1;
		static bool exitFlag = false;
		private Icon idle_icon;
		private Icon busy_icon;

		public ProcessIcon()
		{
			notifyIcon = new NotifyIcon();

			appSettings = ConfigurationManager.AppSettings;
			// NOTE: 
			// System.Array does not contain a definition for 'Contains' and no extension method 'Contains' accepting a first argument of type 'System.Array' could be found 
			// need to add System.Linq (CS1061)
			
			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}

			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}
			if (appSettings.AllKeys.Contains("WaitInterval")) {
				waitInterval = int.Parse(appSettings["WaitInterval"]);
			}

			if (appSettings.AllKeys.Contains("VmName")) {
				vmName = appSettings["VmName"];
			}
			if (appSettings.AllKeys.Contains("FileName")) {
				fileName = appSettings["FileName"];
			}
			if (appSettings.AllKeys.Contains("RunCommand")) {
				runCommand = appSettings["RunCommand"];
			}
			if (appSettings.AllKeys.Contains("Password")) {
				password = appSettings["Password"];
			}
			if (appSettings.AllKeys.Contains("UserName")) {
				userName = appSettings["UserName"];
			}
			if (appSettings.AllKeys.Contains("Script")) {
				script = appSettings["Script"];
			}
			if (appSettings.AllKeys.Contains("ScriptArguments")) {
				scriptArguments = appSettings["ScriptArguments"];
			}
			if (appSettings.AllKeys.Contains("Arguments1")) {
				arguments1 = appSettings["Arguments1"];
			}

			if (appSettings.AllKeys.Contains(runCommand)) {
				arguments2 = appSettings[runCommand];
				arguments2 = arguments2.Replace("%VM%", "{7e261a39-d356-4eb1-a8ed-75675b149241}");
				// the user name may not match login id
				arguments2 = arguments2.Replace("%USERNAME%", "sergueik");
				arguments2 = arguments2.Replace("%PASSWORD%", "password");
				arguments2 = arguments2.Replace("%SCRIPT%", script);
				arguments2 = arguments2.Replace("%SCRIPTARGUMENTS%", scriptArguments);
			}
			if (appSettings.AllKeys.Contains("ToolPath")) {
				toolPath = Environment.ExpandEnvironmentVariables(appSettings["ToolPath"]);
				Debug.WriteLine(String.Format("Expanded Path: {0}", toolPath));
			} else {
				Debug.WriteLine("could not resolve toolpath");
			}
			
			if (appSettings.AllKeys.Contains("LogFile")) {
				logFile = Environment.ExpandEnvironmentVariables(appSettings["LogFile"]);
			}
		}

		public void Display()
		{
			notifyIcon.MouseClick += new MouseEventHandler(ni_MouseClick);
			
			idle_icon = Resources.idle_icon;
			busy_icon = Resources.busy_icon;
			notifyIcon.Icon = idle_icon;
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblydescriptionattribute?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assemblytitleattribute?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.attribute.getcustomattribute?view=netframework-4.5
			string assemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
			// System.ArgumentOutOfRangeException: Text length must be less than 64 characters long.
		
			notifyIcon.Text = (assemblyTitle.Length > 64) ? (assemblyTitle.Substring(0, 61) + "...") : assemblyTitle;
			notifyIcon.Visible = true;
			notifyIcon.ContextMenuStrip = new ContextMenus().Create();
			myTimer.Tick += new EventHandler(TimerEventProcessor);
			myTimer.Interval = collectInterval;
			myTimer.Start();
		}

		public void Dispose()
		{
			notifyIcon.Visible = false;
			notifyIcon.Dispose();
		}

		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				Process.Start("explorer", null);
			}
		}
	
	
		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs) {
			myTimer.Stop();
			nScanCounter++;
			Debug.WriteLine(String.Format(@"Run ""{0}\{1}"" {2}", toolPath, fileName, arguments1));
			Debug.WriteLine(String.Format("Counter: {0}", nScanCounter.ToString()));
			is_busy = !is_busy;
			notifyIcon.Visible = false;
			if (is_busy)
				notifyIcon.Icon = busy_icon;
			else
				notifyIcon.Icon = idle_icon;
			notifyIcon.Visible = true;
			var processRunner = new ProcessRunner();
			// NOTE: can not run under SharpDevelop: the %PROGRAMFILES% will expand to C:\Program Files (x86) 
			// Debug.WriteLine(String.Format("filename: {0}", String.Format(@"{0}\{1}", toolPath, fileName)));
			// Debug.WriteLine(String.Format("arguments: {0}", arguments));
			processRunner.Run(String.Format(@"{0}\{1}", toolPath, fileName), arguments1);
			// Debug.WriteLine(String.Format(@"{0} ""{1}""", "STDOUT:", String.Join("", processRunner.StandardOutput)));
			// Debug.WriteLine(String.Format(@"{0} ""{1}""", "STDERR:", String.Join("", processRunner.StandardError)));
			var fileHelper = new FileHelper();
				
			fileHelper.Retries = retries;
			fileHelper.FilePath = logFile;
			fileHelper.Interval = 500;
			fileHelper.Append = true;
			fileHelper.Text = String.Format("{0} \"{1}\"\n", "STDOUT:", String.Join("", processRunner.StandardOutput));

			fileHelper.WriteContents();
			// Thread.Sleep(1000);
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
