using System;
using System.Windows.Forms;
using System.Drawing;
using System.Resources;
using Utils;

namespace Program {
	public class WinForm : Form {
		private void InitializeComponent() {
			var resources = new System.Resources.ResourceManager(typeof(WinForm));
			notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
			notifyIcon.Text = "notifyIcon1";
			notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 133);
			// this.Controls.Add(this.label1);
			// this.Controls.Add(this.label);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(260, 160);
			this.Name = "WinForm";
			this.Text = "WinForm";
			this.ResumeLayout(false);
		}

		MinTrayBtn mybutton;
		NotifyIcon notifyIcon = new NotifyIcon();
		public WinForm()
		{
    	
			mybutton = new MinTrayBtn(this);
			mybutton.MinTrayBtnClicked += 
          new MinTrayBtnClickedEventHandler(TrayBtn_clicked);
		}
		private void TrayBtn_clicked(object sender, EventArgs e)
		{
			this.Hide();
			this.notifyIcon.Visible = true;
		}


		private void notifyIcon_Click(object sender, System.EventArgs e)
		{
			this.Show();
			this.notifyIcon.Visible = false;
		}
    
	}

}