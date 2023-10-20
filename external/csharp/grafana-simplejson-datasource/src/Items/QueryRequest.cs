using System;
using System.Collections.Generic;

namespace Grafana.SimpleJson.Example.Items
{

    /* Request JSON Example             
     {
          "panelId": 1,
          "range": {
            "from": "2019-10-31T06:33:44.866Z",
            "to": "2019-10-31T12:33:44.866Z",
            "raw": {
              "from": "now-6h",
              "to": "now"
            }
          },
          "rangeRaw": {
            "from": "now-6h",
            "to": "now"
          },
          "interval": "30s",
          "intervalMs": 30000,
          "targets": [
             { "target": "upper_50", "refId": "A", "type": "timeserie" },
             { "target": "upper_75", "refId": "B", "type": "timeserie" }
          ],
          "adhocFilters": [{
            "key": "City",
            "operator": "=",
            "value": "Berlin"
          }],
          "format": "json",
          "maxDataPoints": 550
    }*/

    public class QueryRequest
    {
        public int PanelId { get; set; }
        public Range Range { get; set; }
        public RangeRaw RangeRaw { get; set; }
        public string Interval { get; set; }
        public int IntervalMs { get; set; }
        public List<TargetObj> Targets { get; set; }
        public List<AdhocFilter> AdhocFilters { get; set; }
        public string Format { get; set; }
        public int MaxDataPoints { get; set; }
    }

    public class Raw
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class Range
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Raw Raw { get; set; }
    }

    public class RangeRaw
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class TargetObj
    {
        public string Target { get; set; }
        public string RefId { get; set; }
        public string Type { get; set; }
    }

    public class AdhocFilter
    {
        public string Key { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
