using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using NPOI.XWPF.UserModel;
using NPOI;
using NPOI.Util;

namespace Utils {
	public struct WordResult {
		public int Index;
		public string Text;
		public string Location { get { return String.Format("{0}", Index); } }
	}
	public class WordSearch {

		private List<WordResult> results;

		public WordSearch(string filename, string text) : this(filename, new[] { text }) {
			// NOTE: cannot call a constructor inside the body of another constructor
		}
		public WordSearch( string filename, IEnumerable<string> texts) {
			if (String.IsNullOrWhiteSpace(filename)) // better than IsNullOrEmpty
				throw new ArgumentException("Filename cannot be blank.", "filename");
			this.filename = filename;

			if (texts == null)
				throw new ArgumentNullException("texts");

			var textsList = texts.ToList();

			if (textsList.Count == 0)
				throw new ArgumentException("At least one search term is required.", "texts");

			if (textsList.Any(string.IsNullOrWhiteSpace))
				throw new ArgumentException("Search terms cannot be blank.", "texts");

			this.texts = new HashSet<string>(textsList, StringComparer.Ordinal);

		}

		private string filename;
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
		private HashSet<string> texts;
		public IEnumerable<string> Texts {
			get { return texts; }
			set { texts = new HashSet<string>(value, StringComparer.Ordinal); }
		}

		public List<WordResult> findText() {
			int index = 0;
			results = new List<WordResult>();
			using (var stream = File.OpenRead(filename)) {
				var document = new XWPFDocument(stream);
				Debug.WriteLine(String.Format("Reading {0}", filename));
				foreach (var paragraph in document.Paragraphs) {
					index++;
					// https://github.com/nissl-lab/npoi/blob/master/ooxml/XWPF/Usermodel/XWPFParagraph.cs#L529
					// https://github.com/nissl-lab/npoi/blob/master/ooxml/XWPF/Usermodel/XWPFParagraph.cs#L257
					// TODO: PictureText,FootnoteText
					// Alternatively try SearchText()
					// https://github.com/nissl-lab/npoi/blob/master/ooxml/XWPF/Usermodel/XWPFParagraph.cs#L1420C28-L1420C38
					var paragraphText = paragraph.ParagraphText;
					// NOTE: accepted "lambda expression can be simplified to a method group"
					if (texts.Any(paragraphText.Contains)) {
						Debug.WriteLine(String.Format("Found one of texts: {0} on paragraph {1}", String.Join(",", texts), index));
						// TODO: collect match capture into result						
						results.Add(new WordResult {
							Index = index,
							Text = paragraphText
						});
					}
				}
			}
			return results;
		}
	}
}
