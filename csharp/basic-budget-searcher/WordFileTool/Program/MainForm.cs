using System;
using System.IO;
using System.Windows.Forms;
using NPOI.XWPF.UserModel;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Program
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void btnSelectFile_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
				txtDocDirectory.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btnReplaceText_Click(object sender, EventArgs eventArgs)
		{
			string docDirectory = txtDocDirectory.Text;
			FileInfo[] files = { };
			DirectoryInfo directoryInfo = null;
			string filePath = null;
			ThreadPool.QueueUserWorkItem(
				// Error CS1593: Delegate 'System.Threading.WaitCallback' does not take 0 arguments
				(object state) => {
					directoryInfo = new DirectoryInfo(docDirectory);
					Debug.WriteLine(String.Format("Scanning {0}", directoryInfo.FullName));
					
					files = directoryInfo.GetFiles("*.pdf");
					
					foreach (FileInfo fileInfo in files) {
						// origin: https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs							
						filePath = fileInfo.FullName; 
						using (var document = PdfDocument.Open(filePath)) {
							Debug.WriteLine(String.Format("Reading {0}", filePath));
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
		private void ReplaceKeyword(XWPFParagraph paragraph, IDictionary<string, string> redic)
		{
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
		void MainFormLoad(object sender, EventArgs e)
		{
	
		}
		void Button1Click(object sender, EventArgs e)
		{
			var o = new System.Windows.Forms.OpenFileDialog();
			o.Title = "Select a file";
			o.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			o.Filter = "SpreadSheet (*.xlsx)|*.xls|Text (*.txt)|*.docx|All files (*.*)| *.*";
			if (o.ShowDialog() == DialogResult.OK){
				textBox1.Tag =  o.FileName;
				var size = 20; // approx width in characters
				textBox1.Text = "…" + o.FileName.Substring(o.FileName.Length-size,size);
			}

		}
		void TxtDocDirectoryTextChanged(object sender, EventArgs e)
		{
	
		}
	}
}