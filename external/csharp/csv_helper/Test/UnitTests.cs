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
	public class UnitTests {

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

		public UnitTests() {
		}

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
		public void CsvReader_TestReadingFromFile() {
			File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);

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

		[Test]
		public void CsvReader_TestReadingFromStream() {
			using (var memoryStream = new MemoryStream(TEST_DATA_1.Length)) {
				using (var streamWriter = new StreamWriter(memoryStream)) {
					streamWriter.Write(TEST_DATA_1);
					streamWriter.Flush();

					using (var reader = new CsvReader(memoryStream, Encoding.Default)) {
						var records = new List<List<string>>();

						while (reader.ReadNextRecord())
							records.Add(reader.Fields);

						Assert.IsTrue(records.Count == 2);

						CsvFile csvFile = CreateCsvFile(records[0], records[1]);
						VerifyTestData1(csvFile.Headers, csvFile.Records);
					}
				}
			}        
		}

		[Test]
		public void CsvReader_TestReadingFromString() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData1(csvFile.Headers, csvFile.Records);
			}   
		}

		[Test]
		public void CsvReader_TestReadIntoDataTableWithTypes() {
			var dataTable = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true }) {
				dataTable = reader.ReadIntoDataTable(new[] {
					typeof(int),
					typeof(string),
					typeof(DateTime)
				});
			}

			CsvFile file = CreateCsvFileFromDataTable(dataTable);
			VerifyTestData1(file.Headers, file.Records);
		}

		[Test]
		public void CsvReader_TestReadIntoDataTableWithoutTypes() {
			var dataTable = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_1) { HasHeaderRow = true }) {
				dataTable = reader.ReadIntoDataTable();
			}

			CsvFile file = CreateCsvFileFromDataTable(dataTable);
			VerifyTestData1(file.Headers, file.Records);
		}

		[Test]
		public void CsvReader_TestReadingFromStringWithSampleData2() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_2)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData2(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvReader_TestReadingFromStringWithSampleData3() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_3)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData3(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvReader_TestReadingFromStringWithSampleData4() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_4)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData4(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvReader_TestReadingFromStringWithSampleData5() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData5(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvReader_TestReadingFromStringWithSampleData6() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_6)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData6(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvReader_TestColumnTrimming() {
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_6) { TrimColumns = true }) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 2);

				CsvFile csvFile = CreateCsvFile(records[0], records[1]);
				VerifyTestData6Trimmed(csvFile.Headers, csvFile.Records);
			}
		}

		[Test]
		public void CsvFile_PopulateFromFileWithHeader() {
			CsvFile csvFile1 = new CsvFile();
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				csvFile1 = CreateCsvFile(records[0], records[1]);
			}

			if (File.Exists(FilePath))
				File.Delete(FilePath);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvFile1, FilePath, Encoding.Default);
			}

			var file = new CsvFile();
			file.Populate(FilePath, true);
			VerifyTestData5(file.Headers, file.Records);

			File.Delete(FilePath);
		}

		[Test]
		public void CsvFile_PopulateFromFileWithoutHeader() {
			CsvFile csvFile1 = new CsvFile();
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				csvFile1 = CreateCsvFile(records[0], records[1]);
			}

			if (File.Exists(FilePath))
				File.Delete(FilePath);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvFile1, FilePath, Encoding.Default);
			}

			var file = new CsvFile();
			file.Populate(FilePath, false);
			VerifyTestData5Alternative(file.Records);

			File.Delete(FilePath);
		}

		[Test]
		public void CsvFile_PopulateFromStream() {
			using (var memoryStream = new MemoryStream(TEST_DATA_5.Length)) {
				using (var streamWriter = new StreamWriter(memoryStream)) {
					streamWriter.Write(TEST_DATA_5);
					streamWriter.Flush();

					var file = new CsvFile();
					file.Populate(memoryStream, true);
					VerifyTestData5(file.Headers, file.Records);
				}
			}
		}

		[Test]
		public void CsvFile_PopulateFromString() {
			var file = new CsvFile();
			file.Populate(true, TEST_DATA_5);
			VerifyTestData5(file.Headers, file.Records);
		}

		[Test]
		public void CsvFile_Indexers() {
			var file = new CsvFile();
			file.Populate(true, TEST_DATA_2);

			Assert.IsTrue(file[0] == file.Records[0]);
			Assert.IsTrue(string.Compare(file[0, 1], "data, 2") == 0);
			Assert.IsTrue(string.Compare(file[0, "column two"], "data, 2") == 0);

		}

		[Test]
		public void CsvWriter_WriteCsvFileObjectToFile() {
			if (File.Exists(FilePath))
				File.Delete(FilePath);

			var csvFile = new CsvFile();
			csvFile.Populate(true, TEST_DATA_5);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvFile, FilePath);
			}

			csvFile = new CsvFile();
			csvFile.Populate(FilePath, true);

			VerifyTestData5(csvFile.Headers, csvFile.Records);

			File.Delete(FilePath);
		}

		[Test]
		public void CsvWriter_WriteCsvFileObjectToStream() {
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				var csvFile = new CsvFile();
				csvFile.Populate(true, TEST_DATA_5);
                
				using (var writer = new CsvWriter()) {
					writer.WriteCsv(csvFile, memoryStream);
					using (StreamReader reader = new StreamReader(memoryStream)) {
						content = reader.ReadToEnd();
					}
				}
			}

			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void CsvWriter_WriteCsvFileObjectToString() {
			var csvFile = new CsvFile();
			csvFile.Populate(true, TEST_DATA_5);
			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(csvFile, Encoding.Default);
			}

			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void CsvWriter_WriteDataTableToFile() {    
			if (File.Exists(FilePath))
				File.Delete(FilePath);

			var table = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				table = reader.ReadIntoDataTable();
			}

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(table, FilePath);
			}

			CsvFile csvFile = CreateCsvFileFromDataTable(table);
			VerifyTestData5(csvFile.Headers, csvFile.Records);
			File.Delete(FilePath);
		}

		[Test]
		public void CsvWriter_WriteDataTableToStream() {
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				var table = new DataTable();

				using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
					table = reader.ReadIntoDataTable();
				}

				using (var writer = new CsvWriter()) {
					writer.WriteCsv(table, memoryStream);
                    
					using (StreamReader reader = new StreamReader(memoryStream)) {
						content = reader.ReadToEnd();
					}
				}

			}

			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void CsvWriter_WriteDataTableToString() {
			if (File.Exists(FilePath))
				File.Delete(FilePath);

			var table = new DataTable();

			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				table = reader.ReadIntoDataTable();
			}

			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(table, Encoding.Default);
			}

			File.Delete(FilePath);
			Assert.IsTrue(string.Compare(content, TEST_DATA_5) == 0);
		}

		[Test]
		public void CsvWriter_VerifyThatCarriageReturnsAreHandledCorrectlyInFieldValues() {
			var csvFile = new CsvFile();
			csvFile.Headers.Add("header ,1");
			csvFile.Headers.Add("header\r\n2");
			csvFile.Headers.Add("header 3");

			CsvRecord record = new CsvRecord();
			record.Fields.Add("da,ta 1");
			record.Fields.Add("\"data\" 2");
			record.Fields.Add("data\n3");
			csvFile.Records.Add(record);

			string content = string.Empty;

			using (var writer = new CsvWriter()) {
				content = writer.WriteCsv(csvFile, Encoding.Default);
			}

			Assert.IsTrue(string.Compare(content, "\"header ,1\",\"header,2\",header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",\"data,3\"\r\n") == 0);

			using (var writer = new CsvWriter() { ReplaceCarriageReturnsAndLineFeedsFromFieldValues = false }) {
				content = writer.WriteCsv(csvFile, Encoding.Default);
			}

			Assert.IsTrue(string.Compare(content, "\"header ,1\",header\r\n2,header 3\r\n\"da,ta 1\",\"\"\"data\"\" 2\",data\n3\r\n") == 0);


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
