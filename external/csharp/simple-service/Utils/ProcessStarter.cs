using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utils {

	public class ProcessStarter {

		public string ScriptPath { get; set; }
		public string ScriptArguments { get; set; }
		private ProcessStartInfo psi;
		private Process process;
		private ICollection<string> _output;
		private ICollection<string> _error;
		
		public string ProcessOutput {
			get {
				return String.Join("\r\n", _output);
				// return String.Format("{0} lines: {1}", _output.Count , String.Join("\r\n", _output));
			}
		}

		public string ProcessError {
			get {
				return String.Join("\r\n", _error);
				// return String.Format("{0} lines: {1}", _error.Count ,String.Join("\r\n", _error));
			}
		}

		public ProcessStarter() {
			psi = new ProcessStartInfo();
			process = new Process();	
			_output = new Collection<string>();
			_error = new Collection<string>();
		}

		protected virtual void onScriptExited(object sender, EventArgs e) {
			// NOTE: the EventArgs type is too generic to be useful here 
		}

		
		public void Start(string ScriptPath, string ScriptParameters, string workingDirectory) {

			psi.CreateNoWindow = true;
			psi.UseShellExecute = false;
			psi.WorkingDirectory = workingDirectory;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.FileName = @"C:\windows\system32\windowspowershell\v1.0\powershell.exe";
			psi.Arguments = "-ExecutionPolicy bypass -File " + ScriptPath + " " + ScriptParameters;

			process.StartInfo = psi;
			process.EnableRaisingEvents = true;
			process.Exited += new System.EventHandler(onScriptExited);
		
			process.OutputDataReceived += (Object sender, DataReceivedEventArgs  EventArgs) => _output.Add(EventArgs.Data);
			process.ErrorDataReceived += (Object sender, DataReceivedEventArgs EventArgs) => collectError(EventArgs);		

			// TODO: ignore empty error ?
			bool status = process.Start();
			if (!status) {
				_error.Add("Failed to start: " + process.ExitCode);
			}

			// NOTE: Begin*ReadLine must be set after psi has executed.

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}

		private void collectError(DataReceivedEventArgs eventArgs) {
			if (!String.IsNullOrEmpty(eventArgs.Data))
				_error.Add(eventArgs.Data); 
		}

		public void Stop() {
			if (!process.HasExited) {
				process.Kill();
			}
		}
	}
}
