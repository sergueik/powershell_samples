using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvHelper {

    [Serializable]
    public sealed class CsvFile {

        #region Properties

        public readonly List<string> Headers = new List<string>();

        public readonly CsvRecords Records = new CsvRecords();

        public int HeaderCount {
            get
            {
                return Headers.Count;
            }
        }

        public int RecordCount {   
            get
            {
                return Records.Count;   
            }
        }

        #endregion Properties

        #region Indexers

        public CsvRecord this[int recordIndex] {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                return Records[recordIndex];
            }
        }

        public string this[int recordIndex, int fieldIndex] {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];
                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0}.", fieldIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        public string this[int recordIndex, string fieldName] {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0) 
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0)
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public void Populate(string filePath, bool hasHeaderRow) {
            Populate(filePath, null, hasHeaderRow, false);
        }

        public void Populate(string filePath, bool hasHeaderRow, bool trimColumns) {
            Populate(filePath, null, hasHeaderRow, trimColumns);
        }

        public void Populate(string filePath, Encoding encoding, bool hasHeaderRow, bool trimColumns) {
            using (CsvReader reader = new CsvReader(filePath, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        public void Populate(Stream stream, bool hasHeaderRow) {
            Populate(stream, null, hasHeaderRow, false);
        }

        public void Populate(Stream stream, bool hasHeaderRow, bool trimColumns) {
            Populate(stream, null, hasHeaderRow, trimColumns);
        }

        public void Populate(Stream stream, Encoding encoding, bool hasHeaderRow, bool trimColumns) {
            using (CsvReader reader = new CsvReader(stream, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        public void Populate(bool hasHeaderRow, string csvContent) {
            Populate(hasHeaderRow, csvContent, null, false);
        }

        public void Populate(bool hasHeaderRow, string csvContent, bool trimColumns) {
            Populate(hasHeaderRow, csvContent, null, trimColumns);
        }

        public void Populate(bool hasHeaderRow, string csvContent, Encoding encoding, bool trimColumns) {
            using (CsvReader reader = new CsvReader(encoding, csvContent){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        private void PopulateCsvFile(CsvReader reader) {
            Headers.Clear();
            Records.Clear();

            bool addedHeader = false;

            while (reader.ReadNextRecord())
            {
                if (reader.HasHeaderRow && !addedHeader)
                {
                    reader.Fields.ForEach(field => Headers.Add(field));
                    addedHeader = true;
                    continue;
                }

                CsvRecord record = new CsvRecord();
                reader.Fields.ForEach(field => record.Fields.Add(field));
                Records.Add(record);
            }
        }

        #endregion Methods

    }

    [Serializable]
    public sealed class CsvRecords : List<CsvRecord> {  
    }

    [Serializable]
    public sealed class CsvRecord {
        #region Properties

        public readonly List<string> Fields = new List<string>();

        public int FieldCount
        {
            get
            {
                return Fields.Count;
            }
        }

        #endregion Properties
    }
}
