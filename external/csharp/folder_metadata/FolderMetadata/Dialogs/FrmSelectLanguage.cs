using HKS.FolderMetadata.Configuration.Internationalization;
using HKS.FolderMetadata.Dialogs.Helpers;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs
{
	/// <summary>
	/// The FrmSelectLanguage form allows to select the language in which the application will run.
	/// </summary>
	public partial class FrmSelectLanguage : Form
	{
		/// <summary>
		/// This constructor is called by any other constructor.
		/// </summary>
		public FrmSelectLanguage()
		{
			InitializeComponent();
			TranslateForm();
			this.Icon = SystemIcons.Question;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="currentLanguage">The current language of the system.</param>
		/// <param name="languageFamily">The current language family of the system.</param>
		public FrmSelectLanguage(string currentLanguage, string languageFamily)
			: this()
		{
			AddLanguages(currentLanguage, languageFamily);
		}

		private void TranslateForm()
		{
			this.Text = Strings.FrmSelectLanguage_Text;
			this.btnOK.Text = Strings.FrmSelectLanguage_btnOK_Text;
			this.colLanguage.Text = Strings.FrmSelectLanguage_colLanguage_Text;
			this.colName.Text = Strings.FrmSelectLanguage_colName_Text;
		}

		private void AddLanguages(string currentLanguage, string languageFamily)
		{
			string[] supportedLanguages = SupportedLanguages.GetSupportedLanguages();
			foreach (string supportedLanguage in supportedLanguages)
			{
				AddLanguage(supportedLanguage, currentLanguage, languageFamily);
			}
		}

		/// <summary>
		/// Adds the specified language and selects it if it is the language the user uses.
		/// </summary>
		/// <param name="languageName"></param>
		/// <param name="currentLanguage"></param>
		/// <param name="languageFamily"></param>
		private void AddLanguage(string languageName, string currentLanguage, string languageFamily)
		{
			CultureInfo cultureInfo = new CultureInfo(languageName);

			ListViewItem item = lvLanguages.Items.Add(cultureInfo.DisplayName);
			item.SubItems.Add(languageName);
			item.Tag = languageName;

			if (lvLanguages.SelectedItems.Count > 0)
			{
				return;
			}

			if (string.Equals(languageName, currentLanguage, StringComparison.InvariantCultureIgnoreCase) ||
				string.Equals(languageName, languageFamily, StringComparison.InvariantCultureIgnoreCase))
			{
				item.Selected = true;
			}
		}

		public string SelectedLanguageName
		{
			get
			{
				if (HasSelectedItem())
				{
					return (string)lvLanguages.SelectedItems[0].Tag;
				}

				return null;
			}
		}

		private bool HasSelectedItem()
		{
			return (lvLanguages.SelectedItems != null && lvLanguages.SelectedItems.Count > 0);
		}

		private void lvLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = HasSelectedItem();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			ConfirmSelection();
		}

		private void lvLanguages_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && btnOK.Enabled)
			{
				ConfirmSelection();
			}
		}

		private void ConfirmSelection()
		{
			this.DialogResult = DialogResult.OK;
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
