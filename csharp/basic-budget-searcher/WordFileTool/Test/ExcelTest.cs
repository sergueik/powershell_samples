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

// based on: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests
{

	[TestFixture]
	public class ExcelTest
	{

		private void getText(Stream stream, string region, StringBuilder stringBuilder)
		{
		}

		[Test]
		public void test1() {
			var doc = File.OpenRead("sample-simple-2.xls");
			IWorkbook workbook = WorkbookFactory.Create(doc, false);
			Assert.Greater(workbook.NumberOfSheets, 0, "expect at least one sheet");
			// hey, Embedded statement cannot be a declaration or labeled statement (CS1023) -
			foreach (var sheet in workbook.getSheets() ) {
				// System.ArgumentOutOfRangeException : Index was out of range. Must be non-negative and less than the size of the collection.
				// Parameter name: index 
				var dataFormatter = new DataFormatter();
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
						string text = null;
						try {
							// https://github.com/nissl-lab/npoi/blob/master/main/SS/UserModel/DataFormatter.cs#L1171
							text = dataFormatter.FormatCellValue(cell, null);
						// not certain if any exception is thrown						
						} catch (Exception e) {
							text = cell.ToString();
						}
						Console.Error.WriteLine(String.Format("read sheet: {0} cell: {1} row: {2} col: {3} text: {4}", sheet.SheetName, cell.Address.FormatAsString(), cell.Address.Row + 1, cell.Address.Column + 1, text));
					}
				}
		}
	}
		[Test]
		public void test2() {
			var doc = File.OpenRead("sample-simple-2.xls");
			IWorkbook workbook = WorkbookFactory.Create(doc, false);
			var dataFormatter = new DataFormatter();
			for (int num = 0; num != workbook.NumberOfSheets; num++) {
				var sheet = workbook.GetSheetAt(num);
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
						string text = null;
						try {
							// https://github.com/nissl-lab/npoi/blob/master/main/SS/UserModel/DataFormatter.cs#L1171
							text = dataFormatter.FormatCellValue(cell, null);
						// not certain if any exception is thrown						
						} catch (Exception e) {						
							text = cell.ToString();
						}
						Console.Error.WriteLine(String.Format("read sheet: {0} cell: {1} row: {2} col: {3} text: {4}", sheet.SheetName, cell.Address.FormatAsString(), cell.Address.Row + 1, cell.Address.Column + 1, text));
					}
				}
			}
		}
	}

	public static class ExcelHelper {
		// Custom iterator methods
		public static IEnumerable<ISheet> getSheets(this IWorkbook workbook)
		{
			int numberOfSheets = workbook.NumberOfSheets;
			for (int sheetNumber = 0; sheetNumber != numberOfSheets; sheetNumber++) {
				yield return workbook.GetSheetAt(sheetNumber);
			}
		}
	}
}
