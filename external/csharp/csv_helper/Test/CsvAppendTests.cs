using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using CsvHelper;
using NUnit.Framework;
using System.Linq;

namespace TestProject1 {
	[TestFixture]
	public class CsvAppendTests {

		private const string TEST_DATA_1 = @"column one,column two,column three
1,data 2,2010-05-01 11:26:01
";

		private const string TEST_DATA_2 = @"""column, one"",column two,""column, three""
data 1,""data, 2"",data 3
";

		private const string TEST_DATA_3 = @"""column, one"",""column """"two"",""column, three""
""data """"1"",""data, 2"",data 3
";

		private const string TEST_DATA_4 = @"""column, one"",""column """"two"",""column, three""
""data """",1"",""data, 2"",data 3
";

		private const string TEST_DATA_5 = @"""column, one"",""column """"two"",""column, three""
""data """""""",1"",""dat""""""""""""sa, 2"",data 3
";

		private const string TEST_DATA_6 = @" column one ,  column two  ,   column three   
   1   ,  data 2  , 2010-05-01 11:26:01 
";
		private TestContext testContextInstance;

		public TestContext TestContext {
			get {
				return testContextInstance;
			}
			set {
				testContextInstance = value;
			}
		}

		private static string FilePath {
			get {
				string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

				if (!filePath.EndsWith("\\"))
					filePath += "\\";

				return filePath + "abc123xyz.csv";
			}
		}

		public string Location {
			get {
				string location = ConfigurationManager.AppSettings["TestFilesPath"];

				if (!location.EndsWith("\\"))
					location += "\\";

				return location;
			}
		}

		[TestFixtureTearDown]
		public static void Cleanup() {
			if (File.Exists(FilePath))
				File.Delete(FilePath);
		}

		[Test]
		public void TestAppendToFile() {
			File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);
			var file = new CsvFile();
			file.Populate(true, TEST_DATA_1);
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(file, FilePath, Encoding.Default);
			}
			using (var reader = new CsvReader(FilePath, Encoding.Default)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData1(csvFile.Headers, csvFile.Records);
			}

			File.Delete(FilePath);
		}

		private CsvFile CreateCsvFileFromDataTable(DataTable table) {
			var file = new CsvFile();

			foreach (DataColumn column in table.Columns)
				file.Headers.Add(column.ColumnName);

			foreach (DataRow row in table.Rows) {
				CsvRecord record = new CsvRecord();

				foreach (object o in row.ItemArray) {
					if (o is DateTime)
						record.Fields.Add(((DateTime)o).ToString("yyyy-MM-dd hh:mm:ss"));
					else
						record.Fields.Add(o.ToString());
				}

				file.Records.Add(record);
			}

			return file;
		}

		private CsvFile CreateCsvFile(List<string> headers, List<string> fields) {
			var csvFile = new CsvFile();

			headers.ForEach(header => csvFile.Headers.Add(header));
			CsvRecord record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvFile.Records.Add(record);
			return csvFile;
		}


		private void VerifyTestData1(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column one", headers[0]);
			Assert.AreEqual("column two", headers[1]);
			Assert.AreEqual("column three", headers[2]);
			Assert.AreEqual("1", records[0].Fields[0]);
			Assert.AreEqual("data 2", records[0].Fields[1]);
			Assert.AreEqual("2010-05-01 11:26:01", records[0].Fields[2]);
		}

		private void VerifyTestData2(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column, one", headers[0]);
			Assert.AreEqual("column two", headers[1]);
			Assert.AreEqual("column, three", headers[2]);
			Assert.AreEqual("data 1", records[0].Fields[0]);
			Assert.AreEqual("data, 2", records[0].Fields[1]);
			Assert.AreEqual("data 3", records[0].Fields[2]);
		}

		private void VerifyTestData3(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column, one", headers[0]);
			Assert.AreEqual("column \"two", headers[1]);
			Assert.AreEqual("column, three", headers[2]);
			Assert.AreEqual("data \"1", records[0].Fields[0]);
			Assert.AreEqual("data, 2", records[0].Fields[1]);
			Assert.AreEqual("data 3", records[0].Fields[2]);
		}

		private void VerifyTestData4(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column, one", headers[0]);
			Assert.AreEqual("column \"two", headers[1]);
			Assert.AreEqual("column, three", headers[2]);
			Assert.AreEqual("data \",1", records[0].Fields[0]);
			Assert.AreEqual("data, 2", records[0].Fields[1]);
			Assert.AreEqual("data 3", records[0].Fields[2]);
		}

		private void VerifyTestData5(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column, one", headers[0]);
			Assert.AreEqual("column \"two", headers[1]);
			Assert.AreEqual("column, three", headers[2]);
			Assert.AreEqual("data \"\",1", records[0].Fields[0]);
			Assert.AreEqual("dat\"\"\"sa, 2", records[0].Fields[1]);
			Assert.AreEqual("data 3", records[0].Fields[2]);
		}

		private void VerifyTestData5Alternative(CsvRecords records) {
			Assert.IsTrue(records.Count == 2);
			Assert.AreEqual("column, one", records[0].Fields[0]);
			Assert.AreEqual("column \"two", records[0].Fields[1]);
			Assert.AreEqual("column, three", records[0].Fields[2]);
			Assert.AreEqual("data \"\",1", records[1].Fields[0]);
			Assert.AreEqual("dat\"\"\"sa, 2", records[1].Fields[1]);
			Assert.AreEqual("data 3", records[1].Fields[2]);
		}

		private void VerifyTestData6(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual(" column one ", headers[0]);
			Assert.AreEqual("  column two  ", headers[1]);
			Assert.AreEqual("   column three   ", headers[2]);
			Assert.AreEqual("   1   ", records[0].Fields[0]);
			Assert.AreEqual("  data 2  ", records[0].Fields[1]);
			Assert.AreEqual(" 2010-05-01 11:26:01 ", records[0].Fields[2]);
		}

		private void VerifyTestData6Trimmed(List<string> headers, CsvRecords records) {
			Assert.IsTrue(headers.Count == 3);
			Assert.IsTrue(records.Count == 1);
			Assert.AreEqual("column one", headers[0]);
			Assert.AreEqual("column two", headers[1]);
			Assert.AreEqual("column three", headers[2]);
			Assert.AreEqual("1", records[0].Fields[0]);
			Assert.AreEqual("data 2", records[0].Fields[1]);
			Assert.AreEqual("2010-05-01 11:26:01", records[0].Fields[2]);
		}


	}
}
