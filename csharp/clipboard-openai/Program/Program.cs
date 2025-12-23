using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using GenerativeAI;

namespace TalkingClipboard {
	static class Program {
		[STAThread]
		// NOTE: an entry point cannot be marked with the 'async' modifier
		static void Main( ){
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainFrm());
		}
	}
}
