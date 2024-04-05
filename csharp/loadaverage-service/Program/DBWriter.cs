using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System;

using Utils;

namespace TransactionService {
	public class DBWriter : ServiceBase {

		public enum Message {
			Info = 1,
			Collect = 2,
			Average = 3,
			Commit = 4,
			Problem = 99
		}

		// NOTE: using Java style variable naming
		private Boolean autorun = false;
		private Boolean debug;
		private Boolean noop;
		private Container components = null;
		private string resultFile = @"c:\temp\loadaverage.txt";
		private EventLog eventLog;
		private String result;
		// see also ProjectInstaller
		private string serviceName = "LoadAverageService";
		private string eventLogName = "LoadAverageService";
		private string eventLogSource = "LoadAverageCounterService";

		private int autoAverageInterval = 60000;
		private int collectInterval = 1000;
		private Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;
		private int retries = 2;
		private static int capacity = 900;
		// 15 minute worth of data
		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;
		
		// NOTE: the default value os 
		// categoryNAme, counterName and instanceName about to be overwrittedn my app config values
		private string categoryName = "Processor";
		private string counterName = "% Processor Time";
		private string instanceName = "0";

		// NOTE: on a single CPU host the counter "% Total Processor Time" 
		// will be not avaiable in the category "System"
		
		static void Main() {
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new DBWriter() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		public DBWriter() {
			buffer = new CircularBuffer<Data>(capacity);
			AutoLog = false;

			// TODO: read DBWriter.resx <data name="$this.Name"> programmaticallly
			// watch the case
			this.ServiceName = serviceName;

			eventLog = new EventLog();
			// admin-only
//      if(! EventLog.Exists("LoadAverageCounterServiceLog"))
//      {
//        EventLog.CreateEventSource("TransactionService","LoadAverageCounterServiceLog");
//      }
			eventLog.Log = "LoadAverageCounterServiceLog";
			eventLog.Source = "LoadAverageCounterService";
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
			if (appSettings.AllKeys.Contains("Autorun")) {
				autorun = Boolean.Parse(appSettings["Autorun"]);
			}
			if (appSettings.AllKeys.Contains("Noop")) {
				noop = Boolean.Parse(appSettings["Noop"]);
			}

			if (appSettings.AllKeys.Contains("Retries")) {
				retries = int.Parse(appSettings["Retries"]);
			}
			
			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}

			if (appSettings.AllKeys.Contains("AutoAverageInterval")) {
				autoAverageInterval = int.Parse(appSettings["autoAverageInterval"]);
			}

			if (appSettings.AllKeys.Contains("EventLogName")) {
				eventLogName = appSettings["EventLogName"];
			}
			if (appSettings.AllKeys.Contains("CategoryName")) {
				categoryName = appSettings["CategoryName"];
			}
			if (appSettings.AllKeys.Contains("CounterName")) {
				counterName = appSettings["CounterName"];
			}
			if (appSettings.AllKeys.Contains("InstanceName")) {
				instanceName = appSettings["InstanceName"];
			}

			if (appSettings.AllKeys.Contains("EventLogSource")) {
				eventLogSource = appSettings["EventLogSource"];
			}

			resultFile = (appSettings.AllKeys.Contains("Datafile")) ? appSettings["Datafile"] : @"c:\temp\loadaverage.txt";

			eventLog.Log = eventLogName;
			eventLog.Source = eventLogSource;


			timer1 = new System.Timers.Timer();
			timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			timer1.Interval = collectInterval;
			timer1.Enabled = true;
			timer1.Start();
			timer2 = new System.Timers.Timer();
			timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			timer2.Interval = autoAverageInterval;
			timer2.Enabled = true;
			timer2.Start();
			// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog.writeentry?view=netframework-4.5.1
			eventLog.WriteEntry(String.Format("{0} Service Started Successfully on {1}. " + "\r\n" + "Collection Inteval: {2}. AutoAverage Interval: {3}" + "\r\n" + "DEBUG: {4}" + "\r\n" + "NOOP: {5}" + "\r\n" + "AUTORUN: {6}" + "\r\n" + "CATEGORY: {7}" + "\r\n" + "COUNTER: {8}"+ "\r\n" + "INSTANCE: {9}" , this.ServiceName, System.DateTime.Now.ToString(), collectInterval, autoAverageInterval, debug, noop, autorun, this.categoryName, this.counterName, instanceName ), EventLogEntryType.Information);
		}

		private void CollectMetrics() {
			int value = 0;
			var row = new Data();
			row.TimeStamp = DateTime.Now;
			if (noop) {
				if (debug)
					eventLog.WriteEntry("Collecting mockup data", EventLogEntryType.Information,
						(int)Message.Collect, 0);
				value = rand.Next(0, 10);
			} else {
				try {
					// https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
					var performanceCounter = new PerformanceCounter();
					performanceCounter.CategoryName = this.categoryName;
					performanceCounter.CounterName = this.counterName;
					performanceCounter.InstanceName = instanceName == "" ? null : instanceName;
					value = (Int32)performanceCounter.RawValue;
				} catch (InvalidOperationException e) {
					eventLog.WriteEntry(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()), EventLogEntryType.Information, (int)Message.Problem, 0);
					return;
				}
			}
			row.Value = value;
			buffer.AddLast(row);
			if (debug)
				eventLog.WriteEntry("Collected data: " + row.ToString() + ((noop) ? "\r\n" + "NOOP: " + noop : ""), EventLogEntryType.Information,
					(int)Message.Collect, 0);
		}


		private void OnElapsedTimer1(object source, ElapsedEventArgs args) {
			CollectMetrics();
		}

		private void AverageData() {
			var intervals = new int[] { 1, 5, 15 };
			var messages = new List<string>();
			foreach (int minutes in intervals) {
				double average = AverageDataInterval(minutes);
				var message = String.Format("LOAD{0}MIN: {1, 4:f2}", minutes, average);
				messages.Add(message);
			}
			result = String.Join("\n", messages);
		}


		private double AverageDataInterval(int minutes) {
			var rows = buffer.ToList();
			float interval = 1F * minutes;
			var now = DateTime.Now;
			var values = (from row in rows
			              where ((now - row.TimeStamp).TotalMinutes) <= interval
			              select row.Value);
			var average = values.Average();
			var message = String.Format("LOAD{0, 1:f0}MIN: {1, 4:f2}", interval, average);
			eventLog.WriteEntry(String.Format("{0} from {1} samples", message, values.Count()), EventLogEntryType.Information, (int)Message.Average, 0);

			return average;
		}

		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
			if (autorun) {
				AverageData();
			} else {
				if (debug)
					eventLog.WriteEntry("Skipped processing data ", EventLogEntryType.Information,
						(int)Message.Average, 0);
			}
		}

		protected override void OnStop() {
			timer1.Stop();
			timer1.Enabled = false;
			timer2.Stop();
			timer2.Enabled = false;
			eventLog.WriteEntry("Service Stopped Successfully", EventLogEntryType.Information, (int)Message.Info, 0);
		}


		private void Commit() {
			try {
				var fileHelper = new Utils.FileHelper();
				
				fileHelper.Retries = retries;
				fileHelper.FilePath = resultFile;
				fileHelper.Interval = 500;
				fileHelper.Text = result;
				fileHelper.WriteContents();
				eventLog.WriteEntry(String.Format("{0} FiileHelper {1}", this.ServiceName, resultFile), EventLogEntryType.Information, (int)Message.Commit, 0);
			} catch (Exception e) {
				// Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
				eventLog.WriteEntry(String.Format("Exception writing \"{0}\": ", resultFile) + e.ToString(), EventLogEntryType.Information, (int)Message.Problem, 0);
			}
		}

		protected override void OnCustomCommand(int command) {
			base.OnCustomCommand(command);
			switch (command) {

				case(200):
					AverageData();
					Commit();
					break;
				default:
					break;


			}
		}
	}
}
