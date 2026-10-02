using System;
﻿using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

// origin: https://github.com/kevingosse/wslmon/blob/master/src/Program.cs
namespace Utils {
	public class WslCommands {
		// browse UNC path / UNC namespace Windows-facing filesystem projection of the Linux environment
		private static void ListProcesses(string distribution) {
			// see also: https://learn.microsoft.com/en-us/windows/dev-environment/wsl-interop?utm_source=chatgpt.com
			// NOTE: \\wsl$\<distro>\... may become \\wsl.localhost\<distro>\...
			var root = String.Format(@"\\wsl$\{0}", distribution);

			if (!Directory.Exists(root)) {
				Console.WriteLine(String.Format(@"Could not open share for distribution {0}", distribution));
				return;
			}

			var proc = String.Format(@"{0}\proc", root);

			if (!Directory.Exists(proc)) {
				Console.WriteLine(String.Format(@"No /proc/ directory found for distribution {0}", distribution));
				return;
			}

			foreach (var directory in Directory.GetDirectories(proc)) {
				InspectProcess(directory);
			}
		}

		private static void InspectProcess(string path) {
			// C# 5.0 could not do the C# 10 range operator.
			/*
            if (!int.TryParse(path[(path.LastIndexOf('\\') + 1)..], out var pid))
            {
                return;
            }
            */
			int slash = path.LastIndexOf('\\');
			string text = path.Substring(slash + 1);

			int pid;
			if (!int.TryParse(text, out pid)) {
				return;
			}

			var cmdline = File.ReadAllText(String.Format(@"{0}\cmdline", path));

			Console.WriteLine(String.Format(@"{0} => {1}", pid, cmdline));
		}

		// The non-generic type 'System.Collections.IEnumerable' cannot be used with type arguments (CS0308)
		private static IEnumerable<string> ListDistributions() {
			var lxss = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Lxss");

			if (object.ReferenceEquals(lxss, null)) {
				yield break;
			}

			foreach (var guid in lxss.GetSubKeyNames()) {
				var distribution = lxss.OpenSubKey(guid);

				if (object.ReferenceEquals(distribution, null)) {
					continue;
				}
				// null-conditional operator
				// nullable reference types or pattern matching  did not exist in C# 5.0
				/*
                if (distribution.GetValue("DistributionName") is string name)
                {
                    yield return name;
                }
                
                */
				if (!object.ReferenceEquals(distribution.GetValue("DistributionName"), null)) {
					string name = distribution.GetValue("DistributionName") as string;
					yield return name;
				}
			}
		}
	}
}
