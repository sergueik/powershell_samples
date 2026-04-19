using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GrafanaSimpleDataSource.Data
{
    public class TableResponse : IDataResponse
    {
        [JsonProperty("type")]
        public string Type => "table";

        public class Column
        {
            [JsonProperty("text")]
            public string Text { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
        }

        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }


        [JsonProperty("rows")]
        public List<List<object>> Rows { get; set; }



        public TableResponse()
        {
            Columns = new List<Column>();
            Rows = new List<List<object>>();
        }

        public TableResponse(List<Column> columns)
            :this()
        {
            if (null == columns)
                return;

            Columns = columns;
        }


        public enum SortDirection
        {
            Ascending,
            Descending
        }

        public void Sort(int columnIndex, SortDirection sortDirection)
        {
            if (null == Rows || !Rows.Any()) 
                return;
            if (columnIndex < 0 || columnIndex >= Columns.Count)
                return;

            if(sortDirection == SortDirection.Ascending)
            {
                Rows = Rows.OrderBy(r => r[columnIndex]).ToList();
            }
            else if(sortDirection == SortDirection.Descending)
            {
                Rows = Rows.OrderByDescending(r => r[columnIndex]).ToList();
            }
        }
    }
}
