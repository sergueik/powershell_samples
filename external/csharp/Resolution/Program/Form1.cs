using System;
using System.Drawing;
using System.Windows.Forms;

namespace Resolution {
	public class Form1 : Form
	{
		private Button button1;
		private Button button2;
		private Button button3;

		private int tempHeight = 0, tempWidth = 0;
		private int FixHeight = 1024, FixWidth = 768;
		private System.ComponentModel.Container components = null;

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		public Form1() {
			Screen Srn = Screen.PrimaryScreen;
			tempHeight = Srn.Bounds.Width;
			tempWidth = Srn.Bounds.Height;
			
			button1 = new Button();
			button2 = new Button();
			button3 = new Button();
			SuspendLayout();

			button1.Location = new Point(40, 48);
			button1.Name = "button1";
			button1.Size = new Size(208, 24);
			button1.TabIndex = 0;
			button1.Text = "Click Here To Get Resolution";
			button1.Click += new System.EventHandler(this.button1_Click);


			button2.Location = new Point(40, 88);
			button2.Name = "button2";
			button2.Size = new Size(208, 24);
			button2.TabIndex = 1;
			button2.Text = "Click Here To Change Resolution";
			button2.Click += new System.EventHandler(this.button2_Click);

			button3.Name = "button3";
			button3.Size = new Size(208, 24);
			button3.TabIndex = 2;
			button3.Text = "Click Here To Retaine Resolution";
			button3.Click += new System.EventHandler(this.button3_Click);

			AutoScaleBaseSize = new Size(5, 13);
			ClientSize = new Size(312, 206);
			Controls.AddRange(new Control[] {
				button3,
				button2,
				button1
			});

			Name = "Form1";
			Text = "Form1";
			Load += new System.EventHandler(this.Form1_Load);
			ResumeLayout(false);

		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		
		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}

		private void button1_Click(object sender, EventArgs e) {
			//here you will get your current resolution
			MessageBox.Show("User Resolution is " + tempHeight.ToString() + " X " + tempWidth.ToString());
			
		}

		private void button3_Click(object sender, EventArgs e) {
			MessageBox.Show("Resolution is going to change to " + tempHeight.ToString() + " X " + tempWidth.ToString());
			Resolution.CResolution ChangeRes = new Resolution.CResolution(tempHeight, tempWidth);
			
		}

		private void button2_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Resolution is going to change to " + FixHeight.ToString() + " X " + FixWidth.ToString());
			Resolution.CResolution ChangeRes = new Resolution.CResolution(FixHeight, FixWidth);
			
		}
	}
}
