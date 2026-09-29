using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Program {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
		}

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
					Debug.WriteLine(String.Format("Scanning {0}",directoryInfo.FullName));
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

								foreach (var paragraph in doc.Paragraphs) {
									ReplaceKey(paragraph, dic);
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
		private void btnClose_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void ReplaceKey(XWPFParagraph paragraph, IDictionary<string, string> redic) {
			string text = paragraph.ParagraphText;
			foreach (var kv in redic) {
				if (text.Contains(kv.Key)) {
					paragraph.ReplaceText(kv.Key, kv.Value);
				}
			}
		}

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
	}
}