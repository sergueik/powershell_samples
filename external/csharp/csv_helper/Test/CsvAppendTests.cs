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
		private const string TEST_DATA_7 = @"column one,column two,column three,column four
1,data 2,2010-05-01 11:26:01,3
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
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_1);
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(csvData, FilePath, Encoding.Default);
			}
			using (var reader = new CsvReader(FilePath, Encoding.Default)) {
				var records = new List<List<string>>();
				while (reader.ReadNextRecord())
					records.Add(reader.Fields);

				Assert.IsTrue(records.Count == 3);

				csvData = CreateCsvData(records[0], records[2]);
				VerifyTestData1(csvData.Headers, csvData.Records);
				csvData = CreateCsvData(records[0], records[1]);
				VerifyTestData1(csvData.Headers, csvData.Records);
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestBadAppendToFile() {
			File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_7);
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(csvData, FilePath, Encoding.Default);
			}
		}
		
		private CsvData CreateCsvData(List<string> headers, List<string> fields) {
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			CsvRecord record = new CsvRecord();
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

	}
}
