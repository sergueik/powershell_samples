using Elasticsearch.Net;
using System;

namespace Tests {
	public class Telemetry {
		static ElasticLowLevelClient client;

		static void Init() {
			var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"));
			client = new ElasticLowLevelClient(settings);
		}

		static void SendEvent(string index, string message) {
			try {
				var doc = new { timestamp = DateTime.UtcNow, message };
				client.Index<BytesResponse>(index, PostData.Serializable(doc));
			} catch { 
				}
		}
		/*
        var doc = new
        {
            timestamp = DateTime.UtcNow,
            message = "OOM imminent",
            mem = GC.GetTotalMemory(false)
        };

        // Send to index "oom-events"
        var response = client.Index<BytesResponse>("oom-events", PostData.Serializable(doc));
        Console.WriteLine($"Status: {response.HttpStatusCode}");
     */
	}
}
