using System;
using System.Threading;

// based on: https://github.com/buchmoyerm/MutexManager/blob/master/SingleInstance.cs
namespace SystemTrayApp
{
	/// <summary>
	/// Description of SingleInstanceMutexController.
	/// </summary>
	public class SingleInstanceMutexController
	{
		public SingleInstanceMutexController()
		{
		}
		private static Mutex mutex;
		private static bool onlyInstance = false;
		static public bool Start()
		{
			try {
				
				//string mutexName = String.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

				// if you want your app to be limited to a single instance
				// across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
				string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyTitle);

				mutex = new Mutex(true, mutexName, out onlyInstance);
				return onlyInstance;
			} catch {
				return false;
			}
		}

		public static void Stop()
		{
			if (mutex != null) {
				if (onlyInstance) {
					try {
						mutex.GetAccessControl();
						// NOTE: when not owned:
						// System.ApplicationException: Object synchronization method was called from an unsynchronized block of code
						mutex.ReleaseMutex();
					} catch (UnauthorizedAccessException e) { 
					}
				}
			}
		}
	}
}
