using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

using Utils;

namespace Program {
	public partial class MarkdownViewer : Form {
		string rtfText = string.Empty;
		string FileName = "README.md";
		bool errorPopup = false;
		private Thread renderThread;
		private object renderLock = new object();
		private string pendingText = null;

		public MarkdownViewer(string[] args) {
			InitializeComponent();
			ProcessArguments(args);
			OpenFile(FileName);
		}

		private void ProcessArguments(string[] args) {
			if (args.Length == 0)
				return;
			string file = args[0];
			if (File.Exists(file)) {
				FileName = file;
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
			// rtfText = rtfConverter.ConvertText(text);

			string payload = rtfConverter.ConvertText(text);
			if (rtfConverter.Errors.Count > 0 && errorPopup) {
				string errors = LineListToString(rtfConverter.Errors);
				DialogResult result = MessageBox.Show(errors + "\n\n To stop showing errors, press No", "Parsing error", MessageBoxButtons.YesNo);
				if (result == DialogResult.No) {
					errorPopup = false;
				}
			}

			// preserve selection
			int selStart = richTextBoxRtfView.SelectionStart;
			int selLength = richTextBoxRtfView.SelectionLength;
			// MUST set Readonly to false, otherwise images will not load.
			// https://developercommunity.visualstudio.com/t/richtextbox-fails-to-display-image/383903

			bool oldReadonly = richTextBoxRtfView.ReadOnly;
			richTextBoxRtfView.ReadOnly = false;
			richTextBoxRtfView.Rtf = payload;
			richTextBoxRtfView.ReadOnly = oldReadonly;

			// restore selection
			if (selStart <= richTextBoxRtfView.TextLength) {
				richTextBoxRtfView.SelectionStart = selStart;
				richTextBoxRtfView.SelectionLength = selLength;
				richTextBoxRtfView.ScrollToCaret();
			}

			rtfText = payload;
		}

		private string LineListToString(List<string> lines) {
			var sb = new StringBuilder();
			foreach (string line in lines) {
				sb.AppendLine(line);
			}
			return sb.ToString();
		}

		private void btnRenderClick(object sender, EventArgs e) {
			LoadText(textBoxSourceMd.Text, FileName);
		}

		private void textBoxSourceMd_TextChanged(object sender, EventArgs e) {
			// Capture the latest text
			string currentText = textBoxSourceMd.Text;

			lock (renderLock) {
				pendingText = currentText;

				// If a background render is already running, let it pick up the new pendingText
				if (renderThread == null || !renderThread.IsAlive) {
					renderThread = new Thread(RenderBackground);
					renderThread.IsBackground = true;
					renderThread.Start();
				}
			}
		}


		private void RenderBackground() {
			string textToRender;

			while (true) {
				lock (renderLock) {
					if (pendingText == null)
						break; // nothing left to render

					// Get the latest text and clear pending
					textToRender = pendingText;
					pendingText = null;
				}

				try {
					var rtfConverter = new Utils.RtfConverter(FileName);
					string payload = rtfConverter.ConvertText(textToRender);
					var errors = rtfConverter.Errors;

					// Invoke back to UI thread
					if (!IsDisposed && !Disposing) {
						Invoke((MethodInvoker)delegate {
							// preserve selection
							int selStart = richTextBoxRtfView.SelectionStart;
							int selLength = richTextBoxRtfView.SelectionLength;

							bool oldReadonly = richTextBoxRtfView.ReadOnly;
							richTextBoxRtfView.ReadOnly = false;
							richTextBoxRtfView.Rtf = payload;
							richTextBoxRtfView.ReadOnly = oldReadonly;

							if (selStart <= richTextBoxRtfView.TextLength) {
								richTextBoxRtfView.SelectionStart = selStart;
								richTextBoxRtfView.SelectionLength = selLength;
								richTextBoxRtfView.ScrollToCaret();
							}

							// Optional: show errors in custom form to have flexibility
							if (errors.Count > 0 && errorPopup) {
								this.Invoke((Action)(() => {
									var errorForm = new ParsingErrorForm(errors, () => {
										errorPopup = false;
									});
									errorForm.ShowDialog(this); // modal
								}));
							}

						});
					}
				} catch (Exception ex) {
					Debug.WriteLine("Render thread exception: " + ex);
				}
			}
		}

		void btnScrollUp_Click(object sender, EventArgs e) {
			scrollRichTextBox(reverse: true);
		}

		void btnScrollDown_Click(object sender, EventArgs e) {
			scrollRichTextBox();
		}
	}
	public class ParsingErrorForm : Form {
		private TextBox textBoxErrors;
		private Button btnClose;
		private Button btnStopShowing;

		// Action callback to notify the parent form
		private readonly Action action;
		public ParsingErrorForm(List<string> errors, Action action, string text  = "Parsing Errors") {
			this.action = action;
			this.Text = text;
			this.Size = new System.Drawing.Size(600, 400);

			textBoxErrors = new TextBox();
			textBoxErrors.Multiline = true;
			textBoxErrors.ReadOnly = true;
			textBoxErrors.Dock = DockStyle.Fill;
			textBoxErrors.ScrollBars = ScrollBars.Both;
			textBoxErrors.Text = string.Join(Environment.NewLine, errors);

			btnClose = new Button();
			btnClose.Text = "Close";
			btnClose.Dock = DockStyle.Bottom;
			btnClose.Height = 30;
			btnClose.Click += (s, e) => this.Close();

			this.Controls.Add(textBoxErrors);
			this.Controls.Add(btnClose);

			btnStopShowing = new Button {
				Text = "Stop Showing Errors",
				Dock = DockStyle.Bottom,
				Height = 30
			};
			btnStopShowing.Click += BtnStopShowing_Click;

			this.Controls.Add(btnStopShowing);
		}

		private void BtnStopShowing_Click(object sender, EventArgs e) {
			// Notify parent to disable future error popups
			if (action != null)
				action.Invoke();
			this.Close();
		}
	}
}
