using System;
using System.Collections.Generic;

// converted from Selenium 4 status JSON via json2csharp.com
namespace ShareLibraries {
	public class Root {
		public Value value { get; set; }
	}

	public class Value {
		public bool ready { get; set; }
		public string message { get; set; }
		public List<Node> nodes { get; set; }
	}
	public class Id {
		public string hostId { get; set; }
		public string id { get; set; }
	}

	public class Node {
		public string id { get; set; }
		public string uri { get; set; }
		public float maxSessions { get; set; }
		public OsInfo osInfo { get; set; }
		public float heartbeatPeriod { get; set; }
		public string availability { get; set; }
		public string version { get; set; }
		public List<Slot> slots { get; set; }
	}

	public class OsInfo {
		public string arch { get; set; }
		public string name { get; set; }
		public string version { get; set; }
	}


	public class Slot {
		public DateTime lastStarted { get; set; }
		public string session { get; set; }
		public Id id { get; set; }
		public Stereotype stereotype { get; set; }
	}

	public class Stereotype {
		public string browserName { get; set; }
		public string platformName { get; set; }
	}
	
}
