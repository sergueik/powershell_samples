// origin: https://www.codegrepper.com/code-examples/csharp/c#+create+rest+client
// see also: [few Great Ways to Consume RESTful APIs in C#](https://dzone.com/articles/a-few-great-ways-to-consume-restful-apis-in-c)

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using Utils;

namespace Program
{
	public class BasicHttpClient
	{
		private const string url = "http://192.168.0.125:4444/status";
		private static string queryString = "";
		private static string Text = null;

		private static JavaScriptSerializer serializer;
		private static StringBuilder verificationErrors = new StringBuilder();
		private static dynamic data;


		// query string is currently not used

		private static Stream rawDataStream;
		private static byte[] byteArray;

		static void Main(string[] args)
		{
			var client = new HttpClient();
			client.BaseAddress = new Uri(url);
			serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

			// Add an Accept header for JSON format.
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			HttpResponseMessage response = client.GetAsync(queryString).Result;  
			// Blocking call - will wait here until a response is fully received or a timeout occurs
			if (response.IsSuccessStatusCode) {
				// Parse the response body
				// https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=netframework-4.5
				// https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.content?view=netframework-4.5
				// get result value of this Task
				rawDataStream = response.Content.ReadAsStreamAsync().Result;
				// https://docs.microsoft.com/en-us/dotnet/api/system.io.memorystream.read?view=netframework-4.5

				// Dump the response body
				// NOTE: converted to int - assume small response payload
				int numBytesToRead = (int)rawDataStream.Length;
				if (numBytesToRead > 0) {
					byteArray = new byte[numBytesToRead];
					int numBytesRead = 0;
					while (numBytesToRead > 0) {
						int n = rawDataStream.Read(byteArray, numBytesRead, numBytesToRead);
						if (n == 0)
							break;
						numBytesRead += n;
						numBytesToRead -= n;
					}
					numBytesToRead = byteArray.Length;

					if (byteArray.Length > 0)
						// NOTE: Error CS1023: Embedded statement cannot be a declaration or labeled statement
						Text = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
					// Console.WriteLine(Text);
					data = serializer.Deserialize<object>(Text);

					// Predefined type 'Microsoft.CSharp.RuntimeBinder.Binder' is not defined or imported
					// One or more types required to compile a dynamic expression cannot be found
					// solved by adding Microsoft.CSharp in references
					// Console.WriteLine(data);
					try {
						var ready = bool.Parse(data.value.ready.ToString());
						Console.WriteLine("ready: " + ready);
						if (ready) {
							// var value = data.value.nodes;
							// Console.WriteLine(value);
							// System.Collections.Generic.List`1[System.Object]
							foreach (var node in data.value.nodes) {
								var nodeUri = node.uri;
								Console.WriteLine("node uri: " + nodeUri);
							}
						} else {
							var message = bool.Parse(data.value.message);
							Console.WriteLine("hub message: " + message);
						}
					} catch (RuntimeBinderException e) {
						// Unhandled Exception: Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: Cannot perform runtime binding on a null reference
						Console.WriteLine("Exception: " + e.ToString());
					}
				}

/*

                // https://docs.microsoft.com/en-us/previous-versions/aspnet/hh835763(v=vs.118) 
                // HttpContentExtensions.ReadAsAsync Method - 
				// probably an extension method
                // requires System.Net.Http.Formatting.dll which is not available as nuget package,and also has  become (?)  Microsoft.AspNet.WebApi.Client.dll
                var stronglyTypedData = response.Content.ReadAsAsync<String>().Result;  
                foreach (var d in stronglyTypedData)
                {
                    Console.WriteLine("{0}", d.Name);
                }
*/
			} else {
				Console.WriteLine("Error: HTTP status: {0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
			}
			client.Dispose();
		}
	}
	public class DataObject
	{
		public string Name { get; set; }
	}

}
