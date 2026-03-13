using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Utils;

namespace Program {
	public class Form: System.Windows.Forms.Form {
		private Container components = null;
		private TcpServer tcpServer;
		private Button button;
		// private EchoServiceProvider Provider;
        

		public Form() {
			InitializeComponent();
		}


		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void InitializeComponent() {
			button = new Button();
			this.SuspendLayout();

			button.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
			button.Location = new Point(140, 51);
			button.Name = "button";
			button.Size = new Size(135, 39);
			button.TabIndex = 0;
			button.Text = "Close";
			button.Click += new System.EventHandler(button_Click);

			this.AutoScaleBaseSize = new Size(9, 22);
			this.ClientSize = new Size(292, 102);
			this.Controls.Add(button);
			this.Name = "Form";
			this.Text = "TcpServerDemo";
			this.Closed += new System.EventHandler(Form_Closed);
			this.Load += new System.EventHandler(Form_Load);
			this.ResumeLayout(false);

		}
		[STAThread]
		static void Main() {
			Application.Run(new Form());
		}

		private void Form_Load(object sender, EventArgs e) {
			var echoServiceProvider = new EchoServiceProvider();
			tcpServer = new TcpServer(echoServiceProvider, 15555);
			tcpServer.Start();
		}

		private void button_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void Form_Closed(object sender, EventArgs e) {
			tcpServer.Stop();
		}
	}
}
