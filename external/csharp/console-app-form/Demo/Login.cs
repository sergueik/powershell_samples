using System;
using System.Diagnostics;
using System.Threading;
using Program;

namespace Demo {
	class Login {

		[STAThread]
		static void Main(string[] args) {

			ConsoleForm login = ConsoleForm.GetFormInstance(@".\Forms\Login.xml", new ConsoleForm.onFormComplete(login_Complete), new ConsoleForm.onFormCancelled(login_Cancelled));
			login.KeyPressed += new ConsoleForm.onKeyPress(login_KeyPressed);
			login.Render();
		}

		static void login_KeyPressed(ConsoleForm sender, KeyPressEventArgs e) {
			// If an error was displayed, clear it on this keypress.
			if (sender.Labels["lblError"].Text != string.Empty)
				sender.Labels["lblError"].Text = string.Empty;
		}

		private static void login_Cancelled(ConsoleForm sender, EventArgs e) {
			System.Environment.Exit(0);
		}

		private static void login_Complete(ConsoleForm sender, FormCompleteEventArgs e) {
			if (sender.Textboxes["txtLoginID"].Text == "test" &&
			  sender.Textboxes["txtPassword"].Text == "test") {
				Console.Error.WriteLine("Logged in");
				System.Environment.Exit(0);
			} else {
				sender.Labels["lblError"].Text = "Account not found.";
				sender.Textboxes["txtLoginID"].Text = string.Empty;
				sender.Textboxes["txtPassword"].Text = string.Empty;

				sender.SetFocus(sender.Textboxes["txtLoginID"]);
				// Keep the form visible
				e.Cancel = true; 
			}
		}
	}
}