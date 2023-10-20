using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Reflection;
using System.Linq;
namespace Service {
	[RunInstaller(true)]
	public class ProjectInstaller : Installer {

		private ServiceAccount serviceAccount = ServiceAccount.LocalSystem;
		private string eventLog = "PipeServerLog";
		private string eventLogSource = "PipeServerService";
		private string serviceName = "PipeServer";

		private EventLogInstaller eventLogInstaller;
		private PerformanceCounterInstaller performanceCounterInstaller;
		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;

		private Container components = null;
		private KeyValueConfigurationCollection appSettings;

		public ProjectInstaller() {
			
			// https://stackoverflow.com/questions/57316014/get-access-to-appsettings-from-custom-installer-during-uninstall-beforeuninstal
			// https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configurationmanager.openexeconfiguration
			// https://www.delftstack.com/howto/csharp/csharp-get-executable-path/
			String assemblyLocation = Assembly.GetExecutingAssembly().Location;
			Console.Error.WriteLine(String.Format("Reading Assembly {0} Configuration", assemblyLocation));
						
			// string assemblyPath = Path.GetDirectoryName(Assembly. GetExecutingAssembly());
			                                             
			try {
				Configuration configuration = ConfigurationManager.OpenExeConfiguration(assemblyLocation);
			
				appSettings = configuration.AppSettings.Settings;
				if (appSettings.AllKeys.Contains("ServiceName")) {
					serviceName = appSettings["ServiceName"].Value;
					Console.Error.WriteLine("Determined Service Name from App.config: " + serviceName);	
				}
			} catch (ConfigurationErrorsException e) {
				Console.Error.WriteLine("Exception:: " + e.ToString());
			}
			eventLogInstaller = new EventLogInstaller();
			performanceCounterInstaller = new PerformanceCounterInstaller();
			serviceProcessInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			eventLogInstaller.Log = eventLog;
			eventLogInstaller.Source = eventLogSource;

			serviceProcessInstaller.Account = serviceAccount;
			serviceProcessInstaller.Password = null;
			serviceProcessInstaller.Username = null;

			serviceInstaller.ServiceName = serviceName;

			Installers.AddRange(new Installer[] {
				eventLogInstaller,
				// NOTE: corrupts counters
				/* this.performanceCounterInstaller, */
				serviceProcessInstaller,
				serviceInstaller
			});
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	}
}