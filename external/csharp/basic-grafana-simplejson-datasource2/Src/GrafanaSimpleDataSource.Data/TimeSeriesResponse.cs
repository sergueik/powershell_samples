using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GrafanaSimpleDataSource.Data
{
    public class TimeSeriesResponse : IDataResponse
    {
        [JsonProperty("target")]
        public string Metric { get; set; }

        [JsonProperty("datapoints")]
        public List<List<object>> DataPoints { get; set; }  // List<object> = { Metric value as a float , unixtimestamp in milliseconds }

        public TimeSeriesResponse()
            :this(null, new List<List<object>>())
        {
        }

        public TimeSeriesResponse(string metric, List<List<object>> dataPoints)
        {
            Metric = metric;

            if (null != dataPoints)
                DataPoints = dataPoints;
        }
    }
}
