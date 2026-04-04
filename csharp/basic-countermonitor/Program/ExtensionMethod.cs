using System;
using System.Linq;
using System.ComponentModel;

namespace Program {
	public static class ExtensionMethod {
		public static TResult safeInvoke<T, TResult>(this T iSynchronizeInvoke, Func<T, TResult> call) where T : ISynchronizeInvoke  {
			if (iSynchronizeInvoke.InvokeRequired) {
				IAsyncResult result = iSynchronizeInvoke.BeginInvoke(call, new object[] { iSynchronizeInvoke });
				object endResult = iSynchronizeInvoke.EndInvoke(result);
				return (TResult)endResult;
			} else
				return call(iSynchronizeInvoke);
		}

		public static void safeInvoke<T>(this T iSynchronizeInvoke, Action<T> call) where T : ISynchronizeInvoke {
			if (iSynchronizeInvoke.InvokeRequired)
				iSynchronizeInvoke.BeginInvoke(call, new object[] { iSynchronizeInvoke });
			else
				call(iSynchronizeInvoke);
		}
	}
}
