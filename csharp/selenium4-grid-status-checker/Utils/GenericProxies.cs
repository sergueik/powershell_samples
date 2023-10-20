using System.IO;
using System.Net;
using System.Text;
using Utils.Support;

namespace Utils {
	public static partial class GenericProxies {
        public static ClientConfiguration defaultConfiguration { get; set; }

        static GenericProxies() {
            // Initiate the default configuration
            defaultConfiguration = new ClientConfiguration();
            defaultConfiguration.ContentType = "application/json";
            defaultConfiguration.Accept = "application/json";
            defaultConfiguration.RequrieSession = false;
            defaultConfiguration.OutBoundSerializerAdapter = new JavaScriptSerializerAdapter();
            defaultConfiguration.InBoundSerializerAdapter = new JavaScriptSerializerAdapter();
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.net.webrequest.create?view=netframework-4.5
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.-ctor?view=netframework-4.5
        public static HttpWebRequest CreateRequest(string url, ClientConfiguration clientConfiguration) {
            return (clientConfiguration.RequrieSession) ? CookiedHttpWebRequestFactory.Create(url) :
                (HttpWebRequest)WebRequest.Create(url);
        }

        private static void PostData<T>(HttpWebRequest request, ClientConfiguration clientConfiguration, T data) {
            var jsonRequestString = clientConfiguration.OutBoundSerializerAdapter.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(jsonRequestString);

            using (var postStream = request.GetRequestStream()) {
                postStream.Write(bytes, 0, bytes.Length);
            }
        }

		// Receive raw headers from the service (unused)
		public static WebHeaderCollection ReceiveHeaders(HttpWebRequest request, ClientConfiguration clientConfiguration) {
			WebHeaderCollection headers;
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.getresponse?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebresponse?view=netframework-4.5
			using (var response = (HttpWebResponse)request.GetResponse()) {
				headers = response.Headers;
			}
			return headers;
		}
        	
		// Receive HTTP status from the service
		public static int ReceiveStatus(HttpWebRequest request, ClientConfiguration clientConfiguration) {
			HttpStatusCode statusCode;
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.getresponse?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebresponse?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode?view=netframework-4.5
			using (var response = (HttpWebResponse)request.GetResponse()) {
				statusCode = response.StatusCode; // enum
			}
				// return (int) statusCode.ToString();
				return (int) statusCode;
		}

        // Receive raw data from the service
		public static string ReceiveData(HttpWebRequest request, ClientConfiguration clientConfiguration) {
			string data;
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebrequest.getresponse?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.net.httpwebresponse?view=netframework-4.5
			using (var response = (HttpWebResponse)request.GetResponse()) {
				var stream = response.GetResponseStream();
				if (stream == null) { return null; }
				using (var streamReader = new StreamReader(stream)) {
					data = streamReader.ReadToEnd();
				}
			}
			return data;
	}
        // Receive and immediately deserialize data from the service
        public static T ReceiveData<T>(HttpWebRequest request, ClientConfiguration clientConfiguration) {
        	string data = ReceiveData(request, clientConfiguration);
            return data != null ? clientConfiguration.InBoundSerializerAdapter.Deserialize<T>(data): default(T);
        }
    }
}
