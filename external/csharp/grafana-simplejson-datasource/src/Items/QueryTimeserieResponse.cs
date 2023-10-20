using System.Collections.Generic;

namespace Grafana.SimpleJson.Example.Items
{
    public class QueryTimeserieResponse
    {
        public string Target { get; set; }
        public List<List<double>> Datapoints { get; set; }
    }
}
