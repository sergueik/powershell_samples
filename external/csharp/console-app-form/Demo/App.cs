using System;
using System.Diagnostics;
using System.Threading;
using Program;

namespace Demo {
	class App {
		private const Int32 ENTRIES_PER_PAGE = 20;

		private static EventLog _log = new EventLog("Application");
		private static EventLogEntryCollection _entries = null;
		private static Int32 _index = 0;
		// tracks the top
		// of the visible window
		private static Timer _splashTimer = null;

		[STAThread]
		static void Main(string[] args) {
			App.RefreshArray();

			ConsoleForm splash = ConsoleForm.GetFormInstance(@".\Forms\Splash.xml");
			splash.Render();

			_splashTimer = new Timer(new TimerCallback(_splashTimer_Elapsed), splash, 5000, 5000);
		}


		private static void _splashTimer_Elapsed(object o) {
			_splashTimer.Change(Timeout.Infinite, Timeout.Infinite);
			_splashTimer.Dispose();
			_splashTimer = null;

			// Get the splash screen from the timer state object so it can
			// be disposed.
			var f = (ConsoleForm)o;
			((IDisposable)f).Dispose(); // Kill the keypress loop.
			f = null;

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
				// User validated. Show main menu
				ShowMainMenu();
			} else {
				// Account not found.
				sender.Labels["lblError"].Text = "Account not found.";
				sender.Textboxes["txtLoginID"].Text = string.Empty;
				sender.Textboxes["txtPassword"].Text = string.Empty;

				sender.SetFocus(sender.Textboxes["txtLoginID"]);

				e.Cancel = true; // Keep the form visible. Don't Dispose() it.
			}
		}

		private static void ShowMainMenu() {
			var menuForm = new ConsoleForm(80, 30);
			menuForm.Name = "Main Menu";

			var lblTitle = new Label("lblTitle", new Point(1, 2), 10, "Main Menu", ConsoleColor.Green, ConsoleColor.Black);

			var lblBrowse = new Label("lblBrowse", new Point(4, 4), 10, "1. Browse");

			var lblRefresh = new Label("lblRefresh", new Point(4, 5), 16, "2. Refresh Array");

			var lblExit = new Label("lblExit", new Point(4, 12), 10, "9. Exit");
			var lblChoice = new Label("lblChoice", new Point(4, 14), 2, ">>", ConsoleColor.Yellow, ConsoleColor.Black);

			var lblError = new Label("lblError", new Point(4, 16), 40, string.Empty, ConsoleColor.Red, ConsoleColor.Black);

			var txtInput = new Textbox("txtInput", new Point(6, 14), 1, string.Empty);
			menuForm.Labels.Add(lblTitle);
			menuForm.Labels.Add(lblBrowse);
			menuForm.Labels.Add(lblRefresh);
			menuForm.Labels.Add(lblExit);
			menuForm.Labels.Add(lblChoice);
			menuForm.Labels.Add(lblError);

			menuForm.Textboxes.Add(txtInput);

			menuForm.FormComplete += new ConsoleForm.onFormComplete(MenuSelection);
			menuForm.Render();
		}

		private static void RefreshArray() {
			_log = new EventLog("Application"); 
			_entries = _log.Entries;
		}

		private static void MenuSelection(ConsoleForm sender, FormCompleteEventArgs e) {
			string choice = sender.Textboxes["txtInput"].Text;

			// Reset the array index to the start.
			_index = 0;

			switch (choice) {
				case "1":
					App.BrowseLog();
					break;

				case "2":
					App.RefreshArray();
					ConsoleForm.GetFormInstance(@".\Forms\RefreshComplete.xml", null, new ConsoleForm.onFormCancelled(RefreshComplete_FormCancelled)).Render();
					break;

				case "9":
					App.ShowExitConfirmation();
					break;

				default :
					sender.Textboxes["txtInput"].Text = string.Empty;
					sender.Labels["lblError"].Text = "Invalid menu selection";
 					e.Cancel = true;
					break;
			}
		}

		private static void BrowseLog() {
			ConsoleForm browser = ConsoleForm.GetFormInstance(@".\Forms\Browser.xml", new ConsoleForm.onFormComplete(BrowseAction), new ConsoleForm.onFormCancelled(BrowseFinished));

			App.ParseScreen(browser);
			browser.Render();
		}

		private static void ParseScreen(ConsoleForm form) {
			Int32 indexPos = form.Labels["lblIndex"].Location.X;
			Int32 typePos = form.Labels["lblType"].Location.X;
			Int32 datePos = form.Labels["lblDate"].Location.X;
			Int32 timePos = form.Labels["lblTime"].Location.X;
			Int32 sourcePos = form.Labels["lblSource"].Location.X;

			for (Int32 i = 0; i < ENTRIES_PER_PAGE; i++) {
				if ((_index + i) == _entries.Count)
 					break;

				EventLogEntry entry = _entries[_index + i];
				EventLogEntryType entryType = entry.EntryType;
				ConsoleColor typeColour = ConsoleColor.White;

				// Change the colour of the label for the severity of the event log entry.
				switch (entryType) {
					case EventLogEntryType.Information:
						typeColour = ConsoleColor.Green; 
						break;

					case EventLogEntryType.Warning:
						typeColour = ConsoleColor.Yellow;
						break;

					case EventLogEntryType.Error:
						typeColour = ConsoleColor.Red;
						break;
				}

				string indexValue = (i + _index).ToString();

				Label indexLabel = new Label("lblIndex" + indexValue.ToString(), new Point(indexPos, i + 6), 5, indexValue);
				indexLabel.Foreground = ConsoleColor.DarkGray;

				Label typeLabel = new Label("lblType" + indexValue.ToCharArray(), new Point(typePos, i + 6), 12, entryType.ToString());
				typeLabel.Foreground = typeColour;

				Label dateLabel = new Label("lblDate" + indexValue.ToString(), new Point(datePos, i + 6), 10, entry.TimeGenerated.ToShortDateString());
				dateLabel.Foreground = ConsoleColor.Magenta;

				Label timeLabel = new Label("lblTime" + indexValue.ToString(), new Point(timePos, i + 6), 10, entry.TimeGenerated.ToShortTimeString());

				Label sourceLabel = new Label("lblSource" + indexValue.ToString(), new Point(sourcePos, i + 6), 32, entry.Source);

				// Add labels to the Labels collection
				form.Labels.Add(indexLabel);
				form.Labels.Add(typeLabel);
				form.Labels.Add(dateLabel);
				form.Labels.Add(timeLabel);
				form.Labels.Add(sourceLabel);

				// Create a textbox for each line to allow the user to select 
				// an event log entry.
				var selector = new Textbox("txt" + indexValue, new Point(indexPos - 1, i + 6), 1, string.Empty, ConsoleColor.White, ConsoleColor.DarkBlue);
				form.Textboxes.Add(selector);
			}

			// Add a label telling the user how they can exit the form to the
			// bottom of the screen.
			Label esc = new Label("lblEscape", new Point(3, ENTRIES_PER_PAGE + 7), 29, "Hit ESC to return to the menu");

			form.Labels.Add(esc);
		}

		private static void ShowExitConfirmation() {
			// Create a console form programatically, rather than loading it from
			// disk, to prompt the user for confirmation that they want to exit
			// the application.
			ConsoleForm confirm = new ConsoleForm(80, 30);
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

		private static void RefreshComplete_FormCancelled(ConsoleForm sender, System.EventArgs e) {
			App.ShowMainMenu();
		}

		private static void ExitConfirm_FormComplete(ConsoleForm sender, System.EventArgs e) {
			if (sender.Textboxes["txtInput"].Text.ToUpper() == "Y")
				System.Environment.Exit(0);
			else
				App.ShowMainMenu();
		}

		private static void ExitConfirm_FormCancelled(ConsoleForm sender, System.EventArgs e) {
			ConsoleForm.GetFormInstance(@".\Forms\Menu.xml", new ConsoleForm.onFormComplete(MenuSelection), null).Render();
		}

		private static void BrowseAction(ConsoleForm sender, System.EventArgs e) {
			// Read the textboxes to see if anything was selected.
			Textbox selection = null;

			foreach (Textbox t in sender.Textboxes) {
				if (t.Text.ToUpper() == "S") {
					selection = t;
					break;
				}
			}

			if (selection == null) {
				// No "S" was put beside any entry.
				if ((_entries.Count - _index) > ENTRIES_PER_PAGE) {
					// Not displaying the last page. Show the next page by advancing
					// the pointer and displaying the browser page.
					_index += ENTRIES_PER_PAGE;

					ConsoleForm browser = ConsoleForm.GetFormInstance(@".\Forms\Browser.xml", new ConsoleForm.onFormComplete(BrowseAction), new ConsoleForm.onFormCancelled(BrowseFinished));
					App.ParseScreen(browser);
					browser.Render(false);
				} else
					ConsoleForm.GetFormInstance(@".\Forms\Menu.xml", new ConsoleForm.onFormComplete(MenuSelection), null).Render();
			} else 
				// An "S" was put beside an entry. Show the detail screen for that event log entry.
				App.ShowDetail(Int32.Parse(selection.Name.Substring(3)));
		}
 
		private static void ShowDetail(Int32 index) {
			string message = _entries[index].Message.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
			string[] words = message.Split(' ');
			Int32 line = 4; // Line to start adding labels to.
			Int32 word = 0;

			ConsoleForm detail = ConsoleForm.GetFormInstance(@".\Forms\Detail.xml", null, new ConsoleForm.onFormCancelled(DetailFinished));

			do {
				Label label = new Label("lblLine" + line.ToString(), new Point(1, line++), detail.Width - 10);
				string text = string.Empty;

				do {
					text += words[word++] + " ";
					if ((word == words.Length) || text.Length > detail.Width - 20)
						break;
				} while (true);

				label.Text = text;
				detail.Labels.Add(label);
			} while(word != words.Length);

			detail.Labels.Add(new Label("lblEscToReturn", new Point(1, line + 2), 17, "Hit Esc to return", ConsoleColor.Yellow, ConsoleColor.Black));

			detail.Labels["lblIndex"].Text = index.ToString();
			detail.Render();
		}

		private static void DetailFinished(ConsoleForm sender, System.EventArgs e) {
			App.BrowseLog();
		}

		private static void BrowseFinished(ConsoleForm sender, System.EventArgs e) {
			ConsoleForm.GetFormInstance(@".\Forms\Menu.xml", new ConsoleForm.onFormComplete(MenuSelection), null).Render();
		}
	}
}
