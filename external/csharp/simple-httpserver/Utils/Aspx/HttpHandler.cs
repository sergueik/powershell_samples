using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniHttpd.Aspx
{
	[Serializable]
	public class HttpHandler : MarshalByRefObject
	{
		internal HttpHandler(string verb, string path, string type, bool validate)
		{
			this.verb = verb;
			this.path = path;
			this.type = type;
			this.validate = validate;

			StringBuilder pattern = new StringBuilder();

			string[] verbs = verb.Split(',');
			foreach (string s in verbs) {
				pattern.Append("(");
				pattern.Append(WildcardToRegex(s.Trim()));
				pattern.Append(")|");
			}

			if (pattern.Length != 0)
				pattern.Remove(pattern.Length - 1, 1);

			verbsRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant);

			pattern = new StringBuilder();

			string[] paths = path.Split(';');
			foreach (string s in paths) {
				pattern.Append("(");
				pattern.Append(WildcardToRegex(s.Trim()));
				pattern.Append(")|");
			}

			if (pattern.Length != 0)
				pattern.Remove(pattern.Length - 1, 1);

			pathsRegex = new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.CultureInvariant);
		}

		string verb;
		string path;
		string type;
		bool validate;

		Regex verbsRegex;
		Regex pathsRegex;

		public string Verb {
			get {
				return verb;
			}
		}
		public string Path {
			get {
				return path;
			}
		}

		public string Type {
			get {
				return type;
			}
		}

		public bool Validate {
			get {
				return validate;
			}
		}

		static string WildcardToRegex(string pattern)
		{
			return Regex.Escape(pattern).
				Replace("\\*", ".*").
				Replace("\\?", ".");
		}

		public bool IsMatch(string method, string path)
		{
			return verbsRegex.IsMatch(method) && pathsRegex.IsMatch(path);
		}

		public override bool Equals(object obj)
		{
			HttpHandler other = obj as HttpHandler;
			if (other == null)
				return false;

			if (this.verb == other.verb && this.path == other.path)
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{
			unchecked {
				return unchecked(verb.GetHashCode() * path.GetHashCode());
			}
		}

	}
}
