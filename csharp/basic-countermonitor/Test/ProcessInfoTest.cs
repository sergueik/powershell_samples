using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using System.Diagnostics;

using System.Linq;
using System.Collections;
using System.Reflection;
using System.IO;

using Utils;
using TestUtils;

namespace Test {

	[TestFixture]
	public class ProcessInfoTest {
		
		private string result = null;
		private string appName = "VirtualBox";
		private int pid = -1;
		private PerformanceCounter performanceCounter = null;

		[SetUp]
		public void setUp() {
			performanceCounter = new PerformanceCounter();
			performanceCounter.CategoryName = "Process";
			performanceCounter.CounterName = "Working Set";			
		}

		[Test]
		public void test1()
		{
			// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.getprocessesbyname?view=netframework-4.5
			Process[] processes = Process.GetProcessesByName(appName);

			if (processes.Length > 0) {
				Console.Error.WriteLine(String.Format("Found {0} instances of {1}:", processes.Length, appName));
				foreach (Process process in processes) {
					Console.Error.WriteLine(String.Format("Process name: {0}, ID: {1}", process.ProcessName, process.Id));
					// You can access other properties, e.g., process.WorkingSet64 to read it directly
					// or process.MainModule.FileName to get the full path
					pid = process.Id;
					// long memoryMb = process.WorkingSet64 / 1024 / 1024;
					Console.Error.WriteLine(String.Format("pid={0} | Memory={1}", process.Id, process.WorkingSet64));
				}
				result = Utils.ProcessInfo.getProcessInstanceName(pid);
				Assert.IsNotNull(result);
				StringAssert.IsMatch(String.Format(@"{0}(?:#\d+)?", appName), result);
			
				// https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
				performanceCounter.InstanceName = result;
				var rawValue = (long)performanceCounter.RawValue;
				var computedValue = performanceCounter.NextValue();
				Console.Error.WriteLine(String.Format("Instance:{0} | Raw Value:{1} | Computed Value:{2}", result, rawValue, computedValue));
			} else {
				Console.WriteLine(String.Format("{0} is not currently running.", appName));
			}
		}

		[Test]
		public void test2()
		{
			// https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecountercategory?view=netframework-4.5
			Console.Error.WriteLine("Process.Explorer:");
			foreach (var performanceCounter in new PerformanceCounterCategory("Process").GetCounters("explorer")) {
				Console.Error.WriteLine(performanceCounter.CounterName);
			}
			var counterCategory = new PerformanceCounterCategory("Process");
			Console.Error.WriteLine("Counter Category Process:");
			//
			foreach (string counterName in counterCategory.GetCounters("_Total").Select(( PerformanceCounter p) => p.CounterName)) {
				Console.Error.WriteLine(counterName);
			}
		}

		[Ignore("unstable - windows processes")]
		[Test]
		public void test3() {
			string tempFile = Path.GetTempFileName();
			try {
				File.WriteAllText(tempFile, "");
				var processStartInfo = new ProcessStartInfo {
					FileName = "notepad.exe",
					Arguments = " " + tempFile, // Example command
					UseShellExecute = false,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};

				using (Process process = Process.Start(processStartInfo)) {
					process.WaitForInputIdle(); // Blocks until the window is ready for input
					var value = tempFile.Replace(@"\", @"\\");
					var results = Utils.ProcessInfo.getProcessIDsByCommandLine(processStartInfo.FileName, value);
					Assert.NotNull(results);
					Console.Error.WriteLine("Results: " + String.Join(",", results) + " (" + results.Count + ")");
					Assert.Greater(results.Count, 0 );
					
					process.Kill();
		            process.WaitForExit(2000);
				}
			} finally {
				if (File.Exists(tempFile))
					File.Delete(tempFile);
			}

		}

		[Test]
		public void test4() {
			string value = "notepad.exe";
			List<int> results = Utils.ProcessInfo.getProcessIDsByCommandLine("", value);
			Assert.NotNull(results);
			Console.Error.WriteLine("Results: " + String.Join(",", results) + " (" + results.Count + ")");
			Assert.Greater(results.Count, 0 );
		}
	}
}
