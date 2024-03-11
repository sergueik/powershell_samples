using System;
using System.Diagnostics;

namespace EventSourceTestApp  {
	class Program {
		static void Main(string[] args) {
			EventLog.WriteEvent("MyCustomEventSource", new EventInstance(2, 1, EventLogEntryType.Error), "there was an error logged" );
			EventLog.WriteEvent("MyCustomEventSource", new EventInstance(1, 1, EventLogEntryType.Information), "some information was logged");
		}
	}

}
