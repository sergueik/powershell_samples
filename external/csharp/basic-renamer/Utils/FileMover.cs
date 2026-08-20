using System;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Utils {
	public class FileMover {
		private static readonly Regex resultRegex = new Regex("<([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static string GetNewName(string oldName, string oldNamePattern, string newNamePattern) {
			var dictionary = oldName.FindMatches(oldNamePattern);
			if (dictionary == null || dictionary.Count == 0) {
				Debug.WriteLine("no match \"{0}\" for \"{1}\"", oldName, oldNamePattern);
				return null;
			}
			
			var result = resultRegex.Replace(newNamePattern, (Match match) => {
				string key = match.Groups[1].Value;
				string value;
				return dictionary.TryGetValue(key, out value) ? value : match.Value;
			});
			return result;
		}
	}
}
