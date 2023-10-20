using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Security;



namespace Utils {
	public class Grep {

		private bool recursive;
		private bool ignoreCase;
		private bool justFilenames;
		private bool lineNumbers;
		private bool countLines;
		private string regEx;
		private string files;
		private string dir = Environment.CurrentDirectory; 
		private ArrayList targets = new ArrayList();

		//Properties
		public bool Recursive {
			get { return recursive; }
			set { recursive = value; }
		}

		public bool IgnoreCase {
			get { return ignoreCase; }
			set { ignoreCase = value; }
		}

		public bool JustFilenames {
			get { return justFilenames; }
			set { justFilenames = value; }
		}

		public bool LineNumbers {
			get { return lineNumbers; }
			set { lineNumbers = value; }
		}

		public bool CountLines {
			get { return countLines; }
			set { countLines = value; }
		}

		public string RegEx {
			get { return regEx; }
			set { regEx = value; }
		}

		public string Files {
			get { return files; }
			set { files = value; }
		}

		public string Dir {
			get { return dir; }
			set { dir = value; }
		}

		private void GetFiles(String dir, String ext, bool recursive) {
			string[] f = Directory.GetFiles(dir, ext);
			for (int i = 0; i < f.Length; i++) {
				if (File.Exists(f[i]))
					targets.Add(f[i]);
			}
			if (recursive) {
				string[] d = Directory.GetDirectories(dir);
				for (int i = 0; i < d.Length; i++) {
					GetFiles(d[i], ext, true);
				}
			}
		}

		public void Search() {
			if (!Directory.Exists(dir)){
				errorMsg(String.Format("Directory {0} doesn't exist!", dir));
				return;
			}
			targets.Clear();
			
			String[] f = files.Split(new Char[] { ',' });
			for (int i = 0; i < f.Length; i++) {
				f[i] = f[i].Trim();
				GetFiles(dir, f[i], recursive);
			}
			
			String results = "Grep results:\r\n\r\n";
			String line;
			int n, c;
			bool empty = true;
			IEnumerator t = targets.GetEnumerator();
			var regexOptions = (ignoreCase) ? RegexOptions.IgnoreCase |RegexOptions.Compiled : RegexOptions.Compiled;
            var regex = new Regex(regEx, regexOptions);
			while (t.MoveNext()) {
				try {
					StreamReader s = File.OpenText((string)t.Current);
					n = 0;
					c = 0;
					bool first = true;
					while ((line = s.ReadLine()) != null) {
						n++;
						// Match m = (ignoreCase) ? Regex.Match(line, regEx, RegexOptions.IgnoreCase) : Regex.Match(line, regEx);
						var m = regex.Match( line );					
						if (m.Success) {
							empty = false;
							c++;
							if (first) {
								if (justFilenames) {
									results += (string)t.Current + "\r\n";
									break;
								} else
									results += (string)t.Current + ":\r\n";
								first = false;
							}
							results += ((lineNumbers) ? "  " + n + ": " : "  " ) + line + "\r\n";
						}
					}
					s.Close();
					if (!first) {
						if (countLines)
							results += "  " + c + " Lines Matched\r\n";
						results += "\r\n";
					}
				} catch (SecurityException) {
					results += "\r\n" + (string)t.Current + ": Security Exception\r\n\r\n";	
				} catch (FileNotFoundException) {
					results += "\r\n" + (string)t.Current + ": File Not Found Exception\r\n";
				}
			}
			
			statusMsg((empty) ? "No matches found!" : results);
			
		}

		protected virtual void statusMsg(String message) {
		}
		protected virtual void errorMsg(String message) {
		}
		protected virtual void progressMsg(String message) {
		}
	}
}
