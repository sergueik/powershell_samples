using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Utils;

namespace Tests {

	[TestFixture]
	public class PdfTest {
			private const string filename = "file-sample_150kB.pdf";
			private string [] texts = {"Maecenas tincidunt", "Fusce vitae vestibulum"};

		[Test]
		public void test() {
			var	pdfSearch = new PdfSearch(filename, texts);
			var results = pdfSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((PdfResult result)  => result.Location).ToList()));

			Predicate<PdfResult> match  = ( PdfResult result) => texts.ToList().Exists((string text) => result.Text.Contains(text));
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			Assert.IsEmpty(results.GroupBy(result => result.Location).Where(group => group.Count() > 1), "Result locations should be unique");
		}

	}
}
