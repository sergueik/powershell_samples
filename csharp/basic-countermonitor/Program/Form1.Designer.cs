using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using System.Linq;
using System;
using Utils;

namespace Program {

	partial class Form1 {

		private Boolean debug;
		private Boolean noop;
		private IContainer components = null;
		private string resultFile = @"c:\temp\loadaveragecounterservice.txt";
		private string result;
		private ComputerInfo computerInfo = null;
		private int autoAverageInterval = 60000;
		private int collectInterval = 1000;
		private Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;
		private int retries = 2;
		private static int capacity = 900;

		// 15 minute worth of data
		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;
		
		// NOTE: the default value os 
		// categoryNAme, counterName and instanceName about to be overwrittedn my app config values
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";
		// NOTE: Process\Working Set	
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			if(timer1!=null) {
				timer1.Stop();
				timer1.Enabled = false;
			}
			if(timer2!=null) {
				timer2.Stop();
				timer2.Enabled = false;
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()	{

			buffer = new CircularBuffer<Data>(capacity);
			computerInfo = new ComputerInfo();
                     ulong free = computerInfo.AvailablePhysicalMemory;
                     /*
                      one short timer → sample metrics every 1–2 sec
                      one long timer → auto-close after N minutes
                     */
			appSettings = ConfigurationManager.AppSettings;
			// Error CS1061: 'System.Array' does not contain a definition for 'Contains' and no extension method 'Contains' accepting a first argument of type 'System.Array' could be found (are you missing a using directive or an assembly reference?)
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


			resultFile = (appSettings.AllKeys.Contains("Datafile")) ? appSettings["Datafile"] : @"c:\temp\loadaverage.txt";


/*
 			timer1 = new System.Timers.Timer();
			timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			timer1.Interval = collectInterval;
			timer1.Enabled = true;
			timer1.Start();
			timer2 = new System.Timers.Timer();
			timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			timer2.Interval = autoAverageInterval;
			timer2.Enabled = true;
			timer2.Start();

 */ 			
			button1 = new Button();
			progressBar1 = new ProgressBar();
			lblProcent = new Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new Point(14, 12);
			button1.Name = "button1";
			button1.Size = new Size(99, 23);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(14, 41);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(228, 17);
			progressBar1.Maximum  = 1000;
			progressBar1.TabIndex = 1;

			lblProcent.AutoSize = true;
			lblProcent.Location = new Point(251, 42);
			lblProcent.Name = "lblProcent";
			lblProcent.Size = new Size(21, 13);
			lblProcent.TabIndex = 2;
			lblProcent.Text = "0%";

			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(284, 72);
			this.Controls.Add(lblProcent);
			this.Controls.Add(progressBar1);
			this.Controls.Add(button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Button button1;
		
		// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.progressbar?view=netframework-4.5
		private ProgressBar progressBar1;
		private Label lblProcent;
	}
}

