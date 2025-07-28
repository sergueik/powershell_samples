using System;
using System.Text;
using System.Runtime.InteropServices;
using Markdig;
using CommonMark;
using System.IO;

namespace Utils
{

	public class Helper
	{
		[DllImport("wininet.dll", SetLastError = true)]
		public static extern bool InternetGetCookieEx(
			string url,
			string cookieName,
			StringBuilder cookieData,
			ref int size,
			Int32 dwFlags,
			IntPtr lpReserved);

		private const int INTERNET_COOKIE_HTTPONLY = 0x00002000;
		private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

		public static string GetGlobalCookies(string uri)
		{
			int datasize = 1024;
			var cookieData = new StringBuilder((int)datasize);
			if (InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
			    && cookieData.Length > 0) {
				return cookieData.ToString().Replace(';', ',');
			} else {
				return null;
			}
		}

		public string convert() {
		
			string markdown = File.ReadAllText("README.md");
			// string html = Markdown.ToHtml(markdown);
			var result = CommonMark.CommonMarkConverter.Convert(markdown);
			return result;
		}
	}

}