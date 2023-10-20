
namespace HKS.FolderMetadata.Dialogs
{
	partial class FrmManage
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.chkSort = new System.Windows.Forms.CheckBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.tlpInput = new System.Windows.Forms.TableLayoutPanel();
			this.lstEntries = new HKS.FolderMetadata.Dialogs.Controls.ListBoxEx();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.txtAdd = new System.Windows.Forms.TextBox();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.tlpButtons.SuspendLayout();
			this.tlpInput.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpButtons
			// 
			this.tlpButtons.ColumnCount = 3;
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpButtons.Controls.Add(this.btnCancel, 2, 0);
			this.tlpButtons.Controls.Add(this.chkSort, 0, 0);
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
			// chkSort
			// 
			this.chkSort.AutoSize = true;
			this.chkSort.Location = new System.Drawing.Point(3, 3);
			this.chkSort.Name = "chkSort";
			this.chkSort.Size = new System.Drawing.Size(112, 17);
			this.chkSort.TabIndex = 9;
			this.chkSort.Text = "Sort alphabetically";
			this.chkSort.UseVisualStyleBackColor = true;
			this.chkSort.Click += new System.EventHandler(this.chkSort_CheckedChanged);
			// 
			// btnSave
			// 
			this.btnSave.AutoSize = true;
			this.btnSave.Location = new System.Drawing.Point(425, 3);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 10;
			this.btnSave.Text = "&Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// tlpInput
			// 
			this.tlpInput.ColumnCount = 3;
			this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
			this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tlpInput.Controls.Add(this.lstEntries, 0, 0);
			this.tlpInput.Controls.Add(this.btnAdd, 2, 6);
			this.tlpInput.Controls.Add(this.btnRemove, 2, 3);
			this.tlpInput.Controls.Add(this.txtAdd, 0, 6);
			this.tlpInput.Controls.Add(this.btnUp, 2, 0);
			this.tlpInput.Controls.Add(this.btnDown, 2, 1);
			this.tlpInput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpInput.Location = new System.Drawing.Point(0, 0);
			this.tlpInput.Name = "tlpInput";
			this.tlpInput.RowCount = 7;
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tlpInput.Size = new System.Drawing.Size(584, 326);
			this.tlpInput.TabIndex = 2;
			// 
			// lstEntries
			// 
			this.lstEntries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstEntries.FormattingEnabled = true;
			this.lstEntries.Location = new System.Drawing.Point(3, 3);
			this.lstEntries.Name = "lstEntries";
			this.tlpInput.SetRowSpan(this.lstEntries, 5);
			this.lstEntries.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lstEntries.Size = new System.Drawing.Size(489, 271);
			this.lstEntries.TabIndex = 3;
			this.lstEntries.SelectedIndexChanged += new System.EventHandler(this.lstEntries_SelectedIndexChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Enabled = false;
			this.btnAdd.Location = new System.Drawing.Point(506, 300);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(75, 23);
			this.btnAdd.TabIndex = 8;
			this.btnAdd.Text = "&Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Enabled = false;
			this.btnRemove.Location = new System.Drawing.Point(506, 81);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 6;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// txtAdd
			// 
			this.txtAdd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtAdd.Location = new System.Drawing.Point(3, 300);
			this.txtAdd.Name = "txtAdd";
			this.txtAdd.Size = new System.Drawing.Size(489, 20);
			this.txtAdd.TabIndex = 7;
			this.txtAdd.TextChanged += new System.EventHandler(this.txtAdd_TextChanged);
			this.txtAdd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtAdd_KeyUp);
			// 
			// btnUp
			// 
			this.btnUp.Enabled = false;
			this.btnUp.Location = new System.Drawing.Point(506, 3);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(75, 23);
			this.btnUp.TabIndex = 4;
			this.btnUp.Text = "&Up";
			this.btnUp.UseVisualStyleBackColor = true;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Enabled = false;
			this.btnDown.Location = new System.Drawing.Point(506, 32);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(75, 23);
			this.btnDown.TabIndex = 5;
			this.btnDown.Text = "&Down";
			this.btnDown.UseVisualStyleBackColor = true;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// FrmManage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 361);
			this.Controls.Add(this.tlpInput);
			this.Controls.Add(this.tlpButtons);
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "FrmManage";
			this.Text = "FrmManage";
			this.tlpButtons.ResumeLayout(false);
			this.tlpButtons.PerformLayout();
			this.tlpInput.ResumeLayout(false);
			this.tlpInput.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpButtons;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkSort;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.TableLayoutPanel tlpInput;
		private HKS.FolderMetadata.Dialogs.Controls.ListBoxEx lstEntries;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.TextBox txtAdd;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnDown;
	}
}