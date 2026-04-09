using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Test
{
	[TestFixture]
	public class CsvTests
	{

		private const string TEST_DATA_1 = @"column one,column two,column three
1,data 2,2010-05-01 11:26:01
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

		[TestFixtureTearDown]
		public static void Cleanup()
		{
			if (File.Exists(FilePath))
				File.Delete(FilePath);
		}

		[Test]
		public void testPopulateFromData() {
			var data = new Data();
			data.TimeStamp = DateTime.Now;
			data.Value = (float)42.0;
			var headers = new List<string> { "TimeStamp", "Value" };
			var fields = new List<string> {
				data.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
				data.Value.ToString("F2")
			};
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			var record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			headers.Shuffle();
			Assert.That(csvData.Headers, Is.EquivalentTo(headers.ToArray()));          	
			fields.Shuffle();
			Debug.WriteLine("Headers:" + string.Join(", ", csvData.Headers));
			Assert.That(csvData.Records[0].Fields, Is.EquivalentTo(fields.ToArray()));
			Debug.WriteLine("Record: " + string.Join(", ", csvData.Records[0].Fields));
		}
		
		[Test]
		public void testAppendToFile() {
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
		public void testBadAppendToFile()
		{
			File.WriteAllText(FilePath, TEST_DATA_1, Encoding.Default);
			var csvData = new CsvData();
			csvData.Populate(true, TEST_DATA_7);
			using (var writer = new CsvWriter()) {
				writer.AppendCsv(csvData, FilePath, Encoding.Default);
			}
		}
		
		private CsvData CreateCsvData(List<string> headers, List<string> fields)
		{
			var csvData = new CsvData();

			headers.ForEach(header => csvData.Headers.Add(header));
			CsvRecord record = new CsvRecord();
			fields.ForEach(field => record.Fields.Add(field));
			csvData.Records.Add(record);
			return csvData;
		}

	
		private void VerifyTestData1(List<string> headers, CsvRecords records)
		{
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
