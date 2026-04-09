using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using Utils;

namespace Program {

	public partial class Form1 : Form {

		private NameValueCollection appSettings;
		private Discover discover1 = null;
		private int interval = 100;
		private int cnt = 0;
		private string argument1;
		private string argument2;

		public Form1() {
			appSettings = ConfigurationManager.AppSettings;
			if (appSettings.AllKeys.Contains("Argument1")) {
				argument1 = appSettings["Argument1"];
			}

			if (appSettings.AllKeys.Contains("Argument2")) {
				argument2 = appSettings["Argument2"];
			}

			InitializeComponent();
			this.textbox1.Text = argument1;
			this.textbox2.Text = argument2;

		}

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string text = textbox3.safeInvoke((TextBox textbox) => textbox.Text);
			string status = null;
			string name = null;
			string mainClass = null;
			string pid = null;
			string result = null;

			button1.safeInvoke((Control control) => control.Enabled = false);
			textbox3.safeInvoke((TextBox textbox) => textbox.Text = "");

			if (textbox2.Text != null) {
				name = textbox1.Text;
				mainClass = textbox2.Text;
				Func<string, string, string> getResult2 = (string argument1, string argument2) => {
					List<int> results = ProcessInfo.getProcessIDsByCommandLine(argument1, argument2);
					if (results.Count > 1) {
						// TODO: handle this
					} else {
						status = String.Format("name: {0}| variable: {1}|pid: {2}", argument1, argument2, results[0]);
						Debug.WriteLine(status);
						textbox3.safeInvoke((TextBox textbox) => textbox.Text = status);
					}
					return results[0].ToString();
				};
				var discover2 = new Discover(interval, getResult2, name + ".exe", mainClass);
				discover2.startPollingForResult();
				Thread.Sleep(2500);
				pid = discover2.Result;
				status = String.Format("name: {0}| variable: {1}|pid: {2}", name, mainClass, pid);
				textbox3.safeInvoke((TextBox textbox) => textbox.Text = status);
				Debug.WriteLine(status);
				result = ProcessInfo.getProcessInstanceName(name, pid);
				status = String.Format("name: {0} pid:{1} counter:{2}", name, pid, result);
				textbox3.safeInvoke((TextBox textbox) => textbox.Text = status);
				Debug.WriteLine(status);

			} else {
				Func<string, string> getResult = (string argument) => {
					cnt++;
					Thread.Sleep(100);
					Console.Error.WriteLine("cnt :" + cnt);
					return cnt == 10 ? "DONE" : "";
				};
				discover1 = new Discover(interval, getResult, textbox1.Text);
			}		
			// textbox3.safeInvoke((TextBox textbox) => textbox.Text = discover1.Result);
			button1.safeInvoke((Control control) => control.Enabled = true);
		}

	}
}