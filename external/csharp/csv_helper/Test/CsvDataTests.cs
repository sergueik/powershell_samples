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
	public class CsvDataTests {

		private const string TEST_DATA_2 = @"""column, one"",column two,""column, three""
data 1,""data, 2"",data 3
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
		[Test]
		public void PopulateFromFileWithHeader() {
			var csvData = new CsvData();
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				csvData = CreatecsvData(records[0], records[1]);
			}

			if (File.Exists(FilePath))
				File.Delete(FilePath);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvData, FilePath, Encoding.Default);
			}

			csvData = new CsvData();
			csvData.Populate(FilePath, true);
			VerifyTestData5(csvData.Headers, csvData.Records);

			File.Delete(FilePath);
		}

		[Test]
		public void PopulateFromFileWithoutHeader() {
			var csvData = new CsvData();
			using (var reader = new CsvReader(Encoding.Default, TEST_DATA_5)) {
				var records = new List<List<string>>();

				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				csvData = CreatecsvData(records[0], records[1]);
			}

			if (File.Exists(FilePath))
				File.Delete(FilePath);

			using (var writer = new CsvWriter()) {
				writer.WriteCsv(csvData, FilePath, Encoding.Default);
			}

			csvData = new CsvData();
			csvData.Populate(FilePath, false);
			VerifyTestData5Alternative(csvData.Records);

			File.Delete(FilePath);
		}

		[Test]
		public void PopulateFromStream() {
			using (var memoryStream = new MemoryStream(TEST_DATA_5.Length)) {
				using (var streamWriter = new StreamWriter(memoryStream)) {
					streamWriter.Write(TEST_DATA_5);
					streamWriter.Flush();

					var csvData = new CsvData();
					csvData.Populate(memoryStream, true);
					VerifyTestData5(csvData.Headers, csvData.Records);
				}
			}
		}

		[Test]
		public void PopulateFromData() {
		var data = new Data();
		data.TimeStamp = DateTime.Now;
		data.Value = (float)42.0;
		var headers = new List<string> { "TimeStamp", "Value" };
		var fields = new List<string> {data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),data.Value.ToString("F2") };
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			var record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			Console.Error.WriteLine("Headers:" + string.Join(", ",csvData.Headers));
			Assert.That(csvData.Records[0].Fields, Is.EquivalentTo( fields.ToArray() ));
			Console.Error.WriteLine("Record: " + string.Join(", ",csvData.Records[0].Fields));
		}

		[Test]
		public void PopulateFromString() {
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_5);
			VerifyTestData5(csvData.Headers, csvData.Records);
		}

		[Test]
		public void Indexers() {
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_2);

			Assert.IsTrue(csvData[0] == csvData.Records[0]);
			Assert.IsTrue(string.Compare(csvData[0, 1], "data, 2") == 0);
			Assert.IsTrue(string.Compare(csvData[0, "column two"], "data, 2") == 0);

		}
		private CsvData CreatecsvDataFromDataTable(DataTable table) {
			var csvData = new CsvData();

			foreach (DataColumn column in table.Columns)
				csvData.Headers.Add(column.ColumnName);

			foreach (DataRow row in table.Rows) {
				var record = new CsvRecord();

				foreach (object o in row.ItemArray) {
					if (o is DateTime)
						record.Fields.Add(((DateTime)o).ToString("yyyy-MM-dd hh:mm:ss"));
					else
						record.Fields.Add(o.ToString());
				}

				csvData.Records.Add(record);
			}

			return csvData;
		}

		private CsvData CreatecsvData(List<string> headers, List<string> fields) {
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



		[TestFixtureTearDown]
		public static void Cleanup() {
			if (File.Exists(FilePath))
				File.Delete(FilePath);
		}


	}
	
	public class Data{
		public DateTime TimeStamp { get; set; }
		public float Value { get; set; }
		public override string ToString() {
			return string.Format("TimeStamp={0} {1}, Value={2}", TimeStamp.ToLongDateString(), TimeStamp.ToLongTimeString(), Value);
		}
	}

}
