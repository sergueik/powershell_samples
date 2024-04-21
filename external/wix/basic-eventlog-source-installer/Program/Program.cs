using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Configuration;

namespace EventSourceTestApp  {

	class Program {
		private static string eventLogName = "log4jna";
		private static string eventLogSource = "example.log4jna";
		private static NameValueCollection appSettings;
		private static Boolean debug;
		static void Main(string[] args) {
			appSettings = ConfigurationManager.AppSettings;
			
			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}
			if (appSettings.AllKeys.Contains("EventLogName")) {
				eventLogName = appSettings["EventLogName"];
			}
			if (appSettings.AllKeys.Contains("EventLogSource")) {
				eventLogSource = appSettings["EventLogSource"];
			}

			EventLog.WriteEvent(eventLogSource, new EventInstance(2, 1, EventLogEntryType.Error), "there was an error logged" );
			EventLog.WriteEvent(eventLogSource, new EventInstance(1, 1, EventLogEntryType.Information), "some information was logged");
		}
	}

}
