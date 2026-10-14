using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Utils;
// based on: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests {

	[TestFixture]
	public class PowerPointTest2 {
			// TODO: merge
			private const string filename = "sample_powerpoint_slides.pptx";
			private string region = "note";
			private string text = "Cover slide";

		[Test]
		public void test4() {
			var	powerPointSearch = new PowerPointSearch(filename, region , text);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(text);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain( "Slide Title", results.First().Text,"Slide text is not in slide notes");
		}

	}
}
