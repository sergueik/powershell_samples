using System;
using System.Diagnostics;
using System.Threading;
using Program;

namespace Demo {
	class Confirmation {
		private static bool _confirm = false;
		public static bool Confirm {
			get { return _confirm; }
			set { _confirm = value; }
		}

		[STAThread]
		static void Main(string[] args) {
			var dialog = new Confirmation();
			// TODO: clear screen
			// Console.Error.WriteLine("Confirmed: " + Confirmation.Confirm);
			// System.Environment.Exit(0);
		}

		public Confirmation(){
			// Create a console form programatically, rather than loading it from
			// disk, to prompt the user for confirmation that they want to exit
			// the application.
			var confirm = new ConsoleForm(80, 30);
			confirm.Name = "Leave Event Browser?";

			confirm.Labels.Add(new Label("lblTitle", new Point(1, 2), 12, "Confirm Exit", ConsoleColor.Green, ConsoleColor.Black));

			confirm.Labels.Add(new Label("lblSure", new Point(1, 4), 19, "Are you sure? (y/N)", ConsoleColor.Gray, ConsoleColor.Black));

			confirm.Labels.Add(new Label("lblEsc", new Point(1, 8), 34, "Hit Esc to return to the main menu", ConsoleColor.Yellow, ConsoleColor.Black)); 
 
			confirm.Textboxes.Add(new Textbox("txtInput", new Point(21, 4), 1, "N", ConsoleColor.Black, ConsoleColor.White)); 

			// Show the screen and wait for feedback.
			confirm.FormComplete += new ConsoleForm.onFormComplete(ExitConfirm_FormComplete);
			confirm.FormCancelled += new ConsoleForm.onFormCancelled(ExitConfirm_FormCancelled);

			confirm.Render();
		
		}
		private static void ExitConfirm_FormComplete(ConsoleForm sender, FormCompleteEventArgs e) {
			if (sender.Textboxes["txtInput"].Text.ToUpper() == "Y"){
				Confirm = true;
			}
			else {
				sender.SetFocus(sender.Textboxes["txtInput"]);
				e.Cancel = true;
			}
		}
		private static void ExitConfirm_FormCancelled(ConsoleForm sender, EventArgs e) {
			sender.SetFocus(sender.Textboxes["txtInput"]);
			Confirm = false;
			// NOTE: cannot set Cancel - not the kind of EventArgs
		}
	}
}
