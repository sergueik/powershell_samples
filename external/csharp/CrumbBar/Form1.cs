using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Free.Controls.CrumbBar
{
	public partial class Form1 : Form
	{
    	
    	
		private CrumbBar crumbBar;
		private System.ComponentModel.IContainer components = null;

      
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}


		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.SuspendLayout();
			this.crumbBar = new CrumbBar();
			this.crumbBar.Location = new Point(10, 10);
			this.crumbBar.Font = new Font("Segoe UI", 9);
			this.crumbBar.ForeColor = Color.Black;
			// this.crumbBar.BackColor = Color.Transparent; // optional

			this.crumbBar.Size = new Size(560, 24);
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1113, 615);
            
			this.crumbBar.Add("Home");
			this.crumbBar.Add("Documents");
			this.crumbBar.Add("Projects");
			this.crumbBar.Add("2025");

			this.crumbBar.CrumbClicked += CrumbBar_CrumbClicked;
            
			this.Controls.Add(this.crumbBar);
            
            
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Form1";
			this.Text = "CrumbBar";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
		}



		private void CrumbBar_CrumbClicked(object sender, CrumbBarClickEventArgs e)
		{

			MessageBox.Show(String.Format("Crumb clicked: {0} - Text = {1}", e.Index, GetCrumbText(e.Index)));
		}
     
		private string GetCrumbText(int index)
		{
			var crumbs = this.crumbBar.Crumbs;
			return   	(index >= 0 && index < crumbs.Count) ? this.crumbBar.Crumbs[index] : "";
 
		}

    
	}
}
