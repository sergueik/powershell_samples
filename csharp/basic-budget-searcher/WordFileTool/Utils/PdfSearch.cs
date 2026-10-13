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

using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

// based on: https://github.com/UglyToad/PdfPig/blob/master/examples/ExtractTextWithNewlines.cs
namespace Utils {
		public struct PdfResult {
		public int Index;
		public string Text;
		public string Location { get { return String.Format("{0}",  Index );  }}
	}

	
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
					if (texts.Any((string text) => pageText.Contains(text))) {
						// Debug.WriteLine(String.Format("Found text: {0}", text));
						results.Add(new PdfResult {
							Index = index,
							Text = pageText
						});
					}
				}
			}
			return results;
		}
	}
}
