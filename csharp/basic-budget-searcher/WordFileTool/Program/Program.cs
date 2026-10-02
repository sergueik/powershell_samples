using System;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Program {
	// NOTE: Missing partial modifier on declaration of type 'Program.MainForm';
	// when
	// partial declaration of this type exists (CS0260)
	public class Program : Form {
		
		// NOTE: unnecessary "internal static class Program {...}"
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			// https://learn.microsoft.com/en-us/dotnet/api/application.setcompatibletextrenderingdefault?view=netframework-4.5
			// NOTE: can only call this method before
			// the first window is created by Windows Forms application
			try {
				Application.SetCompatibleTextRenderingDefault(false);
			} catch(InvalidOperationException) {}
			Application.Run(new Program());
		}

		public Program() { InitializeComponent(); }
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		// https://pbdd.org/wp-content/uploads/2015/06/WordPractice2007.docx
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Program));
			this.btnSelectFile = new Button();
			label1 = new Label();
			this.txtDocDirectory = new TextBox();
			label2 = new Label();
			label3 = new Label();
			this.txtSearchKey1 = new TextBox();
			this.txtReplace1 = new TextBox();
			this.btnReplace = new Button();
			this.btnClose = new Button();
			label6 = new Label();
			this.txtSearchKey3 = new TextBox();
			this.txtSearchKey4 = new TextBox();
			label8 = new Label();
			button1 = new Button();
			label12 = new Label();
			this.textBox1 = new TextBox();
			tabControl = new TabControl();
			tabPage = new TabPage();
			txtResult1 = new TextBox();
			this.SuspendLayout();
			// 
			// btnSelectFile
			// 
			this.btnSelectFile.Location = new Point(112, 89);
			this.btnSelectFile.Margin = new Padding(5, 6, 5, 6);
			this.btnSelectFile.Name = "btnSelectFile";
			this.btnSelectFile.Size = new Size(207, 46);
			this.btnSelectFile.TabIndex = 0;
			this.btnSelectFile.Text = "Choose Folder";
			this.btnSelectFile.UseVisualStyleBackColor = true;
			this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(341, 101);
			label1.Margin = new Padding(5, 0, 5, 0);
			label1.Name = "label1";
			label1.Size = new Size(120, 25);
			label1.TabIndex = 1;
			label1.Text = "Base folder：";
			// 
			// txtDocDirectory
			// 
			this.txtDocDirectory.Location = new Point(486, 89);
			this.txtDocDirectory.Margin = new Padding(5, 6, 5, 6);
			this.txtDocDirectory.Name = "txtDocDirectory";
			this.txtDocDirectory.Size = new Size(256, 29);
			this.txtDocDirectory.TabIndex = 2;
			this.txtDocDirectory.TextAlign = HorizontalAlignment.Right;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(154, 161);
			label2.Margin = new Padding(5, 0, 5, 0);
			label2.Name = "label2";
			label2.Size = new Size(124, 25);
			label2.TabIndex = 3;
			label2.Text = "1.Text to find";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = Color.Tan;
			label3.Location = new Point(154, 215);
			label3.Margin = new Padding(5, 0, 5, 0);
			label3.Name = "label3";
			label3.Size = new Size(179, 25);
			label3.TabIndex = 3;
			label3.Text = "Text to be replaced";
			// 
			// txtSearchKey1
			// 
			this.txtSearchKey1.Location = new Point(341, 156);
			this.txtSearchKey1.Margin = new Padding(5, 6, 5, 6);
			this.txtSearchKey1.Name = "txtSearchKey1";
			this.txtSearchKey1.Size = new Size(401, 29);
			this.txtSearchKey1.TabIndex = 4;
			// 
			// txtReplace1
			// 
			this.txtReplace1.BackColor = SystemColors.Window;
			this.txtReplace1.Enabled = false;
			this.txtReplace1.Location = new Point(341, 209);
			this.txtReplace1.Margin = new Padding(5, 6, 5, 6);
			this.txtReplace1.Name = "txtReplace1";
			this.txtReplace1.Size = new Size(401, 29);
			this.txtReplace1.TabIndex = 4;
			// 
			// btnReplace
			// 
			this.btnReplace.Location = new Point(112, 770);
			this.btnReplace.Margin = new Padding(5, 6, 5, 6);
			this.btnReplace.Name = "btnReplace";
			this.btnReplace.Size = new Size(137, 46);
			this.btnReplace.TabIndex = 5;
			this.btnReplace.Text = "Start";
			this.btnReplace.UseVisualStyleBackColor = true;
			this.btnReplace.Click += new System.EventHandler(this.btnReplaceText_Click);
			// 
			// btnClose
			// 
			this.btnClose.Location = new Point(605, 770);
			this.btnClose.Margin = new Padding(5, 6, 5, 6);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new Size(137, 46);
			this.btnClose.TabIndex = 6;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(154, 261);
			label6.Margin = new Padding(5, 0, 5, 0);
			label6.Name = "label6";
			label6.Size = new Size(124, 25);
			label6.TabIndex = 3;
			label6.Text = "3.Text to find";
			// 
			// txtSearchKey3
			// 
			this.txtSearchKey3.Location = new Point(343, 258);
			this.txtSearchKey3.Margin = new Padding(5, 6, 5, 6);
			this.txtSearchKey3.Name = "txtSearchKey3";
			this.txtSearchKey3.Size = new Size(399, 29);
			this.txtSearchKey3.TabIndex = 4;
			// 
			// txtSearchKey4
			// 
			this.txtSearchKey4.Location = new Point(343, 308);
			this.txtSearchKey4.Margin = new Padding(5, 6, 5, 6);
			this.txtSearchKey4.Name = "txtSearchKey4";
			this.txtSearchKey4.Size = new Size(399, 29);
			this.txtSearchKey4.TabIndex = 4;
			this.txtSearchKey4.TextChanged += new System.EventHandler(this.TxtSearchKey4TextChanged);
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new Point(154, 308);
			label8.Margin = new Padding(5, 0, 5, 0);
			label8.Name = "label8";
			label8.Size = new Size(124, 25);
			label8.TabIndex = 3;
			label8.Text = "4.Text to find";
			// 
			// button1
			// 
			button1.Location = new Point(112, 702);
			button1.Margin = new Padding(5, 6, 5, 6);
			button1.Name = "button1";
			button1.Size = new Size(207, 46);
			button1.TabIndex = 7;
			button1.Text = "Choose File";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1Click);
			// 
			// label12
			// 
			label12.AutoSize = true;
			label12.Location = new Point(343, 710);
			label12.Margin = new Padding(5, 0, 5, 0);
			label12.Name = "label12";
			label12.Size = new Size(133, 25);
			label12.TabIndex = 8;
			label12.Text = "Heuristics File";
			// 
			// textBox1
			// 
			this.textBox1.Location = new Point(486, 710);
			this.textBox1.Margin = new Padding(5, 6, 5, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(256, 29);
			this.textBox1.TabIndex = 9;
			this.textBox1.TextAlign = HorizontalAlignment.Right;
			// 
			// tabControl
			// 
			this.tabControl.Location = new Point(20, 358);
			this.tabControl.Margin = new Padding(5, 0, 5, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new Size(720, 320);
			this.tabControl.TabIndex = 12;
			tabControl.Controls.Add(tabPage);
			
			// 
			// tabPage
			// 
			this.tabPage.Location = new Point(4, 28);
			this.tabPage.Margin = new Padding(5, 0, 5, 0);
			this.tabPage.Name = "tabPage";
			this.tabPage.Size = new Size(700, 300);
			this.tabPage.TabIndex = 12;
			this.tabPage.Padding = new Padding(5, 0, 5, 0);
			tabPage.Controls.Add(txtResult1);
			tabPage.Text = "Log";
			tabPage.UseVisualStyleBackColor = true;
			// 
			// txtReult1
			// 
			txtResult1.Location = new Point(4, 5);
			txtResult1.Margin = new Padding(4, 5, 4, 5);
			txtResult1.Name = "txtResult1";
			txtResult1.Size = new Size(692, 292);
			txtResult1.TabIndex = 9;
			txtResult1.TextAlign = HorizontalAlignment.Left;
			txtResult1.Dock  = DockStyle.Fill;
			txtResult1.Multiline = true;
			txtResult1.ScrollBars = ScrollBars.Vertical;
			// 
			// Program
			// 
			this.AutoScaleDimensions = new SizeF(11F, 24F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(834, 931);
			Controls.Add(this.textBox1);
			Controls.Add(this.tabControl);
			Controls.Add(label12);
			Controls.Add(button1);
			Controls.Add(this.btnClose);
			Controls.Add(this.btnReplace);
			Controls.Add(this.txtReplace1);
			Controls.Add(this.txtSearchKey4);
			Controls.Add(this.txtSearchKey3);
			Controls.Add(this.txtSearchKey1);
			Controls.Add(label8);
			Controls.Add(label6);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(this.txtDocDirectory);
			Controls.Add(label1);
			Controls.Add(this.btnSelectFile);
			this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new Padding(5, 6, 5, 6);
			this.Name = "Program";
			this.Text = "word pdf search";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private Button btnSelectFile;
		private Label label1;
		private TextBox txtDocDirectory;
		private Label label2;
		private Label label3;
		private TextBox txtSearchKey1;
		private TextBox txtReplace1;
		private Label label6;
		private TextBox txtSearchKey3;
		private Label label8;
		private TextBox txtSearchKey4;
		private Button btnReplace;
		private Button btnClose;
		private Button button1;
		private Label label12;
		private TextBox textBox1;
		private TabControl tabControl;
		private TabPage tabPage;
		private TextBox txtResult1;

		private void Log(string message){
			if (txtResult1.InvokeRequired){
				txtResult1.Invoke(new Action<string>(Log), message);
              return;
			}
			txtResult1.AppendText(message + Environment.NewLine);
		}

		private void btnSelectFile_Click(object sender, EventArgs e) {
			var folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
				txtDocDirectory.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btnReplaceText_Click(object sender, EventArgs eventArgs) {
			string docDirectory = txtDocDirectory.Text;
			FileInfo[] files = { };
			DirectoryInfo directoryInfo = null;
			string filePath = null;
			ThreadPool.QueueUserWorkItem(
				// Error CS1593: Delegate 'System.Threading.WaitCallback' does not take 0 arguments
				(object state) => {
					directoryInfo = new DirectoryInfo(docDirectory);
					Debug.WriteLine(String.Format("Scanning {0}", directoryInfo.FullName));
					Log(String.Format("Scanning {0}", directoryInfo.FullName));

					files = directoryInfo.GetFiles("*.pdf");
					
					foreach (FileInfo fileInfo in files) {
						// origin: https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs
						filePath = fileInfo.FullName;
						using (var document = PdfDocument.Open(filePath)) {
							Debug.WriteLine(String.Format("Reading {0}", filePath));
							Log(String.Format("Reading {0}", filePath));
							foreach (var page in document.GetPages()) {
								
								var text = ContentOrderTextExtractor.GetText(page, true);

								Debug.WriteLine(String.Format("text: {0}", text));
							}
						}
					}
					files = directoryInfo.GetFiles("*.docx");

					foreach (FileInfo fileInfo in files) {
						filePath = fileInfo.FullName;
						var dic = new Dictionary<string, string> { };

						using (var stream = File.OpenRead(filePath)) {
							var document = new XWPFDocument(stream);

							try {
								if (txtSearchKey1.Text != "") {
									dic.Add(txtSearchKey1.Text, txtReplace1.Text);
								}

								if (txtSearchKey4.Text != "") {
									dic.Add(txtSearchKey4.Text, null);
								}

								if (txtSearchKey3.Text != "") {
									dic.Add(txtSearchKey3.Text, null);
								}

								foreach (var paragraph in document.Paragraphs) {
									ReplaceKey(paragraph, dic);
									//ReplaceKeyword(paragraph, dic);
								}
								// replace - not used
								/*
								using (var newstream = File.Create(fileInfo.Directory + "/" + fileInfo.Name)) { // "/poutput.docx"
									doc.Write(newstream);
									newstream.Flush();
								}
								
								*/
							} catch (Exception e) {
								MessageBox.Show("Exception: " + e.Message);
							}
						}
					}
					MessageBox.Show("Done.");
				});
		}
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ReplaceKey(XWPFParagraph paragraph, IDictionary<string, string> redic)
		{
			string text = paragraph.ParagraphText;
			foreach (var kv in redic) {
				if (text.Contains(kv.Key)) {
					if (!String.IsNullOrEmpty(kv.Value))
						paragraph.ReplaceText(kv.Key, kv.Value);
				}
			}
		}
		// currently unused - keep for possible later use
		private void ReplaceKeyword(XWPFParagraph paragraph, IDictionary<string, string> redic) {
			string text = string.Empty;
			string styleid = paragraph.Style;
			var runs = paragraph.Runs;
			for (int i = 0; i < runs.Count; i++) {
				var run = runs[i];
				text = run.ToString();
				foreach (var kv in redic) {
					if (text.Contains(kv.Key)) {
						text = text.Replace(kv.Key, kv.Value);
					}
				}

				runs[i].SetText(text, 0);
			}
		}

		void button1Click(object sender, EventArgs e) {
			var o = new OpenFileDialog();
			o.Title = "Select a file";
			o.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			o.Filter = "SpreadSheet (*.xlsx)|*.xls|Text (*.txt)|*.docx|All files (*.*)| *.*";
			if (o.ShowDialog() == DialogResult.OK) {
				textBox1.Tag = o.FileName;
				var size = 20; // approx width in characters
				textBox1.Text = "…" + o.FileName.Substring(o.FileName.Length - size, size);
			}

		}
		void TxtDocDirectoryTextChanged(object sender, EventArgs e) {
	
		}
		void TabPageClick(object sender, EventArgs e) {
	
		}
		void TxtSearchKey4TextChanged(object sender, EventArgs e)
		{
	
		}
	}
}