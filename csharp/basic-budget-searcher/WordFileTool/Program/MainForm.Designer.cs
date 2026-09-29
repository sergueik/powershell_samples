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
			btnSelectFile = new Button();
			label1 = new Label();
			txtDocDirectory = new TextBox();
			label2 = new Label();
			label3 = new Label();
			txtSearchKey1 = new TextBox();
			txtReplace1 = new TextBox();
			btnReplace = new Button();
			btnClose = new Button();
			label4 = new Label();
			label5 = new Label();
			txtSearchKey2 = new TextBox();
			txtReplace2 = new TextBox();
			label6 = new Label();
			label7 = new Label();
			txtSearchKey3 = new TextBox();
			txtReplace3 = new TextBox();
			label8 = new Label();
			label9 = new Label();
			txtSearchKey4 = new TextBox();
			txtReplace4 = new TextBox();
			label10 = new Label();
			label11 = new Label();
			txtSearchKey5 = new TextBox();
			txtReplace5 = new TextBox();
			SuspendLayout();
			// 
			// btnSelectFile
			// 
			btnSelectFile.Location = new Point(67, 71);
			btnSelectFile.Margin = new Padding(4, 5, 4, 5);
			btnSelectFile.Name = "btnSelectFile";
			btnSelectFile.Size = new Size(169, 38);
			btnSelectFile.TabIndex = 0;
			btnSelectFile.Text = "Select the folder";
			btnSelectFile.UseVisualStyleBackColor = true;
			btnSelectFile.Click += new System.EventHandler(btnSelectFile_Click);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(278, 89);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(144, 20);
			label1.TabIndex = 1;
			label1.Text = "The root folder：";
			// 
			// txtDocDirectory
			// 
			txtDocDirectory.Location = new Point(430, 82);
			txtDocDirectory.Margin = new Padding(4, 5, 4, 5);
			txtDocDirectory.Name = "txtDocDirectory";
			txtDocDirectory.Size = new Size(210, 27);
			txtDocDirectory.TabIndex = 2;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(126, 134);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(112, 20);
			label2.TabIndex = 3;
			label2.Text = "1.Text to find";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = Color.Tan;
			label3.Location = new Point(126, 179);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(99, 20);
			label3.TabIndex = 3;
			label3.Text = "Text to be replaced";
			// 
			// txtSearchKey1
			// 
			txtSearchKey1.Location = new Point(279, 130);
			txtSearchKey1.Margin = new Padding(4, 5, 4, 5);
			txtSearchKey1.Name = "txtSearchKey1";
			txtSearchKey1.Size = new Size(280, 27);
			txtSearchKey1.TabIndex = 4;
			// 
			// txtReplace1
			// 
			txtReplace1.BackColor = SystemColors.Window;
			txtReplace1.Location = new Point(279, 174);
			txtReplace1.Margin = new Padding(4, 5, 4, 5);
			txtReplace1.Name = "txtReplace1";
			txtReplace1.Size = new Size(280, 27);
			txtReplace1.TabIndex = 4;
			// 
			// btnReplace
			// 
			btnReplace.Location = new Point(218, 608);
			btnReplace.Margin = new Padding(4, 5, 4, 5);
			btnReplace.Name = "btnReplace";
			btnReplace.Size = new Size(112, 38);
			btnReplace.TabIndex = 5;
			btnReplace.Text = "Start replacing";
			btnReplace.UseVisualStyleBackColor = true;
			btnReplace.Click += new System.EventHandler(btnReplaceText_Click);
			// 
			// btnClose
			// 
			btnClose.Location = new Point(380, 606);
			btnClose.Margin = new Padding(4, 5, 4, 5);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(112, 38);
			btnClose.TabIndex = 6;
			btnClose.Text = "Close";
			btnClose.UseVisualStyleBackColor = true;
			btnClose.Click += new System.EventHandler(btnClose_Click);
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(126, 227);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(112, 20);
			label4.TabIndex = 3;
			label4.Text = "2.Text to find";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = Color.Tan;
			label5.Location = new Point(126, 272);
			label5.Margin = new Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new Size(99, 20);
			label5.TabIndex = 3;
			label5.Text = "Text to be replaced";
			// 
			// txtSearchKey2
			// 
			txtSearchKey2.Location = new Point(279, 224);
			txtSearchKey2.Margin = new Padding(4, 5, 4, 5);
			txtSearchKey2.Name = "txtSearchKey2";
			txtSearchKey2.Size = new Size(280, 27);
			txtSearchKey2.TabIndex = 4;
			// 
			// txtReplace2
			// 
			txtReplace2.Location = new Point(279, 267);
			txtReplace2.Margin = new Padding(4, 5, 4, 5);
			txtReplace2.Name = "txtReplace2";
			txtReplace2.Size = new Size(280, 27);
			txtReplace2.TabIndex = 4;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(126, 317);
			label6.Margin = new Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new Size(112, 20);
			label6.TabIndex = 3;
			label6.Text = "3.Text to find";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.BackColor = Color.Tan;
			label7.Location = new Point(126, 362);
			label7.Margin = new Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new Size(99, 20);
			label7.TabIndex = 3;
			label7.Text = "Text to be replaced";
			// 
			// txtSearchKey3
			// 
			txtSearchKey3.Location = new Point(279, 314);
			txtSearchKey3.Margin = new Padding(4, 5, 4, 5);
			txtSearchKey3.Name = "txtSearchKey3";
			txtSearchKey3.Size = new Size(280, 27);
			txtSearchKey3.TabIndex = 4;
			// 
			// txtReplace3
			// 
			txtReplace3.Location = new Point(279, 357);
			txtReplace3.Margin = new Padding(4, 5, 4, 5);
			txtReplace3.Name = "txtReplace3";
			txtReplace3.Size = new Size(280, 27);
			txtReplace3.TabIndex = 4;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point(126, 410);
			label8.Margin = new Padding(4, 0, 4, 0);
			label8.Name = "label8";
			label8.Size = new Size(112, 20);
			label8.TabIndex = 3;
			label8.Text = "4.Text to find";
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.BackColor = Color.Tan;
			label9.Location = new Point(126, 455);
			label9.Margin = new Padding(4, 0, 4, 0);
			label9.Name = "label9";
			label9.Size = new Size(99, 20);
			label9.TabIndex = 3;
			label9.Text = "Text to be replaced";
			// 
			// txtSearchKey4
			// 
			txtSearchKey4.Location = new Point(279, 407);
			txtSearchKey4.Margin = new Padding(4, 5, 4, 5);
			txtSearchKey4.Name = "txtSearchKey4";
			txtSearchKey4.Size = new Size(280, 27);
			txtSearchKey4.TabIndex = 4;
			// 
			// txtReplace4
			// 
			txtReplace4.Location = new Point(279, 450);
			txtReplace4.Margin = new Padding(4, 5, 4, 5);
			txtReplace4.Name = "txtReplace4";
			txtReplace4.Size = new Size(280, 27);
			txtReplace4.TabIndex = 4;
			// 
			// label10
			// 
			label10.AutoSize = true;
			label10.Location = new Point(124, 502);
			label10.Margin = new Padding(4, 0, 4, 0);
			label10.Name = "label10";
			label10.Size = new Size(112, 20);
			label10.TabIndex = 3;
			label10.Text = "5.Text to find";
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.BackColor = Color.Tan;
			label11.Location = new Point(124, 547);
			label11.Margin = new Padding(4, 0, 4, 0);
			label11.Name = "label11";
			label11.Size = new Size(99, 20);
			label11.TabIndex = 3;
			label11.Text = "Text to be replaced";
			// 
			// txtSearchKey5
			// 
			txtSearchKey5.Location = new Point(278, 499);
			txtSearchKey5.Margin = new Padding(4, 5, 4, 5);
			txtSearchKey5.Name = "txtSearchKey5";
			txtSearchKey5.Size = new Size(280, 27);
			txtSearchKey5.TabIndex = 4;
			// 
			// txtReplace5
			// 
			txtReplace5.Location = new Point(278, 542);
			txtReplace5.Margin = new Padding(4, 5, 4, 5);
			txtReplace5.Name = "txtReplace5";
			txtReplace5.Size = new Size(280, 27);
			txtReplace5.TabIndex = 4;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(9F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(682, 703);
			Controls.Add(btnClose);
			Controls.Add(btnReplace);
			Controls.Add(txtReplace5);
			Controls.Add(txtReplace4);
			Controls.Add(txtReplace3);
			Controls.Add(txtReplace2);
			Controls.Add(txtReplace1);
			Controls.Add(txtSearchKey5);
			Controls.Add(txtSearchKey4);
			Controls.Add(txtSearchKey3);
			Controls.Add(txtSearchKey2);
			Controls.Add(txtSearchKey1);
			Controls.Add(label11);
			Controls.Add(label10);
			Controls.Add(label9);
			Controls.Add(label8);
			Controls.Add(label7);
			Controls.Add(label6);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(txtDocDirectory);
			Controls.Add(label1);
			Controls.Add(btnSelectFile);
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			Margin = new Padding(4, 5, 4, 5);
			Name = "MainForm";
			Text = "word search replace";
			ResumeLayout(false);
			PerformLayout();

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
		private Label label4;
		private Label label5;
		private TextBox txtSearchKey2;
		private TextBox txtReplace2;
		private Label label6;
		private Label label7;
		private TextBox txtSearchKey3;
		private TextBox txtReplace3;
		private Label label8;
		private Label label9;
		private TextBox txtSearchKey4;
		private TextBox txtReplace4;
		private Label label10;
		private Label label11;
		private TextBox txtSearchKey5;
		private TextBox txtReplace5;
		private Button btnReplace;
		private Button btnClose;
	}
}

