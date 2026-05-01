using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Timers;
using System.Linq;
using System;
using Utils;

// WARNING: switching between "Design" and "Source" makes SharpDevelop regenerate the source
namespace Program {

	partial class Form1 {

		private IContainer components = null;
		// 15 minute worth of data
		private NameValueCollection appSettings;
			private Boolean debug;
		private string dataFile = @"c:\temp\loadaveragecounterservice.txt";
		private int averageInterval = 60000;
		private int collectInterval = 1000;
		private int rounds = 100;
		private int capacity = 900;
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";
		// NOTE: the default values of
		// dataFile,  averageInterval, collectInterval, categoryNAme, counterName, and instanceName about to be overwrittedn with app config values

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.userControl = new Utils.UserControl1();

			this.userControl.Location = new System.Drawing.Point(0, 0);
			userControl.CounterName = this.counterName;
			userControl.CategoryName = this.categoryName;
			userControl.InstanceName = this.instanceName;

			userControl.AverageInterval = this.averageInterval;
			userControl.CollectInterval = this.collectInterval;
			userControl.Capacity = this.capacity;
			userControl.Debug = this.debug;
			userControl.DataFile = this.dataFile;
			userControl.Rounds = this.rounds;

			this.SuspendLayout();

			this.AutoScaleDimensions = new SizeF(11F, 24F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(549, 133);
			this.Controls.Add(userControl);

			this.Margin = new Padding(6);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private UserControl1 userControl;
	}
}

