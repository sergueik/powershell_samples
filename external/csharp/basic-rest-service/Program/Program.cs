using System;
using System.Reflection;
using System.Linq;

using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace ScriptServices {
	class Program {

		static void Main(string[] args) {

			HostFactory.Run((HostConfigurator hostConfigurator) => {

				hostConfigurator.Service<hosting.WindowsService>((ServiceConfigurator<hosting.WindowsService> serviceConfigurator) => {
			        serviceConfigurator.ConstructUsing((string name) => new hosting.WindowsService());
				serviceConfigurator.WhenStarted((hosting.WindowsService callback) => callback.Start());
				serviceConfigurator.WhenStopped((hosting.WindowsService callback) => callback.Stop());
				});
				hostConfigurator.RunAsLocalSystem();

				if (Attribute.IsDefined(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute))) {
				    var assemblyDescriptionAttribute = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute( Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute));
				    if (assemblyDescriptionAttribute != null)
					hostConfigurator.SetDescription(assemblyDescriptionAttribute.Description);

				}
				var assemblyTitleAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
				hostConfigurator.SetDisplayName(assemblyTitleAttribute.Title);
				hostConfigurator.SetServiceName(Assembly.GetExecutingAssembly().GetName().Name);
			});
		}
	}
}
