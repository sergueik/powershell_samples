using Microsoft.Win32;
using System;
using System.IO;
using System.Security;
using System.Windows.Forms;

static class StartupRegistrar {
	private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
	private const string AppName = "WSLManager";

	private static string ExePath {
		get { return Application.ExecutablePath; }
	}

	public static bool IsEnabled() {
		using (var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false)) {
			var value = key == null ? null : key.GetValue(AppName) as string;
			if (string.IsNullOrEmpty(value))
				return false;

			var expectedQuoted = "\"" + ExePath + "\"";
			return value.StartsWith(expectedQuoted, StringComparison.OrdinalIgnoreCase)
			|| value.StartsWith(ExePath, StringComparison.OrdinalIgnoreCase);
		}
	}

	public static void SetEnabled(bool enable, string startupArgs = null) {
		try {
			// .NET Framework では「OpenSubKey(..., true)」→ なければ「CreateSubKey(...)」
			RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
			if (key == null)
				key = Registry.CurrentUser.CreateSubKey(RunKeyPath);

			if (key == null)
				throw new InvalidOperationException(String.Format("Could not create/open the registry key {0}", RunKeyPath ));

			using (key) {
				if (enable) {
					var command = "\"" + ExePath + "\"";
					if (!string.IsNullOrEmpty(startupArgs))
						command += " " + startupArgs;

					key.SetValue(AppName, command, RegistryValueKind.String);
				} else {
					key.DeleteValue(AppName, false);
				}
			}
		} catch (UnauthorizedAccessException) {
			ShowError("Insufficient permissions for changing the startup settings. Consult with your administrator or organization policy.");
			throw;
		} catch (SecurityException) {
			ShowError("The operation was blocked by the security policy");
			throw;
		} catch (IOException) {
			ShowError("I/O error accessing the registry");
			throw;
		} catch (Exception ex) {
			ShowError("Unexpected exception during Startup configuration: " + ex.Message);
			throw;
		}
	}

	private static void ShowError(string message) {
		try {
			MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		} catch { /* Ignore this to handle cases where there is no UI, such as when running as a service */
		}
	}
}
