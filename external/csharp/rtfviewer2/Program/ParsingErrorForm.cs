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
