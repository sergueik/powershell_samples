using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
	partial class MainForm {
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.btnSelectFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtDocDirectory = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSearchKey1 = new System.Windows.Forms.TextBox();
			this.txtReplace1 = new System.Windows.Forms.TextBox();
			this.btnReplace = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.txtSearchKey3 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtSearchKey4 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnSelectFile
			// 
			this.btnSelectFile.Location = new System.Drawing.Point(112, 89);
			this.btnSelectFile.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnSelectFile.Name = "btnSelectFile";
			this.btnSelectFile.Size = new System.Drawing.Size(207, 46);
			this.btnSelectFile.TabIndex = 0;
			this.btnSelectFile.Text = "Choose Folder";
			this.btnSelectFile.UseVisualStyleBackColor = true;
			this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(341, 101);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "Base folder：";
			// 
			// txtDocDirectory
			// 
			this.txtDocDirectory.Location = new System.Drawing.Point(486, 89);
			this.txtDocDirectory.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtDocDirectory.Name = "txtDocDirectory";
			this.txtDocDirectory.Size = new System.Drawing.Size(256, 29);
			this.txtDocDirectory.TabIndex = 2;
			this.txtDocDirectory.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtDocDirectory.TextChanged += new System.EventHandler(this.TxtDocDirectoryTextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(154, 161);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 25);
			this.label2.TabIndex = 3;
			this.label2.Text = "1.Text to find";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Tan;
			this.label3.Location = new System.Drawing.Point(154, 215);
			this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(179, 25);
			this.label3.TabIndex = 3;
			this.label3.Text = "Text to be replaced";
			// 
			// txtSearchKey1
			// 
			this.txtSearchKey1.Location = new System.Drawing.Point(341, 156);
			this.txtSearchKey1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey1.Name = "txtSearchKey1";
			this.txtSearchKey1.Size = new System.Drawing.Size(401, 29);
			this.txtSearchKey1.TabIndex = 4;
			// 
			// txtReplace1
			// 
			this.txtReplace1.BackColor = System.Drawing.SystemColors.Window;
			this.txtReplace1.Enabled = false;
			this.txtReplace1.Location = new System.Drawing.Point(341, 209);
			this.txtReplace1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtReplace1.Name = "txtReplace1";
			this.txtReplace1.Size = new System.Drawing.Size(401, 29);
			this.txtReplace1.TabIndex = 4;
			// 
			// btnReplace
			// 
			this.btnReplace.Location = new System.Drawing.Point(112, 770);
			this.btnReplace.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Size = new System.Drawing.Size(137, 46);
			this.btnReplace.TabIndex = 5;
			this.btnReplace.Text = "Start replacing";
			this.btnReplace.UseVisualStyleBackColor = true;
			this.btnReplace.Click += new System.EventHandler(this.btnReplaceText_Click);
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(605, 770);
			this.btnClose.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(137, 46);
			this.btnClose.TabIndex = 6;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(154, 261);
			this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(124, 25);
			this.label6.TabIndex = 3;
			this.label6.Text = "3.Text to find";
			// 
			// txtSearchKey3
			// 
			this.txtSearchKey3.Location = new System.Drawing.Point(343, 258);
			this.txtSearchKey3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey3.Name = "txtSearchKey3";
			this.txtSearchKey3.Size = new System.Drawing.Size(399, 29);
			this.txtSearchKey3.TabIndex = 4;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(154, 308);
			this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(124, 25);
			this.label8.TabIndex = 3;
			this.label8.Text = "4.Text to find";
			// 
			// txtSearchKey4
			// 
			this.txtSearchKey4.Location = new System.Drawing.Point(343, 308);
			this.txtSearchKey4.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.txtSearchKey4.Name = "txtSearchKey4";
			this.txtSearchKey4.Size = new System.Drawing.Size(399, 29);
			this.txtSearchKey4.TabIndex = 4;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(112, 702);
			this.button1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(207, 46);
			this.button1.TabIndex = 7;
			this.button1.Text = "Choose File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1Click);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(343, 710);
			this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(133, 25);
			this.label12.TabIndex = 8;
			this.label12.Text = "Heuristics File";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(486, 710);
			this.textBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(256, 29);
			this.textBox1.TabIndex = 9;
			this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(834, 931);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnReplace);
			this.Controls.Add(this.txtReplace1);
			this.Controls.Add(this.txtSearchKey4);
			this.Controls.Add(this.txtSearchKey3);
			this.Controls.Add(this.txtSearchKey1);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtDocDirectory);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSelectFile);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.Name = "MainForm";
			this.Text = "word pdf search";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Button btnSelectFile;
		private Label label1;
		private TextBox txtDocDirectory;
		private Label label2;
		private Label label3;
		private TextBox txtSearchKey1;
		private TextBox txtReplace1;
		private Button button2;
		private Button button3;
		private Label label6;
		private TextBox txtSearchKey3;
		private Label label8;
		private TextBox txtSearchKey4;
		private Button btnReplace;
		private Button btnClose;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBox1;
	}
}

