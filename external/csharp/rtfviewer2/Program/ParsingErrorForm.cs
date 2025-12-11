using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

using Utils;

namespace Program {
	public partial class ParsingErrorForm : Form {

		// Action callback to notify the parent form
		private readonly Action action;
		private List<string> errors = new List<string>();

		public ParsingErrorForm(List<string> errors, Action action, string text  = "Parsing Errors") {
			this.action = action;
			this.Text = text;
			this.errors.AddRange(errors);
			InitializeComponent();
		}


protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Fill after designer is done
        this.textBoxErrors.Text = string.Join(
            Environment.NewLine,
            this.errors
        );
    }
private void BtnStopShowing_Click(object sender, EventArgs e) {
			// Notify parent to disable future error popups
			if (action != null)
				action.Invoke();
			this.Close();
		}
	}
}
