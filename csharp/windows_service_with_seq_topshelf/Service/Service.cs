
using System;
using Serilog;
using Topshelf;

namespace WindowServiceTemplate
{
	class Service : ServiceControl
	{
		private string serviceName = "Windows Service Name";
		public string ServiceName { get { return serviceName; } set { serviceName = value; } }
		bool ServiceControl.Start(HostControl hostControl)
		{
			Log.Logger.Information("{ServiceName} has {event}", ServiceName, "started");
			return true;
		}

		bool ServiceControl.Stop(HostControl hostControl)
		{
			Log.Logger.Information("{ServiceName} has {event}", ServiceName, "stopped");
			return true;
		}
	}
}