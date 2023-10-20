using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Zabbix
{
	public class SenderResponse
	{
		[JsonProperty(PropertyName = "response")]
		public string Response { get; set; }

		[JsonProperty(PropertyName = "info")]
		public string Info { get; set; }
		public bool Success { get { return Response == "success"; }}

		public int Processed { 
			get {
				return Convert.ToInt32(
					GetValueFromInfo("processed")
				);
			 }
		}

		public int Failed { 
			get {
				return Convert.ToInt32(
					GetValueFromInfo("failed")
				);
			 }
		}

		public int Total { 
			get {
				return Convert.ToInt32(
					GetValueFromInfo("total")
				);
			 }
		}

		public double SecondsSpent { 
			get {
				return Convert.ToDouble(
					GetValueFromInfo("seconds_spent"),
					System.Globalization.CultureInfo.InvariantCulture
				);
			 }
		}

		private string pattern = 
		@"processed: (?<processed>\d+); failed: (?<failed>\d+); total: (?<total>\d+); seconds spent: (?<seconds_spent>\d\.\d+)";

		private string GetValueFromInfo(string name)
		{
			var reg = new Regex( pattern );
			var match = reg.Match(Info);
			
			return match.Groups[name].Value;
		}
	}
}
