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

namespace Tests {
	[TestFixture]
	public class TelemetryTest {
		// see also https://github.com/serilog-contrib/serilog-sinks-elasticsearch/blob/dev/sample/Serilog.Sinks.Elasticsearch.Sample/Program.cs

		LoggerConfiguration loggerConfiguration = null;

		// [SetUp]  // NUnit 2.x compatible attribute
		[TestFixtureSetUp] 
		public void Setup() {
			var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri("http://localhost:9200"));
			elasticsearchSinkOptions.DetectElasticsearchVersion = false;			
			elasticsearchSinkOptions.AutoRegisterTemplate = true;
			elasticsearchSinkOptions.AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7;
			elasticsearchSinkOptions.IndexFormat = "serilog-test";
			elasticsearchSinkOptions.ModifyConnectionSettings = conn => conn.BasicAuthentication("elastic", "5mOz5+0BJKzXNyxHcZ*D");
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
		
	}
}
