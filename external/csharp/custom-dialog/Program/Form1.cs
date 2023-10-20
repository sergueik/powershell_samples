using System;
using System.Windows.Forms;

namespace CustomDialog {

	public class Form1 : System.Windows.Forms.Form {
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;

		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;

		private void InitializeComponent() {
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(110, 196);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(160, 28);
			this.button1.TabIndex = 0;
			this.button1.Text = "Sample Dialog";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(8, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(364, 180);
			this.label1.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(380, 233);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		public Form1() {
			InitializeComponent();
		}

		private void button1_Click(object sender, System.EventArgs e) {
			SampleDialog sd = new SampleDialog();
			if(sd.ShowDialog()==DialogResult.OK)
				this.label1.Text = sd.TypedText;
			else
				this.label1.Text = "Dialog was cancelled!";
		}

	}

}
