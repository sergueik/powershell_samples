using System;
using System.Linq;

using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace ScriptServices {
	class Program {
		
		static void Main(string[] args) {
			// 
			HostFactory.Run((HostConfigurator hostConfigurator) => {

				hostConfigurator.Service<hosting.WindowsService>((ServiceConfigurator<hosting.WindowsService> serviceConfigurator) => {
			        serviceConfigurator.ConstructUsing((string name) => new hosting.WindowsService());
					serviceConfigurator.WhenStarted((hosting.WindowsService callback) => callback.Start());
					serviceConfigurator.WhenStopped((hosting.WindowsService callback) => callback.Stop());
				});
				hostConfigurator.RunAsLocalSystem();

				hostConfigurator.SetDescription("Exposes powershell scripts as REST-based micro services");
				hostConfigurator.SetDisplayName("ScriptServices");
				hostConfigurator.SetServiceName("ScriptServices");
			});
		}
	}
}
