using System;
using System.Collections.Generic;
using System.Timers;

namespace TopshelfDemoService {
    internal class HealthMonitorService {
        private readonly Timer _timer;
        private int _monitorInterval = 10;
        private List<DaemonApplicationInfo> _daemonApps { get; set; }

        public HealthMonitorService() {
            _daemonApps = new List<DaemonApplicationInfo> {
                new DaemonApplicationInfo {
                    ProcessName = "TopshelfDemo.Client",
                    AppDisplayName = "TopshelfDemo Client",
                    AppFilePath = @"C:\developer\sergueik\powershell_ui_samples\external\csharp\topshelf-demo\TopshelfDemo.Client\bin\Debug\TopshelfDemo.Client.exe" // 请根据你的情况填写
                }
            };
            _timer = new Timer(_monitorInterval*1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => Monitor();
        }

        private void Monitor() {
            foreach (var app in _daemonApps) {
                if (ProcessorHelper.IsProcessExists(app.ProcessName)) {
                    Console.WriteLine("Application[{0}] already exists.", app.ProcessName);
                    return;
                }
                try  {
                    ProcessorHelper.RunProcess(app.AppFilePath, app.Args);
                } catch (Exception ex)  {
                    Console.WriteLine("Start application failed:{0}", ex);
                }
            }            
        }

        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
    }
}
