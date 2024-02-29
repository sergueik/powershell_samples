using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using System;

namespace MyCustomActionProject {
	public class CustomActions {
		private const string logPath = @"c:\temp";
		// private static string logPath = @"c:\Program Files";
		// the file is created if the directory is writable tothe executing user
		// if the 	path is not writbale, the install will roll back in the first run
		// net.exe user app  /add
		// use existing enent source 
		private const string  eventSource = "MyCustomEventSource1";
		[CustomAction]
		public static ActionResult MyCustomActionMethod(Session session) {
			var logFile = logPath + @"\" + "installed.txt";
			session.Log("Executing CustomAction");
			try {
				File.CreateText(logFile);
				// NOTE: the custom log has to be created before running
				// EventLog.WriteEvent("Generic Name",...)
				// leads
				// System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.Security.SecurityException: The source was not found, but some or all event logs could not be searched.  To create the source, you need permission to read all event logs to make sure that the new source name is unique.  Inaccessible logs: Security.
				//
			
				EventLog.WriteEvent(eventSource, new EventInstance(701, 1, EventLogEntryType.Information), "Written log file: " + logFile);
			} catch (UnauthorizedAccessException e) {
				EventLog.WriteEvent(eventSource, new EventInstance(800, 1, EventLogEntryType.Error), "no access to log file: " + logFile);
			} catch (ArgumentException e) {
				EventLog.WriteEvent(eventSource, new EventInstance(800, 1, EventLogEntryType.Error), "Invalid path to log file:" + logFile);
			}
			return ActionResult.Success;
		}
	}
}
