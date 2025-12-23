
namespace Program {
    partial class FormFind
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.findTextField = new System.Windows.Forms.TextBox();
            this.findLabel1 = new System.Windows.Forms.Label();
            this.findButtonFindMore = new System.Windows.Forms.Button();
            this.findButtonCancel = new System.Windows.Forms.Button();
            this.findCheckRegister = new System.Windows.Forms.CheckBox();
            this.findCheckFlowing = new System.Windows.Forms.CheckBox();
            this.findGroupBoxRadioButtonUp = new System.Windows.Forms.RadioButton();
            this.findGroupBoxRadio = new System.Windows.Forms.GroupBox();
            this.findGroupBoxRadioButtonDown = new System.Windows.Forms.RadioButton();
            this.findLabel2 = new System.Windows.Forms.Label();
            this.findGroupBoxRadio.SuspendLayout();
            this.SuspendLayout();
            // 
            // findTextField
            // 
            this.findTextField.Location = new System.Drawing.Point(60, 20);
            this.findTextField.Name = "findTextField";
            this.findTextField.Size = new System.Drawing.Size(250, 22);
            this.findTextField.TabIndex = 0;
            // 
            // findLabel1
            // 
            this.findLabel1.AutoSize = true;
            this.findLabel1.Location = new System.Drawing.Point(20, 20);
            this.findLabel1.Name = "findLabel1";
            this.findLabel1.Size = new System.Drawing.Size(37, 17);
            this.findLabel1.TabIndex = 1;
            this.findLabel1.Text = "Что:";
            // 
            // findButtonFindMore
            // 
            this.findButtonFindMore.Location = new System.Drawing.Point(340, 20);
            this.findButtonFindMore.Name = "findButtonFindMore";
            this.findButtonFindMore.Size = new System.Drawing.Size(120, 30);
            this.findButtonFindMore.TabIndex = 2;
            this.findButtonFindMore.Text = "Найти далее";
            this.findButtonFindMore.UseVisualStyleBackColor = true;
            this.findButtonFindMore.Click += new System.EventHandler(this.findButtonFindMore_Click);
            // 
            // findButtonCancel
            // 
            this.findButtonCancel.Location = new System.Drawing.Point(340, 70);
            this.findButtonCancel.Name = "findButtonCancel";
            this.findButtonCancel.Size = new System.Drawing.Size(120, 30);
            this.findButtonCancel.TabIndex = 3;
            this.findButtonCancel.Text = "Отмена";
            this.findButtonCancel.UseVisualStyleBackColor = true;
            this.findButtonCancel.Click += new System.EventHandler(this.findButtonCancel_Click);
            // 
            // findCheckRegister
            // 
            this.findCheckRegister.AutoSize = true;
            this.findCheckRegister.Location = new System.Drawing.Point(23, 70);
            this.findCheckRegister.Name = "findCheckRegister";
            this.findCheckRegister.Size = new System.Drawing.Size(153, 21);
            this.findCheckRegister.TabIndex = 4;
            this.findCheckRegister.Text = "С учетом регистра";
            this.findCheckRegister.UseVisualStyleBackColor = true;
            // 
            // findCheckFlowing
            // 
            this.findCheckFlowing.AutoSize = true;
            this.findCheckFlowing.Location = new System.Drawing.Point(23, 98);
            this.findCheckFlowing.Name = "findCheckFlowing";
            this.findCheckFlowing.Size = new System.Drawing.Size(160, 21);
            this.findCheckFlowing.TabIndex = 5;
            this.findCheckFlowing.Text = "Обтекание текстом";
            this.findCheckFlowing.UseVisualStyleBackColor = true;
            this.findCheckFlowing.Visible = false;
            // 
            // findGroupBoxRadioButtonUp
            // 
            this.findGroupBoxRadioButtonUp.AutoSize = true;
            this.findGroupBoxRadioButtonUp.Location = new System.Drawing.Point(14, 22);
            this.findGroupBoxRadioButtonUp.Name = "findGroupBoxRadioButtonUp";
            this.findGroupBoxRadioButtonUp.Size = new System.Drawing.Size(67, 21);
            this.findGroupBoxRadioButtonUp.TabIndex = 6;
            this.findGroupBoxRadioButtonUp.Text = "Вверх";
            this.findGroupBoxRadioButtonUp.UseVisualStyleBackColor = true;
            // 
            // findGroupBoxRadio
            // 
            this.findGroupBoxRadio.Controls.Add(this.findGroupBoxRadioButtonDown);
            this.findGroupBoxRadio.Controls.Add(this.findGroupBoxRadioButtonUp);
            this.findGroupBoxRadio.Location = new System.Drawing.Point(182, 48);
            this.findGroupBoxRadio.Name = "findGroupBoxRadio";
            this.findGroupBoxRadio.Size = new System.Drawing.Size(152, 77);
            this.findGroupBoxRadio.TabIndex = 7;
            this.findGroupBoxRadio.TabStop = false;
            this.findGroupBoxRadio.Text = "Направление:";
            // 
            // findGroupBoxRadioButtonDown
            // 
            this.findGroupBoxRadioButtonDown.AutoSize = true;
            this.findGroupBoxRadioButtonDown.Checked = true;
            this.findGroupBoxRadioButtonDown.Location = new System.Drawing.Point(14, 50);
            this.findGroupBoxRadioButtonDown.Name = "findGroupBoxRadioButtonDown";
            this.findGroupBoxRadioButtonDown.Size = new System.Drawing.Size(61, 21);
            this.findGroupBoxRadioButtonDown.TabIndex = 7;
            this.findGroupBoxRadioButtonDown.TabStop = true;
            this.findGroupBoxRadioButtonDown.Text = "Вниз";
            this.findGroupBoxRadioButtonDown.UseVisualStyleBackColor = true;
            // 
            // findLabel2
            // 
            this.findLabel2.AutoSize = true;
            this.findLabel2.Location = new System.Drawing.Point(23, 126);
            this.findLabel2.Name = "findLabel2";
            this.findLabel2.Size = new System.Drawing.Size(0, 17);
            this.findLabel2.TabIndex = 8;
            // 
            // FormFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 153);
            this.Controls.Add(this.findLabel2);
            this.Controls.Add(this.findGroupBoxRadio);
            this.Controls.Add(this.findCheckFlowing);
            this.Controls.Add(this.findCheckRegister);
            this.Controls.Add(this.findButtonCancel);
            this.Controls.Add(this.findButtonFindMore);
            this.Controls.Add(this.findLabel1);
            this.Controls.Add(this.findTextField);
            this.Name = "FormFind";
            this.Text = "Найти";
            this.findGroupBoxRadio.ResumeLayout(false);
            this.findGroupBoxRadio.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox findTextField;
        private System.Windows.Forms.Label findLabel1;
        private System.Windows.Forms.Button findButtonFindMore;
        private System.Windows.Forms.Button findButtonCancel;
        private System.Windows.Forms.CheckBox findCheckRegister;
        private System.Windows.Forms.CheckBox findCheckFlowing;
        private System.Windows.Forms.RadioButton findGroupBoxRadioButtonUp;
        private System.Windows.Forms.GroupBox findGroupBoxRadio;
        private System.Windows.Forms.RadioButton findGroupBoxRadioButtonDown;
        private System.Windows.Forms.Label findLabel2;
    }
}