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
		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

		
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
			timer1 = new System.Timers.Timer();
			timer2 = new System.Timers.Timer();
			button1 = new Button();
			progressBar1 = new ProgressBar();
			lblProcent = new Label();
			((System.ComponentModel.ISupportInitialize)(timer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(timer2)).BeginInit();
			this.SuspendLayout();

			timer1.Enabled = true;
			timer1.Interval = 1000D;
			timer1.SynchronizingObject = this;

			timer2.Enabled = true;
			timer2.Interval = 60000D;
			timer2.SynchronizingObject = this;

			button1.Location = new Point(26, 22);
			button1.Margin = new Padding(6, 6, 6, 6);
			button1.Name = "button1";
			button1.Size = new Size(182, 42);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(26, 76);
			progressBar1.Margin = new Padding(6, 6, 6, 6);
			progressBar1.Maximum = 1000;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(418, 31);
			progressBar1.TabIndex = 1;

			lblProcent.AutoSize = true;
			lblProcent.Location = new Point(460, 78);
			lblProcent.Margin = new Padding(6, 0, 6, 0);
			lblProcent.Name = "lblProcent";
			lblProcent.Size = new Size(41, 25);
			lblProcent.TabIndex = 2;
			lblProcent.Text = "0%";

			this.AutoScaleDimensions = new SizeF(11F, 24F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(549, 133);
			this.Controls.Add(lblProcent);
			this.Controls.Add(progressBar1);
			this.Controls.Add(button1);
			this.Margin = new Padding(6, 6, 6, 6);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(timer1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(timer2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		
		// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.progressbar?view=netframework-4.5
		private ProgressBar progressBar1;
		private Label lblProcent;
		private Button button1;
	}
}

