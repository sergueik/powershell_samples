
namespace Program {
    partial class FormFindNone {
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
            this.findNoneLabel = new System.Windows.Forms.Label();
            this.findNoneButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // findNoneLabel
            // 
            this.findNoneLabel.AutoSize = true;
            this.findNoneLabel.Location = new System.Drawing.Point(30, 30);
            this.findNoneLabel.Name = "findNoneLabel";
            this.findNoneLabel.Size = new System.Drawing.Size(0, 17);
            this.findNoneLabel.TabIndex = 0;
            // 
            // findNoneButton
            // 
            this.findNoneButton.Location = new System.Drawing.Point(170, 111);
            this.findNoneButton.Name = "findNoneButton";
            this.findNoneButton.Size = new System.Drawing.Size(100, 30);
            this.findNoneButton.TabIndex = 1;
            this.findNoneButton.Text = "ОК";
            this.findNoneButton.UseVisualStyleBackColor = true;
            this.findNoneButton.Click += new System.EventHandler(this.findNoneButton_Click);
            // 
            // FormFindNone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 153);
            this.Controls.Add(this.findNoneButton);
            this.Controls.Add(this.findNoneLabel);
            this.Name = "FormFindNone";
            this.Text = "Блокнот";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label findNoneLabel;
        private System.Windows.Forms.Button findNoneButton;
    }
}