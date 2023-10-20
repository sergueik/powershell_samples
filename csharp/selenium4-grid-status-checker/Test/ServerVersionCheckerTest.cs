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
	public class ServerVersionCheckerTest {
private readonly GridStatusLoader gridStatusLoader = new GridStatusLoader();	
public ClientConfiguration clientConfig = GenericProxies.defaultConfiguration;
		private string payload4 = null;
		private string payload3 = null;

		[TestFixtureSetUp]
		public void SetUp() {
			payload3 = @"
{
  ""status"": 0,
  ""value"": {
    ""ready"": true,
    ""message"": ""Hub has capacity"",
    ""build"": {
      ""revision"": ""63f7b50"",
      ""time"": ""2018-02-07T22:42:28.403Z"",
      ""version"": ""3.9.1""
    },
    ""os"": {
      ""arch"": ""x86"",
      ""name"": ""Windows 7"",
      ""version"": ""6.1""
    },
    ""java"": {
      ""version"": ""1.8.0_101""
    }
  }
}";
			payload4 = @"
			{
  ""value"": {
    ""ready"": true,
    ""message"": ""Selenium Grid ready."",
    ""nodes"": [
      {
        ""id"": ""340232e5-36dd-4014-9a86-7770e45579a6"",
        ""uri"": ""http:\u002f\u002f10.0.2.15:5555"",
        ""maxSessions"": 1,
        ""osInfo"": {
          ""arch"": ""amd64"",
          ""name"": ""Windows 10"",
          ""version"": ""10.0""
        },
        ""heartbeatPeriod"": 60000,
        ""availability"": ""UP"",
        ""version"": ""4.0.0 (revision 3a21814679)"",
        ""slots"": [
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""340232e5-36dd-4014-9a86-7770e45579a6"",
              ""id"": ""49d6090b-798d-4b0b-9ce7-8a7a7400e962""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""340232e5-36dd-4014-9a86-7770e45579a6"",
              ""id"": ""e6928dba-2a7b-4f4c-9c39-51e2ed542db6""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      }
    ]
  }
}";
		}

		[Test]
		public void test01() {
			Grid4 root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid4>(payload3);
			Assert.NotNull(root);
			// Both Selenium 3.x and 4.x has "value"
			var value = root.value;
			Assert.NotNull(value);
			var nodes = value.nodes;
			Assert.Null(nodes);
			// NOTE: with Selenium 4.x  "nodes" may be null 
			// before nodes have registered
			// NOTE: with Selenium 3.x there is no "build"		
			// var build = value.build;
			// Assert.NotNull(build);
			// var version = build.version;
		}

		[Test]
		public void test02() {
			Grid3 root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid3>(payload3);
			Assert.NotNull(root);
			// Both Selenium 3.x and 4.x has "value"
			var value = root.value;
			Assert.NotNull(value);
			// NOTE: with Selenium 3.x there is no "nodes"
			// var nodes = value.nodes;
			// Assert.Null(nodes);
			

			var build = value.build;
			Assert.NotNull(build);
			var version = build.version;
						
			Assert.NotNull(version);
			Console.Error.WriteLine("Selenium Hub version: " + version);

		}

		[Test]
		public void test03() {
			Grid3 root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid3>(payload4);
			Assert.NotNull(root);
			// Both Selenium 3.x and 4.x has "value"
			var value = root.value;
			Assert.NotNull(value);
			// NOTE: with Selenium 3.x there is no "nodes"
			// var nodes = value.nodes;
			// Assert.Null(nodes);
			

			var build = value.build;
			Assert.Null(build);

		}
		
		[Test]
		public void test04() {
			Grid4 root = clientConfig.InBoundSerializerAdapter.Deserialize<Grid4>(payload4);
			Assert.NotNull(root);
			// Both Selenium 3.x and 4.x has "value"
			var value = root.value;
			Assert.NotNull(value);
			var nodes = value.nodes;
			Assert.NotNull(nodes);
		}

		[Test]
		public void test05() {
			Assert.AreEqual("4.0.0", gridStatusLoader.Selenium4Detected(payload4));
			Assert.AreEqual("3.9.1", gridStatusLoader.Selenium4Detected(payload3));
		}

	}
}
