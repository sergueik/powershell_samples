using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Utils;

namespace Tests {

	[TestFixture]
	public class WordTest {
			private const string filename = "sample-simple.docx";
			private string [] texts = {"Text Formatting", "Runs can be bold, italic, underlined or coloured", "Marker"};

		[Test]
		public void test() {
			var	wordSearch = new WordSearch(filename, texts);
			var results = wordSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((WordResult result)  => result.Location).ToList()));
			Predicate<WordResult> match  = ( WordResult result) => texts.ToList().Exists((string text) => result.Text.Contains(text));
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			Assert.IsEmpty(results.GroupBy(result => result.Location).Where(group => group.Count() > 1), "Result locations should be unique");
		}

	}
}
