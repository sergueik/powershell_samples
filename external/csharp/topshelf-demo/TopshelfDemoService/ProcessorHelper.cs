using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TopshelfDemoService {
    internal class ProcessorHelper {
		
        public static List<Process> GetProcessList() {
            return GetProcesses().ToList();
        }

        public static Process[] GetProcesses() {
            var processList = Process.GetProcesses();
            return processList;
        }

        public static bool IsProcessExists(string processName) {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        public static void RunProcess(string applicationPath, string args = "") {
            try {
                ProcessExtensions.StartProcessAsCurrentUser(applicationPath, args);
            } catch (Exception e) {
                var psi = new ProcessStartInfo 
                {
                    FileName = applicationPath,
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = args
                };
                Process.Start(psi);
            }
        }
    }
}
