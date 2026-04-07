using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Program {

	public partial class Form1 : Form {
		private TextBox txtCustID;
		private TextBox txtCountry;
		private Label label1;
		private Label label4;

		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.txtCustID = new System.Windows.Forms.TextBox();
			this.txtCountry = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtCustID
			// 
			this.txtCustID.Location = new System.Drawing.Point(245, 95);
			this.txtCustID.Name = "txtCustID";
			this.txtCustID.Size = new System.Drawing.Size(180, 29);
			this.txtCustID.TabIndex = 0;
			// 
			// txtCountry
			// 
			this.txtCountry.Location = new System.Drawing.Point(245, 352);
			this.txtCountry.Name = "txtCountry";
			this.txtCountry.Size = new System.Drawing.Size(317, 29);
			this.txtCountry.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(43, 95);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(180, 39);
			this.label1.TabIndex = 7;
			this.label1.Text = "Customer ID";
			this.label1.Click += new System.EventHandler(this.Label1Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(43, 352);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(180, 39);
			this.label4.TabIndex = 10;
			this.label4.Text = "Country";
			// 
			// CustomerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(9, 22);
			this.ClientSize = new System.Drawing.Size(1240, 508);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtCountry);
			this.Controls.Add(this.txtCustID);
			this.Name = "CustomerForm";
			this.Text = "Customer Form";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

	}
}
