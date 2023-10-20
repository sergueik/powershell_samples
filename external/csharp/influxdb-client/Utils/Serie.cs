using System.Collections.Generic;
using Newtonsoft.Json;

namespace Utils
{
    public class Serie
    {
        public Serie()
        {
            this.Points = new List<object[]>();
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "columns")]
        public string[] ColumnNames { get; set; }

        [JsonProperty(PropertyName = "points")]
        public List<object[]> Points { get; set; }
    }
}