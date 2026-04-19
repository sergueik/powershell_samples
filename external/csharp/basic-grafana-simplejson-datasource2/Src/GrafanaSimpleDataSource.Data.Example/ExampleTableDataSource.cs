using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrafanaSimpleDataSource.Data.Example
{
    public class ExampleTableDataSource : IDataSource
    {
        public string Metric => "Fruit Sales";

        public async Task<IDataResponse> GetDataAsync(TimeSeriesRequest request)
        {
            TableResponse response = new TableResponse( new List<TableResponse.Column>() {
                new TableResponse.Column() { Text = "Fruit", Type = "text" },
                new TableResponse.Column() { Text = "Number Sold", Type = "number" }
            });

            response.Rows.Add(new List<object>() { "Bananas", 10 });
            response.Rows.Add(new List<object>() { "Apples", 13 });
            response.Rows.Add(new List<object>() { "Tomatoes", 16 });
            response.Rows.Add(new List<object>() { "Peaches", 4 });
            response.Rows.Add(new List<object>() { "Grapes", 8 });

            response.Sort(1, TableResponse.SortDirection.Descending);

            return await Task.FromResult(response);
        }
    }
}
