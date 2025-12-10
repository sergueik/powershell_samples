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
using Utils;

namespace Program {
	public partial class Form1 : Form {

		private CrumbBar crumbBar1,crumbBar2,crumbBar3;
		private System.ComponentModel.IContainer components = null;

      
		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {

		}


		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.SuspendLayout();

			this.crumbBar1 = new CrumbBar();
			this.crumbBar1.Location = new Point(10, 10);
			this.crumbBar1.Font = new Font("Segoe UI", 9);
			this.crumbBar1.ForeColor = Color.Black;
			// this.crumbBar.BackColor = Color.Transparent; // optional

			this.crumbBar1.Size = new Size(560, 24);
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1113, 615);
            
			this.crumbBar1.Add("Home");
			this.crumbBar1.Add("Documents");
			this.crumbBar1.Add("Projects");
			this.crumbBar1.Add("2025");

			this.crumbBar1.CrumbClicked += CrumbBar_CrumbClicked;
            
			this.Controls.Add(this.crumbBar1);

			this.crumbBar2 = new CrumbBar();
			this.crumbBar2.Location = new Point(10, 50);
			this.crumbBar2.Font = new Font("Segoe UI", 9);
			this.crumbBar2.ForeColor = Color.Black;
			// this.crumbBar.BackColor = Color.Transparent; // optional

			this.crumbBar2.Size = new Size(560, 24);
			this.crumbBar2.AddRange( new List<string> { "Home", "Documents", "Projects" } );
			this.Controls.Add(this.crumbBar2);

			this.crumbBar3 = new CrumbBar();
			this.crumbBar3.Location = new Point(10, 90);
			this.crumbBar3.Font = new Font("Segoe UI", 9);
			this.crumbBar3.ForeColor = Color.Black;
			// this.crumbBar.BackColor = Color.Transparent; // optional

			this.crumbBar3.Size = new Size(560, 24);
			this.crumbBar3.AddPath("https://example.com/home/products/widgets", '/');
			this.Controls.Add(this.crumbBar3);

			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1113, 615);
			
            
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Form1";
			this.Text = "CrumbBar";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
		}

		private void CrumbBar_CrumbClicked(object sender, CrumbBarClickEventArgs e) {
			MessageBox.Show(String.Format("Crumb clicked: {0} - Text = {1}", e.Index, GetCrumbText(e.Index)));
		}
     
		private string GetCrumbText(int index) {
			var crumbs = this.crumbBar1.Crumbs;
			return (index >= 0 && index < crumbs.Count) ? this.crumbBar1.Crumbs[index] : "";
		}
	}
}
