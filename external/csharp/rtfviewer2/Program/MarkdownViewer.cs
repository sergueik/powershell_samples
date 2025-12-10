using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

using Utils;

namespace Program {
	public partial class MarkdownViewer : Form {
		string rtfText = string.Empty;
		string FileName = "README.md";
		bool errorPopup = true;
		private Thread renderThread;
		private object renderLock = new object();
		private const string versionString = "0.9.0";
		private int selectionIndex = 0 ;

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

		// NOTE: Error CS1736: Default parameter value for 'position' must be a compile-time constant
		private void scrollRichTextBox(string search = "\u200B\u200B\u200B", int position = 0, bool reverse = false) {
			int findIndex = -1;
			if (selectionIndex != 0)
			if (position == 0)
				position = selectionIndex;
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextbox.find?view=windowsdesktop-10.0#system-windows-forms-richtextbox-find(system-string-system-int32-system-windows-forms-richtextboxfinds)
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.richtextboxfinds?view=netframework-4.5
			// NOTE: if search is not in richTextBox.Text, RichTextBox cannot scroll to it.
			RichTextBoxFinds options = reverse ? RichTextBoxFinds.Reverse | RichTextBoxFinds.NoHighlight : RichTextBoxFinds.NoHighlight;
			// Ensure that a search string has been specified and a valid start point.
			if (search.Length > 0 && position >= 0) {
				// Debug.WriteLine(BitConverter.ToString(Encoding.UTF8.GetBytes(search));
				if (reverse) {
					Debug.WriteLine(String.Format("reverse search within {0}...{1} with {2}", 0, position, options));

					findIndex =
					richTextBoxRtfView.Find(search, 0, position, options);
					// NOTE: For a reverse search on RichTextBox,
					// use 4 argument override of find method: int findIndex = richTextBox1.Find(textToFind, startIndex, endIndex, RichTextBoxFinds.Reverse);
					// where startIndex should typically be set to 0 (the beginning of the text).
					// endIndex should be set to the desired end point of your reverse search.
					// This tells the Find method to search the text before the endIndex,
					// starting from the endIndex and moving backward towards startIndex
				} else {
					Debug.WriteLine(String.Format("search from {0} with {1}", position, options));
					findIndex = richTextBoxRtfView.Find(search, position, options);
				}
				if (findIndex != -1) {
					Debug.WriteLine(String.Format("Found: {0}", findIndex));
					selectionIndex = reverse ? Math.Max(findIndex - search.Length, 0) : findIndex + search.Length;
					richTextBoxRtfView.Select(findIndex, 0);
					richTextBoxRtfView.ScrollToCaret();
				} else
					Debug.WriteLine("Not Found.");
			}
		}

		private void LoadText(string text, string fileName) {
			var rtfConverter = new RtfConverter(fileName);

			string payload = rtfConverter.ConvertText(text);
			if (rtfConverter.Errors.Count > 0 && errorPopup) {
				var errorForm = new ParsingErrorForm(rtfConverter.Errors, () => errorPopup = false);
	            errorForm.ShowDialog(this); // application modal
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

		private void btnRenderClick(object sender, EventArgs e) {
			LoadText(textBoxSourceMd.Text, FileName);
		}

		private void textBoxSourceMd_TextChanged(object sender, EventArgs e) {
			// NOTE: guard against
			// System.InvalidOperationException: Invoke or BeginInvoke cannot be called on a control until
			// the window handle has been created.

			if (!this.IsHandleCreated) return;
		    var text = textBoxSourceMd.Text;
		    var file = FileName;

		    var thread = new Thread(() => {
		        var rtfConverter = new RtfConverter(file);
		        var payload = rtfConverter.ConvertText(text);

		        // Invoke UI update
		        this.Invoke((Action)(() => {
		            bool oldReadonly = richTextBoxRtfView.ReadOnly;
		            richTextBoxRtfView.ReadOnly = false;
		            richTextBoxRtfView.Rtf = payload;
		            richTextBoxRtfView.ReadOnly = oldReadonly;

		            // Show errors in custom form
		            if (rtfConverter.Errors.Count > 0 && errorPopup) {
		                var errorForm = new ParsingErrorForm(rtfConverter.Errors, () => errorPopup = false);
		                errorForm.ShowDialog(this); // application modal
		            }
		        }));
		    });

		    thread.IsBackground = true;
		    thread.SetApartmentState(ApartmentState.STA); // just in case
		    thread.Start();
		}

		void btnScrollUp_Click(object sender, EventArgs e) {
			scrollRichTextBox(reverse: true);
		}

		void btnScrollDown_Click(object sender, EventArgs e) {
			scrollRichTextBox();
		}
	}

}
