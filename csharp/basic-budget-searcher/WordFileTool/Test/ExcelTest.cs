using System;
using System.Collections.Generic;using System.Linq;

using NUnit.Framework;
using Utils;

namespace Tests {

	[TestFixture]
	public class ExcelTest {
		private readonly static string filename  = "sample-simple.xlsx";

		[Test]
		public void test() {
			const string searchText =  "Lorem ipsum";
			var	excelSearch = new ExcelSearch(filename, searchText);
			var results = excelSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Console.Error.WriteLine("Found in {0}", String.Join(",", results.Select((ExcelResult result)  => result.Location).ToList()));

			Predicate<ExcelResult> match  = ( ExcelResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			// old style:
			foreach (ExcelResult result in results)
	            StringAssert.Contains(searchText, result.Text, "Every result should contain the search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			// NOTE: Prefer the set-operation version that expresses the invariant
			// avoid the nested loop version tied to the mechanics of checking it
			/*
				for (int i = 0; i < results.Count; i++){
				    for (int j = i + 1; j < results.Count; j++) {
				        Assert.IsFalse(
				            results[i].Location == results[j].Location,
				            "Duplicate result location: " + results[i].Location);
				    }
				}
			 */
		}
	}
}
