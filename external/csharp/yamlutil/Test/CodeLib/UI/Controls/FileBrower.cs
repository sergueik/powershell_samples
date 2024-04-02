using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace QiHe.CodeLib
{
	public class FileBrower : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button buttonBrows1;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.TextBox textBoxFile1;

		private System.ComponentModel.Container components = null;

		public FileBrower()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.buttonBrows1 = new System.Windows.Forms.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.textBoxFile1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonBrows1
			// 
			this.buttonBrows1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrows1.AutoSize = true;
			this.buttonBrows1.Location = new System.Drawing.Point(401, 3);
			this.buttonBrows1.Name = "buttonBrows1";
			this.buttonBrows1.Size = new System.Drawing.Size(79, 24);
			this.buttonBrows1.TabIndex = 6;
			this.buttonBrows1.Text = "选取文件";
			this.buttonBrows1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonBrows1.Click += new System.EventHandler(this.buttonBrows1_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Location = new System.Drawing.Point(6, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(35, 12);
			this.labelTitle.TabIndex = 5;
			this.labelTitle.Text = "文件1";
			// 
			// textBoxFile1
			// 
			this.textBoxFile1.Location = new System.Drawing.Point(47, 5);
			this.textBoxFile1.Name = "textBoxFile1";
			this.textBoxFile1.Size = new System.Drawing.Size(350, 21);
			this.textBoxFile1.TabIndex = 4;
			// 
			// FileBrower
			// 
			this.Controls.Add(this.buttonBrows1);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.textBoxFile1);
			this.Name = "FileBrower";
			this.Size = new System.Drawing.Size(488, 34);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FileBrower_Layout);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void buttonBrows1_Click(object sender, System.EventArgs e)
		{
			try {
				if (FilePath != String.Empty) {
					FileSelector.InitialPath = Path.GetDirectoryName(FilePath);
				}
			} catch {
			}
			string file = ForSave ?
                FileSelector.BrowseFileForSave(fileType) :
                FileSelector.BrowseFile(fileType);
			if (file != null) {
				FilePath = file;
				if (FilePathBrowsed != null) {
					FilePathBrowsed(sender, e);
				}
			}
		}

		[Category("Appearance")]
		public string Caption {
			get {
				return labelTitle.Text;
			}
			set {
				labelTitle.Text = value;
			}
		}

		[Category("Appearance")]
		public string ButtonText {
			get {
				return buttonBrows1.Text;
			}
			set {
				buttonBrows1.Text = value;
			}
		}

		FileType fileType = FileType.Txt;
		public FileType FileType {
			get {
				return fileType;
			}
			set {
				fileType = value;
			}
		}

		bool forSave = false;
		public bool ForSave {
			get {
				return forSave;
			}
			set {
				forSave = value;
			}
		}


		public string FilePath {
			get {
				return textBoxFile1.Text;
			}
			set {
				textBoxFile1.Text = value;
			}
		}

		private void FileBrower_Layout(object sender, LayoutEventArgs e)
		{
			textBoxFile1.Left = labelTitle.Right;
			buttonBrows1.Left = this.Width - buttonBrows1.Width - 2;
			int width = buttonBrows1.Left - labelTitle.Right - 2;
			textBoxFile1.Width = width > 0 ? width : 0;
		}
		public event EventHandler FilePathBrowsed;
	}
}
