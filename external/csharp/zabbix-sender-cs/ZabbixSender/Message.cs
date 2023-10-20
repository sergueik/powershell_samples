using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Zabbix
{
	sealed public class Message
	{
		[JsonProperty(PropertyName = "request")]
		private readonly string request = "sender data"; 

		[JsonProperty(PropertyName = "data")] 
		private readonly List<SendValue> data;

		public Message(SendValue sendvalue)
		{
			data = new List<SendValue> { sendvalue };
		}
		public Message(IEnumerable<SendValue> sendvalues)
		{
			data = new List<SendValue>();
			data.AddRange(sendvalues);
		}
	}
}
