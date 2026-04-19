using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafanaGenericSimpleJsonDataSource.Data
{
    public class Column
    {
        public Column(string text, string type)
        {
            Text = text;
            Type = type;
        }
        [JsonProperty("text")]
        public string Text { get; private set; }
        [JsonProperty("type")]
        public string Type { get; private set; }

    }
}
