using System.Windows.Forms;
using System.Drawing;
namespace Program {
	partial class Form1 {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()	{
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
			// 
			// progressBar1
			// 
			progressBar1.Location = new Point(14, 41);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(228, 17);
			progressBar1.TabIndex = 1;
			// 
			// lblProcent
			// 
			lblProcent.AutoSize = true;
			lblProcent.Location = new Point(251, 42);
			lblProcent.Name = "lblProcent";
			lblProcent.Size = new Size(21, 13);
			lblProcent.TabIndex = 2;
			lblProcent.Text = "0%";
			// 
			// Form1
			// 
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
		private ProgressBar progressBar1;
		private Label lblProcent;
	}
}

