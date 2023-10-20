using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;

namespace TransactionService {
	[RunInstaller(true)]
	public class ProjectInstaller : Installer {

		private ServiceAccount serviceAccount = ServiceAccount.LocalSystem;
		private string eventLog = "LoadAverageCounterServiceLog";
		private string eventLogSource = "LoadAverageCounterService";
		private string serviceName = "LoadAverageService";

		private EventLogInstaller eventLogInstaller;
		private PerformanceCounterInstaller performanceCounterInstaller;
		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;

		private Container components = null;

		public ProjectInstaller() {
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
			serviceInstaller});
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
