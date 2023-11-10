namespace WindowsFormsApplication2 {
	partial class DBIns {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtDBName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.chkBoxCreateDB = new System.Windows.Forms.CheckBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnNext = new System.Windows.Forms.Button();
			this.btnBack = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.txtDBName);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.chkBoxCreateDB);
			this.panel1.Controls.Add(this.txtPassword);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.txtUser);
			this.panel1.Controls.Add(this.txtServer);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(28, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(300, 250);
			this.panel1.TabIndex = 0;
			// 
			// txtDBName
			// 
			this.txtDBName.Location = new System.Drawing.Point(133, 53);
			this.txtDBName.Name = "txtDBName";
			this.txtDBName.Size = new System.Drawing.Size(120, 20);
			this.txtDBName.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(42, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Database Name";
			// 
			// chkBoxCreateDB
			// 
			this.chkBoxCreateDB.AutoSize = true;
			this.chkBoxCreateDB.Location = new System.Drawing.Point(57, 221);
			this.chkBoxCreateDB.Name = "chkBoxCreateDB";
			this.chkBoxCreateDB.Size = new System.Drawing.Size(107, 17);
			this.chkBoxCreateDB.TabIndex = 6;
			this.chkBoxCreateDB.Text = "Create DataBase";
			this.chkBoxCreateDB.UseVisualStyleBackColor = true;
			// 
			// txtPassword
			// 
			this.txtPassword.Location = new System.Drawing.Point(133, 122);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(120, 20);
			this.txtPassword.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(42, 129);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Password";
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(133, 88);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(120, 20);
			this.txtUser.TabIndex = 3;
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(133, 18);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(120, 20);
			this.txtServer.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(44, 92);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "User";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(44, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textBox1);
			this.panel2.Location = new System.Drawing.Point(31, 9);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(300, 250);
			this.panel2.TabIndex = 1;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Window;
			this.textBox1.Location = new System.Drawing.Point(15, 15);
			this.textBox1.MaxLength = 65000;
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(271, 216);
			this.textBox1.TabIndex = 0;
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(184, 295);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(100, 30);
			this.btnNext.TabIndex = 2;
			this.btnNext.Text = "Next->";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// btnBack
			// 
			this.btnBack.Location = new System.Drawing.Point(44, 293);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(100, 30);
			this.btnBack.TabIndex = 3;
			this.btnBack.Text = "<-Back";
			this.btnBack.UseVisualStyleBackColor = true;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// DBIns
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(361, 353);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnBack);
			this.Controls.Add(this.btnNext);
			this.Name = "DBIns";
			this.Text = "DBIns";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnBack;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.CheckBox chkBoxCreateDB;
		private System.Windows.Forms.TextBox txtDBName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ErrorProvider errorProvider1;
	}
}

