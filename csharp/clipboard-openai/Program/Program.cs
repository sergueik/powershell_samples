using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GenerativeAI;

namespace TalkingClipboard {
	static class Program {
		[STAThread]
		static void Main( ){
			// NOTE: Windows Form apps do not have access to Environment
			var apiKey = "AIzaSyC_ozaYQt9z91jQEryUhhIQBEr9Q1yhvC0";
			Environment.SetEnvironmentVariable("GOOGLE_API_KEY", apiKey, EnvironmentVariableTarget.User);
			var model = new GenerativeModel(apiKey, GoogleAIModels.Gemini2Flash);
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainFrm());
		}
	}
}
