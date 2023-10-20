using System;
using System.Windows.Forms;
using System.Drawing;
using AltoControls;

namespace Test {

	// NOTE: the form class must be the defined  first in the file
	// in order the form resources to be compiled correctly,
	// all other classes has to be moved below

	public class TestForm : Form {
		public TestForm() {
			InitializeComponent();
		}
		
		private System.ComponentModel.IContainer components = null;
		private AltoControls.AltoNMUpDown upDownButton;
		private AltoButton altoButton1;
		private AltoButton altoButton2;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			upDownButton = new AltoControls.AltoNMUpDown();
			this.SuspendLayout();
			//
			// upDownButton
			//
			upDownButton.Font = new Font("Comic Sans MS", 12F);
			upDownButton.Location = new Point(12, 12);
			upDownButton.Name = "upDownButton";
			upDownButton.SignColor = Color.White;
			upDownButton.Size = new Size(75, 23);
			upDownButton.TabIndex = 0;
			upDownButton.Text = "Refresh";
			upDownButton.Value = 0D;
			
			altoButton1 =  new AltoButton();
			altoButton1.Location = new Point(12, 42);
			altoButton1.Name = "altoButton1";
			// NOTE: do not set BackColor
			altoButton1.ForeColor = Color.White;
			// NOTE: text wrap is automatic
			// altoButton1.Size = new Size(125, 48);
			altoButton1.Size = new Size(163, 48);
			altoButton1.TabIndex = 1;
			altoButton1.Text = "Disabled Button";
			altoButton1.Enabled = false;

			altoButton2 =  new AltoButton();
			altoButton2.Location = new Point(202, 42);
			
			// custom colors
			altoButton2.Active1 = Color.FromArgb(32, 32, 138);
			altoButton2.Active2 = Color.FromArgb(44, 44, 210);
			altoButton2.Name = "altoButton1";
			// NOTE: do not set BackColor
			altoButton2.ForeColor = Color.White;
			// NOTE: text wrap is automatic
			// altoButton2.Size = new Size(125, 48);
			altoButton2.Size = new Size(154, 48);
			altoButton2.TabIndex = 2;
			altoButton2.Text = "Enabled Button";
			altoButton2.Enabled = true;

			//
			// TestForm
			//
			this.AutoScaleDimensions = new SizeF(8F, 16F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(383, 120);
			this.Controls.Add(upDownButton);
			this.Controls.Add(altoButton1);
			this.Controls.Add(altoButton2);
			this.Margin = new Padding(4);
			this.Name = "TestForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
	}

	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TestForm());
		}
	}
}
