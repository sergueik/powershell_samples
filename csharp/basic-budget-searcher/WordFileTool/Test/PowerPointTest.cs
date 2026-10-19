using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Utils;
// based on: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests {

	[TestFixture]
	public class PowerPointTest {
		private string filename = null;
			private string region = null;
			private string text = null;

		[Test]
		public void test1() {
			filename = "sample-presentation.pptx";
			text = "Lorem ipsum";
			var	powerPointSearch = new PowerPointSearch(filename, text);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((PowerPointResult result)  => result.Location).ToList()));

			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(text);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain( "Product A", results.First().Text,"legend is not in slides");
		}

		[Test]
		public void test2() {
			filename = "sample-presentation.pptx";
			text = "Product A";
			region = "chart";
			var	powerPointSearch = new PowerPointSearch(filename, region, text);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");

			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((PowerPointResult result)  => result.Location).ToList()));
			Predicate<PowerPointResult> match = ( PowerPointResult result) => result.Text.Contains(text);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			StringAssert.DoesNotContain( "Sales by Quarter", results.First().Text,"chart title is not in chart legend");
		}

		[Test]
		public void test3() {
			// NOTE: const is not final
			// There is no final keyword in C#
			// A const field of a reference type other than string can only be initialized with null. (CS0134)
			// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
			var texts = new List<string> {
				"Lorem ipsum",
				"vitae libero",
				"Embedded Image"
			};
			// NOTE: reset region - or else
			// debug success when run alone, failure when run all tests
			filename = "sample-presentation.pptx";
			region = "slide";
			var	powerPointSearch = new PowerPointSearch(filename, region, texts);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((PowerPointResult result)  => result.Location).ToList()));

			Predicate<PowerPointResult> match = ( PowerPointResult result) => texts.Exists((string text) => result.Text.Contains(text));
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain at least one search term");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain("Product A", results.First().Text, "legend is not in slides");
			Assert.IsFalse(
				results.Any(result => result.Text.Contains("Product A")),
				"Chart legend text should not appear in slide results");
		}

		[Test]
		public void test4() {
			filename = "sample-presentation.pptx";
			text = "fermentum nisl vitae";
			region = "note";
			var	powerPointSearch = new PowerPointSearch(filename, region , text);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((PowerPointResult result)  => result.Location).ToList()));

			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(text);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain( "Slide Title", results.First().Text,"Slide text is not in slide notes");
		}

	}
}
