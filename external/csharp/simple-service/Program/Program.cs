using System;
using System.Linq;
using System.ServiceProcess;

namespace WindowsService.NET {
    static class Program {
        static void Main() {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]{
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
