using System;
using System.Diagnostics;

namespace Utils {
	public class ProcessInfo {
		public static string getProcessInstanceName(int pid) {
			var performanceCounterCategory =
				new PerformanceCounterCategory("Process");

			foreach (string instance in performanceCounterCategory.GetInstanceNames()) {
				using (var performanceCounter = new PerformanceCounter("Process", "ID Process", instance, true)) {
					int rawValue = (int)performanceCounter.RawValue;

					if (rawValue == pid)
						return instance;
				}
			}

			return null;
		}
	}
}
