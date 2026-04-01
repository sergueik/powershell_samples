using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils {

    [Serializable]
    public sealed class CsvData {
    	
        private readonly List<string> headers = new List<string>();

        private readonly CsvRecords records = new CsvRecords();

        public List<string> Headers { get  { return headers;} }
        public CsvRecords Records { get { return records;} }

        public int HeaderCount {
            get
            {
                return headers.Count;
            }
        }

        public int RecordCount {   
            get
            {
                return records.Count;   
            }
        }

        public CsvRecord this[int recordIndex] {
            get
            {
                if (recordIndex > (records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("wrong record index {0}", recordIndex));

                return records[recordIndex];
            }
        }

        public string this[int recordIndex, int fieldIndex] {
            get
            {
                if (recordIndex > (records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("wrong record index {0}", recordIndex));

                CsvRecord record = records[recordIndex];
                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("from field index {0} in record {1}", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = records[recordIndex];

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0}.", fieldIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        public string this[int recordIndex, string fieldName] {
            get
            {
                if (recordIndex > (records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = records[recordIndex];

                int fieldIndex = -1;

                for (int index = 0; index < headers.Count; index++){
                    if (string.Compare(headers[index], fieldName) != 0) 
                        continue;

                    fieldIndex = index;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("wrong field header name '{0}", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("there is no field named {0} in record {1}", fieldName, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("wrong record index {0}", recordIndex));

                CsvRecord record = records[recordIndex];

                int fieldIndex = -1;

                for (int index = 0; index < headers.Count; index++)
                {
                    if (string.Compare(headers[index], fieldName) != 0)
                        continue;

                    fieldIndex = index;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        public void Populate(string filePath, bool hasHeaderRow) {
            Populate(filePath, null, hasHeaderRow, false);
        }

        public void Populate(string filePath, bool hasHeaderRow, bool trimColumns) {
            Populate(filePath, null, hasHeaderRow, trimColumns);
        }

        public void Populate(string filePath, Encoding encoding, bool hasHeaderRow, bool trimColumns) {
            using (var reader = new CsvReader(filePath, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvData(reader);
            }
        }

        public void Populate(Stream stream, bool hasHeaderRow) {
            Populate(stream, null, hasHeaderRow, false);
        }

        public void Populate(Stream stream, bool hasHeaderRow, bool trimColumns) {
            Populate(stream, null, hasHeaderRow, trimColumns);
        }

        public void Populate(Stream stream, Encoding encoding, bool hasHeaderRow, bool trimColumns) {
            using (var reader = new CsvReader(stream, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvData(reader);
            }
        }

        public void Populate(bool hasHeaderRow, string csvContent) {
            Populate(hasHeaderRow, csvContent, null, false);
        }

        public void Populate(bool hasHeaderRow, string csvContent, bool trimColumns) {
            Populate(hasHeaderRow, csvContent, null, trimColumns);
        }

        public void Populate(bool hasHeaderRow, string csvContent, Encoding encoding, bool trimColumns) {
            using (var reader = new CsvReader(encoding, csvContent){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvData(reader);
            }
        }

        private void PopulateCsvData(CsvReader reader) {
            headers.Clear();
            records.Clear();

            bool addedHeader = false;

            while (reader.ReadNextRecord())
            {
                if (reader.HasHeaderRow && !addedHeader)
                {
                    reader.Fields.ForEach(field => headers.Add(field));
                    addedHeader = true;
                    continue;
                }

                var record = new CsvRecord();
                reader.Fields.ForEach(field => record.Fields.Add(field));
                records.Add(record);
            }
        }

    }

    [Serializable]
    public sealed class CsvRecords : List<CsvRecord> {  
    }

    [Serializable]
    public sealed class CsvRecord {

        private readonly List<string> fields = new List<string>();
        public List<string> Fields {get {return fields;}}
        public int FieldCount
        {
            get
            {
                return fields.Count;
            }
        }

    }
}
