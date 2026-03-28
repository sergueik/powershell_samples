using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System;
using Utils;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

namespace Program {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			Thread thread = new Thread(new ThreadStart(StartCalculation));
			thread.Start();
		}

		public void StartCalculation() {
			button1.SafeInvoke(d => d.Enabled = false);

			for (int i = 0; i <= 1000; i++) {                
				Thread.Sleep(1000);
				string textForLabel = (i) + "%";
				CollectMetrics();
				if (i%100 == 0) {
				AverageData();
				Commit();
				}
				lblProcent.SafeInvoke(d => d.Text = textForLabel);
				progressBar1.SafeInvoke(d => d.Value = i);
				string labelText = lblProcent.SafeInvoke(d => d.Text);
				this.SafeInvoke(d => d.SetText("test", "test1"));
			}
			button1.SafeInvoke(d => d.Enabled = true);
		}

		public void SetText(string test1, string test2)
		{

		}
		
		private void CollectMetrics() {
			int value = 0;
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
					value = (Int32)performanceCounter.RawValue;
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

		private void AverageData() {
			var intervals = new int[] { 1, 3, 5 };
			var messages = new List<string>();
			foreach (int minutes in intervals) {
				double average = AverageDataInterval(minutes);
				var message = String.Format("LOAD{0}MIN: {1, 4:f2}", minutes, average);
				messages.Add(message);
			}
			result = String.Join("\n", messages);
		}


		private double AverageDataInterval(int minutes) {
			var rows = buffer.ToList();
			float interval = 1F * minutes;
			var now = DateTime.Now;
			var values = (from row in rows
			              where ((now - row.TimeStamp).TotalMinutes) <= interval
			              select row.Value);
			var average = values.Average();
			var message = String.Format("LOAD{0, 1:f0}MIN: {1, 4:f2}", interval, average);
			Console.Error.WriteLine(String.Format("{0} from {1} samples", message, values.Count()));

			return average;
		}

		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
				AverageData();
		}

		private void Commit() {
			try {
				var fileHelper = new Utils.FileHelper();
				
				fileHelper.Retries = retries;
				fileHelper.FilePath = resultFile;
				fileHelper.Interval = 500;
				fileHelper.Text = result;
				fileHelper.WriteContents();
				Console.Error.WriteLine(String.Format("FiileHelper {0}",resultFile));
			} catch (Exception e) {
				// Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
				Console.Error.WriteLine(String.Format("Exception writing \"{0}\": ", resultFile) + e.ToString());
			}
		}
	}
}
