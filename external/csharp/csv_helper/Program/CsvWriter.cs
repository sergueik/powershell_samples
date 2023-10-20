using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CsvHelper {
	public sealed class CsvWriter : IDisposable {

		#region Members

		private StreamWriter _streamWriter;
		private bool _replaceCarriageReturnsAndLineFeedsFromFieldValues = true;
		private string _carriageReturnAndLineFeedReplacement = ",";

		#endregion Members

		#region Properties

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

		#endregion Properties

		#region Methods

		#region CsvFile write methods

		public void WriteCsv(CsvFile csvFile, string filePath) {
			WriteCsv(csvFile, filePath, null);
		}

		public void WriteCsv(CsvFile csvFile, string filePath, Encoding encoding) {
			if (File.Exists(filePath))
				File.Delete(filePath);

			using (StreamWriter writer = new StreamWriter(filePath, false, encoding ?? Encoding.Default)) {
				WriteToStream(csvFile, writer);
				writer.Flush();
				writer.Close();
			}
		}

		public void WriteCsv(CsvFile csvFile, Stream stream) {
			WriteCsv(csvFile, stream, null);
		}

		public void WriteCsv(CsvFile csvFile, Stream stream, Encoding encoding) {
			stream.Position = 0;
			_streamWriter = new StreamWriter(stream, encoding ?? Encoding.Default);
			WriteToStream(csvFile, _streamWriter);
			_streamWriter.Flush();
			stream.Position = 0;
		}

		public string WriteCsv(CsvFile csvFile, Encoding encoding) {
			string content = string.Empty;

			using (MemoryStream memoryStream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(memoryStream, encoding ?? Encoding.Default)) {
					WriteToStream(csvFile, writer);
					writer.Flush();
					memoryStream.Position = 0;

					using (StreamReader reader = new StreamReader(memoryStream, encoding ?? Encoding.Default)) {
						content = reader.ReadToEnd();
						writer.Close();
						reader.Close();
						memoryStream.Close();
					}
				}
			}

			return content;
		}

		#endregion CsvFile write methods

		#region DataTable write methods

		public void WriteCsv(DataTable dataTable, string filePath) {
			WriteCsv(dataTable, filePath, null);
		}

		public void WriteCsv(DataTable dataTable, string filePath, Encoding encoding) {
			if (File.Exists(filePath))
				File.Delete(filePath);

			using (StreamWriter writer = new StreamWriter(filePath, false, encoding ?? Encoding.Default)) {
				WriteToStream(dataTable, writer);
				writer.Flush();
				writer.Close();
			}
		}

		public void WriteCsv(DataTable dataTable, Stream stream) {
			WriteCsv(dataTable, stream, null);
		}

		public void WriteCsv(DataTable dataTable, Stream stream, Encoding encoding) {
			stream.Position = 0;
			_streamWriter = new StreamWriter(stream, encoding ?? Encoding.Default);
			WriteToStream(dataTable, _streamWriter);
			_streamWriter.Flush();
			stream.Position = 0;
		}

		public string WriteCsv(DataTable dataTable, Encoding encoding) {
			string content = string.Empty;

			using (MemoryStream memoryStream = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(memoryStream, encoding ?? Encoding.Default)) {
					WriteToStream(dataTable, writer);
					writer.Flush();
					memoryStream.Position = 0;

					using (StreamReader reader = new StreamReader(memoryStream, encoding ?? Encoding.Default)) {
						content = reader.ReadToEnd();
						writer.Close();
						reader.Close();
						memoryStream.Close();
					}
				}
			}

			return content;
		}

		#endregion DataTable write methods

		private void WriteToStream(CsvFile csvFile, TextWriter writer) {
			if (csvFile.Headers.Count > 0)
				WriteRecord(csvFile.Headers, writer);

			csvFile.Records.ForEach(record => WriteRecord(record.Fields, writer));
		}

		private void WriteToStream(DataTable dataTable, TextWriter writer) {
			List<string> fields = (from DataColumn column in dataTable.Columns
			                       select column.ColumnName).ToList();
			WriteRecord(fields, writer);

			foreach (DataRow row in dataTable.Rows) {
				fields.Clear();
				fields.AddRange(row.ItemArray.Select(o => o.ToString()));
				WriteRecord(fields, writer);
			}
		}

		private void WriteRecord(IList<string> fields, TextWriter writer) {
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

		public void Dispose() {
			if (_streamWriter == null)
				return;

			_streamWriter.Close();
			_streamWriter.Dispose();
		}

		#endregion Methods

	}
}
