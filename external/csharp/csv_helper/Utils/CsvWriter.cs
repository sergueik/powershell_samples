using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
	public sealed class CsvWriter : IDisposable
	{
		private StreamWriter _streamWriter;
		private bool _replaceCarriageReturnsAndLineFeedsFromFieldValues = true;
		private string _carriageReturnAndLineFeedReplacement = ",";


		public bool ReplaceCarriageReturnsAndLineFeedsFromFieldValues {
			get {
				return _replaceCarriageReturnsAndLineFeedsFromFieldValues;
			}
			set {
				_replaceCarriageReturnsAndLineFeedsFromFieldValues = value;
			}
		}
		public string CarriageReturnAndLineFeedReplacement {
			get {
				return _carriageReturnAndLineFeedReplacement;   
			}
			set {
				_carriageReturnAndLineFeedReplacement = value;
			}
		}

		public void WriteCsv(CsvData csvData, string filePath)
		{
			WriteCsv(csvData, filePath, null);
		}

		public void WriteCsv(CsvData csvData, string filePath, Encoding encoding)
		{
			if (File.Exists(filePath))
				File.Delete(filePath);

			using (var writer = new StreamWriter(filePath, false, encoding ?? Encoding.Default)) {
				WriteToStream(csvData, writer);
				writer.Flush();
				writer.Close();
			}
		}

		public void WriteCsv(CsvData csvData, Stream stream)
		{
			WriteCsv(csvData, stream, null);
		}

		public void WriteCsv(CsvData csvData, Stream stream, Encoding encoding)
		{
			stream.Position = 0;
			_streamWriter = new StreamWriter(stream, encoding ?? Encoding.Default);
			WriteToStream(csvData, _streamWriter);
			_streamWriter.Flush();
			stream.Position = 0;
		}

		public string WriteCsv(CsvData csvData, Encoding encoding)
		{
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				using (var writer = new StreamWriter(memoryStream, encoding ?? Encoding.Default)) {
					WriteToStream(csvData, writer);
					writer.Flush();
					memoryStream.Position = 0;

					using (var reader = new StreamReader(memoryStream, encoding ?? Encoding.Default)) {
						content = reader.ReadToEnd();
						writer.Close();
						reader.Close();
						memoryStream.Close();
					}
				}
			}

			return content;
		}

		public void WriteCsv(DataTable dataTable, string filePath)
		{
			WriteCsv(dataTable, filePath, null);
		}

		public void WriteCsv(DataTable dataTable, string filePath, Encoding encoding)
		{
			if (File.Exists(filePath))
				File.Delete(filePath);

			using (var writer = new StreamWriter(filePath, false, encoding ?? Encoding.Default)) {
				WriteToStream(dataTable, writer);
				writer.Flush();
				writer.Close();
			}
		}

		public void WriteCsv(DataTable dataTable, Stream stream)
		{
			WriteCsv(dataTable, stream, null);
		}

		public void WriteCsv(DataTable dataTable, Stream stream, Encoding encoding)
		{
			stream.Position = 0;
			_streamWriter = new StreamWriter(stream, encoding ?? Encoding.Default);
			WriteToStream(dataTable, _streamWriter);
			_streamWriter.Flush();
			stream.Position = 0;
		}

		public string WriteCsv(DataTable dataTable, Encoding encoding)
		{
			string content = string.Empty;

			using (var memoryStream = new MemoryStream()) {
				using (var writer = new StreamWriter(memoryStream, encoding ?? Encoding.Default)) {
					WriteToStream(dataTable, writer);
					writer.Flush();
					memoryStream.Position = 0;

					using (var reader = new StreamReader(memoryStream, encoding ?? Encoding.Default)) {
						content = reader.ReadToEnd();
						writer.Close();
						reader.Close();
						memoryStream.Close();
					}
				}
			}

			return content;
		}

		private void WriteToStream(CsvData csvData, TextWriter writer)
		{
			if (csvData.Headers.Count > 0)
				WriteRecord(csvData.Headers, writer);

			csvData.Records.ForEach(record => WriteRecord(record.Fields, writer));
		}

		private void WriteToStream(DataTable dataTable, TextWriter writer)
		{
			List<string> fields = (from DataColumn column in dataTable.Columns
			                       select column.ColumnName).ToList();
			WriteRecord(fields, writer);

			foreach (DataRow row in dataTable.Rows) {
				fields.Clear();
				fields.AddRange(row.ItemArray.Select(o => o.ToString()));
				WriteRecord(fields, writer);
			}
		}

		private void WriteRecord(IList<string> fields, TextWriter writer)
		{
			for (int i = 0; i < fields.Count; i++) {
				bool quotesRequired = fields[i].Contains(",");
				bool escapeQuotes = fields[i].Contains("\"");
				string fieldValue = (escapeQuotes ? fields[i].Replace("\"", "\"\"") : fields[i]);

				if (ReplaceCarriageReturnsAndLineFeedsFromFieldValues && (fieldValue.Contains("\r") || fieldValue.Contains("\n"))) {
					quotesRequired = true;
					fieldValue = fieldValue.Replace("\r\n", CarriageReturnAndLineFeedReplacement);
					fieldValue = fieldValue.Replace("\r", CarriageReturnAndLineFeedReplacement);
					fieldValue = fieldValue.Replace("\n", CarriageReturnAndLineFeedReplacement);
				}

				writer.Write(string.Format("{0}{1}{0}{2}", 
					(quotesRequired || escapeQuotes ? "\"" : string.Empty),
					fieldValue, 
					(i < (fields.Count - 1) ? "," : string.Empty)));
			}

			writer.WriteLine();
		}
		public void AppendCsv(CsvData csvData, string filePath, Encoding encoding = null, bool writeHeaderIfNew = true)
		{
			bool fileExists = File.Exists(filePath);
			bool append = true;
			IList<string> existingHeaders = ReadHeader(filePath, encoding);
			
			if (!HeadersMatch(existingHeaders, csvData.Headers)) {
				throw new InvalidOperationException(
					String.Format("CSV headers mismatch. existing: [{0}] new: [{1}]", string.Join(",", existingHeaders), string.Join(",", csvData.Headers)));
			}		
			using (var writer = new StreamWriter(filePath, append, encoding ?? Encoding.Default)) {
				if (!fileExists && writeHeaderIfNew && csvData.Headers.Count > 0) {
					WriteRecord(csvData.Headers, writer);
				}

				csvData.Records.ForEach(record => WriteRecord(record.Fields, writer));
			}
		}
		
		private IList<string> ReadHeader(string filePath, Encoding encoding)
		{
			using (var reader = new StreamReader(filePath, encoding ?? Encoding.Default)) {
				var line = reader.ReadLine();
				if (line == null)
					return new List<string>();

				return ParseHeaderLine(line);
			}
		}
		private bool HeadersMatch(IList<string> h1, IList<string> h2)
		{
			if (h1.Count != h2.Count)
				return false;

			for (int i = 0; i < h1.Count; i++) {
				if (!string.Equals(h1[i], h2[i], StringComparison.Ordinal))
					return false;
			}

			return true;
		}
		private IList<string> ParseHeaderLine(string line)
		{
			// keep consistent with your writer (simple CSV)
			return line.Split(',').Select(h => h.Trim('"')).ToList();
		}
		public void Dispose()
		{
			if (_streamWriter == null)
				return;

			_streamWriter.Close();
			_streamWriter.Dispose();
		}

	}
}
