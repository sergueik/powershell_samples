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

		private void button1_Click(object sender, EventArgs e) {
			string text = textbox3.safeInvoke((TextBox textbox) => textbox.Text);

			button1.safeInvoke((Control control) => control.Enabled = false);
			textbox3.safeInvoke((TextBox textbox) => textbox.Text = "");

			if (textbox2.Text != null) {
				Func<string, string, string> getResult = (string argument1, string argument2) => {
					cnt++;
					Thread.Sleep(100);
					Console.Error.WriteLine("cnt :" + cnt);
					return cnt == 10 ? "DONE" : "";
				};
				discover1 = new Discover(interval, getResult, textbox1.Text, textbox2.Text);
			} else {
				Func<string, string> getResult = (string argument) => {
					cnt++;
					Thread.Sleep(100);
					Console.Error.WriteLine("cnt :" + cnt);
					return cnt == 10 ? "DONE" : "";
				};
				discover1 = new Discover(interval, getResult, textbox1.Text);
			}
			cnt = 0;
			discover1.startPollingForResult();
			Thread.Sleep(2500);
		
			textbox3.safeInvoke((TextBox textbox) => textbox.Text = discover1.Result);
			button1.safeInvoke((Control control) => control.Enabled = true);
		}

	}
}