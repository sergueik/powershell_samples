using System;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;
using ShareLibraries;

namespace Tests {
	[TestFixture]
	public class JsonTests {

		[Test]
		public void test01() {
			const string payload = @"
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
			Root root = JsonConvert.DeserializeObject<Root>(payload);
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(1, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid ready.", root.value.message);			
			Assert.AreEqual(true, Boolean.Parse(root.value.ready.ToString()));

		}
		[Test]
		public void test02() {
			const string payload = @"
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
      },
      {
        ""id"": ""a5227c28-7e4a-4e4e-998e-1005be850447"",
        ""uri"": ""http:\u002f\u002f10.0.2.15:5556"",
        ""maxSessions"": 1,
        ""osInfo"": {
          ""arch"": ""amd64"",
          ""name"": ""Windows 10"",
          ""version"": ""10.0""
        },
        ""heartbeatPeriod"": 60000,
        ""availability"": ""DOWN"",
        ""version"": ""4.0.0 (revision 3a21814679)"",
        ""slots"": [
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""a5227c28-7e4a-4e4e-998e-1005be850447"",
              ""id"": ""2769b0ff-569e-4fd2-a8f4-f5e6b72576e5""
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
              ""hostId"": ""a5227c28-7e4a-4e4e-998e-1005be850447"",
              ""id"": ""a510d576-28e1-4982-822b-1aa3e1550ebe""
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
			Root root = JsonConvert.DeserializeObject<Root>(payload);
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(2, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid ready.",root.value.message);			
			Assert.AreEqual(true,Boolean.Parse(root.value.ready.ToString()));
		}
		
		[Test]
		public void test03() {
			const string payload = @"
{
  ""value"": {
    ""ready"": false,
    ""message"": ""Selenium Grid not ready."",
    ""nodes"": [
    ]
  }
}";
			Root root = JsonConvert.DeserializeObject<Root>(payload);
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(0, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid not ready.",root.value.message);
			Assert.AreEqual(false,Boolean.Parse(root.value.ready.ToString()));
		}
		[Test]
		public void test04() {
			const string payload = @"
{
  ""value"": {
    ""ready"": false,
    ""message"": ""Selenium Grid not ready."",
    ""nodes"": [
    ]
  }
}";
			ShareLibrariesNewtonsoft.Root root = JsonConvert.DeserializeObject<ShareLibrariesNewtonsoft.Root>(payload);
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			ShareLibrariesNewtonsoft.Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(0, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid not ready.",root.value.message);
			Assert.AreEqual(false,Boolean.Parse(root.value.ready.ToString()));
		}
	}
}
