using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System;

using Utils;

namespace Program {
	public partial class Form1 : Form {
		private CircularProgressControl circularProgressControl1;
		private Button button1;
		private Label label1;
		private NameValueCollection appSettings;

		private Boolean debug;

		public Form1() {
			InitializeComponent();
		}

		private void InitializeComponent() {

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

			} else {
				button1.Text = "Start";
				circularProgressControl1.Stop();
				label1.safeInvoke((Label label) => label.BorderStyle = BorderStyle.FixedSingle);
				label1.safeInvoke((Label label) => label.Text = circularProgressControl1.Result);
				label1.safeInvoke((Label label) => label.BorderStyle = BorderStyle.None);
			}
		}

	}
}
