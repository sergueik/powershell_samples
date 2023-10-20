btnSearch.Text =using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Resources;
using System.Threading;
using Utils;

namespace Client {

	public class Form1 : Form {
		private WinGrep wingrep;
		private Button btnBrowse;
		private Label lblResults;
		private TextBox txtResults;
		
		private Label lblSearchText;
		private TextBox txtSearchText;
		private Label lblFiles;
		private TextBox txtFiles;
		private Button btnSearch;
		private Label lblDir;
		private TextBox txtDir;
		private Label lblCurFile;
		private TextBox txtCurFile;
		private CheckBox ckCountLines;
		private CheckBox ckRecursive;
		private CheckBox ckLineNumbers;
		private CheckBox ckIgnoreCase;
		private CheckBox ckJustFiles;
		private Thread m_searchthread = null;
		// ArrayList m_arrFiles = new ArrayList();

		public Form1() {
			InitializeComponent();
		}

		public TextBox TxtResults {
			get { return txtResults; }
		}


		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					btnBrowse.Dispose();
					lblResults.Dispose();
					txtResults.Dispose();
					lblSearchText.Dispose();
					txtSearchText.Dispose();
					lblFiles.Dispose();
					txtFiles.Dispose();
					btnSearch.Dispose();
					ckRecursive.Dispose();
					lblDir.Dispose();
					txtDir.Dispose();
					lblCurFile.Dispose();
					txtCurFile.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		private void InitializeComponent() {
			wingrep = new WinGrep(this);
			ResourceManager resources = new ResourceManager(typeof(Form1));
			txtResults = new TextBox();
			lblResults = new Label();
			txtFiles = new TextBox();
			
			lblDir = new Label();
			btnBrowse = new Button();
			txtSearchText = new TextBox();
			lblFiles = new Label();
			lblSearchText = new Label();
			btnSearch = new Button();
			txtDir = new TextBox();
			ckCountLines = new CheckBox();
			ckCountLines.Checked = true;
			lblCurFile = new Label();
			txtCurFile = new TextBox();
			ckRecursive = new CheckBox();
			ckRecursive.Checked = true;
			ckLineNumbers = new CheckBox();
			ckLineNumbers.Checked = true;
			ckIgnoreCase = new CheckBox();
			ckJustFiles = new CheckBox();
			this.SuspendLayout();

			txtResults.BackColor = SystemColors.Info;
			txtResults.Cursor = Cursors.Arrow;
			txtResults.Location = new Point(12, 204);
			txtResults.Multiline = true;
			txtResults.Name = "txtResults";
			txtResults.ReadOnly = true;
			txtResults.ScrollBars = ScrollBars.Both;
			txtResults.Size = new Size(472, 216);
			txtResults.TabIndex = 9;
			txtResults.Text = "";
			txtResults.WordWrap = false;

			lblResults.Location = new Point(12, 192);
			lblResults.Name = "lblResults";
			lblResults.Size = new Size(56, 12);
			lblResults.TabIndex = 10;
			lblResults.Text = "Results";

			txtFiles.BackColor = SystemColors.Window;
			txtFiles.Location = new Point(12, 68);
			txtFiles.Name = "txtFiles";
			txtFiles.Size = new Size(180, 20);
			txtFiles.TabIndex = 5;
			txtFiles.Text = "";
			txtFiles.KeyDown += new KeyEventHandler(this.txtFiles_KeyDown);

			lblDir.Location = new Point(12, 12);
			lblDir.Name = "lblDir";
			lblDir.Size = new Size(60, 12);
			lblDir.TabIndex = 0;
			lblDir.Text = "Directory";

			btnBrowse.Location = new Point(456, 24);
			btnBrowse.Name = "btnBrowse";
			btnBrowse.Size = new Size(28, 20);
			btnBrowse.TabIndex = 2;
			btnBrowse.Text = "...";
			btnBrowse.Click += new EventHandler(this.btnDir_Click);

			txtSearchText.BackColor = SystemColors.Window;
			txtSearchText.Location = new Point(204, 68);
			txtSearchText.Name = "txtSearchText";
			txtSearchText.Size = new Size(280, 20);
			txtSearchText.TabIndex = 7;
			txtSearchText.Text = "";
			txtSearchText.KeyDown += new KeyEventHandler(this.txtSearchText_KeyDown);
			txtSearchText.TextChanged += new EventHandler(this.txtSearchText_TextChanged);

			lblFiles.Location = new Point(12, 56);
			lblFiles.Name = "lblFiles";
			lblFiles.Size = new Size(84, 12);
			lblFiles.TabIndex = 4;
			lblFiles.Text = "Files";

			lblSearchText.Location = new Point(204, 56);
			lblSearchText.Name = "lblSearchText";
			lblSearchText.Size = new Size(196, 12);
			lblSearchText.TabIndex = 6;
			lblSearchText.Text = "Search Pattern (Regular Expression)";

			btnSearch.Enabled = false;
			btnSearch.Location = new Point(424, 116);
			btnSearch.Name = "btnSearch";
			btnSearch.Size = new Size(60, 24);
			btnSearch.TabIndex = 8;
			btnSearch.Text = "Search";
			btnSearch.Click += new EventHandler(this.btnSearch_Click);

			txtDir.BackColor = SystemColors.Window;
			txtDir.Location = new Point(12, 24);
			txtDir.Name = "txtDir";
			txtDir.Size = new Size(436, 20);
			txtDir.TabIndex = 1;
			txtDir.Text = "";
			txtDir.KeyDown += new KeyEventHandler(this.txtDir_KeyDown);
			txtDir.TextChanged += new EventHandler(this.txtDir_TextChanged);

			ckCountLines.Location = new Point(12, 124);
			ckCountLines.Name = "ckCountLines";
			ckCountLines.Size = new Size(84, 16);
			ckCountLines.TabIndex = 3;
			ckCountLines.Text = "Count Lines";

			lblCurFile.Location = new Point(12, 152);
			lblCurFile.Name = "lblCurFile";
			lblCurFile.Size = new Size(84, 12);
			lblCurFile.TabIndex = 11;
			lblCurFile.Text = "Current File";

			txtCurFile.BackColor = SystemColors.Info;
			txtCurFile.Location = new Point(12, 164);
			txtCurFile.Name = "txtCurFile";
			txtCurFile.ReadOnly = true;
			txtCurFile.Size = new Size(472, 20);
			txtCurFile.TabIndex = 12;
			txtCurFile.Text = "";

			ckRecursive.Location = new Point(12, 100);
			ckRecursive.Name = "ckRecursive";
			ckRecursive.Size = new Size(84, 16);
			ckRecursive.TabIndex = 13;
			ckRecursive.Text = "Recursive";

			ckLineNumbers.Location = new Point(104, 100);
			ckLineNumbers.Name = "ckLineNumbers";
			ckLineNumbers.Size = new Size(96, 16);
			ckLineNumbers.TabIndex = 14;
			ckLineNumbers.Text = "Line Numbers";

			ckIgnoreCase.Location = new Point(104, 124);
			ckIgnoreCase.Name = "ckIgnoreCase";
			ckIgnoreCase.Size = new Size(96, 16);
			ckIgnoreCase.TabIndex = 15;
			ckIgnoreCase.Text = "Ignore Case";

			ckJustFiles.Location = new Point(208, 100);
			ckJustFiles.Name = "ckJustFiles";
			ckJustFiles.Size = new Size(84, 16);
			ckJustFiles.TabIndex = 16;
			ckJustFiles.Text = "Just Files";
			ckJustFiles.Click += new EventHandler(this.ckJustFiles_Click);

			AutoScaleBaseSize = new Size(5, 13);
			ClientSize = new Size(494, 432);
			Controls.AddRange(new Control[] {
				ckJustFiles,
				ckIgnoreCase,
				ckLineNumbers,
				ckRecursive,
				lblCurFile,
				txtCurFile,
				btnBrowse,
				lblResults,
				txtResults,
				lblSearchText,
				txtSearchText,
				lblFiles,
				txtFiles,
				btnSearch,
				ckCountLines,
				lblDir,
				txtDir
			});
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
	//		this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormWinGrep";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Win Grep";
			this.ResumeLayout(false);
		}

		protected void SearchThread() {
			if (m_searchthread == null) {
				
				wingrep.Dir = this.txtDir.Text;
				wingrep.Files = this.txtFiles.Text;
				wingrep.RegEx = this.txtSearchText.Text;
				m_searchthread = new Thread(new ThreadStart(wingrep.Search));
				m_searchthread.IsBackground = true;
				m_searchthread.Start();
				btnSearch.Text = "Stop";
			} else {
				m_searchthread.Abort();
				m_searchthread = null;
				txtCurFile.Text = "";
				txtResults.Text = "User Requested Search Abortion!";
				btnSearch.Text = "Search";
			}
		}

		protected void txtFiles_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter && btnSearch.Enabled == true) {
				SearchThread();
			}
		}

		protected void txtDir_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter && btnSearch.Enabled == true) {
				SearchThread();
			}
		}

		protected void txtSearchText_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter && btnSearch.Enabled == true) {
				SearchThread();
			}
		}


		protected void btnSearch_Click(object sender, EventArgs e) {
			SearchThread();
		}

		protected void VerifySearchBtn() {
			if (txtDir.Text != "" && txtSearchText.Text != "") {
				btnSearch.Enabled = true;
			} else
				btnSearch.Enabled = false;
		}

		protected void txtSearchText_TextChanged(object sender, EventArgs e) {
			VerifySearchBtn();
		}

		protected void txtDir_TextChanged(object sender, System.EventArgs e) {
			VerifySearchBtn();
		}

		protected void btnDir_Click(object sender, EventArgs e) {
			OpenFileDialog fdlg = new OpenFileDialog();
			fdlg.Title = "Select a file";
			fdlg.InitialDirectory = Directory.GetCurrentDirectory();
			fdlg.Filter = "All files (*.*)|*.*";
			if (fdlg.ShowDialog() == DialogResult.OK) {
				String strFile = fdlg.FileName;
				//File Extension
				String strExt;
				//Get the Directory and file extension
				txtDir.Text = Path.GetDirectoryName(strFile);
				strExt = Path.GetExtension(strFile);
				txtFiles.Text = "*" + strExt;
			}
		}

		private void ckJustFiles_Click(object sender, EventArgs e) {
			if (ckJustFiles.Checked == true) {
				ckLineNumbers.Enabled = false;
				ckCountLines.Enabled = false;
			} else {
				ckLineNumbers.Enabled = true;
				ckCountLines.Enabled = true;
			}
		}

		[STAThread]
		public static void Main(string[] args) {
			Application.Run(new Form1());
		}
	}
}

