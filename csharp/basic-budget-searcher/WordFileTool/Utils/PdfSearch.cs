using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace Utils {
	public class PdfSearch {

		private List<PdfResult> results;

		public PdfSearch(string filename, string text)
			: this(filename, new[] { text }){
			// NOTE: cannot call a constructor inside the body of another constructor
		}
		public PdfSearch(
			string filename, IEnumerable<string> texts) {
			if (String.IsNullOrWhiteSpace(filename)) // better than IsNullOrEmpty
				throw new ArgumentException("Filename cannot be blank.", "filename");
			this.filename = filename;

			if (texts == null)
				throw new ArgumentNullException("texts");

			var textsList = texts.ToList();

			if (textsList.Count == 0)
				throw new ArgumentException( "At least one search term is required.", "texts");

			if (textsList.Any(string.IsNullOrWhiteSpace))
				throw new ArgumentException( "Search terms cannot be blank.", "texts");

			this.texts = new HashSet<string>( textsList, StringComparer.Ordinal);

		}

		private string filename;
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
		private HashSet<string> texts;
		public IEnumerable<string> Texts {
			get { return texts; }
			set { texts = new HashSet<string>( value, StringComparer.Ordinal); }
		}

		public List<PdfResult> findText() {
			int index = 0;
			results = new List<PdfResult>();
			using (var document = PdfDocument.Open(filename)) {
				Debug.WriteLine(String.Format("Reading {0}", filename));
				foreach (var page in document.GetPages()) {
					index++;
					var pageText = ContentOrderTextExtractor.GetText(page, true);
					String matchedText;
					if ((matchedText = texts.FirstOrDefault(pageText.Contains)) != null) {
						Debug.WriteLine(String.Format("Found one of texts: {0} on page {1}", String.Join(",",texts), index));
						// TODO: collect match capture into result
						results.Add(new PdfResult {
							Index = index,
							MatchedText = matchedText,
							Text = pageText
						});
					}
				}
			}
			return results;
		}
	}
	public struct PdfResult {
		public int Index;
		public string Location { get { return String.Format("{0}",  Index );  }}
		public string MatchedText;
		public string Text;
	}	
}
