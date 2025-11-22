using Elasticsearch.Net;
using System;

namespace Utils {
	public class Telemetry {
		static ElasticLowLevelClient client;
		private const string endpoint = "http://192.168.99.100:9200";
		// docker-machine ip
		// "http://localhost:9200" if installed locally

		public static void init(string endpoint) {
			var settings = new ConnectionConfiguration(new Uri(endpoint));
			client = new ElasticLowLevelClient(settings);
		}

		public static void init() {
			var settings = new ConnectionConfiguration(new Uri(endpoint));
			client = new ElasticLowLevelClient(settings);
		}

		public static BytesResponse sendEvent<T>(string index, T doc) {
            if (client == null)
                init();

			try {
				return client.Index<BytesResponse>(index, PostData.Serializable(doc));
				// immediate flush
            } catch (Exception e){
				// swallow exception
				Console.Error.WriteLine("Exception in sendEvent: "+ e.Message);
                return null;  // fail-safe: never throw from Telemetry
			}
		}
	}
}
