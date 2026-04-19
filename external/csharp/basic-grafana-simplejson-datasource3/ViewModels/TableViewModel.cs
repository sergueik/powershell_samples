using GrafanaGenericSimpleJsonDataSource.Data;
using GrafanaGenericSimpleJsonDataSource.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
    public class TableViewModel : IResponseViewModel
    {
        public TableViewModel(IRepository repository)
        {
            List<Column> cols = new List<Column>();
            foreach (var item in repository.GetColumns())
            {
                cols.Add(new Column(item.Key, item.Value));
            }
            ColumnList = cols.ToArray();

            var dataPoints = new List<CsvData>();

            dataPoints.AddRange((repository as CsvRepository).GetAll());
            DataPoints = dataPoints.ToArray();

        }

        [JsonProperty("columns")]
        public Column[] ColumnList { get; set; }

        [JsonProperty("rows")]
        public CsvData[] DataPoints { get; set; }

        [JsonProperty("type")]
        public string Type => "table";

    }
}
