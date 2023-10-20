using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using Utils;


namespace Client {

	// based on: http://www.java2s.com/Code/CSharp/GUI-Windows-Form/TextBoxandbuttononform.htm
	public class ConfigServer: Form {
  
		private Button button2;
		private TextBox textBox1;
		private Label label1;
		private SimpleHTTPServer pageServer;
		private int port = 0;

		private System.ComponentModel.Container components = null;

		public ConfigServer() {
			InitializeComponent();
		}

		private void InitializeComponent() {
			String documentRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
			
			Console.Error.WriteLine(String.Format("Using document root path: {0}", documentRoot));
			pageServer = new SimpleHTTPServer(documentRoot);
			// NOTE: the constructor calls 
			// pageServer.Initialize() and pageServer.Listen();
			port = pageServer.Port;
			
			label1 = new Label();
			label1.Text = "Port";
			label1.Top = 32;
			label1.Left = 20;		
	  		label1.Anchor = AnchorStyles.Left | AnchorStyles.Top;
		  	label1.Size = new Size(100,23);
			  	
			this.SuspendLayout();

			button2 = new Button();
			button2.Location = new Point(30, 64);
			button2.Name = "button2";
			button2.Size = new Size(90,23);
			button2.TabIndex = 2;
			button2.Text = "Stop Server";
			button2.Click += button2_Click;
				
			textBox1 = new TextBox();
			textBox1.Location = new Point(41, 37);
			textBox1.Name = "textBox1";
			textBox1.Top = 30;
			textBox1.Left = 60;
			textBox1.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			textBox1.Size = new Size(60,23);
			textBox1.TabIndex = 0;
			textBox1.Text = String.Format("{0}", port);

			this.AutoScaleBaseSize = new Size(6, 15);
			this.ClientSize = new Size(340, 280);
			Controls.Add(button2);
			Controls.Add(textBox1);
			Controls.Add(label1);
			this.Name = "ConfigServer";
			this.Text = "ButtonTextForm";
			this.Load += new System.EventHandler(this.ConfigServer_Load);
			this.Size = new Size(152,162);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		[STAThread]
		static void Main() {
			Application.Run(new ConfigServer());
		}

		private void ConfigServer_Load(object sender, EventArgs e) {

		}

		private void button2_Click(object sender, EventArgs e) {
			pageServer.Stop();
			Application.Exit();
		}
	}
}
