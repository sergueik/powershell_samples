using System.Reflection;

namespace AssemblyAsResource.Log4NetProxy {
	class LogImpl : ILog {
		private readonly object instance;

		internal LogImpl(object instance) {
			this.instance = instance;
		}

		public void Info(object message) {
			MethodInfo method = instance.GetType().GetMethod("Info", new[] { message.GetType() });
			method.Invoke(instance, new[] { message });
		}
	}
}