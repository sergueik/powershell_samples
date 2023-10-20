using System;
using System.Collections;
using System.ComponentModel;

using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using Utils;

namespace TestService {
	[RunInstaller(true)]
	public class ProjectInstaller : Installer {
		private ServiceProcessInstaller spi;


		private ServiceInstallerEx si;
		private Container components = null;

		public ProjectInstaller() {
			spi = new ServiceProcessInstaller();
			si = new ServiceInstallerEx();

			spi.Account = ServiceAccount.LocalSystem;
			spi.Password = null;
			spi.Username = null;

			si.ServiceName = "TestService";
			si.AfterInstall += new InstallEventHandler(_AfterInstall);


			Installers.AddRange(new Installer[] {
				spi,
				si
			});


			// TODO: Add any initialization after the InitializeComponent call

			// NOTE: Setting these properties here does not make them immediately effective.  
			// The ServiceInstallerEx configures the service AFTER the base installer is done 
			// doing its job. That is when the Committed event is fired from the base installer

			this.si.Description = "Description";

			// The fail run command is used to spawn another process when this service fails
			// it should include the entire command line as would be passed to Win32::CreateProcess()
			si.FailRunCommand = "SomeCommand.exe";

			// The fail count reset time resets the failure count after N seconds of no failures
			// on the service.  This value is set in seconds, though note that the SCM GUI only
			// displays it in increments of days.
			si.FailCountResetTime = 60 * 60 * 24 * 4;

			// The fail reboot message is used when a reboot action is specified and works in 
			// conjunction with the RecoverAction.Reboot type.

			si.FailRebootMsg = "problem detected";

			// Set some failure actions : Isn't this easy??
			// Do note that if you specify less than three actions, the remaining actions will take on
			// the value of the last action.  For example, if you only set one action to RunCommand,
			// failure 2 and failure 3 will also take on the default action of RunCommand. This is 
			// a "feature" of the ChangeServiceConfig2() method; Use RecoverAction.None to disable
			// unwanted actions.

			si.FailureActions.Add(new FailureAction(RecoverAction.Restart, 60000));
			si.FailureActions.Add(new FailureAction(RecoverAction.RunCommand, 2000));
			si.FailureActions.Add(new FailureAction(RecoverAction.Reboot, 3000));

			// Configure the service to start right after it is installed.  We do not want the user to
			// have to reboot their machine or to have some other process start it.  Do be careful because
			// if this service is dependent upon other services, they must be installed PRIOR to this one
			// for the service to be started properly

			si.StartOnInstall = true;

		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
			// NOTE: if named just "AfterInstall" the
// "TestService.ProjectInstaller.AfterInstall( object, System.Configuration.Install.InstallEventArgs)" would
// hide inherited member "System.Configuration.Install.Installer.AfterInstall". 

		private void  _AfterInstall(object sender, InstallEventArgs e) {	}

	}
}
