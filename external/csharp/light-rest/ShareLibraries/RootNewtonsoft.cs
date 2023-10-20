using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// converted from Selenium 4 status JSON via json2csharp.com
// twin of ShareLibraries.Root but using NewtonSoft annotations
// to flexible property names

namespace ShareLibrariesNewtonsoft
{
	public class Root{
		// NOTE: Attribute 'JsonProperty' is not valid on 'class' declaration type.
		// It is only valid on 'property, indexer, field, param' declarations.
		[JsonProperty("value")]
		public Value value { get; set; }
	}

	public class Value {
		[JsonProperty("ready")]
		public bool ready { get; set; }
		[JsonProperty("message")]
		public string message { get; set; }
		[JsonProperty("nodes")]
		public List<Node> nodes { get; set; }
	}

	public class Id {
		[JsonProperty("hostId")]
		public string hostId { get; set; }
		[JsonProperty("id")]
		public string id { get; set; }
	}

	public class Node {
		[JsonProperty("id")]
		public string id { get; set; }
		[JsonProperty("uri")]
		public string uri { get; set; }
		[JsonProperty("maxSessions")]
		public float maxSessions { get; set; }
		[JsonProperty("osInfo")]
		public OsInfo osInfo { get; set; }
		[JsonProperty("heartbeatPeriod")]
		public float heartbeatPeriod { get; set; }
		[JsonProperty("availability")]
		public string availability { get; set; }
		[JsonProperty("version")]
		public string version { get; set; }
		[JsonProperty("slots")]
		public List<Slot> slots { get; set; }
	}

	public class OsInfo {
		[JsonProperty("arch")]
		public string arch { get; set; }
		[JsonProperty("name")]
		public string name { get; set; }
		[JsonProperty("version")]
		public string version { get; set; }
	}


	public class Slot
	{
		[JsonProperty("lastStarted")]
		public DateTime lastStarted { get; set; }
		[JsonProperty("session")]
		public string session { get; set; }
		[JsonProperty("id")]
		public Id id { get; set; }
		[JsonProperty("stereotype")]
		public Stereotype stereotype { get; set; }
	}

	public class Stereotype
	{
		public string browserName { get; set; }
		[JsonProperty("platformName")]
		public string platformName { get; set; }
	}
	
}
