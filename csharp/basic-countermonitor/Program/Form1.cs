using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

using Utils;

namespace Program {

	public partial class Form1 : Form {

		private Random rand = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));

		public Form1() {

			appSettings = ConfigurationManager.AppSettings;

			var customSettings =
				(Utils.CustomSettingsSection)ConfigurationManager.GetSection("customSettings");

			foreach (CustomSettingElement customSettingElement in customSettings.Settings) {
				Console.WriteLine(String.Format("{0} => {1}", customSettingElement.Name, customSettingElement.Text));
			}

			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}

			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}

			if (appSettings.AllKeys.Contains("AverageInterval")) {
				averageInterval = int.Parse(appSettings["AverageInterval"]);
			}

			if (appSettings.AllKeys.Contains("CategoryName")) {
				categoryName = appSettings["CategoryName"];
			}
			if (appSettings.AllKeys.Contains("CounterName")) {
				counterName = appSettings["CounterName"];
			}
			if (appSettings.AllKeys.Contains("InstanceName")) {
				instanceName = appSettings["InstanceName"];
			}
			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}

			if (appSettings.AllKeys.Contains("Rounds")) {
				rounds = int.Parse(appSettings["Rounds"]);
			}

			dataFile = EnvVars.ResolveEnvVars((appSettings.AllKeys.Contains("Datafile")) ? appSettings["Datafile"] : @"${temp}\loadaverage.csv");

			InitializeComponent();
		}

		private void OpenFileAction() {
			var openFileDialog = new OpenFileDialog() {
				Filter = "Text|*.txt",
				// ShowPinnedPlaces = true,
				// ShowPreview = true
				// 'System.Windows.Forms.OpenFileDialog' does not contain a definition for 'ShowPinnedPlaces' (CS0117)
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				dataFile = openFileDialog.FileName;
			}
		}
	}
}
