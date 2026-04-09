using System;
using System.Drawing;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

using Utils;

namespace Program {
	public partial class Form1 : Form {
		private CircularProgressControl cbc1;
		private CircularProgressControl cbc2;
		private Button button1;

		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

        
		private Boolean debug;
		private string dataFile = @"c:\temp\loadaveragecounterservice.txt";
		private int averageInterval = 30000;
		private int collectInterval = 1000;
		private Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
		private int rounds = 100;
		private static int capacity = 900;
		// NOTE: the default value os
		// categoryNAme, counterName and instanceName about to be overwrittedn my app config values
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";

		
		public Form1() {
			InitializeComponent();
		}

		private void InitializeComponent() {
			buffer = new CircularBuffer<Data>(capacity);
			timer1 = new System.Timers.Timer();
			timer2 = new System.Timers.Timer();

			this.ClientSize = new Size(170, 140);
			this.Text = "OS X Progress Control";
			this.BackColor = Color.LightGray;

			cbc1 = new CircularProgressControl {
				Location = new Point(10, 20),
				MinimumSize = new Size(56, 56),
				Size = new Size(56, 56),
				TickColor = Color.DarkBlue,
				Clockwise = true,
				StartAngle = 270
			};

			cbc2 = new CircularProgressControl {
				Location = new Point(10, 80),
				MinimumSize = new Size(56, 56),
				Size = new Size(56, 56),
				TickColor = Color.Yellow,
				Clockwise = false,
				StartAngle = 270
			};

			button1 = new Button {
				Location = new Point(70, 80),
				Size = new Size(75, 23),
				Text = "Start"
			};
			button1.Click += Button1_Click;

			this.Controls.Add(cbc1);
			this.Controls.Add(cbc2);
			this.Controls.Add(button1);
		}

		private void Button1_Click(object sender, EventArgs e) {
			if (button1.Text == "Start") {
				button1.Text = "Stop";
				cbc1.Start();
				cbc2.Start();
				timer1.Interval = collectInterval;
				timer1.Enabled = true;
				timer1.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => CollectMetrics());
				timer1.Start();

				timer2.Interval = averageInterval;
				timer2.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => Commit());
				timer2.Enabled = true;
				timer2.Start();  
			} else {
				button1.Text = "Start";
				cbc1.Stop();
				cbc2.Stop();
				if (timer2 != null) {
					timer1.Stop();
					timer1.Enabled = false;
				}
				if (timer2 != null) {
					timer2.Stop();
					timer2.Enabled = false;
				}
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
				Debug.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()));
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
				Debug.WriteLine(String.Format("{0} from {1} samples", average, values.Count()));
			} catch (Exception e) {
				// System.InvalidOperationException: Sequence contains no elements
				Debug.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
			/*
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
			*/
		}
	}
}
