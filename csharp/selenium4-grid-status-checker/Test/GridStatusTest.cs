using System;
using NUnit.Framework;

using Utils;
using Utils.Support;
using TestUtils;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Test {

	[TestFixture]
	public class GridStatusTest {
		private readonly GridStatusLoader gridStatusLoader = new GridStatusLoader();
		public ClientConfiguration clientConfig = GenericProxies.defaultConfiguration;
		private SimpleHTTPServer pageServer;
		private String filePath = null;
		[TestFixtureSetUp]
		public void SetUp() {
			filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
			Console.Error.WriteLine(String.Format("Using Webroot path: {0}", filePath));
		}

		[Test]
		public void test01() {
			const string payload = @"
{
  ""value"": {
    ""ready"": false,
    ""message"": ""Selenium Grid not ready."",
    ""nodes"": [
    ]
  }
}";
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid4>(payload);
			
			var check = root.GetType().Name;
			Console.Error.WriteLine("type: " + check );

			var nodes = gridStatusLoader.processDocument(root);
			Assert.AreEqual(0, nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid not ready.", root.value.message);
			Assert.AreEqual(false, Boolean.Parse(root.value.ready.ToString()));
		}

		[Test]
		public void test02()
		{
			string payload =
				File.ReadAllText(String.Format(@"{0}\{1}", filePath, "grid4.json"));
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid4>(payload);
			var nodes = gridStatusLoader.processDocument(root);
			Assert.AreEqual(3, nodes.Count);
			Console.WriteLine(nodes[0]);
		}

		[Ignore]
		[Test]
		public void test03() {
			Assert.AreEqual(true, ElevationChecker.IsProcessElevated(false));
		}

		[Test]
		public void test04() {
			if (ElevationChecker.IsProcessElevated(false)) {
				try {
					// should perform elevation check, try...catch does not help
					pageServer = new SimpleHTTPServer(filePath);
					var port = pageServer.Port;
					Console.Error.WriteLine(String.Format("Using Port {0}", port));

					// Make a synchronous POST Rest service call
					try {
						var serviceUrl = String.Format("http://localhost:{0}/grid4.json", port);
						HttpWebRequest httpWebRequest = GenericProxies.CreateRequest(serviceUrl, GenericProxies.defaultConfiguration);
						httpWebRequest.Accept = "application/json";
						httpWebRequest.Method = "GET";

						var root = GenericProxies.ReceiveData<Grid4>(httpWebRequest, GenericProxies.defaultConfiguration);
						var nodes = new List<Node>();
						nodes.AddRange(root.value.nodes);

						Assert.AreEqual(5, nodes.Count);
						Console.Error.WriteLine("Loaded {0} nodes", nodes.Count);
						Console.WriteLine(nodes[0].uri);
					} catch (Exception ex) {
						Console.Error.WriteLine("Failed to call the service - " + ex.Message);
					}				
				} catch (System.Net.HttpListenerException) {
				}
			}
		}

	}
}
