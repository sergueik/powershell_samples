using System.Windows.Forms;
using System.ComponentModel;

namespace RealTimeEventLogReader
{
	partial class LogNamePrompt
	{
		private IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			lblLogName = new Label();
			txtLogName = new TextBox();
			btnCancel = new Button();
			btnOk = new Button();
			SuspendLayout();
			// 
			// lblLogName
			// 
			lblLogName.AutoSize = true;
			lblLogName.Location = new System.Drawing.Point(12, 9);
			lblLogName.Name = "lblLogName";
			lblLogName.Size = new System.Drawing.Size(56, 13);
			lblLogName.TabIndex = 0;
			lblLogName.Text = "Log Name";
			// 
			// txtLogName
			// 
			txtLogName.Location = new System.Drawing.Point(75, 6);
			txtLogName.Name = "txtLogName";
			txtLogName.Size = new System.Drawing.Size(287, 20);
			txtLogName.TabIndex = 1;
			// 
			// btnCancel
			// 
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new System.Drawing.Point(287, 44);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(75, 23);
			btnCancel.TabIndex = 2;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			btnOk.Location = new System.Drawing.Point(206, 44);
			btnOk.Name = "btnOk";
			btnOk.Size = new System.Drawing.Size(75, 23);
			btnOk.TabIndex = 3;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			// 
			// LogNamePrompt
			// 
			AcceptButton = btnOk;
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = btnCancel;
			ClientSize = new System.Drawing.Size(374, 79);
			Controls.Add(btnOk);
			Controls.Add(btnCancel);
			Controls.Add(txtLogName);
			Controls.Add(lblLogName);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Name = "LogNamePrompt";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Enter Log Name";
			ResumeLayout(false);
			PerformLayout();

		}

		private Label lblLogName;
		private TextBox txtLogName;
		private Button btnCancel;
		private Button btnOk;
	}
}
