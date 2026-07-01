using System;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Utils
{
	public static class MetricSinkHelper
	{
		// NOTE: avoid using (var client = new HttpClient())
		// which is a widespread anti-pattern in older codebases.
		private static readonly HttpClient httpClient = new HttpClient();
		public static void push(string targetUrl, string body)
		{
			
			
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.postasync?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.continuewith?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.stringcontent.-ctor?view=netframework-4.5#system-net-http-stringcontent-ctor(system-string-system-text-encoding-system-string)
			httpClient.PostAsync( targetUrl, new StringContent(body, Encoding.UTF8, "text/plain")).ContinueWith(
				( Task<HttpResponseMessage> task) => {
				if (task.IsFaulted) {
					Console.Error.WriteLine(
						task.Exception.GetBaseException().Message);
				} else if (task.Result != null && !task.Result.IsSuccessStatusCode) {
					Console.Error.WriteLine(String.Format( "HTTP error: {}", (int)task.Result.StatusCode));
				}
				if (null != task.Result)
					task.Result.Dispose();
			}, TaskContinuationOptions.None);
			
		}
	}
}
