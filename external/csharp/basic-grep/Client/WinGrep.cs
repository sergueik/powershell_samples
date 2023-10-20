using System;
using System.Windows.Forms;

namespace Utils {
	public class WinGrep: Grep {
		private Client.Form1 form;
		public WinGrep(Client.Form1 form) {
			this.form = form;
		}
		protected override  void statusMsg(String message) {
			// NOTE: System.InvalidOperationException: Cross-thread operation not valid: Control 'txtResults' accessed from a thread other than the thread it was created on.
			// see also:
			// https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
			
			// this.form.TxtResults.Text =  message;
			
			this.form.TxtResults.Invoke(new MethodInvoker(delegate {
				this.form.TxtResults.Text = message;
			}));
		}
		
		protected override void progressMsg(String message) {
			
		}
	
		protected override void errorMsg(String message){
			this.form.TxtResults.Invoke(new MethodInvoker(delegate {
				MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}));
		}

	}
}
