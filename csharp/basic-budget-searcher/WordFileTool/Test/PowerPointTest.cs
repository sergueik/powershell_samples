using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Utils;
// based on: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests {

	[TestFixture]
	public class PowerPointTest {
			private const string filename = "sample-presentation.pptx";
			private string region = null;
			private string searchText = "Sample Presentation";

		[Test]
		public void test1() {
			searchText = "Sample Presentation";
			var	powerPointSearch = new PowerPointSearch(filename, searchText);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain( "Product A", results.First().Text,"legend is not in slides");
		}

		[Test]
		public void test2() {
			searchText = "Product A";
			region = "chart";
			var	powerPointSearch = new PowerPointSearch(filename, region, searchText);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			StringAssert.DoesNotContain( "Sample Presentation", results.First().Text,"slide title is not in chart legend");
		}

		[Test]
		public void test3() {
			var texts = new List<string> {
				"Sample Presentation",
				"Agenda",
				"Roadmap",
				"Project Goals",
				"Reduce churn",
				"Quarterly Revenue",
				"Sales by Quarter",
				"Embedded Image",
				"Trade-offs"
			};
			// NOTE: reset region - or else 
			// debug success when run alone, failure when run all tests
			region = "slide";
			var	powerPointSearch = new PowerPointSearch(filename, region, texts);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match = ( PowerPointResult result) => texts.Exists((string text) => result.Text.Contains(text));
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain at least one search term");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain("Product A", results.First().Text, "legend is not in slides");
			Assert.IsFalse(
				results.Any(result => result.Text.Contains("Product A")),
				"Chart legend text should not appear in slide results");
		}
	}
}
