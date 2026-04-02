using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

using Utils;

namespace Program
{
	public partial class Form1 : Form
	{

		private Boolean debug;
		private Boolean noop;
		private string resultFile = @"c:\temp\loadaveragecounterservice.txt";
		private string result;
		private ComputerInfo computerInfo = null;
		private int autoAverageInterval = 60000;
		private int collectInterval = 1000;
		private Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
		private int retries = 2;
		private static int capacity = 900;
		// NOTE: the default value os
		// categoryNAme, counterName and instanceName about to be overwrittedn my app config values
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";


		public Form1() {
			
			buffer = new CircularBuffer<Data>(capacity);
			computerInfo = new ComputerInfo();
			ulong free = computerInfo.AvailablePhysicalMemory;
			appSettings = ConfigurationManager.AppSettings;
			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}
			if (appSettings.AllKeys.Contains("Noop")) {
				noop = Boolean.Parse(appSettings["Noop"]);
			}

			if (appSettings.AllKeys.Contains("Retries")) {
				retries = int.Parse(appSettings["Retries"]);
			}
			
			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}

			if (appSettings.AllKeys.Contains("AutoAverageInterval")) {
				autoAverageInterval = int.Parse(appSettings["autoAverageInterval"]);
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
			if (appSettings.AllKeys.Contains("Noop")) {
				noop = Boolean.Parse(appSettings["Noop"]);
			}

			if (appSettings.AllKeys.Contains("Retries")) {
				retries = int.Parse(appSettings["Retries"]);
			}
			
			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}

			if (appSettings.AllKeys.Contains("AutoAverageInterval")) {
				autoAverageInterval = int.Parse(appSettings["autoAverageInterval"]);
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


			resultFile = EnvVars.ResolveEnvVars((appSettings.AllKeys.Contains("Datafile")) ? appSettings["Datafile"] : @"${temp}\loadaverage.txt");

			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var thread = new Thread(new ThreadStart(StartCalculation));
			thread.Start();

			timer1.Interval = collectInterval;
			timer1.Enabled = true;
			timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			timer1.Start();
			
			timer2.Interval = autoAverageInterval;
			timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			timer2.Enabled = true;
			timer2.Start();

		}

		private Action<Control> setEnabled = (Control control) => control.Enabled = true;
		public void StartCalculation() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invoke?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.delegate?view=netframework-4.5
			button1.SafeInvoke((Control control) => control.Enabled = false);

			for (int i = 0; i <= 1000; i++) {                
				Thread.Sleep(1000);
				string textForLabel = ((float)i / 10).ToString("F2") + " %";
				lblProcent.SafeInvoke(d => d.Text = textForLabel);
				progressBar1.SafeInvoke(d => d.Value = i);
				string labelText = lblProcent.SafeInvoke(d => d.Text);
			}
			button1.SafeInvoke((Control control) => control.Enabled = true);

			if (timer2 != null) {
				timer1.Stop();
				timer1.Enabled = false;
			}
			if (timer2 != null) {
				timer2.Stop();
				timer2.Enabled = false;
			}
		}
		
		private void CollectMetrics() {
			float value = 0;
			var row = new Data();
			row.TimeStamp = DateTime.Now;
			if (noop) {
				if (debug)
					value = rand.Next(0, 10);
			} else {
				try {
					// https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
					var performanceCounter = new PerformanceCounter();
					performanceCounter.CategoryName = this.categoryName;
					performanceCounter.CounterName = this.counterName;
					performanceCounter.InstanceName = instanceName == "" ? null : instanceName;
					// value = (long)performanceCounter.RawValue;
					value = performanceCounter.NextValue();
				} catch (InvalidOperationException e) {
					Console.Error.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()));
					return;
				}
			}
			row.Value = value;
			buffer.AddLast(row);
			if (debug)
				Console.Error.WriteLine("Collected data: " + row.ToString() + ((noop) ? "\r\n" + "NOOP: " + noop : ""));
		}

		private void OnElapsedTimer1(object source, ElapsedEventArgs args) {
			CollectMetrics();
		}

		private double AverageDataInterval(int interval) {
			var rows = buffer.ToList();
			var now = DateTime.Now;
			var values = (from row in rows
			              where ((now - row.TimeStamp).TotalMilliseconds) <= (float)interval
			              select row.Value);
			var average = values.Average();
			var message = String.Format("LOAD{0, 1:f0}MIN: {1, 4:f2}", interval, average);
			Console.Error.WriteLine(String.Format("{0} from {1} samples", message, values.Count()));

			return average;
		}

		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
			Commit();
		}

		private void Commit() {
				
			var headers = new List<string> { "TimeStamp", "Value" };
			double averageData = AverageDataInterval(this.autoAverageInterval);
			var fields = new List<string> {
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				averageData.ToString("F2")
			};
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			var record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			var filePath = EnvVars.ResolveEnvVars(@"${temp}\loadaverage.csv");
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(csvData, filePath);
			}

		}

		private void OpenFileAction()
		{
			var openFileDialog = new OpenFileDialog() {
				Filter = "Text|*.txt",
				// ShowPinnedPlaces = true,
				// ShowPreview = true
				// 'System.Windows.Forms.OpenFileDialog' does not contain a definition for 'ShowPinnedPlaces' (CS0117)
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				resultFile = openFileDialog.FileName;
			}
		}

	
	}
}
