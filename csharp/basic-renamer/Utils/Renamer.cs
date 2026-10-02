using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

// public class ${caller_class} {
namespace Utils {
	public class Renamer {
		private string oldNamePattern;
		public string OldNamePattern {
			get { return oldNamePattern; }
			set { oldNamePattern = value; }
		}
		private string newNamePattern;
		public string NewNamePattern {
			get { return newNamePattern; }
			set { newNamePattern = value; }
		}

		private string directoryName;
		public string DirectoryName {
			get { return directoryName; }
			set { directoryName = value; }
		}

		private string extension = "flac";
		public string Extension {
			get { return extension; }
			set { extension = value; }
		}
		private static readonly Regex resultRegex = new Regex("<([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public static string GetNewName(string oldName, string oldNamePattern, string newNamePattern) {
			var dictionary = FindMatches(oldName, oldNamePattern);
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
		public static Dictionary<string, string> FindMatches(string text, string matchPattern) {
			var dictionary = new Dictionary<string, string>();

			var regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var matches = regex.Matches(text);

			foreach (Match match in matches) {
				if (match.Length != 0) {
					foreach (string name in regex.GetGroupNames()) {
						if (name != "0") {
							dictionary[name] = match.Groups[name].Value;
						}
					}
					break;
				}
			}

			return dictionary;
		}
		public void Rename() {
			Debug.WriteLine(String.Format("GetFiles: {0} {1}", this.directoryName, String.Format("*.{0}", this.extension)));
			string[] files = { };
			try {
				files = Directory.GetFiles(this.directoryName, String.Format("*.{0}", this.extension));
			} catch (ArgumentException e) {
				Debug.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
			foreach (var file in files) {
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var fileExtension = Path.GetExtension(file);
				var fileDirectoryName = Path.GetDirectoryName(file);
				var newFileName = GetNewName(fileNameWithoutExtension, this.oldNamePattern , this.newNamePattern);

				if (String.IsNullOrEmpty(newFileName)) {
					Debug.WriteLine(String.Format("Not renaming {0}", file));
					continue;
				}
				var newFilePath = Path.Combine(directoryName, newFileName + fileExtension);

				Debug.WriteLine(String.Format("filename: \"{0}\" extension: \"{1}\" directory: \"{2}\" new name: \"{3}\" new file path: \"{4}\"", fileNameWithoutExtension, fileExtension, fileDirectoryName, newFileName, newFilePath));

				if (!File.Exists(file)) {
					Debug.WriteLine(String.Format("Source file missing"));
					continue;
				}

				if (File.Exists(newFilePath)) {
					Debug.WriteLine(String.Format("Target file \"{0}\" already exists", newFilePath));
					continue;
				}
				File.Move(file, newFilePath);
			}
		}
	}
}
