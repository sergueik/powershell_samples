using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;


namespace Utils {

	public class ProcessRunner {
		public List<string> StandardOutput { get; private set; }
		public List<string> StandardError  { get; private set; }

		public int ExitCode { get; private set; }

		public ProcessRunner() {
			StandardOutput = new List<string>();
			StandardError = new List<string>();
		}

		public int Run(string fileName, string arguments)
		{
			StandardOutput.Clear();
			StandardError.Clear();

			// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo?view=netframework-4.5
			try {
				var processStartInfo = new ProcessStartInfo {
					FileName = fileName,
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				};

				using (var process = new Process()) {
					process.StartInfo = processStartInfo;

					// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.datareceivedeventargs.data?view=netframework-4.5 
					process.OutputDataReceived += (object sender, DataReceivedEventArgs e) => {
						if (e.Data != null)
							StandardOutput.Add(e.Data);
					};

					// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.outputdatareceived?view=netframework-4.5
					process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => {
						if (e.Data != null)
							StandardError.Add(e.Data);
					};

					process.Start();

					process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					process.WaitForExit();

					ExitCode = process.ExitCode;
					return ExitCode;
				}
			} catch (Exception e) {
				StandardError.Add("Exception: " + e.Message);
				ExitCode = -1;
				return ExitCode;
			}
		}
	}
}