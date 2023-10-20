
namespace HKS.FolderMetadata.Dialogs
{
	partial class FrmMain
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
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.tlpInputControls = new System.Windows.Forms.TableLayoutPanel();
			this.txtTitle = new System.Windows.Forms.TextBox();
			this.lstTags = new HKS.FolderMetadata.Dialogs.Controls.ListBoxEx();
			this.btnManageTags = new System.Windows.Forms.Button();
			this.txtSubject = new System.Windows.Forms.TextBox();
			this.txtComment = new System.Windows.Forms.TextBox();
			this.btnManageAuthors = new System.Windows.Forms.Button();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnlSubject = new System.Windows.Forms.Panel();
			this.lblSubject = new System.Windows.Forms.Label();
			this.pnlAuthor = new System.Windows.Forms.Panel();
			this.lblAuthor = new System.Windows.Forms.Label();
			this.pnlComment = new System.Windows.Forms.Panel();
			this.lblComment = new System.Windows.Forms.Label();
			this.pnlTags = new System.Windows.Forms.Panel();
			this.lblTags = new System.Windows.Forms.Label();
			this.lstAuthors = new HKS.FolderMetadata.Dialogs.Controls.ListBoxEx();
			this.tlpButtons.SuspendLayout();
			this.tlpInputControls.SuspendLayout();
			this.pnlTitle.SuspendLayout();
			this.pnlSubject.SuspendLayout();
			this.pnlAuthor.SuspendLayout();
			this.pnlComment.SuspendLayout();
			this.pnlTags.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpButtons
			// 
			this.tlpButtons.ColumnCount = 3;
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.Controls.Add(this.btnCancel, 2, 0);
			this.tlpButtons.Controls.Add(this.btnSave, 1, 0);
			this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tlpButtons.Location = new System.Drawing.Point(0, 326);
			this.tlpButtons.Name = "tlpButtons";
			this.tlpButtons.RowCount = 1;
			this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.Size = new System.Drawing.Size(584, 35);
			this.tlpButtons.TabIndex = 0;
			// 
			// btnCancel
			// 
			this.btnCancel.AutoSize = true;
			this.btnCancel.Location = new System.Drawing.Point(506, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.AutoSize = true;
			this.btnSave.Location = new System.Drawing.Point(425, 3);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 20;
			this.btnSave.Text = "&Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// tlpInputControls
			// 
			this.tlpInputControls.ColumnCount = 3;
			this.tlpInputControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpInputControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpInputControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpInputControls.Controls.Add(this.txtTitle, 1, 0);
			this.tlpInputControls.Controls.Add(this.lstTags, 1, 4);
			this.tlpInputControls.Controls.Add(this.btnManageTags, 2, 4);
			this.tlpInputControls.Controls.Add(this.txtSubject, 1, 1);
			this.tlpInputControls.Controls.Add(this.txtComment, 1, 3);
			this.tlpInputControls.Controls.Add(this.btnManageAuthors, 2, 2);
			this.tlpInputControls.Controls.Add(this.pnlTitle, 0, 0);
			this.tlpInputControls.Controls.Add(this.pnlSubject, 0, 1);
			this.tlpInputControls.Controls.Add(this.pnlAuthor, 0, 2);
			this.tlpInputControls.Controls.Add(this.pnlComment, 0, 3);
			this.tlpInputControls.Controls.Add(this.pnlTags, 0, 4);
			this.tlpInputControls.Controls.Add(this.lstAuthors, 1, 2);
			this.tlpInputControls.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpInputControls.Location = new System.Drawing.Point(0, 0);
			this.tlpInputControls.Name = "tlpInputControls";
			this.tlpInputControls.RowCount = 5;
			this.tlpInputControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInputControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInputControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpInputControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInputControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpInputControls.Size = new System.Drawing.Size(584, 326);
			this.tlpInputControls.TabIndex = 2;
			// 
			// txtTitle
			// 
			this.txtTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtTitle.Location = new System.Drawing.Point(69, 3);
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.Size = new System.Drawing.Size(431, 20);
			this.txtTitle.TabIndex = 5;
			// 
			// lstTags
			// 
			this.lstTags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstTags.FormattingEnabled = true;
			this.lstTags.Location = new System.Drawing.Point(69, 205);
			this.lstTags.Name = "lstTags";
			this.lstTags.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstTags.Size = new System.Drawing.Size(431, 118);
			this.lstTags.TabIndex = 18;
			this.lstTags.Click += new System.EventHandler(this.lst_Click);
			// 
			// btnManageTags
			// 
			this.btnManageTags.Location = new System.Drawing.Point(506, 205);
			this.btnManageTags.Name = "btnManageTags";
			this.btnManageTags.Size = new System.Drawing.Size(75, 23);
			this.btnManageTags.TabIndex = 19;
			this.btnManageTags.Text = "Manage";
			this.btnManageTags.UseVisualStyleBackColor = true;
			this.btnManageTags.Click += new System.EventHandler(this.btnManageTags_Click);
			// 
			// txtSubject
			// 
			this.txtSubject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSubject.Location = new System.Drawing.Point(69, 29);
			this.txtSubject.Name = "txtSubject";
			this.txtSubject.Size = new System.Drawing.Size(431, 20);
			this.txtSubject.TabIndex = 8;
			// 
			// txtComment
			// 
			this.txtComment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtComment.Location = new System.Drawing.Point(69, 179);
			this.txtComment.Name = "txtComment";
			this.txtComment.Size = new System.Drawing.Size(431, 20);
			this.txtComment.TabIndex = 15;
			// 
			// btnManageAuthors
			// 
			this.btnManageAuthors.Location = new System.Drawing.Point(506, 55);
			this.btnManageAuthors.Name = "btnManageAuthors";
			this.btnManageAuthors.Size = new System.Drawing.Size(75, 23);
			this.btnManageAuthors.TabIndex = 12;
			this.btnManageAuthors.Text = "Manage";
			this.btnManageAuthors.UseVisualStyleBackColor = true;
			this.btnManageAuthors.Click += new System.EventHandler(this.btnManageAuthors_Click);
			// 
			// pnlTitle
			// 
			this.pnlTitle.AutoSize = true;
			this.pnlTitle.Controls.Add(this.lblTitle);
			this.pnlTitle.Location = new System.Drawing.Point(3, 3);
			this.pnlTitle.Name = "pnlTitle";
			this.pnlTitle.Size = new System.Drawing.Size(36, 16);
			this.pnlTitle.TabIndex = 3;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Location = new System.Drawing.Point(3, 3);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(30, 13);
			this.lblTitle.TabIndex = 4;
			this.lblTitle.Text = "&Title:";
			// 
			// pnlSubject
			// 
			this.pnlSubject.AutoSize = true;
			this.pnlSubject.Controls.Add(this.lblSubject);
			this.pnlSubject.Location = new System.Drawing.Point(3, 29);
			this.pnlSubject.Name = "pnlSubject";
			this.pnlSubject.Size = new System.Drawing.Size(52, 16);
			this.pnlSubject.TabIndex = 6;
			// 
			// lblSubject
			// 
			this.lblSubject.AutoSize = true;
			this.lblSubject.Location = new System.Drawing.Point(3, 3);
			this.lblSubject.Name = "lblSubject";
			this.lblSubject.Size = new System.Drawing.Size(46, 13);
			this.lblSubject.TabIndex = 7;
			this.lblSubject.Text = "S&ubject:";
			// 
			// pnlAuthor
			// 
			this.pnlAuthor.AutoSize = true;
			this.pnlAuthor.Controls.Add(this.lblAuthor);
			this.pnlAuthor.Location = new System.Drawing.Point(3, 55);
			this.pnlAuthor.Name = "pnlAuthor";
			this.pnlAuthor.Size = new System.Drawing.Size(47, 18);
			this.pnlAuthor.TabIndex = 9;
			// 
			// lblAuthor
			// 
			this.lblAuthor.AutoSize = true;
			this.lblAuthor.Location = new System.Drawing.Point(3, 5);
			this.lblAuthor.Name = "lblAuthor";
			this.lblAuthor.Size = new System.Drawing.Size(41, 13);
			this.lblAuthor.TabIndex = 10;
			this.lblAuthor.Text = "&Author:";
			// 
			// pnlComment
			// 
			this.pnlComment.AutoSize = true;
			this.pnlComment.Controls.Add(this.lblComment);
			this.pnlComment.Location = new System.Drawing.Point(3, 179);
			this.pnlComment.Name = "pnlComment";
			this.pnlComment.Size = new System.Drawing.Size(60, 16);
			this.pnlComment.TabIndex = 13;
			// 
			// lblComment
			// 
			this.lblComment.AutoSize = true;
			this.lblComment.Location = new System.Drawing.Point(3, 3);
			this.lblComment.Name = "lblComment";
			this.lblComment.Size = new System.Drawing.Size(54, 13);
			this.lblComment.TabIndex = 14;
			this.lblComment.Text = "C&omment:";
			// 
			// pnlTags
			// 
			this.pnlTags.AutoSize = true;
			this.pnlTags.Controls.Add(this.lblTags);
			this.pnlTags.Location = new System.Drawing.Point(3, 205);
			this.pnlTags.Name = "pnlTags";
			this.pnlTags.Size = new System.Drawing.Size(40, 18);
			this.pnlTags.TabIndex = 16;
			// 
			// lblTags
			// 
			this.lblTags.AutoSize = true;
			this.lblTags.Location = new System.Drawing.Point(3, 5);
			this.lblTags.Name = "lblTags";
			this.lblTags.Size = new System.Drawing.Size(34, 13);
			this.lblTags.TabIndex = 17;
			this.lblTags.Text = "&Tags:";
			// 
			// lstAuthors
			// 
			this.lstAuthors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstAuthors.FormattingEnabled = true;
			this.lstAuthors.Location = new System.Drawing.Point(69, 55);
			this.lstAuthors.Name = "lstAuthors";
			this.lstAuthors.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstAuthors.Size = new System.Drawing.Size(431, 118);
			this.lstAuthors.TabIndex = 11;
			this.lstAuthors.Click += new System.EventHandler(this.lst_Click);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 361);
			this.Controls.Add(this.tlpInputControls);
			this.Controls.Add(this.tlpButtons);
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "FrmMain";
			this.Text = "Edit Folder Meta Data";
			this.tlpButtons.ResumeLayout(false);
			this.tlpButtons.PerformLayout();
			this.tlpInputControls.ResumeLayout(false);
			this.tlpInputControls.PerformLayout();
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			this.pnlSubject.ResumeLayout(false);
			this.pnlSubject.PerformLayout();
			this.pnlAuthor.ResumeLayout(false);
			this.pnlAuthor.PerformLayout();
			this.pnlComment.ResumeLayout(false);
			this.pnlComment.PerformLayout();
			this.pnlTags.ResumeLayout(false);
			this.pnlTags.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TableLayoutPanel tlpButtons;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.TableLayoutPanel tlpInputControls;
		private System.Windows.Forms.TextBox txtTitle;
		private HKS.FolderMetadata.Dialogs.Controls.ListBoxEx lstTags;
		private System.Windows.Forms.Button btnManageTags;
		private System.Windows.Forms.TextBox txtSubject;
		private System.Windows.Forms.TextBox txtComment;
		private System.Windows.Forms.Button btnManageAuthors;
		private System.Windows.Forms.Panel pnlTitle;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Panel pnlSubject;
		private System.Windows.Forms.Label lblSubject;
		private System.Windows.Forms.Panel pnlAuthor;
		private System.Windows.Forms.Label lblAuthor;
		private System.Windows.Forms.Panel pnlComment;
		private System.Windows.Forms.Label lblComment;
		private System.Windows.Forms.Panel pnlTags;
		private System.Windows.Forms.Label lblTags;
		private HKS.FolderMetadata.Dialogs.Controls.ListBoxEx lstAuthors;
	}
}

