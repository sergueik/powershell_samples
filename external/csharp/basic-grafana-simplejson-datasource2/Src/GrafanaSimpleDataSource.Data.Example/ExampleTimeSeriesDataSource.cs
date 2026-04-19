using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Techtoniq.Core.Lib.Temporal;

namespace GrafanaSimpleDataSource.Data.Example
{
    public class ExampleTimeSeriesDataSource : IDataSource
    {
        public string Metric => "CPU Usage";

        public async Task<IDataResponse> GetDataAsync(TimeSeriesRequest request)
        {
            var unix = DateTime.UtcNow.ToUnixTimestamp();


            TimeSeriesResponse response = new TimeSeriesResponse(Metric, new List<List<object>>() {
                 new List<object> { 622, (long)DateTime.UtcNow.ToUnixTimestamp() * 1000 },
                 new List<object> { 321, (long)DateTime.UtcNow.AddHours(-1).ToUnixTimestamp()  * 1000},
                 new List<object> { 342, (long)DateTime.UtcNow.AddHours(-2).ToUnixTimestamp()  * 1000},
                 new List<object> { 666, (long)DateTime.UtcNow.AddHours(-3).ToUnixTimestamp()  * 1000},
                 new List<object> { 775, (long)DateTime.UtcNow.AddHours(-4).ToUnixTimestamp()  * 1000},
                 new List<object> { 999, (long)DateTime.UtcNow.AddHours(-5).ToUnixTimestamp()  * 1000},
            });

            return await Task.FromResult(response);
        }
    }
}
