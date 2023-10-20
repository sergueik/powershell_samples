using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKS.FolderMetadata.Configuration.Internationalization
{
	/// <summary>
	/// The class SupportedLanguages contains constants of all languages that are supported by this application.
	/// Important: When adding new language constants, add them to the function <see cref="GetSupportedLanguages"/> as well.
	/// For further language code information, you might want to access the following document:
	/// "[MS-LCID]: Appendix A: Product Behavior | Microsoft Docs" at https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c.
	/// </summary>
	public static class SupportedLanguages
	{
		public const string ENGLISH_US = "en-US";//The associated resource file is Strings.resx.
		public const string FRENCH = "fr";
		public const string GERMAN = "de";
		public const string JAPANESE = "ja";
		public const string CHINESE_SIMPLIFIED = "zh-CN"; //We specify zh-CN here, but the resource file is named only "zh".
		public const string CHINESE_TRADITIONAL = "zh-TW";

		/// <summary>
		/// Returns an of supported language names.		
		/// 
		/// ************************ IMPORTANT ************************
		/// * When adding language support for new languages, ensure  *
		/// * that you add this language in this function as well.    *
		/// * This is required for Dialogs.FrmSelectLanguage.         *
		/// ***********************************************************
		/// </summary>
		/// <returns>Returns an of supported language names.</returns>
		public static string[] GetSupportedLanguages()
		{
			return new string[]
			{
				CHINESE_SIMPLIFIED,
				CHINESE_TRADITIONAL,
				ENGLISH_US,
				FRENCH,
				GERMAN,
				JAPANESE
			};
		}
	}
}
