using AssemblyAsResource.Log4NetProxy;

namespace AssemblyAsResource {
	class Program {
		static void Main(){
			// Configure Log4net
			XmlConfigurator.Configure();

			// Trace program start
			ILog logger = LogManager.GetLogger("AssemblyAsResource");
			logger.Info("Program start");

			// Trace program end
			logger.Info("Program end");
	}
		}
}
