using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace TestService {

	public class Service1 : ServiceBase {
		private IContainer components = null;

		public Service1() {
			components = new Container();
			this.ServiceName = "Service1";
		}
		static void Main() {
			ServiceBase[] ServicesToRun;	
			ServicesToRun = new ServiceBase[] { new Service1() };
			ServiceBase.Run(ServicesToRun);
		}

		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnStart(string[] args) { }
		protected override void OnStop() {  }
	}
}
