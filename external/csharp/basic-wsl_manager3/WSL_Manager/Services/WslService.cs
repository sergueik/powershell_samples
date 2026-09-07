using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WslManagerFramework.Models;

namespace WslManagerFramework.Services {
	public class WslService {
		private static string GetWslPath() {
			var systemDir = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess
                ? @"C:\Windows\Sysnative"
                : @"C:\Windows\System32";
			return System.IO.Path.Combine(systemDir, "wsl.exe");
		}

		public static bool CheckWslStatus() {
			bool status = true;
			var processStartInfo = new ProcessStartInfo {
				FileName = GetWslPath(),
				Arguments = "--status",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.Unicode,
				StandardErrorEncoding = Encoding.Unicode
			};

			using (var process = new Process { StartInfo = processStartInfo }) {
				if (!process.Start()) {
					Debug.WriteLine("wsl.exe failed to start");
					throw new InvalidOperationException("wsl.exe failed to start");
				}
				string standardOutput = process.StandardOutput.ReadToEnd();
				string standardError = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (process.ExitCode != 0) {
					Debug.WriteLine("wsl.exe error: " + standardError);
					status = false;
				}
			}
			return status;      
		}

		public static string[] ListDistros() {
			var processStartInfo = new ProcessStartInfo {
				FileName = GetWslPath(),
				Arguments = "-l -q",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				StandardOutputEncoding = Encoding.Unicode,
				StandardErrorEncoding = Encoding.Unicode
			};

			using (var process = new Process { StartInfo = processStartInfo }) {
				if (!process.Start()){
					Debug.WriteLine("wsl.exe failed to start");
					throw new InvalidOperationException("wsl.exe failed to start");
				}
				string standardOutput = process.StandardOutput.ReadToEnd();
				string standardError = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (process.ExitCode != 0) {
					Debug.WriteLine("wsl.exe error: " + standardError);
					// "T\0h\0e\0 \0W\0i\0n\0d\0o\0w\0s\0 \0S\0u\0b\0s\0y\0s\0t\0e\0m\0 \0f\0o\0r\0 \0L\0i\0n\0u\0x\0 \0i\0s\0 \0n\0o\0t\0 \0i\0n\0s\0t\0a\0l\0l\0e\0d\0.\0 \0Y\0o\0u\0 \0c\0a\0n\0 \0i\0n\0s\0t\0a\0l\0l\0 \0b\0y\0 \0r\0u\0n\0n\0i\0n\0g\0 \0'\0w\0s\0l\0.\0e\0x\0e\0...
					throw new InvalidOperationException("wsl.exe error: " + standardError);
				}
				var lines = standardOutput.Replace("\0", "")
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.StartsWith("*") ? s.Substring(1).Trim() : s)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToArray();

				return lines;
			}
		}

		public static string GetDistroStatus(string distroName) {
			try {
				var processStartInfo = new ProcessStartInfo {
					FileName = GetWslPath(),
					Arguments = "-l -v",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
					StandardOutputEncoding = Encoding.Unicode
				};

				using (var process = new Process { StartInfo = processStartInfo }) {
					if (!process.Start())
						return "Unknown (Failed to start)";

					string standardOutput = process.StandardOutput.ReadToEnd();
					string standardError = process.StandardError.ReadToEnd();
					process.WaitForExit();

					if (process.ExitCode != 0)
						return String.Format("Unknown (Exit code: {0})", process.ExitCode);

					var lines = standardOutput.Replace("\0", "").Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

					foreach (var line in lines) {
						var trimmedLine = line.Trim();
						if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.Contains("NAME") || trimmedLine.Contains("----"))
							continue;

						var cleanLine = trimmedLine;
						if (cleanLine.StartsWith("*")) {
							cleanLine = cleanLine.Substring(1).Trim();
						}
                        
						var parts = cleanLine.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length >= 2) {
							var name = parts[0];
							var status = parts[1];
                            
							if (name.Equals(distroName, StringComparison.OrdinalIgnoreCase)) {
								return status;
							}
						}
					}

					return "Unknown (Not found)";
				}
			} catch (Exception ex) {
				return String.Format("Unknown (Exception: {0})", ex.Message);
			}
		}

		public static async Task LaunchWslBackgroundAsync(string distroName)
		{
			var cleanName = distroName.Trim();
			var wslPath = GetWslPath();

			var processStartInfo = new ProcessStartInfo {
				FileName = wslPath,
				Arguments = String.Format("-d {0} --exec echo 'WSL started'", cleanName),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			};

			using (var process = Process.Start(processStartInfo)) {
				if (process != null) {
					await Task.Run(() => process.WaitForExit(10000));
                    
					if (process.ExitCode != 0) {
						var error = await Task.Run(() => process.StandardError.ReadToEnd());
						Debug.WriteLine(String.Format("{0} Failed to start. Error: {1}", cleanName, error));
						throw new Exception(String.Format("{0} Failed to start. Error: {1}", cleanName, error));
					}
				}
			}
		}

		public static async Task StopWslAsync(string distroName)
		{
			var cleanName = distroName.Trim();
			var wslPath = GetWslPath();

			var processStartInfo = new ProcessStartInfo {
				FileName = wslPath,
				Arguments = String.Format("--terminate {0}", cleanName),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			};

			using (var process = Process.Start(processStartInfo)) {
				// TODO:  configure in App.config
				if (process != null) {
					await Task.Run(() => process.WaitForExit(10000));
                    
					if (process.ExitCode != 0) {
						string standardError = await Task.Run(() => process.StandardError.ReadToEnd());
						Debug.WriteLine(String.Format("{0} Failed to stop. Error: {1}", cleanName, standardError));
						throw new Exception(String.Format("{0} Failed to stop. Error: {1}", cleanName, standardError));
					}
				}
			}
		}

		public static void LaunchInCmd(string distroName)
		{
			var cleanName = distroName.Trim();
			var wslPath = GetWslPath();
			var commandLine = String.Format("/k \"\"{0}\" -d {0}\"", wslPath, cleanName);
            
			var processStartInfo = new ProcessStartInfo {
				FileName = "cmd.exe",
				Arguments = commandLine,
				UseShellExecute = true,
				CreateNoWindow = false,
				WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			};
			Process.Start(processStartInfo);
		}

		public static void LaunchWslDirect(string distroName)
		{
			var cleanName = distroName.Trim();
			var wslPath = GetWslPath();

			var processStartInfo = new ProcessStartInfo {
				FileName = wslPath,
				Arguments = String.Format("-d {0}", cleanName),
				UseShellExecute = true,
				CreateNoWindow = false,
				WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			};
			Process.Start(processStartInfo);
		}
	}
}
