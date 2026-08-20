#Copyright (c) 2026 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

# surprisingly, the PowerShell version becomes lighter, not heavier
# usage $0 owner repo directory_path branch destination
#       $0 url


param (
	[string] $old = '^(?<id>[0-9]+)_(?<artist>[^-]+) - (?<title>.+)$',
	[string] $new = '<id> - <title> - <artist>',
	[string] $extension = 'wav',
  [switch]$flag
  # currently unused
)

write-host -nonewline 'implicit arguments: '
write-host -nonewline ('"{0}" = "{1}" ' -f 'old', $old)
write-host -nonewline ('"{0}" = "{1}" ' -f 'new', $new)
write-host -nonewline ('"{0}" = "{1}" ' -f 'extension', $extension)
if ($args.Count -gt 1 ) {
  write-host ('{0} positional arguments: {1}' -f $args.Count, ($args -join ','))
} else {
  write-host 'no positional artguments'
}
<#
if ($args.Count -gt 1 ) {
  $old = $args[0]
  $new= $args[1]
  if ($args.Count -gt 2 ) {
    $extension = $args[2]
  }
  $output_dir = if ($args.Count -gt 3) { $args[2] } else { '.' }
} else {
  # warn	
  write-error ('invalid arguments: need at least 2' )
  exit 1
}
#>
$caller_class = 'Renamer'
Add-Type -TypeDefinition @"

using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
	public class ${caller_class} {
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
				var extension = Path.GetExtension(file);
				var fileDirectoryName = Path.GetDirectoryName(file);
				var newName = GetNewName(fileNameWithoutExtension, "^(?<id>[0-9]+)_(?<artist>[^-]+) - (?<title>.+)$", "<id> - <title> - <artist>");
				if (String.IsNullOrEmpty(newName)) {
					Debug.WriteLine(String.Format("Not renaming {0}", file));
					continue;
				}
				var newFilePath = Path.Combine(directoryName, newName + extension);

				Debug.WriteLine(String.Format("filename: \"{0}\" extension: \"{1}\" directory: \"{2}\" new name: \"{3}\" new file path: \"{4}\"", fileNameWithoutExtension, extension, fileDirectoryName, newName, newFilePath));

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
"@ -ReferencedAssemblies 'System.Windows.Forms.dll'

$o = new-object Utils.Renamer

$o.Extension = $extension
$o.DirectoryName = $directory
$o.NewNamePattern = $new
$o.OldNamePattern = $old
$o.Rename()
