using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Utils;
using Serilog;
using Serilog.Sinks.Elasticsearch;

using Serilog.Core;
using Serilog.Debugging;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;
// using Serilog.Sinks.Console;

using Elasticsearch;

namespace Tests
{
	[TestFixture]
	public class TocReaderTests
	{
		private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		// see also https://github.com/serilog-contrib/serilog-sinks-elasticsearch/blob/dev/sample/Serilog.Sinks.Elasticsearch.Sample/Program.cs

		LoggerConfiguration loggerConfiguration = null;

		[SetUp]
		// [TestFixtureSetUp]   // NUnit 2.x compatible attribute
	    public void Setup() {
			var elasticsearchSinkOptions = new ElasticsearchSinkOptions(new Uri("http://localhost:9200"));
			elasticsearchSinkOptions.DetectElasticsearchVersion = false;
			elasticsearchSinkOptions.AutoRegisterTemplate = true;
			elasticsearchSinkOptions.AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6;
			loggerConfiguration = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Elasticsearch(elasticsearchSinkOptions);

	    	Log.Logger = loggerConfiguration.CreateLogger();
	
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
		public void test2()
		{
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
		public void test3()
		{
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
			 Log.Information("Hello, world!");
		}
		
	}
}