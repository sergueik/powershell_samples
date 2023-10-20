using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NanoTube.Collections;

namespace NanoTube {

	public static class KeyHelper {
		private const string _badChars = "! ;:/()\\#%$^*";
		private readonly static SimpleObjectPool<StringBuilder> _builderPool 
			= new SimpleObjectPool<StringBuilder>(1, pool => new StringBuilder(200));
		private readonly static Regex _validKey = new Regex(@"^[^!\s;:/\.\(\)\\#%\$\^\*]+$", RegexOptions.Compiled);

		public static bool IsValidKey(this string key)
		{
			return string.IsNullOrEmpty(key) || _validKey.IsMatch(key);
		}

		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "IsValidKey is validating key")]
		public static string Sanitize(this string key)
		{
			if (IsValidKey(key)) {
				return key;
			}

			StringBuilder sanitized = null;

			try {
				sanitized = _builderPool.Pop();
				if (null != sanitized) {
					sanitized.Clear();
					sanitized.Append(key);
				}
				//autogrow pool as necessary
				else {
					sanitized = new StringBuilder(key);
				}

				for (int i = 0; i < key.Length; ++i) {
					if (_badChars.Contains(key[i])) {
						sanitized[i] = '_';
					}
				}

				return sanitized.ToString();
			} finally {
				if (null != sanitized) {
					_builderPool.Push(sanitized);
				}
			}
		}
	}
}