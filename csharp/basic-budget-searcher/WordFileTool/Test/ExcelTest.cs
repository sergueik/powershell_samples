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
		public void test1()
		{
			var context = new Dictionary<String, POIXMLDocumentPart>();
			var stringBuilder = new StringBuilder(); 
			// NOTE: on C#, StringBuilder is a reference type
			// When passed a StringBuilder arg into a method, a reference to that object is 
			// passed by value meaning callee can modify its internal contents inside the method without needing 
			// the ref keyword - explicit is dicouraged
			var doc = File.OpenRead("sample-simple-2.xls");
			IWorkbook workbook = WorkbookFactory.Create(doc, false);
			//
// Using the common IWorkbook interface or specific workbook class (HSSFWorkbook / XSSFWorkbook)
			Assert.Greater(workbook.NumberOfSheets, 0, "expect at least one sheet");
// for (var sheet in workbook.getSheets() ) 
			// Error CS1002: ; expected ?
//	var name = sheet.SheetName;


//for (int num in workbook.getSheetNumbers() )
//	var sheet = workbook.GetSheetAt(num);
			// Error CS1002: ; expected ?
//				var name = sheet.SheetName;
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
						} catch (Exception e) {
							// not cerain if one is thrown
						
							text = cell.ToString();
						}
						// 	Debug.WriteLine(String.Format("read text: {0}", text));
						Console.Error.WriteLine(String.Format("read sheet: {0} row: {1} col: {2} text: {3}", sheet.SheetName, rowNum, cellNum, text));
					}
				}
			}
		}
	}

	public static class ExcelHelper
	{
		// Custom iterator methods
		
		public static IEnumerable<int> getSheetNumbers(this IWorkbook workbook)
		{
			int numberOfSheets = workbook.NumberOfSheets;
			for (int sheetNumber = 0; sheetNumber <= numberOfSheets; sheetNumber++) {
				yield return sheetNumber;
			}
		}

		
		public static IEnumerable<ISheet> getSheets(this IWorkbook workbook)
		{
			int numberOfSheets = workbook.NumberOfSheets;
			for (int sheetNumber = 0; sheetNumber <= numberOfSheets; sheetNumber++) {
				yield return workbook.GetSheetAt(sheetNumber);
			}
		}
	}
}
