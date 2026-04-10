using System;
using System.Drawing;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Timers;

using Utils;

namespace Program {
	public partial class Form1 : Form {
		private CircularProgressControl circularProgressControl1;
		private Button button1;
		private Label label1;

		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

		private Boolean debug;
		private int averageInterval = 30000;
		private int collectInterval = 1000;
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

			this.ClientSize = new Size(257, 119);
			this.Text = "Progress Control";
			this.label1 = new System.Windows.Forms.Label();
			this.BackColor = Color.LightGray;

			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 78);
			this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 27);
			this.label1.TabIndex = 2;
			this.label1.Text = "         ";
			//
			// this.label1.BorderStyle = BorderStyle.FixedSingle;
			circularProgressControl1 = new CircularProgressControl {
				Location = new Point(10, 20),
				MinimumSize = new Size(56, 56),
				Size = new Size(56, 56),
				TickColor = Color.DarkBlue,
				Clockwise = true,
				StartAngle = 270
			};

			button1 = new Button {
				Location = new Point(170, 40),
				Size = new Size(75, 23),
				Text = "Start"
			};
			button1.Click += Button1_Click;
			this.Controls.Add(this.label1);

			this.Controls.Add(circularProgressControl1);
			this.Controls.Add(button1);
		}

		private void Button1_Click(object sender, EventArgs e) {
			if (button1.Text == "Start") {
				button1.Text = "Stop";
				circularProgressControl1.Start();
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
				circularProgressControl1.Stop();
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
				var status = String.Format("{0} from {1} samples", average, values.Count());
				label1.safeInvoke((Label label) => label.BorderStyle = BorderStyle.FixedSingle);
				label1.safeInvoke((Label label) => label.Text = status);
				label1.safeInvoke((Label label) => label.BorderStyle = BorderStyle.None);
				Debug.WriteLine(status);

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
