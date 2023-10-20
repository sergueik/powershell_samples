using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.ServiceProcess;
using System.Threading;
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

		private Boolean autorun = false;
		private Boolean debug;
		private Boolean noop;
		private Container components = null;
		private string resultFile = @"c:\temp\loadaverage.txt";
		private EventLog myLog;
		private String result;
		// see also ProjectInstaller
		private string serviceName = "LoadAverageService";
		private int autoAverageInterval = 60000;
		private int collectInterval = 1000;
		private Random rand = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

		private System.Timers.Timer Timer1;
		private System.Timers.Timer Timer2;
		private static int capacity = 900;
		// 15 minute worth of data
		private CircularBuffer<Data> buffer;
		private NameValueCollection appSettings;

		static void Main() {
			System.ServiceProcess.ServiceBase[] ServicesToRun;
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new DBWriter() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		public DBWriter() {
			appSettings = ConfigurationManager.AppSettings;
			buffer = new CircularBuffer<Data>(capacity);
			resultFile = (appSettings.AllKeys.Contains("Datafile")) ? appSettings["Datafile"] : @"c:\temp\loadaverage.txt";

			AutoLog = false;

			// TODO: read DBWriter.resx <data name="$this.Name"> programmaticallly
			// watch the case
			this.ServiceName = serviceName;

			myLog = new EventLog();
			// admin-only
//      if(! EventLog.Exists("LoadAverageCounterServiceLog"))
//      {
//        EventLog.CreateEventSource("TransactionService","LoadAverageCounterServiceLog");
//      }
			myLog.Log = "LoadAverageCounterServiceLog";
			myLog.Source = "LoadAverageCounterService";
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

			if (appSettings.AllKeys.Contains("CollectInterval")) {
				collectInterval = int.Parse(appSettings["CollectInterval"]);
			}

			if (appSettings.AllKeys.Contains("AutoAverageInterval")) {
				autoAverageInterval = int.Parse(appSettings["autoAverageInterval"]);
			}

		    Timer1 = new System.Timers.Timer();
			Timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			Timer1.Interval = collectInterval;
			Timer1.Enabled = true;
			Timer1.Start();
		    Timer2 = new System.Timers.Timer();
			Timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			Timer2.Interval = autoAverageInterval;
			Timer2.Enabled = true;
			Timer2.Start();
			// https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.eventlog.writeentry?view=netframework-4.5.1
			myLog.WriteEntry(String.Format("{0} Service Started Successfully on {1}. " + "\r\n" + "Collection Inteval: {2}. AutoAverage Interval: {3}" + "\r\n" + "DEBUG: {4}" + "\r\n" + "NOOP: {5}" + "\r\n" + "AUTORUN: {6}", this.ServiceName, System.DateTime.Now.ToString(), collectInterval, autoAverageInterval, debug, noop, autorun), EventLogEntryType.Information);
		}

		private void CollectMetrics() {
			int value = 0;
			var row = new Data();
			row.TimeStamp = DateTime.Now;
			if (noop) {
				if (debug)
					myLog.WriteEntry("Collecting mockup data", EventLogEntryType.Information,
						(int)Message.Collect, 0);
				value = rand.Next(0, 10);
			} else {
				try {
					var performanceCounter = new PerformanceCounter();
					performanceCounter.CategoryName = "System";
					performanceCounter.CounterName = "Processor Queue Length";
					performanceCounter.InstanceName = null;
					value = (Int32)performanceCounter.RawValue;
				} catch (InvalidOperationException e) {
					myLog.WriteEntry("Exception reading \"Processor Queue Length\": " + e.ToString(), EventLogEntryType.Information, (int)Message.Problem, 0);
					return;
				}
			}
			row.Value = value;
			buffer.AddLast(row);
			if (debug)
				myLog.WriteEntry("Collected data: " + row.ToString() + ((noop) ? "\r\n" + "NOOP: "  + noop : ""), EventLogEntryType.Information,
					(int)Message.Collect, 0);
		}


		private void OnElapsedTimer1(object source, ElapsedEventArgs args){
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
			myLog.WriteEntry(String.Format("{0} from {1} samples", message, values.Count()), EventLogEntryType.Information, (int)Message.Average, 0);

			return average;
		}

		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
			if (autorun) {
				AverageData();
			} else {
				if (debug)
					myLog.WriteEntry("Skipped processing data ", EventLogEntryType.Information,
						(int)Message.Average, 0);
			}
		}

		protected override void OnStop() {
			Timer1.Stop();
			Timer1.Enabled = false;
			Timer2.Stop();
			Timer2.Enabled = false;
			myLog.WriteEntry("Service Stopped Successfully", EventLogEntryType.Information, (int)Message.Info, 0);
		}


		private void Commit() {
			try {
				var fileHelper = new Utils.FileHelper();
				int retries = 2;
				fileHelper.Retries = retries;
				fileHelper.FilePath = resultFile;
				fileHelper.Interval = 500;
				fileHelper.Text = result;
				fileHelper.WriteContents();
				myLog.WriteEntry(String.Format("{0} FiileHelper {1}", this.ServiceName, resultFile), EventLogEntryType.Information, (int)Message.Commit, 0);
			} catch (Exception e) {
				// Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
				myLog.WriteEntry(String.Format("Exception writing \"{0}\": ", resultFile) + e.ToString(), EventLogEntryType.Information, (int)Message.Problem, 0);
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
