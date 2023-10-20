using System;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;

using System.Threading;
using System.Timers;
using System.ComponentModel;

using Utils;

namespace WindowsService.NET {

	public class Service1 : ServiceBase {
		
		private IContainer components = null;

		private string serviceName = null;
		private string eventLogSource = null;
		private string eventLogName = null;
		private string url = null;
		private string uploadfile = null;
		private string scriptPath = null;
		private string ScriptParameters = null;
		private string workingDirectory = null;
		private int Interval2 = 60000;
		private int Interval1 = 1000;
		private SimpleEventLog simpleEventLog;

		private CircularBuffer<Data> buffer = new CircularBuffer<Data>(60);

		System.Timers.Timer Timer1 = new System.Timers.Timer();
		System.Timers.Timer Timer2 = new System.Timers.Timer();
		// 'Timer' is an ambiguous reference between 'System.Threading.Timer' and 'System.Timers.Timer'
		ProcessStarter processStarter;

		public Service1() {
			components = new Container();
			eventLogSource =(ConfigurationManager.AppSettings.AllKeys.Contains("EventLogSource")) ?
				ConfigurationManager.AppSettings["EventLogSource"]: "TestLog";
			
			eventLogName = (ConfigurationManager.AppSettings.AllKeys.Contains("EventLogName")) ?
				ConfigurationManager.AppSettings["EventLogName"] :  "TestLog";
			// eventLog = new EventLog();
			WriteLog(String.Format("Service is creating event log {0}" , eventLogName), true, false);
			simpleEventLog = new SimpleEventLog(eventLogName);
			WriteLog(String.Format("Service has created event log {0}" , eventLogName));
			/*
			if(! EventLog.Exists(eventLogName))
			{
				// TODO: process System.ArgumentException
				// ArgumentException: source is an empty string ("") or null 
				// or
				// logName is not a valid event log name. 
				// Event log names must consist of printable characters, and cannot include the characters '*', '?', or '\'.- or - 
				// The log name specified in sourceData is not valid for user log creation. 
				// The event log names AppEvent, SysEvent, and SecEvent are reserved for system use.
				// or
				// The log name matches an existing event source name.
				// or 
				// The source name results in a registry key path longer than 254 characters.
				// or 
				//The first 8 characters of logName match the first 8 characters of an existing event log name.
				// or 
				// The source cannot be registered because it already exists on the local computer.
				//or 
				// The source name matches an existing event log name.
				EventLog.CreateEventSource(eventLogSource,eventLogName);
			}
			eventLog.Log = eventLogName;
			eventLog.Source = eventLogSource;
*/
			/*
			this.eventLog = new System.Diagnostics.EventLog();
			((System.ComponentModel.ISupportInitialize)(this.eventLog)).BeginInit();
			this.eventLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(this.eventLog_EntryWritten);
			((System.ComponentModel.ISupportInitialize)(this.eventLog)).EndInit();
			*/

			if (ConfigurationManager.AppSettings.AllKeys.Contains("ServiceName"))
				serviceName = ConfigurationManager.AppSettings["ServiceName"];
			else 
				serviceName = "WindowsService.NET";
			// NOTE: invoking the setter
			this.ServiceName = serviceName;


		}

		protected override void OnStart(string[] args) {
			WriteLog("Service has been started");
			Timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			Timer1.Interval = Interval1;
			Timer1.Enabled = true;
			Timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			Timer2.Interval = Interval2;
			Timer2.Enabled = true;
		}
		
		private void OnElapsedTimer1(object source, ElapsedEventArgs args) {
				
				try {
					var performanceCounter = new PerformanceCounter();
					performanceCounter.CategoryName = "System";
					performanceCounter.CounterName = "Processor Queue Length";
					performanceCounter.InstanceName = null;
					var value = (Int32)performanceCounter.RawValue;
					var row = new Data();
					row.TimeStamp = DateTime.Now;
					row.Value = value;
					buffer.AddLast(row);
					WriteLog("procesing Timer1" + row.ToString());
				}  catch (System.InvalidOperationException e) {
   					// Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
   					WriteLog("procesing Timer1 Exception: "  + e.ToString());
   				}

		}

		private void AverageData() {
			var rows = buffer.ToList();
			var values = (from row in rows select row.Value);
			var result = values.Average();
			WriteLog("Average: " + result);
		}
		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
			AverageData();
		}

		private void OnElapsedTimerUnused(object source, ElapsedEventArgs args) {
				
			WriteLog(String.Format("{0} ms elapsed.", Interval2));
			var cookies = new CookieContainer();
			var querystring = new NameValueCollection();
			querystring["operation"] = "send";
			querystring["param"] = "something";			
			
			url = (ConfigurationManager.AppSettings.AllKeys.Contains("Url")) ? ConfigurationManager.AppSettings["Url"] : "http://localhost:8085/upload";
			uploadfile = (ConfigurationManager.AppSettings.AllKeys.Contains("Datafile")) ? ConfigurationManager.AppSettings["Datafile"] : @"c:\temp\data.txt";

			Uploader.UploadFile(uploadfile, url, "file", "text/plain",	querystring, cookies);
			WriteLog(String.Format("Uploaded file: {0} to {1}", uploadfile, url));
			var fileHelper = new Utils.FileHelper();
			int retries = 2;
			fileHelper.Retries = retries;
			fileHelper.FilePath = uploadfile;
			fileHelper.ReadContents();
			WriteLog(String.Format("Read file {0}", uploadfile));
			var text = fileHelper.Text;
			WriteLog(String.Format("Read text {0}", text));
			var helper = new UpdateDataHelper();
			helper.Text = text;
			var newdata = new Dictionary<string,string>();
			WMIDataCollector.CollectData(newdata);

			newdata["Line1"] = "ONE";
			newdata["Line5"] = "five";
			helper.UpdateData(newdata);
			text = helper.Text;
			WriteLog(String.Format("Save text {0}", text));
			fileHelper.Text = text;
			fileHelper.WriteContents();

			scriptPath = (ConfigurationManager.AppSettings.AllKeys.Contains("ScriptPath")) ? ConfigurationManager.AppSettings["ScriptPath"] : "test.ps1";
			if (ConfigurationManager.AppSettings.AllKeys.Contains("Interval"))
				Interval2 = Int32.Parse(ConfigurationManager.AppSettings["Interval"]);
			
			
			if (ConfigurationManager.AppSettings.AllKeys.Contains("ScriptParameters"))
				ScriptParameters = ConfigurationManager.AppSettings["ScriptParameters"];
			else
				ScriptParameters = @"-outputfile c:\temp\b.log";
			
			// https://stackoverflow.com/questions/3295293/how-to-check-if-an-appsettings-key-exists
			workingDirectory = (ConfigurationManager.AppSettings.AllKeys.Contains("WorkingDirectory")) ? ConfigurationManager.AppSettings["WorkingDirectory"] : AppDomain.CurrentDomain.BaseDirectory;
			
			processStarter = new ProcessStarter();
			processStarter.ScriptPath = workingDirectory + scriptPath;
			// NOTE
			WriteLog(String.Format("powershell process script: {0} parameters: {1} working directory: {2}", scriptPath, ScriptParameters, workingDirectory));
			processStarter.Start(workingDirectory + scriptPath, ScriptParameters, workingDirectory);
			// TODO: configurable timeout
			Thread.Sleep(1500);
			WriteLog(String.Format(
				"powershell process output: {0}", processStarter.ProcessOutput + ((processStarter.ProcessError.Length > 0) ? " error: " + processStarter.ProcessError : "")));
			
			if (processStarter.ProcessError.Length > 0)
				WriteLog(String.Format(
					"powershell process error: " + processStarter.ProcessError));
			var Results = new Collection<PSObject>();			
			PowerShell ps = PowerShell.Create();
			// var script = "Get-ChildItem -path $env:TEMP | Measure-Object -Property length -Minimum -Maximum -Sum -Average";
			var script = "get-computerinfo";
			WriteLog(String.Format("running script: {0}", script));
			try {
				if (Utils.PowershellCommandAdapter.RunPS(ps, script, out Results)) {
					WriteLog("Powershell script result:");
					for (var cnt = 0; cnt != Results.Count; cnt++) {
						var row = Results[cnt];
						var columnEnumerator = row.Properties.GetEnumerator();
						columnEnumerator.Reset();
						while (columnEnumerator.MoveNext()) {
							var column = columnEnumerator.Current;
							WriteLog(String.Format("{0}: {1}", column.Name, column.Value));
						}
					}
				} else {
					// TODO: trigger and nicely format the error
					WriteLog(String.Format("Powershell command error: " + Results[0].ToString()));
				} 
			} catch (System.Management.Automation.RuntimeException e) {
				WriteLog("Exception (ignored): " + e.ToString());
			}                                         			
		}

		protected override void OnStop() {
			if(processStarter!= null){
				WriteLog("Stopping launched process: " + processStarter.ScriptPath);
				processStarter.Stop();
			}
			Timer1.Stop();
			Timer2.Stop();
			WriteLog("Service has been stopped.");
		}

		public void WriteLog(string logMessage, bool addTimeStamp = true, bool writeEventLog = true ) {
			var path = AppDomain.CurrentDomain.BaseDirectory;
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var filePath = String.Format("{0}\\{1}_{2}.txt",
				               path,
				               ServiceName,
				               DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture)
			               );

			if (addTimeStamp)
				logMessage = String.Format("[{0}] - {1}\r\n",
					DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
					logMessage);

			File.AppendAllText(filePath, logMessage);
			if (writeEventLog)
				simpleEventLog.WriteInformation(logMessage);
		}
		
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnCustomCommand(int command) {
                       base.OnCustomCommand(command);
                       switch (command) {
                               case(100):
                       		WriteLog("processing custom command " + command);
                                       AverageData();
                                       break;
                               default:
                                       break;
                       }
               }

		// protected override void OnContinue() { }
		// private void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e) { }
	}
}
