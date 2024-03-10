using System;
using System.Diagnostics;

namespace EventSourceTestApp {
	class Program {
		static void Main(string[] args) {
			EventLog.WriteEvent("MyCustomEventSource", new EventInstance(100, 1, EventLogEntryType.Error));
			EventLog.WriteEvent("MyCustomEventSource", new EventInstance(101, 1, EventLogEntryType.Information), "75 percent");
		}
	}

}
