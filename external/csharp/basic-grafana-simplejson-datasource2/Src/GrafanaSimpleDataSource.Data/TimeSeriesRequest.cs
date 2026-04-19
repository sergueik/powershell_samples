using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GrafanaSimpleDataSource.Data
{
    public class TimeSeriesRequest
    {
        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// Unique panel identifier
        /// </summary>
        /// <example>1</example>
        [JsonProperty("panelId")]
        public long PanelId { get; set; }

        [JsonProperty("dashboardId")]
        public int DashboardId { get; set; }

        [JsonProperty("range")]
        public TimeRange Range { get; set; }

        [JsonProperty("timeInfo")]
        public string TimeInfo { get; set; }

        [JsonProperty("interval")]
        public string Interval { get; set; }

        [JsonProperty("intervalMs")]
        public long IntervalMs { get; set; }

        [JsonProperty("targets")]
        public IEnumerable<Target> Targets { get; set; }


        [JsonProperty("maxDataPoints")]
        public int MaxDataPoints { get; set; }


        [JsonProperty("scopedVars")]
        public Dictionary<string, Dictionary<string, string>> ScopedVars { get; set; }


        [JsonProperty("startTime")]
        public long StartTime { get; set; }



        [JsonProperty("rangeRaw")]
        public TimeRangeText RawRange { get; set; }


        [JsonProperty("adhocFilters")]
        public IEnumerable<AdhocFilter> AdhocFilters { get; set; }




        public class TimeRange
        {
            [JsonProperty("from")]
            public DateTime From { get; set; }

            [JsonProperty("to")]
            public DateTime To { get; set; }

            [JsonProperty("raw")]
            public TimeRangeText Raw { get; set; }
        }



        public class TimeRangeText
        {
            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }
        }



        public class Target
        {
            [JsonProperty("target")]
            public string Metric { get; set; }

            [JsonProperty("refId")]
            public string RefId { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }



        public class AdhocFilter
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("operator")]
            public string Operator { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
