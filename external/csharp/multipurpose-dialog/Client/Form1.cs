using System;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using Utils;

namespace Client {

	public partial class Form1 : Form {

		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}
		private void InitializeComponent() { }
		
		public Form1() {
			InitializeComponent();
			List<string> astr;
			do { //crappy endless loop just to demonstrate dialog, can press ESC or Cancel to exit Dialog
				string res = new CMsgDlg().ShowDialog("Multi Option Choice",
					"Filter Warning", new string[] { "Filter", "Skip", "Cancel" });
				if (res == "Cancel")
					break; // Exit loop
                else if (res == "Skip")
					continue; // Don’t filter and continue to next iteration
				// ...else just Filter it
			} while (null == null);

			do { //crappy endless loop just to demonstrate dialog, can press ESC or Cancel to exit Dialog
				astr = new CMsgDlg().ShowDialogTextBox("Units", new string[] { "Enter units:", "  " });
				string unit = astr[1].Trim(); // Entered string
			} while (astr[0] != "Cancel");
		}
		
		protected override void Dispose(bool disposing) {
			if (disposing) {
			}
			base.Dispose(disposing);
		}
	}
}
