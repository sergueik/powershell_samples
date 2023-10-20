using HKS.FolderMetadata.Configuration;
using System;
using System.Windows.Forms;
using HKS.FolderMetadata.Dialogs;
using HKS.FolderMetadata.Configuration.Internationalization;

namespace HKS.FolderMetadata
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//Upgrade settings so we don't get an exception if values were changed.
			Properties.Settings.Default.Upgrade();

			//Set UI settings
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//Parse command line arguments
			ArgParser argParser = new ArgParser();

			//Set the language to use with this application.
			LanguageConfigurator.SetLanguage(argParser.Language);

			if (argParser.Register)
			{
				Registrator registrator = new Registrator();
				registrator.Register();
			}
			else if (argParser.Unregister)
			{
				Registrator registrator = new Registrator();
				registrator.Unregister();
			}
			else if (argParser.ConfigurationMode != FrmMain.ConfigureMode.None)
			{
				Application.Run(new FrmMain(argParser.ConfigurationMode));
			}
			else if (argParser.ShowHelp)
			{
				Application.Run(new FrmHelp());
			}
			else
			{
				Application.Run(new FrmMain(argParser.FolderPath));
			}			
		}
	}
}
