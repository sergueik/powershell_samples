namespace SerialisationTest {
  partial class FormToBeSerialised {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
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
    private void InitializeComponent() {
      this.label1 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.chkHappy = new System.Windows.Forms.CheckBox();
      this.grpHappy = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.cmbWhy = new System.Windows.Forms.ComboBox();
      this.btnOK = new System.Windows.Forms.Button();
      this.chkSerialise = new System.Windows.Forms.CheckBox();
      this.lstInterests = new System.Windows.Forms.ListBox();
      this.userControl12 = new SerialisationTest.UserControl1();
      this.userControl11 = new SerialisationTest.UserControl1();
      this.grpHappy.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name";
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(63, 10);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(100, 20);
      this.txtName.TabIndex = 1;
      // 
      // chkHappy
      // 
      this.chkHappy.AutoSize = true;
      this.chkHappy.Checked = true;
      this.chkHappy.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkHappy.Location = new System.Drawing.Point(63, 37);
      this.chkHappy.Name = "chkHappy";
      this.chkHappy.Size = new System.Drawing.Size(100, 17);
      this.chkHappy.TabIndex = 2;
      this.chkHappy.Text = "Are you happy?";
      this.chkHappy.UseVisualStyleBackColor = true;
      this.chkHappy.CheckedChanged += new System.EventHandler(this.chkHappy_CheckedChanged);
      // 
      // grpHappy
      // 
      this.grpHappy.Controls.Add(this.label2);
      this.grpHappy.Controls.Add(this.cmbWhy);
      this.grpHappy.Location = new System.Drawing.Point(63, 61);
      this.grpHappy.Name = "grpHappy";
      this.grpHappy.Size = new System.Drawing.Size(200, 54);
      this.grpHappy.TabIndex = 3;
      this.grpHappy.TabStop = false;
      this.grpHappy.Text = "Happy";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(7, 23);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Why?";
      // 
      // cmbWhy
      // 
      this.cmbWhy.FormattingEnabled = true;
      this.cmbWhy.Items.AddRange(new object[] {
            "because I\'m awake",
            "because I have everything I need",
            "why not?"});
      this.cmbWhy.Location = new System.Drawing.Point(48, 20);
      this.cmbWhy.Name = "cmbWhy";
      this.cmbWhy.Size = new System.Drawing.Size(121, 21);
      this.cmbWhy.TabIndex = 0;
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(63, 286);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 4;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // chkSerialise
      // 
      this.chkSerialise.AutoSize = true;
      this.chkSerialise.Checked = true;
      this.chkSerialise.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkSerialise.Location = new System.Drawing.Point(144, 290);
      this.chkSerialise.Name = "chkSerialise";
      this.chkSerialise.Size = new System.Drawing.Size(71, 17);
      this.chkSerialise.TabIndex = 5;
      this.chkSerialise.Text = "Serialise?";
      this.chkSerialise.UseVisualStyleBackColor = true;
      // 
      // lstInterests
      // 
      this.lstInterests.FormattingEnabled = true;
      this.lstInterests.Items.AddRange(new object[] {
            "swimming",
            "cycling",
            "sailing",
            "golf",
            "going to the gym",
            "reading interesting books",
            "writing C# programs"});
      this.lstInterests.Location = new System.Drawing.Point(63, 185);
      this.lstInterests.Name = "lstInterests";
      this.lstInterests.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
      this.lstInterests.Size = new System.Drawing.Size(200, 95);
      this.lstInterests.TabIndex = 8;
      // 
      // userControl12
      // 
      this.userControl12.Location = new System.Drawing.Point(63, 155);
      this.userControl12.Name = "userControl12";
      this.userControl12.Size = new System.Drawing.Size(274, 27);
      this.userControl12.TabIndex = 7;
      // 
      // userControl11
      // 
      this.userControl11.Location = new System.Drawing.Point(63, 122);
      this.userControl11.Name = "userControl11";
      this.userControl11.Size = new System.Drawing.Size(274, 27);
      this.userControl11.TabIndex = 6;
      // 
      // FormToBeSerialised
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(281, 321);
      this.Controls.Add(this.lstInterests);
      this.Controls.Add(this.userControl12);
      this.Controls.Add(this.userControl11);
      this.Controls.Add(this.chkSerialise);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.grpHappy);
      this.Controls.Add(this.chkHappy);
      this.Controls.Add(this.txtName);
      this.Controls.Add(this.label1);
      this.Name = "FormToBeSerialised";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Child";
      this.Load += new System.EventHandler(this.FormToBeSerialised_Load);
      this.grpHappy.ResumeLayout(false);
      this.grpHappy.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.CheckBox chkHappy;
    private System.Windows.Forms.GroupBox grpHappy;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cmbWhy;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.CheckBox chkSerialise;
    private UserControl1 userControl11;
    private UserControl1 userControl12;
    private System.Windows.Forms.ListBox lstInterests;
  }
}