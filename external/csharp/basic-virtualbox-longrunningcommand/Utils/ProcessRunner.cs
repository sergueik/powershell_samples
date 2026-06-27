using System;
using System.Diagnostics;
using System.Text;

namespace Utils
{

	public static class ProcessRunner
	{
		public static void RunAndCapture(string fileName, string arguments)
		{
			var stdout = new StringBuilder();
			var stderr = new StringBuilder();

			var psi = new ProcessStartInfo {
				FileName = fileName,
				Arguments = arguments,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			using (var process = new Process()) {
				process.StartInfo = psi;

				process.OutputDataReceived += (s, e) => {
					if (e.Data != null)
						stdout.AppendLine(e.Data);
				};

				process.ErrorDataReceived += (s, e) => {
					if (e.Data != null)
						stderr.AppendLine(e.Data);
				};

				process.Start();

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();

				Console.Error.WriteLine("Exit code: " + process.ExitCode);

				if (stdout.Length > 0)
					Console.Error.WriteLine("<OUTPUT>\n" + stdout + "\n</OUTPUT>");

				if (stderr.Length > 0)
					Console.Error.WriteLine("<ERROR>\n" + stderr + "\n</ERROR>");
			}
		}
	}
}