using System;
using System.Text;
using System.Globalization;

namespace MiniHttpd {
	public class UrlEncoding {
		private UrlEncoding() { }

		static readonly string[] urlEncStrings = InitUrlStrings();

		static string[] InitUrlStrings() {
			string[] urlEncStrings = new string[256];
			for (int i = 0; i < 255; i++)
				urlEncStrings[i] = "%" + i.ToString("X2");
			return urlEncStrings;
		}

		public static string Encode(string value) {
			if (value == null)
				return null;

			StringBuilder ret = new StringBuilder(value.Length);

			for (int i = 0; i < value.Length; i++) {
				char ch = value[i];
				if (ch == ' ')
					ret.Append('+');
				else if (!IsSafe(ch))
					ret.Append(urlEncStrings[ch]);
				else
					ret.Append(ch);
			}

			return ret.ToString();
		}

		public static string Decode(string value) {
			if (value == null)
				return null;

			return decoder.Decode(value);
		}

		static bool IsSafe(char ch) {
			if (char.IsLetterOrDigit(ch))
				return true;

			switch (ch) {
				case '\'':
				case '(':
				case ')':
				case'[':
				case']':
				case '*':
				case '-':
				case '.':
				case '!':
				case '_':
					return true;
			}

			if (ch > 255)
				return true;

			return false;
		}

		static Decoder decoder = new Decoder();

		class Decoder : Uri {
			public Decoder() : base("http://localhost") { }

			public string Decode(string str) {
				return Unescape(str.Replace("+", "%20"));
			}
		}
	}
}
