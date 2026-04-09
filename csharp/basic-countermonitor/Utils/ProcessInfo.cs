using System;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Utils {
	public class ProcessInfo {
		public static string getProcessInstanceName(int pid) {
			Console.Error.WriteLine(String.Format("Searching Performance Counter for process with pid: {0}", pid));
			Debug.WriteLine(String.Format("Searching Performance Counter for process with pid: {0}", pid));
			var performanceCounterCategory =
				new PerformanceCounterCategory("Process");

			foreach (string instanceName in performanceCounterCategory.GetInstanceNames()) {
				Console.Error.WriteLine(String.Format("Counter: {0}", instanceName));
				Debug.WriteLine(String.Format("Counter: {0}", instanceName));
				using (var performanceCounter = new PerformanceCounter("Process", "ID Process", instanceName, true)) {
					int rawValue = (int)performanceCounter.RawValue;

					if (rawValue == pid)
						return instanceName;
				}
			}
			return null;
		}
		public static string getProcessInstanceName(string value) {
			int pid = 0;
			Int32.TryParse(value, out pid);
			return getProcessInstanceName(pid);
		}
		public static string getProcessInstanceName(string name, int pid) {
			Debug.WriteLine(String.Format("Searching Performance Counter for process with name: {0} pid: {1}", name, pid));
			Console.Error.WriteLine(String.Format("Searching Performance Counter for process with name: {0} pid: {1}", name, pid));
			var performanceCounterCategory =
				new PerformanceCounterCategory("Process");

			foreach (string instanceName in performanceCounterCategory.GetInstanceNames()) {
				if (instanceName.IndexOf(name) == -1)
					continue;
				Console.Error.WriteLine(String.Format("Counter: {0}", instanceName));
				Debug.WriteLine(String.Format("Counter: {0}", instanceName));
				using (var performanceCounter = new PerformanceCounter("Process", "ID Process", instanceName, true)) {
					int rawValue = (int)performanceCounter.RawValue;

					if (rawValue == pid)
						return instanceName;
				}
			}
			return null;
		}
		public static string getProcessInstanceName(string name, string value) {
			int pid = 0;
			Int32.TryParse(value, out pid);
			return getProcessInstanceName(name, pid);
		}
		public static List<int> getProcessIDsByCommandLine(string filename, string value) {
		 var results = new List<int>();
			// https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-process
			// NOTE: take advantage of WMI pseudo-SQL WQL LIKE with %value% wildcard matching against Win32_Process.CommandLine
			// to locate the target ProcessId by partial command-line contents.
			// NOTE: preserve WMI vendor class/property mixed camel snake style for readability.
			var query = String.Format("SELECT Name, Caption, ProcessId, CommandLine FROM Win32_Process WHERE CommandLine LIKE '%{0}%' AND CommandLine LIKE '%{1}%'", filename, value);
			Console.Error.WriteLine(String.Format("query: {0}",query));
			Debug.WriteLine(String.Format("query: {0}",query));
			// NOTE: 
			// The WMIC.exe command
			// wmic:root\cli>path win32_process get commandline,caption,name,processid where (processid=30448)
			// returns an "Invalid GET Expression" error
			// because the WHERE clause must come before the GET clause in the WMIC command
			try {
				using (var managementObjectSearcher = new ManagementObjectSearcher(query))
				using (var managementObjectCollection = managementObjectSearcher.Get()) {
					Console.Error.WriteLine(String.Format("examine the results: {0} rows" , managementObjectCollection.Count));
					Debug.WriteLine(String.Format("examine the results: {0} rows" , managementObjectCollection.Count));
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection) {
						Console.Error.WriteLine(String.Format("name: {0}|caption: {1}|commandline: {2}", managementBaseObject["Name"], managementBaseObject["Caption"], managementBaseObject["CommandLine"]));
						/*
							Console.Error.WriteLine("properties:");
							var propertyDataEnumerator  = managementBaseObject.Properties.GetEnumerator();
							while (propertyDataEnumerator.MoveNext()) {
								Console.Error.WriteLine("property:" + propertyDataEnumerator.Current.Name);
							}
						*/
						// Extract the ProcessId property
						if (managementBaseObject["ProcessId"] != null) {
							Console.Error.WriteLine(String.Format("Collected the result: {0}", managementBaseObject["ProcessId"]));
							results.Add(Convert.ToInt32(managementBaseObject["ProcessId"]));
						}
					}
				}
			} catch (ManagementException e) {
				Console.Error.WriteLine(String.Format("ManagementException occurred while querying WMI: {0}", e.Message));
			}
			return results;
		}
	}
}
