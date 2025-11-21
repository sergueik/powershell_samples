using Elasticsearch.Net;
using System;

namespace Utils {
	public class Telemetry {
		static ElasticLowLevelClient client;

		public static void init() {
			var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"));
			client = new ElasticLowLevelClient(settings);
		}

		public static BytesResponse sendEvent<T>(string index, T doc) {
            if (client == null)
                init();

			try {
				return client.Index<BytesResponse>(index, PostData.Serializable(doc));
				// immediate flush
			} catch {
				// swallow exception				
                return null;  // fail-safe: never throw from Telemetry
			}
		}
	}
}
