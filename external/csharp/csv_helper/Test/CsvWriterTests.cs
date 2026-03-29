using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Utils;
using NUnit.Framework;
using System.Linq;

namespace TestProject1 {
	[TestFixture]
	public class CsvWriterTests {

		private const string TEST_DATA_1 = @"column one,column two,column three
1,data 2,2010-05-01 11:26:01
";
		private const string TEST_DATA_5 = @"""column, one"",""column """"two"",""column, three""
""data """""""",1"",""dat""""""""""""sa, 2"",data 3
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
		public void WriteCsvDataObjectToFile() {
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_5);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvData, FilePath);
			}

			csvData = new CsvData();
			csvData.Populate(FilePath, true);

			VerifyTestData5(csvData.Headers, csvData.Records);

			File.Delete(FilePath);
		}

		//[Ignore("Line Endings not Handled")]
		[Test]
		public void WriteCsvDataObjectToStream() {
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				var csvData = new CsvData();
				csvData.Populate(true, TEST_DATA_5);
                
				using (var writer = new CsvWriter()) {
					writer.WriteCsv(csvData, memoryStream);
					using (var reader = new StreamReader(memoryStream)) {
						content = reader.ReadToEnd();
					}
				}
			}
			content = Normalize(content);
			StringAssert.AreEqualIgnoringCase(content, TEST_DATA_5);
			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		// [Ignore("Line Endings not Handled")]
		[Test]
		public void WriteCsvDataObjectToString() {
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_5);
			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(csvData, Encoding.Default);
			}
			content = Normalize(content);
			StringAssert.AreEqualIgnoringCase(content, TEST_DATA_5);	
			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void WriteDataTableToFile() {    
			var table = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				table = reader.ReadIntoDataTable();
			}

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(table, FilePath);
			}

			CsvData csvData = CreateCsvDataFromDataTable(table);
			VerifyTestData5(csvData.Headers, csvData.Records);
		}
		
		// [Ignore("Line Endings not Handled")]
		[Test]
		public void WriteDataTableToStream() {
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				var table = new DataTable();

				using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1)) {
					table = reader.ReadIntoDataTable();
				}

				using (var writer = new CsvWriter()) {
					writer.WriteCsv(table, memoryStream);
                    
					using (var reader = new StreamReader(memoryStream)) {
						content = reader.ReadToEnd();
					}
				}

			}
			content = Normalize(content);
			StringAssert.AreEqualIgnoringCase(content, TEST_DATA_1);
			Assert.IsTrue(string.Compare(content, TEST_DATA_1) == 0);
		}

		// [Ignore("Line Endings not Handled")]
		[Test]
		public void WriteDataTableToString() {
			var table = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				table = reader.ReadIntoDataTable();
			}

			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(table, Encoding.Default);
			}
			content = Normalize(content);
			StringAssert.AreEqualIgnoringCase(content, TEST_DATA_5);
			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void VerifyThatCarriageReturnsAreHandledCorrectlyInFieldValues() {
			var csvData = new CsvData();
			csvData.Headers.Add("header ,1");
			csvData.Headers.Add("header\r\n2");
			csvData.Headers.Add("header 3");

			CsvRecord record = new CsvRecord();
			record.Fields.Add("da,ta 1");
			record.Fields.Add("\"data\" 2");
			record.Fields.Add("data\n3");
			csvData.Records.Add(record);

			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(csvData, Encoding.Default);
			}

			Assert.IsTrue(string.Compare(content, "\"header ,1\",\"header,2\",header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",\"data,3\"\r\n") == 0);

			using (var writer = new CsvWriter() { ReplaceCarriageReturnsAndLineFeedsFromFieldValues = false }) {
				content = writer.WriteCsv(csvData, Encoding.Default);
			}

			Assert.IsTrue(string.Compare(content, "\"header ,1\",header\r\n2,header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",data\n3\r\n") == 0);


		}

		private CsvData CreateCsvDataFromDataTable(DataTable table) {
			var file = new CsvData();

			foreach (DataColumn column in table.Columns)
				file.Headers.Add(column.ColumnName);

			foreach (DataRow row in table.Rows) {
				var record = new CsvRecord();

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

		private CsvData CreateCsvData(List<string> headers, List<string> fields) {
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			var record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			return csvData;
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

		private string Normalize(string s)
		{
		    return s.Replace("\r\n", "\n").Replace("\r", "\n");
		}
	}
}
