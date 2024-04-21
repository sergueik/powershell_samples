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

			// https://stackoverflow.com/questions/25725151/write-to-windows-application-event-log-without-event-source-registration		
			// when usin static methods will end up writing to Application Event Log
			// EventLog.WriteEvent(eventLogSource, new EventInstance(2, 1, EventLogEntryType.Error), "there was an error logged" );
			// EventLog.WriteEvent(eventLogSource, new EventInstance(1, 1, EventLogEntryType.Information), "some information was logged");
			
			var eventLog = new EventLog(eventLogName);
			eventLog.Source = eventLogSource;
			// NOTE: 
			// Unhandled Exception: System.ArgumentException: The source 'MyCustomEventSource' is not registered in log 'mycustomlog2'. (It is registered in log 'Application'.) " 
				// The Source and Log properties must be matched, or you may set Log to the empty string, and it will automatically be matched to the Source property.
//   at System.Diagnostics.EventLogInternal.VerifyAndCreateSource(String sourceName, String currentMachineName)
//   at System.Diagnostics.EventLogInternal.WriteEntry(String message, EventLogEntryType type, Int32 eventID, Int16 category, Byte[] rawData)
//   at System.Diagnostics.EventLog.WriteEntry(String message)
			eventLog.WriteEntry("Test log message");
			// NOTE: Member 'System.Diagnostics.EventLog.WriteEvent(string, System.Diagnostics.EventInstance, params object[])' cannot be accessed with an instance reference; qualify it with a type name instead
			// eventLog.WriteEvent(eventLogSource, new EventInstance(2, 1, EventLogEntryType.Error), "there was an error logged" );
			// eventLog.WriteEvent(eventLogSource, new EventInstance(1, 1, EventLogEntryType.Information), "some information was logged");
		}
	}

}
