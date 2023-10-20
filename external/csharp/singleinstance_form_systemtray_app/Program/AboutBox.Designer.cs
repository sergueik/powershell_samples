using System.Windows.Forms;
using System.Drawing;

namespace SystemTrayApp {
	partial class AboutBox {
		private TableLayoutPanel tableLayoutPanel;
		private PictureBox logoPictureBox;
		private Label labelProductName;
		private Label labelVersion;
		private Label labelCopyright;
		private Label labelCompanyName;
		private TextBox textBoxDescription;
		private Button okButton;
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			logoPictureBox = new System.Windows.Forms.PictureBox();
			labelProductName = new System.Windows.Forms.Label();
			labelVersion = new System.Windows.Forms.Label();
			labelCopyright = new System.Windows.Forms.Label();
			labelCompanyName = new System.Windows.Forms.Label();
			textBoxDescription = new System.Windows.Forms.TextBox();
			okButton = new System.Windows.Forms.Button();
			tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(logoPictureBox)).BeginInit();
			SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			tableLayoutPanel.ColumnCount = 2;
			tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
			tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
			tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
			tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
			tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
			tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
			tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
			tableLayoutPanel.Controls.Add(okButton, 1, 5);
			tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
			tableLayoutPanel.Name = "tableLayoutPanel";
			tableLayoutPanel.RowCount = 6;
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			tableLayoutPanel.Size = new System.Drawing.Size(417, 265);
			tableLayoutPanel.TabIndex = 0;
			// 
			// logoPictureBox
			// 
			logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			logoPictureBox.Location = new System.Drawing.Point(3, 3);
			logoPictureBox.Name = "logoPictureBox";
			tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
			logoPictureBox.Size = new System.Drawing.Size(131, 259);
			logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			logoPictureBox.TabIndex = 12;
			logoPictureBox.TabStop = false;
			// 
			// labelProductName
			// 
			labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
			labelProductName.Location = new System.Drawing.Point(143, 0);
			labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
			labelProductName.Name = "labelProductName";
			labelProductName.Size = new System.Drawing.Size(271, 17);
			labelProductName.TabIndex = 19;
			labelProductName.Text = "Product Name";
			labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			labelVersion.Location = new System.Drawing.Point(143, 26);
			labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
			labelVersion.Name = "labelVersion";
			labelVersion.Size = new System.Drawing.Size(271, 17);
			labelVersion.TabIndex = 0;
			labelVersion.Text = "Version";
			labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCopyright
			// 
			labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
			labelCopyright.Location = new System.Drawing.Point(143, 52);
			labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
			labelCopyright.Name = "labelCopyright";
			labelCopyright.Size = new System.Drawing.Size(271, 17);
			labelCopyright.TabIndex = 21;
			labelCopyright.Text = "Copyright";
			labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCompanyName
			// 
			labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
			labelCompanyName.Location = new System.Drawing.Point(143, 78);
			labelCompanyName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
			labelCompanyName.Name = "labelCompanyName";
			labelCompanyName.Size = new System.Drawing.Size(271, 17);
			labelCompanyName.TabIndex = 22;
			labelCompanyName.Text = "Company Name";
			labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxDescription
			// 
			textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			textBoxDescription.Location = new System.Drawing.Point(143, 107);
			textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			textBoxDescription.Multiline = true;
			textBoxDescription.Name = "textBoxDescription";
			textBoxDescription.ReadOnly = true;
			textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			textBoxDescription.Size = new System.Drawing.Size(271, 126);
			textBoxDescription.TabIndex = 23;
			textBoxDescription.TabStop = false;
			textBoxDescription.Text = "Description";
			// 
			// okButton
			// 
			okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			okButton.Location = new System.Drawing.Point(339, 239);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(75, 23);
			okButton.TabIndex = 24;
			okButton.Text = "&OK";
			// 
			// AboutBox
			// 
			AcceptButton = okButton;
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(435, 283);
			Controls.Add(tableLayoutPanel);
			Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutBox";
			Padding = new System.Windows.Forms.Padding(9);
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "About SystemTrayApp";
			tableLayoutPanel.ResumeLayout(false);
			tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(logoPictureBox)).EndInit();
			ResumeLayout(false);

		}

	}
}
