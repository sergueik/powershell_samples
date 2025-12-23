
namespace Program {
    partial class FormSave {
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
            this.saveButtonSave = new System.Windows.Forms.Button();
            this.saveButtonDontSave = new System.Windows.Forms.Button();
            this.saveButtonCancel = new System.Windows.Forms.Button();
            this.saveLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // saveButtonSave
            // 
            this.saveButtonSave.Location = new System.Drawing.Point(35, 90);
            this.saveButtonSave.Name = "saveButtonSave";
            this.saveButtonSave.Size = new System.Drawing.Size(120, 30);
            this.saveButtonSave.TabIndex = 0;
            this.saveButtonSave.Text = "Сохранить";
            this.saveButtonSave.UseVisualStyleBackColor = true;
            this.saveButtonSave.Click += new System.EventHandler(this.saveButtonSave_Click);
            // 
            // saveButtonDontSave
            // 
            this.saveButtonDontSave.Location = new System.Drawing.Point(190, 90);
            this.saveButtonDontSave.Name = "saveButtonDontSave";
            this.saveButtonDontSave.Size = new System.Drawing.Size(120, 30);
            this.saveButtonDontSave.TabIndex = 1;
            this.saveButtonDontSave.Text = "Не сохранять";
            this.saveButtonDontSave.UseVisualStyleBackColor = true;
            this.saveButtonDontSave.Click += new System.EventHandler(this.saveButtonDontSave_Click);
            // 
            // saveButtonCancel
            // 
            this.saveButtonCancel.Location = new System.Drawing.Point(345, 90);
            this.saveButtonCancel.Name = "saveButtonCancel";
            this.saveButtonCancel.Size = new System.Drawing.Size(120, 30);
            this.saveButtonCancel.TabIndex = 2;
            this.saveButtonCancel.Text = "Отмена";
            this.saveButtonCancel.UseVisualStyleBackColor = true;
            this.saveButtonCancel.Click += new System.EventHandler(this.saveButtonCancel_Click);
            // 
            // saveLabel
            // 
            this.saveLabel.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.saveLabel.AutoSize = true;
            this.saveLabel.Font = new System.Drawing.Font("Microsoft PhagsPa", 10.11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.saveLabel.ForeColor = System.Drawing.Color.SlateBlue;
            this.saveLabel.Location = new System.Drawing.Point(31, 9);
            this.saveLabel.Name = "saveLabel";
            this.saveLabel.Size = new System.Drawing.Size(363, 19);
            this.saveLabel.TabIndex = 3;
            this.saveLabel.Text = "Вы хотите сохранить изменнения в файле\r\n";
            // 
            // FormSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(482, 153);
            this.Controls.Add(this.saveLabel);
            this.Controls.Add(this.saveButtonCancel);
            this.Controls.Add(this.saveButtonDontSave);
            this.Controls.Add(this.saveButtonSave);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSave";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Блокнот";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButtonSave;
        private System.Windows.Forms.Button saveButtonDontSave;
        private System.Windows.Forms.Button saveButtonCancel;
        private System.Windows.Forms.Label saveLabel;
    }
}