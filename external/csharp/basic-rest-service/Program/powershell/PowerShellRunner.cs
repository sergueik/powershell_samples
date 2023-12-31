using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ScriptServices.powershell {

	public class PowerShellRunner {

		private bool debug;

		public bool Debug {
			get { return debug; }
			set {debug = value; }
		}

		public ScriptResult Execute(string method, string scriptPath, IDictionary<string,string> scriptArguments) {
			string workingDirectory = Path.GetTempPath();

			var launcherScript = Path.Combine(workingDirectory, string.Format("SSLaunch-{0}.ps1.", Guid.NewGuid()));

			// generate a script that will invoke our script with all the required parameters
			using (var writer = new StreamWriter(launcherScript)) {

				writer.WriteLine("$env:SCRIPTSERVICES_VERSION = '{0}'", Assembly.GetExecutingAssembly().GetName().Version);
				writer.WriteLine(String.Format(@"$env:PSModulePath = '{0}';", Environment.GetEnvironmentVariable("PSModulePath"), EnvironmentVariableTarget.User));
				writer.WriteLine("function Write-Host {}");
				writer.WriteLine("function Write-Verbose {}");
				writer.WriteLine("function Write-Debug {}");

				writer.Write(string.Format(@". '{0}' ", scriptPath));

				writer.Write(string.Format("-httpVerb \"{0}\"", method));

				foreach (var key in scriptArguments.Keys) {
					writer.Write(string.Format(" -{0} \"{1}\"", key, scriptArguments[key].Replace("\"", "`\"")));
				}
				writer.Flush();
			}

			try {
				var commandArguments = new StringBuilder();
				commandArguments.Append("-NonInteractive ");
				commandArguments.Append("-NoLogo ");
				commandArguments.Append("-ExecutionPolicy Unrestricted ");
				commandArguments.Append("-NoProfile ");
				commandArguments.AppendFormat("-File \"{0}\"", launcherScript);
				var posh = new PowerShellScriptExecutor(commandArguments.ToString());
				var res = posh.Execute();
				return res;
			} finally {
				if (!debug)
				   File.Delete(launcherScript);
			}
		}
	}
}
