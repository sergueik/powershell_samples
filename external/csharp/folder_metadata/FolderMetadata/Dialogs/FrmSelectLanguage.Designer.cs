
namespace HKS.FolderMetadata.Dialogs
{
	partial class FrmSelectLanguage
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
			this.btnOK = new System.Windows.Forms.Button();
			this.lvLanguages = new System.Windows.Forms.ListView();
			this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tlpButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpButtons
			// 
			this.tlpButtons.ColumnCount = 2;
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.Controls.Add(this.btnOK, 1, 0);
			this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tlpButtons.Location = new System.Drawing.Point(0, 418);
			this.tlpButtons.Name = "tlpButtons";
			this.tlpButtons.RowCount = 1;
			this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpButtons.Size = new System.Drawing.Size(800, 32);
			this.tlpButtons.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.AutoSize = true;
			this.btnOK.Enabled = false;
			this.btnOK.Location = new System.Drawing.Point(722, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lvLanguages
			// 
			this.lvLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colLanguage});
			this.lvLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvLanguages.FullRowSelect = true;
			this.lvLanguages.HideSelection = false;
			this.lvLanguages.Location = new System.Drawing.Point(0, 0);
			this.lvLanguages.Name = "lvLanguages";
			this.lvLanguages.Size = new System.Drawing.Size(800, 418);
			this.lvLanguages.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvLanguages.TabIndex = 2;
			this.lvLanguages.UseCompatibleStateImageBehavior = false;
			this.lvLanguages.View = System.Windows.Forms.View.Details;
			this.lvLanguages.SelectedIndexChanged += new System.EventHandler(this.lvLanguages_SelectedIndexChanged);
			this.lvLanguages.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvLanguages_KeyUp);
			// 
			// colName
			// 
			this.colName.Text = "Name";
			this.colName.Width = 250;
			// 
			// colLanguage
			// 
			this.colLanguage.Text = "Language";
			this.colLanguage.Width = 120;
			// 
			// FrmSelectLanguage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.lvLanguages);
			this.Controls.Add(this.tlpButtons);
			this.Name = "FrmSelectLanguage";
			this.Text = "Select Language";
			this.tlpButtons.ResumeLayout(false);
			this.tlpButtons.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpButtons;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ListView lvLanguages;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.ColumnHeader colLanguage;
	}
}