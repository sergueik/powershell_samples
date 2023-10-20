namespace LoginTest
{
  partial class frmLogIn
  {
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
      if (disposing && (components != null))
      {
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
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnLogin = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.tbName = new System.Windows.Forms.TextBox();
      this.tbPassword = new System.Windows.Forms.TextBox();
      this.linkLabel1 = new System.Windows.Forms.LinkLabel();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.label3 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(262, 187);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(43, 187);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(75, 23);
      this.btnLogin.TabIndex = 2;
      this.btnLogin.Text = "Login";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(52, 52);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(62, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Login name";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(52, 105);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Password";
      // 
      // tbName
      // 
      this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tbName.Location = new System.Drawing.Point(55, 69);
      this.tbName.Name = "tbName";
      this.tbName.Size = new System.Drawing.Size(273, 20);
      this.tbName.TabIndex = 0;
      this.tbName.Tag = "0";
      // 
      // tbPassword
      // 
      this.tbPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tbPassword.Location = new System.Drawing.Point(55, 122);
      this.tbPassword.Name = "tbPassword";
      this.tbPassword.PasswordChar = '*';
      this.tbPassword.Size = new System.Drawing.Size(273, 20);
      this.tbPassword.TabIndex = 1;
      this.tbPassword.Tag = "1";
      // 
      // linkLabel1
      // 
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.linkLabel1.Location = new System.Drawing.Point(42, 155);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new System.Drawing.Size(297, 17);
      this.linkLabel1.TabIndex = 6;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "If you are not registered. Click here to register";
      this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
      this.label3.Location = new System.Drawing.Point(148, 4);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(87, 31);
      this.label3.TabIndex = 7;
      this.label3.Text = "Login ";
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(381, 40);
      this.panel1.TabIndex = 8;
      // 
      // frmLogIn
      // 
      this.AcceptButton = this.btnLogin;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(381, 229);
      this.ControlBox = false;
      this.Controls.Add(this.label3);
      this.Controls.Add(this.linkLabel1);
      this.Controls.Add(this.tbPassword);
      this.Controls.Add(this.tbName);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnLogin);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.panel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "frmLogIn";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Login Form";
      this.Load += new System.EventHandler(this.frmLogIn_Load);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tbName;
    private System.Windows.Forms.TextBox tbPassword;
    private System.Windows.Forms.LinkLabel linkLabel1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Panel panel1;
  }
}

