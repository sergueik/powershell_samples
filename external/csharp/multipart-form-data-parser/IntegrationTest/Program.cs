using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Service {
	class Program {
		static void Main(string[] args) {
			var webServiceHost = new WebServiceHost(typeof(Service));
			var serviceEndpoint = webServiceHost.AddServiceEndpoint(typeof(IService), new WebHttpBinding(), "");
			var serviceDebugBehavior = webServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
			serviceDebugBehavior.HttpHelpPageEnabled = false;
			webServiceHost.Open();
			Console.WriteLine("Service Host started @ " + DateTime.Now.ToString());
			Console.Read();
		}
	}
}
