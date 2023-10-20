using System;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Configuration.Internationalization
{
	/// <summary>
	/// The LanguageConfigurator class contains function for internationalization of the dialogs and texts.
	/// </summary>
	static class LanguageConfigurator
	{
		private static bool forceShowMnemonics = false;
		/// <summary>
		/// For languages with non-alphabetic languages, the keyboard shortcuts (e.g. on buttons or labels) will be shown permanently.
		/// </summary>
		public static bool ForceShowMnemonics
		{
			get
			{
				return forceShowMnemonics;
			}
		}

		/// <summary>
		/// Sets the language for the entire application either to the specified language, environment specific, default (English), or opens a dialog to select the desired language.
		/// </summary>
		/// <param name="language">If null or empty, uses either the language of the system, or English it no translation in the specified language is available.
		/// If value is "select", opens a dialog for language selection.
		/// For any other value, tries to load the translation for the specified language. Should this not be possible, uses default (English).</param>
		public static void SetLanguage(string language)
		{
			bool isCurrentLanguage;
			language = GetLanguage(language, out isCurrentLanguage);

			if (!isCurrentLanguage) //We only need to set the language to the thread if it was changed either by selection from form or command line parameter.
			{
				SetLanguageToThread(language);
			}

			//See if we need to force mnemonics (e.g. for Chinese or Japanese). This is specified in the related resource file for that language.
			if (!bool.TryParse(Strings.ForceMnemonics, out forceShowMnemonics))
			{
				forceShowMnemonics = true; //Show mnemonics if parsing fails.
			}
		}

		/// <summary>
		/// Gets the language if either none has been specified or if the user specified "select" to select it from the dialog.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="isCurrentLanguage"></param>
		/// <returns></returns>
		private static string GetLanguage(string language, out bool isCurrentLanguage)
		{
			string currentLanguage = System.Globalization.CultureInfo.CurrentCulture.Name;
			if (string.IsNullOrEmpty(language))
			{
				language = currentLanguage;
				isCurrentLanguage = true;
			}
			else if (string.Equals(language, "select", StringComparison.InvariantCultureIgnoreCase))
			{
				string currentLanguageFamily = GetFamilyName(currentLanguage);
				using (Dialogs.FrmSelectLanguage frm = new Dialogs.FrmSelectLanguage(currentLanguage, currentLanguageFamily))
				{
					if (frm.ShowDialog() == DialogResult.OK)
					{
						language = frm.SelectedLanguageName;
						isCurrentLanguage = System.Globalization.CultureInfo.CurrentCulture.Name.Equals(language, StringComparison.InvariantCultureIgnoreCase);
					}
					else
					{
						language = currentLanguage;
						isCurrentLanguage = true;
					}
				}
			}
			else
			{
				isCurrentLanguage = System.Globalization.CultureInfo.CurrentCulture.Name.Equals(language, StringComparison.InvariantCultureIgnoreCase);
			}

			if (language.StartsWith("zh-", StringComparison.InvariantCultureIgnoreCase))
			{
				language = GetLanguageZH(language, ref isCurrentLanguage);
			}

			return language;
		}

		/// <summary>
		/// Translate traditional Chinese languages to zh-TW.
		/// </summary>
		/// <param name="language">The language to check</param>
		/// <returns>Returns zh-TW for traditional Chinese languages or the input value.</returns>
		private static string GetLanguageZH(string language, ref bool isCurrentLanguage)
		{
			if (language.Equals("zh-Hant", StringComparison.InvariantCultureIgnoreCase) || //traditional Chinese
				language.Equals("zh-HK", StringComparison.InvariantCultureIgnoreCase) || //Hong Kong
				language.Equals("zh-MO", StringComparison.InvariantCultureIgnoreCase)) //Macao
			{
				isCurrentLanguage = false;
				return SupportedLanguages.CHINESE_TRADITIONAL;
			}

			return language;
		}

		/// <summary>
		/// Sets the specified language to be used by the application.
		/// </summary>
		/// <param name="language">The language to use.</param>
		private static void SetLanguageToThread(string language)
		{
			try
			{
				System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(language);
				System.Threading.Thread.CurrentThread.CurrentCulture = culture;
				System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Gets the family name from a language (e.g. "en" from "en-US").
		/// </summary>
		/// <param name="language">The language to convert (e.g. "en-US")</param>
		/// <returns>Returns the family name.</returns>
		/// <exception cref="System.NullReferenceException"
		private static string GetFamilyName(string language)
		{
			int index = language.LastIndexOf('-'); //We use LastIndexOf, because for some languages there may be several "families" (e.g. cyrillic Mongolian [mn, mn-Cyrl, mn-MN], traditional Mongolian [mn-Mong, mn-Mong-CN, mn-Mong-MN]).
			if (index > 0)
			{
				language = language.Substring(0, index);
			}

			return language;
		}
	}
}
