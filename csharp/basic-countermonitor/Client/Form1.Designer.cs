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
		private TextBox textbox1;
		private TextBox textbox2;
		private TextBox textbox3;
		private Label label1;
		private Label label2;
		private Label label3;
		private Button button1;

		private IContainer components = null;

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
			this.textbox1 = new System.Windows.Forms.TextBox();
			this.textbox2 = new System.Windows.Forms.TextBox();
			this.textbox3 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();

			this.label1.Location = new System.Drawing.Point(43, 82);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(180, 39);
			this.label1.TabIndex = 2;
			this.label1.Text = "Argument 1";
			// this.label1.FormBorderStyle = FormBorderStyle.None;
			
			
			this.textbox1.Location = new System.Drawing.Point(245, 82);
			this.textbox1.Name = "textbox1";
			this.textbox1.Size = new System.Drawing.Size(317, 29);
			this.textbox1.TabIndex = 0;

			this.label2.Location = new System.Drawing.Point(43, 132);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(180, 39);
			this.label2.TabIndex = 10;
			this.label2.Text = "Argument 2";

			this.textbox2.Location = new System.Drawing.Point(245, 132);
			this.textbox2.Name = "textbox2";
			this.textbox2.Size = new System.Drawing.Size(317, 29);
			this.textbox2.TabIndex = 3;

			this.label3.Location = new System.Drawing.Point(43, 182);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(180, 39);
			this.label3.TabIndex = 10;
			this.label3.Text = "Result";

			this.textbox3.Location = new System.Drawing.Point(245, 182);
			this.textbox3.Name = "textbox3";
			this.textbox3.ReadOnly = true;
			this.textbox3.Size = new System.Drawing.Size(317, 29);
			this.textbox3.TabIndex = 3;

			this.button1.Location = new System.Drawing.Point(26, 22);
			this.button1.Margin = new System.Windows.Forms.Padding(6);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(182, 42);
			this.button1.TabIndex = 0;
			this.button1.Text = "Start Calculation";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);

			this.AutoScaleBaseSize = new System.Drawing.Size(9, 22);
			this.ClientSize = new System.Drawing.Size(611, 240);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textbox2);
			this.Controls.Add(this.textbox1);
			this.Controls.Add(this.textbox3);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Customer Form";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

	}
}
