using System;
using System.Diagnostics;
using System.ComponentModel;

// based on: code from https://www.codeproject.com/Articles/2668/Simple-EventLog
// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog.writeentry?view=netframework-4.5
namespace Utils
{
	public class SimpleEventLog : EventLog
	{
	  
		private string strError;
		private bool bError = false;

		public string Error {
			get {
				return strError;
			}
			set {
				strError = value;
			}
		}
		public bool IsError {
			get {
				return bError;
			}
			set {
				bError = value;
			}
		}

		// TODO: provi construcror with argument for soirce
		public SimpleEventLog(string eventLogName)
		{
			if (EventLog.SourceExists(eventLogName) == false) {
				// create the event log on the current computer
				try {
					EventLog.CreateEventSource(eventLogName, eventLogName);
				} catch (ArgumentException) {
					IsError = true;
					Error = "The string passed to CreateEventSource is null";
				} catch (Exception) {
					IsError = true;
					Error = "The system could not open the registry key for " + eventLogName;
				}
			} else {
				Source = eventLogName;
			}
		}


		public SimpleEventLog(string eventLogName, bool clear)
			: this(eventLogName)
		{
			if (clear == true) {
				ClearSimpleEventLog();
			}
		}


		private bool WriteSimpleEntry(string entry, EventLogEntryType eventType)
		{
			IsError = false;
			try {
				WriteEntry(entry, eventType);
			} catch (ArgumentException argExp) {
				IsError = true;
				Error = "Argument exception is " + argExp.Message;
			} catch (InvalidOperationException) {
				IsError = true;
				Error = "You do not have write permission for the event log";
			} catch (Win32Exception winExp) {
				IsError = true;
				Error = "Win32Exception is " + winExp.Message;
			} catch (SystemException) {
				IsError = true;
				Error = "The event log could not be notified to start recieing events";
			} catch (Exception) {
				IsError = true;
				Error = "The registry entry for the log could not be opened on the remote computer";
			}


			if (IsError == true)
				return false;
			else
				return true;
		}



		public bool WriteInformation(string information)
		{
			return WriteSimpleEntry(information, EventLogEntryType.Information);
		}

		public bool WriteWarning(string warning)
		{
			return WriteSimpleEntry(warning, EventLogEntryType.Warning);
		}

		public bool WriteError(string error)
		{
			return WriteSimpleEntry(error, EventLogEntryType.Error);
		}


		public bool ClearSimpleEventLog()
		{
			IsError = false;
			try {
				Clear();
			} catch (Win32Exception winExp) {
				IsError = true;
				Error = "Win 32 Exception " + winExp.Message;
			} catch (ArgumentException) {
				IsError = true;
				Error = "The log name is empty";
			} catch (Exception) {
				IsError = true;
				Error = "The log could not be opened";
			}

			if (IsError == true)
				return false;
			else
				return true;
		}


		public bool DeleteSimpleEventLog()
		{
			IsError = false;

			try {
				EventLog.Delete(Source);
			} catch (ArgumentException) {
				IsError = true;
				Error = "Event Log Name is null";
			} catch (SystemException sysExp) {
				IsError = true;
				Error = "System Exception " + sysExp;
			}

			if (IsError == true)
				return false;
			else
				return true;
		}
	}
}
