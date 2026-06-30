using System;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils {
	public static class MetricsSinkHelper {
		private static readonly HttpClient httpClient = new HttpClient();
		public static void push(string targetUrl, string body) {
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.postasync?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.continuewith?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.stringcontent.-ctor?view=netframework-4.5#system-net-http-stringcontent-ctor(system-string-system-text-encoding-system-string)
			httpClient.PostAsync(targetUrl, new StringContent(body, Encoding.UTF8, "text/plan" /* "text/plan; version=0.0.4" */)).ContinueWith(
				// NOTE: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.FormatException:
				// The format of value 'text/plan; version=0.0.4' is invalid.
				(Task<HttpResponseMessage> task ) => {
				// https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.isfaulted?view=netframework-4.5
				if (task.IsFaulted)
					Console.Error.WriteLine(task.Exception.InnerException.Message);
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}
