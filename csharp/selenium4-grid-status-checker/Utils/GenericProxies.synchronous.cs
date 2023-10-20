using Utils.Support;
namespace Utils {
	
	// Synchronous proxy implementation
    public static partial class GenericProxies {

		// synchronous GET
		// NOTE: The property or indexer 'Utils.GenericProxies.defaultConfiguration' cannot be used in this context because it lacks the get accessor (CS015
		/* 
        public static T RestGet<T>(string url, ClientConfiguration clientConfiguration = defaultConfiguration) {
            var request = CreateRequest(url, clientConfiguration);
            request.Accept = clientConfiguration.Accept;
            request.Method = "GET";
            return ReceiveData<T>(request, clientConfiguration);
        }
		*/
		
		public static T RestGet<T>(string url) {
            return RestGet<T>(url, defaultConfiguration);
        }

    	public static T RestGet<T>(string url, ClientConfiguration configuration) {
            var clientConfig = configuration ?? defaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.Accept = clientConfig.Accept;
            request.Method = "GET";
            return ReceiveData<T>(request, clientConfig);
        }

		
        // synchronous GET, no response expected
        public static void RestGetNonQuery(string url, ClientConfiguration configuration) {
            var clientConfig = configuration ?? defaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.Method = "GET";

            request.GetResponse().Close();
        }

        // synchronous HEAD - untested
        public static int RestHead(string url) {
	            return RestHead(url, defaultConfiguration);
        }

        public static int RestHead(string url, ClientConfiguration configuration) {
            var clientConfig = configuration ?? defaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.Accept = clientConfig.Accept;
            request.Method = "HEAD";
            return ReceiveStatus(request, clientConfig);
        }

        // synchronous POST
        public static void RestGetNonQuery(string url) {
            RestGetNonQuery(url, defaultConfiguration);
        }

        public static TR RestPost<TR, TI>(string url, TI data) {
        	
            return RestPost<TR, TI>(url, data, defaultConfiguration);
        }

        public static TR RestPost<TR, TI>(string url, TI data, ClientConfiguration configuration) {
            var clientConfig = configuration ?? defaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.ContentType = clientConfig.ContentType;
            request.Accept = clientConfig.Accept;
            request.Method = "POST";

            PostData(request, clientConfig, data);
            return ReceiveData<TR>(request, clientConfig);
        }

        // synchronous POST, no respons expected
        public static void RestPostNonQuery<TI>(string url, TI data) {
            RestPostNonQuery(url, data, defaultConfiguration);
        }

        public static void RestPostNonQuery<TI>(string url, TI data, ClientConfiguration configuration) {
            var clientConfig = configuration ?? defaultConfiguration;
            var request = CreateRequest(url, clientConfig);
            request.ContentType = clientConfig.ContentType;
            request.Accept = clientConfig.Accept;
            request.Method = "POST";

            PostData(request, clientConfig, data);
            request.GetResponse().Close();
        }
    }
}
