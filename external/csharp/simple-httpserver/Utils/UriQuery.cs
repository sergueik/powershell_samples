using System;
using System.Collections.Specialized;

namespace MiniHttpd {
	public class UriQuery : NameValueCollection {
		public UriQuery(Uri uri) : this(uri.Query.TrimStart('?'), true) { }

		public UriQuery(string query) : this(query, true) { }

		public UriQuery(string query, bool urlEncoded) {
			for (int i = 0; i < query.Length; i++) {
				int start = i;
				int equalIndex = -1;
				while (i < query.Length) {
					if (query[i] == '=') {
						if (equalIndex < 0)
							equalIndex = i;
					} else if (query[i] == '&')
						break;
					i++;
				}

				string name;
				string value;

				if (equalIndex < 0) {
					name = query.Substring(start, i - start);
					value = string.Empty;
				} else {
					name = query.Substring(start, equalIndex - start);
					value = query.Substring(equalIndex + 1, (i - equalIndex) - 1);
				}

				if (urlEncoded)
					this.Add(UrlEncoding.Decode(name), UrlEncoding.Decode(value));
				else
					this.Add(name, value);

				if (i == query.Length - 1 && query[i] == '&')
					this.Add(null, string.Empty);
			}
		}
	}
}
