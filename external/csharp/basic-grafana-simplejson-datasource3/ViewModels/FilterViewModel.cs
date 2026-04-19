using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrafanaGenericSimpleJsonDataSource.ViewModels
{
    public class FilterViewModel
    {

        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("operator")]
        public string Operator { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

    }
}
