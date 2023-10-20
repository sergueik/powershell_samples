using HKS.FolderMetadata.Configuration.Internationalization;
using HKS.FolderMetadata.Dialogs.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs
{
	/// <summary>
	/// FrmHelp shows a help in HTML format.
	/// </summary>
	public partial class FrmHelp : Form
	{
		public FrmHelp()
		{
			InitializeComponent();
			TranslateForm();
			this.Icon = SystemIcons.Question;
			this.webBrowser1.DocumentText = GetHelpPage();
		}

		private void TranslateForm()
		{
			this.Text = Strings.FrmHelp_Title;
			this.btnClose.Text = Strings.FrmHelp_btnClose_Text;
		}

		/// <summary>
		/// Retrieves the Help page from the resources and replaces the place holders with the translated help texts.
		/// </summary>
		/// <returns>Returns the localized Help page.</returns>
		private string GetHelpPage()
		{
			string exe = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string nameNoExe = System.IO.Path.GetFileNameWithoutExtension(exe);
			string html = Properties.Resources.Help;
			
			html = LocalizeHelpPage(html);
			html = html.Replace("%%NAME_EXE%%", exe);
			html = html.Replace("%%NAME_NOEXE%%", nameNoExe);

			return html;
		}

		/// <summary>
		/// Replaces the place holders with the translated help texts.
		/// </summary>
		/// <param name="html">The HTML of the help page</param>
		/// <returns>Returns the localized Help page.</returns>
		private string LocalizeHelpPage(string html)
		{
			html = html.Replace("%%TITLE%%", Strings.FrmHelp_Title);
			html = html.Replace("%%OPTION%%", Strings.FrmHelp_Option);
			html = html.Replace("%%EXAMPLE%%", Strings.FrmHelp_Example);
			html = html.Replace("%%DESCRPTION%%", Strings.FrmHelp_Description);

			html = html.Replace("%%INTRODUCTION%%", Strings.FrmHelp_Introduction);

			AddHelp(ref html, "HELP", "?", Strings.FrmHelp_Help_Description);
			AddHelp(ref html, "register", Strings.FrmHelp_Register_Description);
			AddHelp(ref html, "unregister", Strings.FrmHelp_Unregister_Description);
			AddHelp(ref html, "dir", "%%NAME_EXE%% /dir &quot;C:\\Documents\\My Games&quot;", Strings.FrmHelp_Dir_Description);
			AddHelp(ref html, "configure", "%%NAME_EXE%% /configure authors<br>%%NAME_EXE%% /configure tags", Strings.FrmHelp_Configure_Description);
			AddHelp(ref html, "lang", "%%NAME_EXE%% /lang DE /register", Strings.FrmHelp_Lang_Description);

			return html;
		}

		private void AddHelp(ref string html, string option, string description)
		{
			AddHelp(ref html, option.ToUpperInvariant(), option, "%%NAME_EXE%% /" + option, description);
		}

		private void AddHelp(ref string html, string option, string example, string description)
		{
			AddHelp(ref html, option.ToUpperInvariant(), option, example, description);
		}
		
		private void AddHelp(ref string html, string name, string option, string example, string description)
		{
			string tdOption = "%%OPTION_" + name + "%%";
			string tdExample = "%%EXAMPLE_" + name + "%%";
			string tdDescription = "%%DESCRIPTION_" + name + "%%";

			html = html.Replace(tdOption, option);
			html = html.Replace(tdExample, example);
			html = html.Replace(tdDescription, description);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		protected override void WndProc(ref Message m)
		{
			if (LanguageConfigurator.ForceShowMnemonics)
			{
				WndProcOverrides.ShowMnemonics(ref m);
			}
			base.WndProc(ref m);
		}
	}
}
