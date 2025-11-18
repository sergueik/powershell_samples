using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WindowServiceTemplate
{
	class Program
	{
		static void Main(string[] args)
		{
			InitializeLogging();

			HostFactory.Run(x => {
				x.Service<Service>();
				x.UseSerilog();
			});
		}

		static void InitializeLogging()
		{
			Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"logs/log-{Date}.txt")
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
		}
	}
}
