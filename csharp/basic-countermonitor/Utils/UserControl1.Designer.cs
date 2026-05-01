// -

using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using System;
using System.Collections.Generic;

namespace Utils {
	partial class UserControl1 {
    private System.ComponentModel.IContainer components = null;
 		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

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

    private void InitializeComponent() {
		timer1 = new System.Timers.Timer();
			timer2 = new System.Timers.Timer();
  		timer1.Enabled = true;
			timer1.Interval = 1000D;
			timer1.SynchronizingObject = this;

			timer2.Enabled = true;
			timer2.Interval = 60000D;
			timer2.SynchronizingObject = this;

			button1 = new Button();
			progressBar1 = new ProgressBar();
			label1 = new Label();
			this.SuspendLayout();

			button1.Location = new Point(26, 22);
			button1.Margin = new Padding(6);
			button1.Name = "button1";
			button1.Size = new Size(182, 42);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(26, 76);
			progressBar1.Margin = new Padding(6);
			progressBar1.Maximum = 1000;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(418, 31);
			progressBar1.TabIndex = 1;

      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			button1.Location = new Point(26, 22);
			button1.Margin = new Padding(6);
			button1.Name = "button1";
			button1.Size = new Size(182, 42);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(26, 76);
			progressBar1.Margin = new Padding(6);
			progressBar1.Maximum = 1000;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(418, 31);
			progressBar1.TabIndex = 1;

			label1.AutoSize = true;
			label1.Location = new Point(460, 78);
			label1.Margin = new Padding(6, 0, 6, 0);
			label1.Name = "label1";
			label1.Size = new Size(41, 25);
			label1.TabIndex = 2;
			label1.Text = "0%";

			this.AutoScaleDimensions = new SizeF(11F, 24F);
			this.AutoScaleMode = AutoScaleMode.Font;
      // this.Size = new System.Drawing.Size(274, 27);
			this.ClientSize = new Size(549, 133);
			this.Controls.Add(label1);
			this.Controls.Add(progressBar1);
			this.Controls.Add(button1);
			((System.ComponentModel.ISupportInitialize)(timer1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(timer2)).EndInit();


      this.Name = "UserControl1";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

		// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.progressbar?view=netframework-4.5
		private ProgressBar progressBar1;
		private Label label1;
		private Button button1;

  }
}
