using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GenerativeAI;

namespace TalkingClipboard
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
    	// Windows Form apps do not have access to Environment
    	var apiKey = ""; // Environment.GetEnvironmentVariable("GOOGLE_API_KEY", EnvironmentVariableTarget.User);
		var model = new GenerativeModel(apiKey, GoogleAIModels.Gemini2Flash);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainFrm());
    }
  }
}
