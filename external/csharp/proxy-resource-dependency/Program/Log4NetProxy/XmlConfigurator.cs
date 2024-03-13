using System;
using System.IO;
using System.Reflection;

namespace AssemblyAsResource.Log4NetProxy {
	static class XmlConfigurator {
		public static void Configure() {
			Type xmlConfigurator = Loader.GetType("log4net.Config.XmlConfigurator");
			if (xmlConfigurator != null) {
				MethodInfo method = xmlConfigurator.GetMethod("Configure", Type.EmptyTypes);
				method.Invoke(null, null);
			}
		}

		public static void Configure(FileInfo configFile) {
			Type xmlConfigurator = Loader.GetType("log4net.Config.XmlConfigurator");
			if (xmlConfigurator != null) {
				MethodInfo method = xmlConfigurator.GetMethod("Configure", new[] { configFile.GetType() });
				method.Invoke(null, new[] { configFile });
			}
		}

	}
}