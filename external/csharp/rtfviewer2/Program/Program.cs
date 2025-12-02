using System;
using System.Linq;
using System.Windows.Forms;

namespace Program {
	// NOTE: Static class 'Program.Program' cannot derive from type 'System.Windows.Forms.Form'.
	// Static classes must derive from object. (CS0713) - C:\developer\sergueik\powershell_samples\external\csharp\rtfviewer2\Program\Program.cs:11,33
	internal static class Program {
        [STAThread]
        static void Main(string[] args) {
            // NOTE: target
            // https://learn.microsoft.com/en-us/dotnet/desktop/winforms/whats-new/net60?view=netdesktop-6.0#new-application-bootstrap
            // see also: https://aka.ms/applicationconfiguration.
            // ApplicationConfiguration.Initialize();
            // To customize application configuration such as set high DPI settings or default font,

            Application.EnableVisualStyles();
	        Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MarkdownViewer(args));
        }
    }
}