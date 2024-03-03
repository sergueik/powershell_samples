using System;
using System.Windows.Forms;
using System.Drawing;

namespace Program  {
	public class ClosePromptForm : Form {
	
		private Button okButton;
		private Label messageText;
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.okButton = new Button();
			this.messageText = new Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = DialogResult.OK;
			this.okButton.Location = new Point(200, 67);
			this.okButton.Name = "okButton";
			this.okButton.Size = new Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// messageText
			// 
			this.messageText.AutoSize = true;
			this.messageText.Font = new Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.messageText.Location = new Point(12, 25);
			this.messageText.MaximumSize = new Size(460, 80);
			this.messageText.Name = "messageText";
			this.messageText.Size = new Size(0, 13);
			this.messageText.TabIndex = 1;
			// 
			// ClosePromptForm
			// 
			this.AutoScaleDimensions = new SizeF(6F, 13F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(474, 102);
			this.Controls.Add(this.messageText);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ClosePromptForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Application needs to be closed";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		public ClosePromptForm(string text) {
			InitializeComponent();
			messageText.Text = text;
		}

		private void OkButtonClick(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
