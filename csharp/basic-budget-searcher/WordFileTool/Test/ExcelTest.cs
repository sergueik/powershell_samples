using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;

using NPOI;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

using NUnit.Framework;

namespace Tests {

	[TestFixture]
	public class ExcelTest {
		private readonly static string filename  = "sample-simple-2.xls";

		[Test]
		public void test() {
			var searchText =  "test1";
			var	excelSearch = new ExcelSearch(filename, searchText);
			var results = excelSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<ExcelResult> match  = ( ExcelResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			// old style:
			foreach (ExcelResult result in results)
	            StringAssert.Contains(searchText, result.Text, "Every result should contain the search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			// NOTE: Prefer the set-operation version that expresses the invariant, avoid the nested loop
			// version tied to the mechanics of checking it
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

	public class ExcelSearch {
		public ExcelSearch(string filename, string text ){
			if (String.IsNullOrWhiteSpace(filename)) // better than IsNullOrEmpty
        		throw new ArgumentException( "Filename cannot be blank.", "filename");
			this.filename = filename;
			if (String.IsNullOrWhiteSpace(text))
				throw new ArgumentException("text cannot be blank", "text");
			this.text = text;
		}

		private string filename;
		private string text;
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
		public string Text {
			get { return text; }
			set { text = value; }
		}

		public List<ExcelResult> findText() {
			var dataFormatter = new DataFormatter();
			var result = new List<ExcelResult>();
			using (var doc = File.OpenRead(filename)) {
				IWorkbook workbook = WorkbookFactory.Create(doc, false);
				foreach (var sheet in workbook.getSheets()) {
					// System.ArgumentOutOfRangeException : Index was out of range. Must be non-negative and less than the size of the collection.
					// Parameter name: index
					var name = sheet.SheetName;
					for (int rowNum = sheet.FirstRowNum; rowNum <= sheet.LastRowNum; rowNum++) {
						IRow row = sheet.GetRow(rowNum);
						if (Object.Equals(null, row))
							continue;
						for (int cellNum = row.FirstCellNum; cellNum < row.LastCellNum; cellNum++) {
							if (cellNum < 0)
								continue;
							ICell cell = row.GetCell(cellNum);
							if (Object.Equals(null, cell))
								continue;

							CellType cellType = cell.CellType;

							if (CellType.Blank == cellType)
								continue;
							if (CellType.Formula == cellType)
								continue;
							string cellText = null;
							try {
								// https://github.com/nissl-lab/npoi/blob/master/main/SS/UserModel/DataFormatter.cs#L1171
								cellText = dataFormatter.FormatCellValue(cell, null);
								// not certain if any exception is thrown
							} catch (Exception e) {
								cellText = cell.ToString();
							}
							Console.Error.WriteLine(String.Format("read sheet: {0} cell: {1} row: {2} col: {3} text: {4}", sheet.SheetName, cell.Address.FormatAsString(), cell.Address.Row + 1, cell.Address.Column + 1, cellText));
							if (cellText.Contains(text)) {
								result.Add(new ExcelResult {
									SheetName = sheet.SheetName,
									CellAddress = cell.Address.FormatAsString(),
									Text = cellText
								});
							}
						}
					}
				}
			}
			return result;
		}
	}

	public struct ExcelResult {
		public string SheetName;
		public string CellAddress;
		public string Text;
		public string Location { get { return String.Format("{0}!{1}",  SheetName , CellAddress);  }}
	}

	public static class ExcelHelper {
		// Custom iterator methods
		public static IEnumerable<ISheet> getSheets(this IWorkbook workbook) {
			int numberOfSheets = workbook.NumberOfSheets;
			for (int sheetNumber = 0; sheetNumber != numberOfSheets; sheetNumber++) {
				yield return workbook.GetSheetAt(sheetNumber);
			}
		}
	}
}
