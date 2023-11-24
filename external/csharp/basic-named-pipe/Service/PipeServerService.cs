using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.IO.Pipes;
using System;

using Utils;

namespace Service {

	public class PipeServerService : ServiceBase {

		public enum Message {
			Info = 1,
			Chat = 2,
			Problem = 99
		}

		private Boolean debug;
		private Container components = null;
		private string logFile;
		private readonly EventLog myLog;
		private PipeServer pipeServer;
		private const string eventLog = "PipeServerLog";
		private const string eventLogSource = "PipeServerService";
		private const string serviceName = "PipeServer";
		private NameValueCollection appSettings;
		

		private Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
		
		static void Main() {
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new PipeServerService() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		public PipeServerService() {
			AutoLog = false;

			// TODO: read DBWriter.resx <data name="$this.Name"> programmaticallly
			// watch the case
			this.ServiceName = serviceName;

			myLog = new EventLog();
			// admin-only
//      if(! EventLog.Exists(eventLog))
//      {
//        EventLog.CreateEventSource(serviceName,eventLog);
//      }
			myLog.Log = eventLog;
			myLog.Source = eventLogSource;
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}


		protected override void OnStart(string[] args) {
			appSettings = ConfigurationManager.AppSettings;
			if (appSettings.AllKeys.Contains("Debug")) {
				debug = Boolean.Parse(appSettings["Debug"]);
			}
			pipeServer = new PipeServer("demo", PipeDirection.InOut);
			pipeServer.MessageReceived += (Object s, MessageReceivedEventArgs o) => pipeServer.Send(o.Message);
			pipeServer.MessageReceived += OnMessageReceived;
			pipeServer.Start();

			var message = String.Format("Service {0} Started Successfully" + "\r\n" + "DEBUG: {1}", this.ServiceName, debug);
			var fileHelper = new Utils.FileHelper();
			int retries = 2;
			fileHelper.Retries = retries;
			logFile = (appSettings.AllKeys.Contains("Logfile")) ? appSettings["Logfile"] : @"c:\temp\service.log";
			fileHelper.FilePath = logFile;
			fileHelper.Text = message;
			fileHelper.WriteContents();

			
			// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog.writeentry?view=netframework-4.5.1
			myLog.WriteEntry(message, EventLogEntryType.Information);
		}

		private void OnMessageReceived(Object s, MessageReceivedEventArgs o) {			 
			myLog.WriteEntry(String.Format("Message received: {0}", o.Message), EventLogEntryType.Information, (int)Message.Chat, 0);
			// NOTE:  brute force restart cannot be performed from a callback
		}

		protected override void OnStop() {
			pipeServer.Stop();
			myLog.WriteEntry("Service Stopped Successfully", EventLogEntryType.Information, (int)Message.Info, 0);
			
		}
		
	}
}
