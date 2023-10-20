using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Grafana.SimpleJson.Example
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                 .UseKestrel()
                 .UseContentRoot(Directory.GetCurrentDirectory())
                 .ConfigureLogging((hostingContext, logging) =>
                 {                                    
                 })
                 .UseStartup<Startup>()
                 .Build();

            host.Run();
        }
    }
}
