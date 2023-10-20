using System;
using System.Linq;
using System.Windows.Forms;

namespace RealTimeEventLogReader {
	public partial class LogNamePrompt : Form {
		public LogNamePrompt() {
			InitializeComponent();
			btnOk.Click += BtnOk_Click;
			btnCancel.Click += BtnCancel_Click;
		}

		private void BtnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void BtnOk_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			this.Close();
		}

		public string GetLogName() {
			return txtLogName.Text;
		}
	}
}
