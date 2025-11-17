using System;
using System.Windows.Forms;
using System.Drawing;

// based on:
// https://github.com/chris-mackay/DialogMessage
// A homemade messagebox for Windows

namespace Utils {

	// NOTE:
	// Static class 'Utils.DMessage' cannot derive from type 'System.Windows.Forms.Form'. Static classes must derive from object. (CS0713)
	public /* static  */ class DialogMessage: Form  {

		public enum MsgIcons {
			None = 0,
			Question = 1,
			Info = 2,
			Warning = 3,
			Error = 4,
			Shield = 5
		}

		// Message button enum for switch statement in ShowMessage
		// to the properties of the form buttons and the response code DialogResult
		public enum MsgButtons {
			OK = 0,
			OKCancel = 1,
			YesNo = 2
		}


		public static DialogResult ShowMessage(string _windowTitle, string _mainInstruction, MsgButtons _msgButtons, MsgIcons _msgIcons = MsgIcons.None, string _content = "", bool debug = false, string clipboardText = "") {
			var main = new DialogMessage();
			main.ClipboardText = clipboardText;
			main.Debug = debug;
			main.Height = 157;
			main.Text = _windowTitle;
			main.mainInstruction.Text = _mainInstruction;
			main.content.Text = _content;
			switch (_msgButtons) {
			// Button1 is the left button
			// Button2 is the right button

				case MsgButtons.OK:

					main.Button1.Visible = false;
					main.Button2.DialogResult = DialogResult.OK;
					main.Button2.Text = "OK";
					main.AcceptButton = main.Button2;
					main.Button2.TabIndex = 0;
					main.ActiveControl = main.Button2;

					break;

				case MsgButtons.OKCancel:

					main.Button1.DialogResult = DialogResult.OK;
					main.Button2.DialogResult = DialogResult.Cancel;
					main.Button1.Text = "OK";
					main.Button2.Text = "Cancel";
					main.AcceptButton = main.Button2;
					main.Button1.TabIndex = 1;
					main.Button2.TabIndex = 0;
					main.ActiveControl = main.Button2;

					break;

				case MsgButtons.YesNo:

					main.Button1.DialogResult = DialogResult.Yes;
					main.Button2.DialogResult = DialogResult.No;
					main.Button1.Text = "Yes";
					main.Button2.Text = "No";
					main.AcceptButton = main.Button2;
					main.Button1.TabIndex = 1;
					main.Button2.TabIndex = 0;
					main.ActiveControl = main.Button2;

					break;

				default:
					break;
			}

			if (_msgIcons != MsgIcons.None) {
				main.msgIcon.Visible = true;

				switch (_msgIcons) {
					case MsgIcons.Question:

						main.msgIcon.Image = SystemIcons.Question.ToBitmap();
						break;

					case MsgIcons.Info:

						main.msgIcon.Image = SystemIcons.Information.ToBitmap();
						break;

					case MsgIcons.Warning:

						main.msgIcon.Image = SystemIcons.Warning.ToBitmap();
						break;

					case MsgIcons.Error:

						main.msgIcon.Image = SystemIcons.Error.ToBitmap();
						break;

					case MsgIcons.Shield:

						main.msgIcon.Image = SystemIcons.Shield.ToBitmap();
						break;

					default:
						break;
				}
			} else {
				main.msgIcon.Visible = false;
			}
			return main.ShowDialog();
		}

		private bool debug;
				public bool Debug {
			get { return debug; }
			set {
				debug = value;
			}
		}
		private string clipboardText;
			public string ClipboardText {
			get { return clipboardText; }
			set {
				clipboardText = value;
				}
			}

		private System.ComponentModel.IContainer components = null;
		internal Panel whiteSpace;
		internal TableLayoutPanel tablePanelLayout;
		internal Label content;
		internal Label mainInstruction;
		internal Button Button1;
		internal Button Button2;
		internal PictureBox msgIcon;

		public DialogMessage() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			whiteSpace = new Panel();
			content = new Label();
			mainInstruction = new Label();
			tablePanelLayout = new TableLayoutPanel();
			Button1 = new Button();
			Button2 = new Button();
			msgIcon = new PictureBox();
			whiteSpace.SuspendLayout();
			tablePanelLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(msgIcon)).BeginInit();
			SuspendLayout();

			whiteSpace.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
			| AnchorStyles.Left)
			| AnchorStyles.Right)));
			whiteSpace.BackColor = Color.White;
			whiteSpace.Controls.Add(content);
			whiteSpace.Controls.Add(mainInstruction);
			whiteSpace.Location = new Point(0, 0);
			whiteSpace.Name = "whiteSpace";
			whiteSpace.Size = new Size(383, 79);
			whiteSpace.TabIndex = 1;

			content.AutoSize = true;
			content.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			content.Location = new Point(54, 51);
			content.MaximumSize = new Size(305, 0);
			content.Name = "content";
			content.Size = new Size(44, 13);
			content.TabIndex = 1;
			content.Text = "Content";

			mainInstruction.AutoSize = true;
			mainInstruction.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			mainInstruction.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(188)))));
			mainInstruction.Location = new Point(54, 13);
			mainInstruction.Margin = new Padding(5, 0, 3, 18);
			mainInstruction.MaximumSize = new Size(305, 0);
			mainInstruction.Name = "mainInstruction";
			mainInstruction.Size = new Size(123, 21);
			mainInstruction.TabIndex = 0;
			mainInstruction.Text = "Main Instruction";

			tablePanelLayout.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
			tablePanelLayout.ColumnCount = 2;
			tablePanelLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tablePanelLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tablePanelLayout.Controls.Add(Button1, 0, 0);
			tablePanelLayout.Controls.Add(Button2, 1, 0);
			tablePanelLayout.Location = new Point(232, 83);
			tablePanelLayout.Name = "tablePanelLayout";
			tablePanelLayout.RowCount = 1;
			tablePanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
			tablePanelLayout.Size = new Size(146, 29);
			tablePanelLayout.TabIndex = 2;

			Button1.Anchor = AnchorStyles.None;
			Button1.Location = new Point(3, 3);
			Button1.Name = "Button1";
			Button1.Size = new Size(67, 23);
			Button1.TabIndex = 0;
			Button1.Text = "Button1";

			Button2.Anchor = AnchorStyles.None;
			Button2.Location = new Point(76, 3);
			Button2.Name = "Button2";
			Button2.Size = new Size(67, 23);
			Button2.TabIndex = 1;
			Button2.Text = "Button2";
			//
			// msgIcon
			//
			msgIcon.BackColor = Color.White;
			msgIcon.Location = new Point(14, 14);
			msgIcon.Name = "msgIcon";
			msgIcon.Size = new Size(32, 32);
			msgIcon.TabIndex = 3;
			msgIcon.TabStop = false;

			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(383, 118);
			Controls.Add(msgIcon);
			Controls.Add(tablePanelLayout);
			Controls.Add(whiteSpace);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(360, 157);
			Name = "DialogMessage";
			ShowIcon = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Window Title";
			Load += new System.EventHandler(DialogMessage_Load);
			whiteSpace.ResumeLayout(false);
			whiteSpace.PerformLayout();
			tablePanelLayout.ResumeLayout(false);
			// add handler to second button
			Button2.Click += new System.EventHandler(button2_Click);
			((System.ComponentModel.ISupportInitialize)(msgIcon)).EndInit();
			ResumeLayout(false);
		}

		private void button2_Click(object sender, EventArgs args) {
			if (debug) {
				MessageBox.Show("You clicked the button 2 and debug was set");
			}
			// NOTE: Excepion
			// System.Runtime.InteropServices.ExternalException
			// (0x800401D0): Requested Clipboard operation did not succeed.
			// at System.Windows.Forms.Clipboard.ThrowIfFailed(Int32 hr)
			// at System.Windows.Forms.Clipboard.SetDataObject(Object data, Boolean copy, Int32 retryTimes, Int32 retryDelay)
			// at System.Windows.Forms.Clipboard.SetDataObject(Object data, Boolean copy)
			if (!string.IsNullOrEmpty(this.clipboardText)) {
				// Handle the case then clipboardText contains just a return
				if (this.clipboardText.Trim(new char[] { '\r', '\n' }) != "") {
					try {
						Clipboard.SetDataObject(this.clipboardText, true);
					} catch (Exception e) {
						MessageBox.Show(e.ToString(), "Excepion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}
			// other handlers still execute
			return;
		}

		private void DialogMessage_Load(object sender, EventArgs e) {
			// Once the ShowMessage function is called and the form appears
			// the code below makes the appropriate adjustments so the text appears properly

			// If no icon will be shown then shift the MainInstruction and Content
			// left to an appropriate location

			// Adjust the MaximumSize to compensate for the shift left.
			if (msgIcon.Visible == false) {
				mainInstruction.Location = new Point(12, mainInstruction.Location.Y);
				mainInstruction.MaximumSize = new Size(353, 0);

				content.Location = new Point(12, content.Location.Y);
				content.MaximumSize = new Size(353, 0);
			}

			// Gets the Y location of the bottom of MainInstruction
			int mainInstructionBottom = mainInstruction.Location.Y + mainInstruction.Height;

			// Gets the Y location of the bottom of Content (unused)
			//int contentBottom = content.Location.Y + content.Height;

			// Offsets the top of Content from the bottom of MainInstruction
			int contentTop = mainInstructionBottom + 18; // 18 just looked nice to me

			// Sets new location of the top of Content
			content.Location = new Point(content.Location.X, contentTop);

			if (string.IsNullOrEmpty(content.Text))
                // If only MainInstruction is provided then make the form a little shorter
                Height += (mainInstruction.Location.Y + mainInstruction.Height) - 50;
			else
				Height += (content.Location.Y + content.Height) - 60;
		}
	}
}
