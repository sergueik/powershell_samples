using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Utils;
using System.Net;

using Serilog;
using Serilog.Core;
// using Serilog.Sinks.Console;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;

using Elasticsearch;

namespace Tests
{
	[TestFixture]
	public class TocReaderTests {
		private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		// see also https://github.com/serilog-contrib/serilog-sinks-elasticsearch/blob/dev/sample/Serilog.Sinks.Elasticsearch.Sample/Program.cs

		LoggerConfiguration loggerConfiguration = null;

		// [SetUp]  // NUnit 2.x compatible attribute
		[TestFixtureSetUp] 
		public void Setup() {
			var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri("https://localhost:9200"));
			elasticsearchSinkOptions.DetectElasticsearchVersion = false;			
			elasticsearchSinkOptions.AutoRegisterTemplate = true;
			elasticsearchSinkOptions.AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7;
			elasticsearchSinkOptions.IndexFormat = "serilog-test";
			elasticsearchSinkOptions.ModifyConnectionSettings = conn => conn.BasicAuthentication("elastic", "5mOz5+0BJKzXNyxHcZ*D");
			elasticsearchSinkOptions.BatchPostingLimit = 1;
elasticsearchSinkOptions.QueueSizeLimit = 10; // small number for testing
// elasticsearchSinkOptions.FlushInterval = TimeSpan.FromSeconds(1);
    
   // elasticsearchSinkOptions.EmitEventFailure = (logEvent, ex) => Console.WriteLine(String.Format("Elasticsearch error: {0}", ex.Message));
// elasticsearchSinkOptions.EmitDebugInformation = true;
			loggerConfiguration = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Elasticsearch(elasticsearchSinkOptions);
System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;
	    	Log.Logger = loggerConfiguration.CreateLogger();
	    	Serilog.Debugging.SelfLog.Enable(msg => Console.Error.WriteLine(msg));
	
	    }
			    
		[Test]
		public void test1() {
			var toclist = Chm.toc_structured(file);

			// Assert
			Assert.IsNotNull(toclist, "The toclist list should not be null");
			Assert.IsNotEmpty(toclist, "The toclist list should not be empty");

			foreach (var entry in toclist) {
				Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
				Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
				Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
			}
		}

		[Test]
		public void test2() {
			Dictionary<string,string> toc = Chm.tocdict_7zip(file);

			// Assert
			Assert.IsNotNull(toc, "The dictionary should not be null");
			Assert.IsNotEmpty(toc, "The dictionary should not be empty");

			foreach (var keyValuePair in toc) {
				Assert.IsFalse(string.IsNullOrEmpty(keyValuePair.Key), "Key (Name) should not be null or empty");
				Assert.IsFalse(string.IsNullOrEmpty(keyValuePair.Value), "Value (Local) should not be null or empty");
				Console.Error.WriteLine("{0}: {1}", keyValuePair.Key, keyValuePair.Value);
			}
		}
		[Test]
		public void test3() {
			var toclist = Chm.toc_7zip(file);

			// Assert
			Assert.IsNotNull(toclist, "The toclist list should not be null");
			Assert.IsNotEmpty(toclist, "The toclist list should not be empty");

			foreach (var entry in toclist) {
				Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
				Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
				Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
			}
		}

		[Test]
		public void test4( ){
			 Log.Information("Hello, elastic! at {Time}", DateTime.UtcNow);
			 Log.CloseAndFlush(); 
		}
		
	}
}