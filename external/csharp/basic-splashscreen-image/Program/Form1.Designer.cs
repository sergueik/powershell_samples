using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace WindowsApplication1
{
	partial class Form1
	{
		private Boolean splash2Checked = true;
		private void splash2CheckedChanged(object sender, EventArgs e)
		{
			splash2Checked = !splash2Checked;
		}

		private System.ComponentModel.IContainer components = null;


		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
        	
			this.splash2 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();

			this.button1.Location = new System.Drawing.Point(89, 121);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(107, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Splash!";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);

			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
            
			this.splash2.AutoSize = true;
			this.splash2.Checked = true;
			this.splash2.CheckState = System.Windows.Forms.CheckState.Checked;
			this.splash2.Location = new System.Drawing.Point(176, 214);
			this.splash2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splash2.Name = "splash2";
			this.splash2.Size = new System.Drawing.Size(66, 21);
			this.splash2.TabIndex = 20;
			this.splash2.Text = "Closing Splash";
			this.splash2.UseVisualStyleBackColor = true;
			this.splash2.CheckedChanged += new System.EventHandler(this.splash2CheckedChanged);

        	
			this.Controls.Add(this.button1);
			this.Controls.Add(this.splash2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.CheckBox splash2;
	}
}

