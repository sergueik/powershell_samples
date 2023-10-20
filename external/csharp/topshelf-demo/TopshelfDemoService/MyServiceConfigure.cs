using System;
using Topshelf;

namespace TopshelfDemoService {
    internal class MyServiceConfigure {
        internal static void Configure() {
            var rc = HostFactory.Run(host =>                                  
            {
                host.Service<HealthMonitorService>(service =>
                {
                    service.ConstructUsing(() => new HealthMonitorService());
                    service.WhenStarted(target => target.Start());
                    service.WhenStopped(target => target.Stop());
                });

                host.RunAsLocalSystem();                                        // 6

                host.EnableServiceRecovery(service =>                           // 7
                {
                    service.RestartService(3);                                  // 8
                });
                host.SetDescription("Windows service based on topshelf");       // 9
                host.SetDisplayName("Topshelf demo service");                   // 10
                host.SetServiceName("TopshelfDemoService");                     // 11
                host.StartAutomaticallyDelayed();                               // 12
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());       // 13
            Environment.ExitCode = exitCode;
        }
    }
}
