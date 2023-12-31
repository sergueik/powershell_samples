﻿using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Configuration;
using System.ComponentModel;

namespace PowershellStarter {

	public class PowershellStarterService : ServiceBase {

		public string ScriptPath { get; set; }
		public string ScriptParameters { get; set; }
		public string output;
		public string errorOutput;
		public ProcessStartInfo process = new ProcessStartInfo();
		public Process PSProcess = new Process();
		private EventLog eventLog;
		private IContainer components = null;
		
		public PowershellStarterService() {
			InitializeComponent();
			// Set eventlog
			if (!EventLog.SourceExists(ConfigurationManager.AppSettings["EventLogSource"])) {
				EventLog.CreateEventSource(ConfigurationManager.AppSettings["EventLogSource"], ConfigurationManager.AppSettings["EventLog"]);
			}

			eventLog.Source = ConfigurationManager.AppSettings["EventLogSource"];
			eventLog.Log = ConfigurationManager.AppSettings["EventLog"];
		}

		// For fetching exited event from script and forcing service to terminate
		protected virtual void onScriptExited(object sender, EventArgs e){

			eventLog.WriteEntry("Script is no longer running, terminating service...");
			Environment.FailFast("Script no longer running, service stopped.");
		}

		protected override void OnStart(string[] args){

			ScriptPath = ConfigurationManager.AppSettings["ScriptPath"];
			ScriptParameters = ConfigurationManager.AppSettings["ScriptParameters"];

			// Define process startinfo

			process.CreateNoWindow = true;
			process.UseShellExecute = false;
			process.WorkingDirectory = ConfigurationManager.AppSettings["WorkingDirectory"];
			process.RedirectStandardOutput = true;
			process.RedirectStandardError = true;
			process.FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
			process.Arguments = "-ExecutionPolicy Unrestricted -File " + ScriptPath + " " + ScriptParameters;

			// Define process error/output event handling and start it.


			PSProcess.StartInfo = process;
			PSProcess.EnableRaisingEvents = true;
			PSProcess.Exited += new System.EventHandler(onScriptExited);

			PSProcess.OutputDataReceived += (sender, EventArgs) => eventLog.WriteEntry(EventArgs.Data);
			PSProcess.ErrorDataReceived += (sender, EventArgs) => eventLog.WriteEntry(EventArgs.Data);
			PSProcess.Start();

			// Begin*ReadLine must be set after process has executed.

			PSProcess.BeginOutputReadLine();
			PSProcess.BeginErrorReadLine();
			eventLog.WriteEntry("PowershellStarter Started powershell service with scriptpath:" + ScriptPath + " and parameter: " + ScriptParameters);
		}

		protected override void OnStop(){
			if (!PSProcess.HasExited) {
				PSProcess.Kill();
			}
			eventLog.WriteEntry("PowershellStarter Stopped.");
		}

		protected override void OnContinue(){
			eventLog.WriteEntry("PowershellStarter does not support continue...");
		}

		private void eventLog_EntryWritten(object sender, EntryWrittenEventArgs e) {

		}
		

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.eventLog = new EventLog();
			((ISupportInitialize)(this.eventLog)).BeginInit();
			this.eventLog.EntryWritten += new EntryWrittenEventHandler(this.eventLog_EntryWritten);
			this.ServiceName = "PSSvc";
			((ISupportInitialize)(this.eventLog)).EndInit();
		}
		
	}
}
