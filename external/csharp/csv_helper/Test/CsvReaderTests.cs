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
	public class CvsReaderTests {

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
		public void TestReadingFromFile() {
			File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);

			using (var reader = new CsvReader(FilePath, Encoding.Default)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData1(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestReadingFromStream() {
			using (var memoryStream = new MemoryStream(TEST_DATA_1.Length)) {
				using (var streamWriter = new StreamWriter(memoryStream)) {
					streamWriter.Write(TEST_DATA_1);
					streamWriter.Flush();

					using (var reader = new CsvReader(memoryStream, Encoding.Default)) {
						var records = new List<List<string>>();

						while (reader.ReadNextRecord())
							records.Add(reader.Fields);

						Assert.IsTrue(records.Count == 2);

						CsvData csvData = CreateCsvData(records[0], records[1]);
						VerifyTestData1(csvData.Headers, csvData.Records);
					}
				}
			}        
		}

		[Test]
		public void TestReadingFromString() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData1(csvData.Headers, csvData.Records);
			}   
		}

		[Test]
		public void TestReadIntoDataTableWithTypes() {
			var dataTable = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true }) {
				dataTable = reader.ReadIntoDataTable(new[] {
					typeof(int),
					typeof(string),
					typeof(DateTime)
				});
			}

			CsvData csvData = CreateCsvDataFromDataTable(dataTable);
			VerifyTestData1(csvData.Headers, csvData.Records);
		}

		[Test]
		public void TestReadIntoDataTableWithoutTypes() {
			var dataTable = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true }) {
				dataTable = reader.ReadIntoDataTable();
			}

			CsvData csvData = CreateCsvDataFromDataTable(dataTable);
			VerifyTestData1(csvData.Headers, csvData.Records);
		}
		[Test]
		public void TestReadingFromStringWithSampleData2() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_2)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData2(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestReadingFromStringWithSampleData3() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_3)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData3(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestReadingFromStringWithSampleData4() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_4)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData4(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestReadingFromStringWithSampleData5() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData5(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestReadingFromStringWithSampleData6() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_6)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData6(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		public void TestColumnTrimming() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_6) { TrimColumns = true }) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvData csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData6Trimmed(csvData.Headers, csvData.Records);
			}
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
