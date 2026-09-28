// using CCWin;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordFile
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码
// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
        private void InitializeComponent()
        {
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
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSearchKey2 = new System.Windows.Forms.TextBox();
            this.txtReplace2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSearchKey3 = new System.Windows.Forms.TextBox();
            this.txtReplace3 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSearchKey4 = new System.Windows.Forms.TextBox();
            this.txtReplace4 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSearchKey5 = new System.Windows.Forms.TextBox();
            this.txtReplace5 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(67, 71);
            this.btnSelectFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(169, 38);
            this.btnSelectFile.TabIndex = 0;
            this.btnSelectFile.Text = "Select the folder";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(278, 89);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "The folder containing the document is：";
            // 
            // txtDocDirectory
            // 
            this.txtDocDirectory.Location = new System.Drawing.Point(430, 82);
            this.txtDocDirectory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDocDirectory.Name = "txtDocDirectory";
            this.txtDocDirectory.Size = new System.Drawing.Size(210, 27);
            this.txtDocDirectory.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 134);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "1.Text to find";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Tan;
            this.label3.Location = new System.Drawing.Point(126, 179);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Text to be replaced";
            // 
            // txtSearchKey1
            // 
            this.txtSearchKey1.Location = new System.Drawing.Point(279, 130);
            this.txtSearchKey1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchKey1.Name = "txtSearchKey1";
            this.txtSearchKey1.Size = new System.Drawing.Size(280, 27);
            this.txtSearchKey1.TabIndex = 4;
            // 
            // txtReplace1
            // 
            this.txtReplace1.BackColor = System.Drawing.SystemColors.Window;
            this.txtReplace1.Location = new System.Drawing.Point(279, 174);
            this.txtReplace1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReplace1.Name = "txtReplace1";
            this.txtReplace1.Size = new System.Drawing.Size(280, 27);
            this.txtReplace1.TabIndex = 4;
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(218, 608);
            this.btnReplace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(112, 38);
            this.btnReplace.TabIndex = 5;
            this.btnReplace.Text = "Start replacing";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplaceText_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(380, 606);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 38);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Closure";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 227);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "2.Text to find";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Tan;
            this.label5.Location = new System.Drawing.Point(126, 272);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Text to be replaced";
            // 
            // txtSearchKey2
            // 
            this.txtSearchKey2.Location = new System.Drawing.Point(279, 224);
            this.txtSearchKey2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchKey2.Name = "txtSearchKey2";
            this.txtSearchKey2.Size = new System.Drawing.Size(280, 27);
            this.txtSearchKey2.TabIndex = 4;
            // 
            // txtReplace2
            // 
            this.txtReplace2.Location = new System.Drawing.Point(279, 267);
            this.txtReplace2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReplace2.Name = "txtReplace2";
            this.txtReplace2.Size = new System.Drawing.Size(280, 27);
            this.txtReplace2.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(126, 317);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "3.Text to find";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Tan;
            this.label7.Location = new System.Drawing.Point(126, 362);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Text to be replaced";
            // 
            // txtSearchKey3
            // 
            this.txtSearchKey3.Location = new System.Drawing.Point(279, 314);
            this.txtSearchKey3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchKey3.Name = "txtSearchKey3";
            this.txtSearchKey3.Size = new System.Drawing.Size(280, 27);
            this.txtSearchKey3.TabIndex = 4;
            // 
            // txtReplace3
            // 
            this.txtReplace3.Location = new System.Drawing.Point(279, 357);
            this.txtReplace3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReplace3.Name = "txtReplace3";
            this.txtReplace3.Size = new System.Drawing.Size(280, 27);
            this.txtReplace3.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(126, 410);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(112, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "4.Text to find";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Tan;
            this.label9.Location = new System.Drawing.Point(126, 455);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 20);
            this.label9.TabIndex = 3;
            this.label9.Text = "Text to be replaced";
            // 
            // txtSearchKey4
            // 
            this.txtSearchKey4.Location = new System.Drawing.Point(279, 407);
            this.txtSearchKey4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchKey4.Name = "txtSearchKey4";
            this.txtSearchKey4.Size = new System.Drawing.Size(280, 27);
            this.txtSearchKey4.TabIndex = 4;
            // 
            // txtReplace4
            // 
            this.txtReplace4.Location = new System.Drawing.Point(279, 450);
            this.txtReplace4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReplace4.Name = "txtReplace4";
            this.txtReplace4.Size = new System.Drawing.Size(280, 27);
            this.txtReplace4.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(124, 502);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "5.Text to find";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Tan;
            this.label11.Location = new System.Drawing.Point(124, 547);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "Text to be replaced";
            // 
            // txtSearchKey5
            // 
            this.txtSearchKey5.Location = new System.Drawing.Point(278, 499);
            this.txtSearchKey5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearchKey5.Name = "txtSearchKey5";
            this.txtSearchKey5.Size = new System.Drawing.Size(280, 27);
            this.txtSearchKey5.TabIndex = 4;
            // 
            // txtReplace5
            // 
            this.txtReplace5.Location = new System.Drawing.Point(278, 542);
            this.txtReplace5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtReplace5.Name = "txtReplace5";
            this.txtReplace5.Size = new System.Drawing.Size(280, 27);
            this.txtReplace5.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 703);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txtReplace5);
            this.Controls.Add(this.txtReplace4);
            this.Controls.Add(this.txtReplace3);
            this.Controls.Add(this.txtReplace2);
            this.Controls.Add(this.txtReplace1);
            this.Controls.Add(this.txtSearchKey5);
            this.Controls.Add(this.txtSearchKey4);
            this.Controls.Add(this.txtSearchKey3);
            this.Controls.Add(this.txtSearchKey2);
            this.Controls.Add(this.txtSearchKey1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDocDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSelectFile);
            //System.Resources.MissingManifestResourceException: Could not find any resources appropriate for the specified culture or the neutral culture.  Make sure "WordFile.MainForm.resources" was correctly embedded or linked into assembly "WordFile" at compile time, or that all the satellite assemblies required are loadable and fully signed.
//   at System.Resources.ManifestBasedResourceGroveler.HandleResourceStreamMissing(String fileName)

//            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "word字符替换程序";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDocDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearchKey1;
        private System.Windows.Forms.TextBox txtReplace1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSearchKey2;
        private System.Windows.Forms.TextBox txtReplace2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSearchKey3;
        private System.Windows.Forms.TextBox txtReplace3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSearchKey4;
        private System.Windows.Forms.TextBox txtReplace4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtSearchKey5;
        private System.Windows.Forms.TextBox txtReplace5;
        private Button btnReplace;
        private Button btnClose;
    }
}

