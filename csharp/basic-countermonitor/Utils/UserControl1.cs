using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System;

namespace Utils {
	public partial class UserControl1 : UserControl {
		private Boolean debug;
		private CircularBuffer<Data> buffer;

		private string dataFile = @"c:\temp\loadaveragecounterservice.txt";
		private Random rand = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
		private int rounds = 100;
		private int averageInterval = 60000;
		private int collectInterval = 1000;
		private static int capacity = 900;
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";

		public string CategoryName {
			get { return categoryName; }
			set  { categoryName = value; }
		}
		public string CounterName {
			get { return counterName; }
			set  { counterName = value; }
		}
		public string InstanceName {
			get { return instanceName; }
			set  { instanceName = value; }
		}
		public string DataFile {
			get { return dataFile; }
			set  { dataFile = value; }
		}
		public int AverageInterval {
			get { return averageInterval; }
			set  { averageInterval = value; }
		}
		public int CollectInterval {
			get { return collectInterval; }
			set  { collectInterval = value; }
		}
		public int Capacity {
			get { return capacity; }
			set  { capacity = value; }
		}
		public int Rounds {
			get { return rounds; }
			set  { rounds = value; }
		}
		public Boolean Debug {
			get { return debug; }
			set  { debug = value; }
		}

		public UserControl1() {
			buffer = new CircularBuffer<Data>(capacity);
			// appSettings = ConfigurationManager.AppSettings;
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			var thread = new Thread(new ThreadStart(StartCalculation));
			thread.Start();

			timer1.Interval = collectInterval;
			timer1.Enabled = true;
			timer1.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => CollectMetrics());
			timer1.Start();

			timer2.Interval = averageInterval;
			timer2.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => Commit());
			timer2.Enabled = true;
			timer2.Start();

		}

		private Action<Control> setEnabled = (Control control) => control.Enabled = true;
		public void StartCalculation() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invoke?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.delegate?view=netframework-4.5
			button1.safeInvoke((Control control) => control.Enabled = false);

			for (int i = 0; i <= rounds; i++) {
				Thread.Sleep(collectInterval);
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.Label?view=netframework-4.5
				label1.safeInvoke((Label label) => label.Text = ((float)i / 10).ToString("F2") + " %");
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.ProgressBar?view=netframework-4.5
				progressBar1.safeInvoke((ProgressBar progressBar) => progressBar.Value = i);
				string labelText = label1.safeInvoke((Label label) => label.Text);
			}
			button1.safeInvoke((Control control) => control.Enabled = true);

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
			try {
				// https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
				var performanceCounter = new PerformanceCounter();
				performanceCounter.CategoryName = this.categoryName;
				performanceCounter.CounterName = this.counterName;
				performanceCounter.InstanceName = instanceName == "" ? null : instanceName;
				// value = (long)performanceCounter.RawValue;
				value = performanceCounter.NextValue();
			} catch (InvalidOperationException e) {
				System.Diagnostics.Debug.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()));
				return;
			}
			row.Value = value;
			buffer.AddLast(row);
		}

		private void Commit() {

			var rows = buffer.ToList();
			var now = DateTime.Now;
			double average = 0;
			IEnumerable<float> values = null;
			try {
				values = (from row in rows
				          where ((now - row.TimeStamp).TotalMilliseconds) <= (float)this.averageInterval
				          select row.Value);
				average = values.Average();
			} catch (InvalidOperationException e) {
				// System.InvalidOperationException: Sequence contains no elements
				System.Diagnostics.Debug.WriteLine(String.Format("Exception: {0}", e.ToString()));
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
			System.Diagnostics.Debug.WriteLine(String.Format("{0} from {1} samples", average, values.Count()));

			var fields = new List<string> {
				DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				average.ToString("F2")
			};
			var csvData = new CsvData();

			var headers = new List<string> { "TimeStamp", "Value" };
			headers.ForEach(header => csvData.Headers.Add(header));
			var record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(csvData, dataFile);
			}

		}

	}
}

