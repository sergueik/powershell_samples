using System;
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsService.NET {
	
	[RunInstaller(true)]
	public class ProjectInstaller : Installer {
		private ServiceInstaller si;
		private ServiceProcessInstaller spi;
		private IContainer components = null;

		public ProjectInstaller() {
			si = new ServiceInstaller();
			spi = new ServiceProcessInstaller();
			spi.Account = ServiceAccount.LocalSystem;
			spi.Password = null;
			spi.Username = null;

			si.ServiceName = "WindowsService.NET";
			si.DisplayName = "WindowsService.NET";
			si.Description = "A sample Windows Service boilerplate written in C#";

			this.Installers.AddRange(new Installer[] {
				spi,
				si
			});
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
