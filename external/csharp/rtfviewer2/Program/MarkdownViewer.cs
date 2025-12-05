using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Utils;

namespace Program {
	public partial class MarkdownViewer : Form {
		string rtfText = string.Empty;
		string FileName = "README.md";
		bool errorPopup = false;

		public MarkdownViewer(string[] args) {
			InitializeComponent();
			UpdateSplitters();
			ProcessArguments(args);
			OpenFile(FileName);
		}

		private void ProcessArguments(string[] args) {
			if (args.Length == 0)
				return;
			string file = args[0];
			if (File.Exists(file)) {
				FileName = file;
				checkBoxShowRtfCode.Checked = false;
				//checkBoxShowSourceMd.Checked = false;
			}
		}

		public void OpenFile(string fileName) {
			if (File.Exists(fileName)) {
				Debug.WriteLine("Loading file: " + fileName);
				string text = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
				textBoxSourceMd.Text = text;
				LoadText(text, FileName);
			} else {
				Debug.WriteLine("Could not load file: " + fileName);
			}

			if (rtfText.Length == 0) {
				rtfText = "";
				Debug.WriteLine("RTF text is 0 long");
			}
		}

		private void scrollRichTextBox(string search = "\u200B\u200B\u200B", int start = 0, bool reverse = false) {


			/*
			the searchable text can be an Invisible token which still occupy a specific character position -
			cannot find RTF structural tags, destination groups, control groups

			Examples:c

			* A tag string like __MARK__ that won’t be displaed by specifying the same color as the background.
			* A zero-width Unicode character (e.g. U+200B ZERO WIDTH SPACE).
			* Hidden text using RTF’s \v (hidden text) control word.
			* Example inside RTF: {\v HIDDEN}

			*/
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox.find?view=windowsdesktop-10.0#system-windows-forms-richtextbox-find(system-string-system-int32-system-windows-forms-richtextboxfinds)
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextboxfinds?view=netframework-4.5
			// NOTE: if text is not in richTextBox.Text, RichTextBox cannot scroll to it.
			RichTextBoxFinds options = reverse ? RichTextBoxFinds.Reverse | RichTextBoxFinds.NoHighlight : RichTextBoxFinds.NoHighlight;
			// Ensure that a search string has been specified and a valid start point.
			if (search.Length > 0 && start >= 0) {
				// Debug.WriteLine(BitConverter.ToString(Encoding.UTF8.GetBytes(richTextBoxRtfView.Text)));
	     		// expect to see repeted E2-80-8B
				// NOTE: RTF will be normalized
				// Debug.WriteLine(String.Format("Searching {0} in {1}", search, richTextBoxRtfView.Rtf ));				int index = richTextBoxRtfView.Find(search, start, options);
				int index = richTextBoxRtfView.Find(search, start, options);
				if (index >= 0) {
					richTextBoxRtfView.Select(index, 0);
					// NOTE: not text.Length in the case of invisible
					richTextBoxRtfView.ScrollToCaret();
					//  richTextBox1.Select(index, search.Length);
					//  richTextBox1.ScrollToCaret();   // makes the surrounding area visible
					//  richTextBox1.Focus();           // optionally focus - not applicable to inbisible control words
				}
			}
		}

		private void LoadText(string text, string fileName) {
			var rtfConverter = new RtfConverter(fileName);
			rtfText = rtfConverter.ConvertText(text);

			if (rtfConverter.Errors.Count > 0 && errorPopup) {
				string errors = LineListToString(rtfConverter.Errors);
				DialogResult result = MessageBox.Show(errors + "\n\n To stop showing errors, press No", "Parsing error", MessageBoxButtons.YesNo);
				if (result == DialogResult.No) {
					errorPopup = false;
				}
			}

			// MUST set Readonly to false, otherwise images will not load. This is a bug in RichTextBox.
			bool oldReadonly = richTextBoxRtfView.ReadOnly;
			richTextBoxRtfView.ReadOnly = false;
			richTextBoxRtfView.Rtf = rtfText;
			richTextBoxRtfView.ReadOnly = oldReadonly;

			// moved this to TextChanged event
			//richTextBoxRtfCode.Text = richTextBoxRtfView.Rtf;
		}

		private string LineListToString(List<string> lines) {
			var sb = new StringBuilder();
			foreach (string line in lines) {
				sb.AppendLine(line);
			}
			return sb.ToString();
		}

		private void CopyToClipboard_Click(object sender, EventArgs e) {
			Clipboard.SetText(rtfText, TextDataFormat.Rtf);
		}

		private void richTextBox1_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.F5) {
				LoadText(textBoxSourceMd.Text, FileName);
			}
			if (e.KeyCode == Keys.O && e.Modifiers == Keys.Control) {
				OpenFileAction();
			}
		}

		private void ButtonLoad_Click(object sender, EventArgs e) {
			OpenFileAction();
		}

		private void OpenFileAction() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=netframework-4.5
			var openFileDialog = new OpenFileDialog() {
				Filter = "Markdown|*.md",
				// ShowPinnedPlaces = true,
				// ShowPreview = true
				// 'System.Windows.Forms.OpenFileDialog' does not contain a definition for 'ShowPinnedPlaces' (CS0117)
			};
			DialogResult result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK) {
				FileName = openFileDialog.FileName;
				OpenFile(FileName);
			}
		}

		private void buttonRefresh_Click(object sender, EventArgs e) {
			LoadText(textBoxSourceMd.Text, FileName);
		}

		private void buttonSave_Click(object sender, EventArgs e) {
			string saveFile = "readme.rtf";
			var saveFileDialog = new SaveFileDialog() {
				Filter = "Rich Text|*.rtf|All files|*.*",
				FileName = saveFile,
				OverwritePrompt = true
			};
			DialogResult result = saveFileDialog.ShowDialog();
			if (result == DialogResult.OK) {
				saveFile = saveFileDialog.FileName;
				File.WriteAllText(saveFile, rtfText); // DON'T specify UTF-8 encoding. It will add the byte markers at the front, making the file incompatible with Word/Wordpad
			}
		}

		private void checkBoxShowSourceMd_CheckedChanged(object sender, EventArgs e) {
			UpdateSplitters();
		}

		private void checkBoxShowRtfCode_CheckedChanged(object sender, EventArgs e) {
			UpdateSplitters();
		}

		private void UpdateSplitters() {
			textBoxSourceMd.Visible = checkBoxShowSourceMd.Checked;
			richTextBoxRtfCode.Visible = checkBoxShowRtfCode.Checked;
			if (checkBoxShowSourceMd.Checked) {
				splitContainer1.SplitterDistance = splitContainer1.Width / 2;
			} else {
				splitContainer1.SplitterDistance = 0;
			}

			if (checkBoxShowRtfCode.Checked) {
				splitContainer2.SplitterDistance = splitContainer2.Width - (splitContainer2.Width / 3);
			} else {
				splitContainer2.SplitterDistance = splitContainer2.Width;
			}
		}

		private void textBoxSourceMd_TextChanged(object sender, EventArgs e) {
			if (checkBoxLiveUpdate.Checked) {
				if (!timerUpdate.Enabled) {
					// Debug.WriteLine("  Start timer");
					timerUpdate.Start();
				} else {
					// Debug.WriteLine("    Timer already running, restarting");
					timerUpdate.Enabled = false;
					timerUpdate.Start();

				}
			}
		}

		private void timerUpdate_Tick(object sender, EventArgs e) {
			//Debug.WriteLine("Timer tick, update text");
			LoadText(textBoxSourceMd.Text, FileName);
			timerUpdate.Stop();

		}

		private void buttonSaveMd_Click(object sender, EventArgs e) {
			string saveFile = FileName;
			var saveFileDialog = new SaveFileDialog() {
				Filter = "Markdown|*.md|All files|*.*",
				FileName = saveFile,
				OverwritePrompt = true
			};
			DialogResult result = saveFileDialog.ShowDialog();
			if (result == DialogResult.OK) {
				saveFile = saveFileDialog.FileName;
				File.WriteAllText(saveFile, textBoxSourceMd.Text); // DON'T specify UTF-8 encoding. It will add the byte markers at the front
			}
		}

		private void richTextBoxRtfView_LinkClicked(object sender, LinkClickedEventArgs e) {
			// Debug.WriteLine(String.Format("Link Clicked: {0}, start: {1}, length{2}",e.LinkText,e.LinkStart,e.LinkLength));
			string linkURL = e.LinkText;
			if (linkURL != null) {
				string docFolder = Path.GetDirectoryName(FileName) + "";
				string linkPath = Path.Combine(docFolder, linkURL);
				Debug.WriteLine(String.Format("Open file: {0} (folder{1}, linkURL{2})", linkPath, docFolder, linkURL));
				if (File.Exists(linkPath)) {
					try {
						OpenFileExternal(linkPath);
					} catch {
						Debug.WriteLine("Exception when opening file: " + linkPath);
					}

				} else {
					try {
						OpenLink(linkURL);
					} catch {
						Debug.WriteLine("Exception when opening link: " + linkURL);
					}
				}
			} else {
				Debug.WriteLine("Link is null");
			}

		}

		public static void OpenLink(string url) {
			Process.Start(new ProcessStartInfo() {
				FileName = url,
				UseShellExecute = true
			});
		}

		private void OpenFileExternal(string file) {
			if (File.Exists(file)) {
				Process.Start(new ProcessStartInfo() {
					FileName = file,
					UseShellExecute = true
				});
			} else {
				Debug.WriteLine("Can't open file, not found: " + file);
			}
		}

		private void richTextBoxRtfView_TextChanged(object sender, EventArgs e) {
			richTextBoxRtfCode.Text = richTextBoxRtfView.Rtf;
		}

		void button5_Click(object sender, EventArgs e) {
			scrollRichTextBox( reverse: true);

		}
		void button6_Click(object sender, EventArgs e) {
			scrollRichTextBox();
		}
	}
}
