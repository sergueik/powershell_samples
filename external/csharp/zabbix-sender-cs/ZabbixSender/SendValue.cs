using System;
using Newtonsoft.Json;

namespace Zabbix
{
		public class SendValue
		{
		[JsonProperty(PropertyName = "host")]
		public string Host { get; set; }

		[JsonProperty(PropertyName = "key")]
		public string Key { get; set; }

		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }
		}
}
