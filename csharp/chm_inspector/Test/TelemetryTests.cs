using System;
using NUnit.Framework;
using System.Linq;
using System.Net;

using Serilog;
using Serilog.Core;
// using Serilog.Sinks.Console;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;

using Elasticsearch;
using Utils;

namespace Tests {

	[TestFixture]
	public class TelemetryTest {
		// see also https://github.com/serilog-contrib/serilog-sinks-elasticsearch/blob/dev/sample/Serilog.Sinks.Elasticsearch.Sample/Program.cs

		private LoggerConfiguration loggerConfiguration = null;
		// docker-machine ip
		private const string endpoint = "http://192.168.99.100:9200";

		// [SetUp]  // NUnit 2.x compatible attribute
		[TestFixtureSetUp]
		public void Setup() {
			var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri(endpoint));
			elasticsearchSinkOptions.DetectElasticsearchVersion = false;
			elasticsearchSinkOptions.AutoRegisterTemplate = true;
			elasticsearchSinkOptions.AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7;
			elasticsearchSinkOptions.IndexFormat = "serilog-test";
			// NOTE: Elasticsearch 7.9.1 don’t have any credentials, security is disabled by default and the "elastic" user does not exist
			// elasticsearchSinkOptions.ModifyConnectionSettings = conn => conn.BasicAuthentication("elastic", "5mOz5+0BJKzXNyxHcZ*D");
			elasticsearchSinkOptions.BatchPostingLimit = 1;
			elasticsearchSinkOptions.QueueSizeLimit = 10;

			loggerConfiguration = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Elasticsearch(elasticsearchSinkOptions);
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
	    	Log.Logger = loggerConfiguration.CreateLogger();
	    	Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));

	    }

		[Test]
		public void test1( ){
			 Log.Information("Hello, elastic! at {Time}", DateTime.UtcNow);
			 Log.CloseAndFlush();
		}

		[Test]
		public void test2( ){
	        var doc = new {
	            timestamp = DateTime.UtcNow,
	            message = "OOM imminent",
	            mem = GC.GetTotalMemory(false)
	        };
	        Telemetry.init();
	        // Send to index "oom-events"
	        var response = Telemetry.sendEvent("oom-events", doc);
	        // verify via
	        // curl -X GET "http://localhost:9200/oom-events/_search?pretty"
	        Assert.AreEqual(201, response.HttpStatusCode);
	        Console.WriteLine(String.Format("Status: {0}", response.HttpStatusCode));
		}
	}
}
