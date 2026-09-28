using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
// not using interop
// using Word = Microsoft.Office.Interop.Word;

namespace WordFile {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
		}

		// private Word.Application G_WordApplication;
		// private object G_Missing = Type.Missing;
		private void btnSelectFile_Click(object sender, EventArgs e) {
			var folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
				txtDocDirectory.Text = folderBrowserDialog.SelectedPath;
			}
		}


		private void btnReplaceText_Click(object sender, EventArgs e) {
			string docDirectory = txtDocDirectory.Text;

			ThreadPool.QueueUserWorkItem(
				(o) => {
					var directoryInfo = new DirectoryInfo(docDirectory);
					FileInfo[] files = directoryInfo.GetFiles("*.docx");

					foreach (FileInfo f in files) {
						string filePath = f.ToString();
						var dic = new Dictionary<string, string> { };

						using (var stream = File.OpenRead(filePath)) {
							var doc = new XWPFDocument(stream);

							try {
								if (txtSearchKey1.Text != "") {
									dic.Add(txtSearchKey1.Text, txtReplace1.Text);
								}

								if (txtSearchKey2.Text != "") {
									dic.Add(txtSearchKey2.Text, txtReplace2.Text);
								}

								if (txtSearchKey3.Text != "") {
									dic.Add(txtSearchKey3.Text, txtReplace3.Text);
								}

								if (txtSearchKey4.Text != "") {
									dic.Add(txtSearchKey4.Text, txtReplace4.Text);
								}

								if (txtSearchKey5.Text != "") {
									dic.Add(txtSearchKey5.Text, txtReplace5.Text);
								}

								foreach (var para in doc.Paragraphs) {
									ReplaceKey(para, dic);
									//ReplaceKeyword(para, dic);
								}

								using (var newstream = File.Create(f.Directory + "/" + f.Name)) { // "/poutput.docx"
									doc.Write(newstream);
									newstream.Flush();
								}
							} catch (Exception ex) {
								MessageBox.Show("Exception: " + ex.Message);
							}
						}
					}

					MessageBox.Show("Done.");
				});
		}
		/*
		private void btnReplaceTwo_Click(object sender, EventArgs e) {
			G_OpenFileDialog = new OpenFileDialog();
			G_OpenFileDialog.Filter = "*.doc|*.doc";
			G_OpenFileDialog.InitialDirectory = textBox1.Text;
			DialogResult P_DialogResult = G_OpenFileDialog.ShowDialog();
			string dir1 = txtDocDirectory.Text;

			ThreadPool.QueueUserWorkItem(
				(o) => {
					DirectoryInfo directoryInfo = new DirectoryInfo(dir1);
					FileInfo[] files = directoryInfo.GetFiles("*.doc");
					G_WordApplication = new Word.Application();
					foreach (FileInfo f in files) {
						object P_FilePath = dir1 + "\\\\" + f; 
						try {
							Word.Document P_Document = G_WordApplication.Documents.Open(//打开Word文档
								                           ref P_FilePath, ref G_Missing, ref G_Missing,
								                           ref G_Missing, ref G_Missing, ref G_Missing,
								                           ref G_Missing, ref G_Missing, ref G_Missing,
								                           ref G_Missing, ref G_Missing, ref G_Missing,
								                           ref G_Missing, ref G_Missing, ref G_Missing,
								                           ref G_Missing);

							Word.Range P_Range = P_Document.Range(ref G_Missing, ref G_Missing);
							Word.Find P_Find = P_Range.Find;

							if (txtSearchKey1.Text != "") {
								this.Invoke(
									(MethodInvoker)(() => {
										P_Find.Text = txtSearchKey1.Text; 
										P_Find.Replacement.Text = txtReplace1.Text; 
									}));
								object P_Replace = Word.WdReplace.wdReplaceAll;
								// Start replacement									            ref G_Missing, ref G_Missing, ref G_Missing,
								bool P_bl = P_Find.Execute(
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
							}

							if (txtSearchKey2.Text != "") {
								this.Invoke(
									(MethodInvoker)(() => { 
										P_Find.Text = txtSearchKey2.Text; 
										P_Find.Replacement.Text = txtReplace2.Text; 
									}));
								object P_Replace = Word.WdReplace.wdReplaceAll; 
								bool P_bl = P_Find.Execute(
									            ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
							}

							if (txtSearchKey3.Text != "") {
								this.Invoke(
									(MethodInvoker)(() => { 
										P_Find.Text = txtSearchKey3.Text; 
										P_Find.Replacement.Text = txtReplace3.Text; 
									}));
								object P_Replace = Word.WdReplace.wdReplaceAll; 
								bool P_bl = P_Find.Execute(
									            ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
							}

							if (txtSearchKey4.Text != "") {
								this.Invoke( 
									(MethodInvoker)(() => { 
										P_Find.Text = txtSearchKey4.Text; 
										P_Find.Replacement.Text = txtReplace4.Text; 
									}));
								object P_Replace = Word.WdReplace.wdReplaceAll;
								bool P_bl = P_Find.Execute(
									            ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
							}

							if (txtSearchKey5.Text != "") {
								this.Invoke(
									(MethodInvoker)(() => { 
										P_Find.Text = txtSearchKey5.Text; 
										P_Find.Replacement.Text = txtReplace5.Text; 
									}));
								object P_Replace = Word.WdReplace.wdReplaceAll; 
								bool P_bl = P_Find.Execute(
									            ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref P_Replace,
									            ref G_Missing, ref G_Missing, ref G_Missing, ref G_Missing);
							}

							G_WordApplication.Documents.Save(
								ref G_Missing, ref G_Missing);
							((Word._Document)P_Document).Close(
								ref G_Missing, ref G_Missing, ref G_Missing);
						} catch (Exception g) {
						}
					}

					((Word._Application)G_WordApplication).Quit( 
						ref G_Missing, ref G_Missing, ref G_Missing);
					MessageBox.Show("Done.");
				});
		}
		*/
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ReplaceKey(XWPFParagraph para, IDictionary<string, string> redic)
		{
			string text = para.ParagraphText;
			foreach (var kv in redic) {
				if (text.Contains(kv.Key)) {
					para.ReplaceText(kv.Key, kv.Value);
				}
			}
		}

		private void ReplaceKeyword(XWPFParagraph para, IDictionary<string, string> redic)
		{
			string text = string.Empty; 
			string styleid = para.Style;
			var runs = para.Runs;
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
	}
}