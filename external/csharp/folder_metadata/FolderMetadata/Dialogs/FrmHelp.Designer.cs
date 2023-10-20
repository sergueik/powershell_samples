
namespace HKS.FolderMetadata.Dialogs
{
	partial class FrmHelp
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
			this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
			this.btnClose = new System.Windows.Forms.Button();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.tlpButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpButtons
			// 
			this.tlpButtons.ColumnCount = 2;
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpButtons.Controls.Add(this.btnClose, 1, 0);
			this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tlpButtons.Location = new System.Drawing.Point(0, 415);
			this.tlpButtons.Name = "tlpButtons";
			this.tlpButtons.RowCount = 1;
			this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.Size = new System.Drawing.Size(800, 35);
			this.tlpButtons.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.AutoSize = true;
			this.btnClose.Location = new System.Drawing.Point(722, 3);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "&Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// webBrowser1
			// 
			this.webBrowser1.AllowNavigation = false;
			this.webBrowser1.AllowWebBrowserDrop = false;
			this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser1.Location = new System.Drawing.Point(0, 0);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.ScriptErrorsSuppressed = true;
			this.webBrowser1.Size = new System.Drawing.Size(800, 415);
			this.webBrowser1.TabIndex = 14;
			// 
			// FrmHelp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.tlpButtons);
			this.Name = "FrmHelp";
			this.Text = "FrmHelp";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.tlpButtons.ResumeLayout(false);
			this.tlpButtons.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpButtons;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.WebBrowser webBrowser1;
	}
}