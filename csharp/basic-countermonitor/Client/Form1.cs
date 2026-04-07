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
		private string argument;

		public Form1() {
			appSettings = ConfigurationManager.AppSettings;
			if (appSettings.AllKeys.Contains("Argument")) {
				argument = appSettings["Argument"];
			}

			InitializeComponent();
			this.textbox1.Text = argument;

		}

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		void Label1Click(object sender, EventArgs e) {
		}

		private void button1_Click(object sender, EventArgs e) {
			button1.safeInvoke((Control control) => control.Enabled = false);
			textbox2.safeInvoke((TextBox textbox) => textbox.Text = "");

			Func<string, string> getResult = (string arg) => {
				cnt++;
				Console.Error.WriteLine("cnt :" + cnt);
				return cnt == 10 ? "DONE" : "";	};
			discover1 = new Discover(  interval, getResult,argument);
			cnt = 0;
			discover1.startPollingForResult();
		
			Thread.Sleep(2500);
			textbox2.safeInvoke((TextBox textbox) => textbox.Text = discover1.Result);
			button1.safeInvoke((Control control) => control.Enabled = true);
		}

	}
}
