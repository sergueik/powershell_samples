using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

using LightCaseClient;
using LightCaseClient.Support;
using ShareLibraries;


namespace Tests
{
	[TestFixture]
	public class LightCaseClientTest
	{
		//
		
		private static string result = null;
		private Boolean browserReady = false;
		private static Regex regex;
		private static MatchCollection matches;

		
		public ClientConfiguration clientConfig = GenericProxies.DefaultConfiguration;
		[Test]
		public void test01()
		{
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

			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Root>(payload);	
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(1, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid ready.", root.value.message);			
			Assert.AreEqual(true, Boolean.Parse(root.value.ready.ToString()));

		}
		
		[Test]
		public void test02()
		{
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
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Root>(payload);	
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(2, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid ready.", root.value.message);			
			Assert.AreEqual(true, Boolean.Parse(root.value.ready.ToString()));
		}
		
		[Test]
		public void test03()
		{
			const string payload = @"
{
  ""value"": {
    ""ready"": false,
    ""message"": ""Selenium Grid not ready."",
    ""nodes"": [
    ]
  }
}";
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Root>(payload);	
			Assert.IsNotNull(root);
			Assert.IsNotNull(root.value);
			Value value = root.value;
			Assert.IsNotNull(value.nodes);
			Assert.AreEqual(0, value.nodes.Count);
			StringAssert.AreEqualIgnoringCase("Selenium Grid not ready.", root.value.message);
			Assert.AreEqual(false, Boolean.Parse(root.value.ready.ToString()));
		}


		[Test]
		public void test04()
		{
			const string payload = @"
{
  ""value"": {
    ""ready"": true,
    ""message"": ""Selenium Grid ready."",
    ""nodes"": [
      {
        ""id"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
        ""uri"": ""http://node1:5555"",
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
              ""hostId"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
              ""id"": ""2ec59c0f-9d92-4426-8a4f-2c2edceb416d""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
              ""id"": ""77ed2893-d75d-433c-b37d-552fe306da9c""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
        ""uri"": ""http://node2:5554"",
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
              ""hostId"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
              ""id"": ""588ae115-bb08-4a58-a21f-f8b6bc11cc47""
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
              ""hostId"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
              ""id"": ""aee68a44-e3d8-4aab-ac7d-6b23211b94d1""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
        ""uri"": ""http://node3:5552"",
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
              ""hostId"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
              ""id"": ""fe10eb71-a805-46a6-aa98-5ab0db2c365e""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
              ""id"": ""b9136c4a-b094-4b3d-adaf-e6370b1d1534""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
        ""uri"": ""http://node4:5551"",
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
              ""hostId"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
              ""id"": ""ea010321-5b54-4afb-9a81-1175d38b7c3d""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
              ""id"": ""b186b433-38dd-49c5-9283-d2a934eb0eaa""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
        ""uri"": ""http://node5:5553"",
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
              ""hostId"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
              ""id"": ""448140eb-86bc-443b-8aee-6bbf549ed5c4""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
              ""id"": ""9f4a5c79-39c5-4631-845b-0817cb555966""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      }
    ]
  }
}
";
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Root>(payload);
			var nodes = processDocument(root);
			Assert.AreEqual(4, nodes.Count);
			Console.WriteLine(nodes[0]);
		}
		
		// NPE value cannot be null
		[Test] 
		public void test05()
		{
			const string payload = @"
{
  ""value"": {
    ""ready"": true,
    ""message"": ""Selenium Grid ready."",
    ""nodes"": [
      {
        ""id"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
        ""uri"": ""http://node00:5555"",
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
              ""hostId"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
              ""id"": ""2ec59c0f-9d92-4426-8a4f-2c2edceb416d""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""c0cdd050-8012-49d2-a841-e63d188c4b61"",
              ""id"": ""77ed2893-d75d-433c-b37d-552fe306da9c""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
        ""uri"": ""http://node01:5555"",
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
              ""hostId"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
              ""id"": ""588ae115-bb08-4a58-a21f-f8b6bc11cc47""
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
              ""hostId"": ""38cf6aeb-9b2c-4656-972f-e0a217c87e8c"",
              ""id"": ""aee68a44-e3d8-4aab-ac7d-6b23211b94d1""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
        ""uri"": ""http://node02:5555"",
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
              ""hostId"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
              ""id"": ""fe10eb71-a805-46a6-aa98-5ab0db2c365e""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""5bba6684-0c39-40d8-82c6-6fa9678fc472"",
              ""id"": ""b9136c4a-b094-4b3d-adaf-e6370b1d1534""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
        ""uri"": ""http://node03:5555"",
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
              ""hostId"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
              ""id"": ""ea010321-5b54-4afb-9a81-1175d38b7c3d""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""8a6258d8-d606-4391-b9f5-42dad1f38802"",
              ""id"": ""b186b433-38dd-49c5-9283-d2a934eb0eaa""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
        ""uri"": ""http://node04:5555"",
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
              ""hostId"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
              ""id"": ""448140eb-86bc-443b-8aee-6bbf549ed5c4""
            },
            ""stereotype"": {
              ""browserName"": ""chrome"",
              ""platformName"": ""WIN10""
            }
          },
          {
            ""lastStarted"": ""1970-01-01T00:00:00Z"",
            ""session"": null,
            ""id"": {
              ""hostId"": ""38d1296a-0fbe-4692-a0fb-17bed8e5558b"",
              ""id"": ""9f4a5c79-39c5-4631-845b-0817cb555966""
            },
            ""stereotype"": {
              ""browserName"": ""firefox"",
              ""platformName"": ""WIN10""
            }
          }
        ]
      },
      {
        ""id"": ""node1 added manually"",
        ""uri"": null,
        ""maxSessions"": 0,
        ""osInfo"": null,
        ""heartbeatPeriod"": 0,
        ""availability"": ""UP"",
        ""version"": null,
        ""slots"": []
      },
      {
        ""id"": ""node2 added manually"",
        ""uri"": null,
        ""maxSessions"": 0,
        ""osInfo"": null,
        ""heartbeatPeriod"": 0,
        ""availability"": ""DOWN"",
        ""version"": null,
        ""slots"": []
      }
    ]
  }
}

";
			var root = clientConfig.InBoundSerializerAdapter.Deserialize<Root>(payload);
			var nodes = new List<String>(); 
			try {
				nodes = processDocument(root);
			} catch (Exception ex) {
				Console.Error.WriteLine("Failed to call the service - " + ex.Message);
			}				
			Assert.AreEqual(5, nodes.Count);
			Console.WriteLine(nodes[0]);

		}

		

		// based on: https://github.com/sergueik/powershell_selenium/blob/master/csharp/protractor-net/Extensions/Extensions.cs
		private static string FindMatch(string text, string matchPattern, string matchTag)
		{
			result = null;
			regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			matches = regex.Matches(text);
			foreach (Match match in matches) {
				if (match.Length != 0) {
					foreach (Capture capture in match.Groups[matchTag].Captures) {
						if (result == null) {
							result = capture.ToString();
						}
					}
				}
			}
			return result;
		}
		
		private List<String> processDocument(Root root)
		{
			List<Node> nodes = root.value.nodes;
			var nodeNames = new List<string>();
			foreach (var node in nodes) {
				if (node.availability != null && node.availability.CompareTo("UP") == 0) {
					String text = node.uri;
					// to provoke NPE, uncomment
					if (text != null) {
						var hostname = FindMatch(text, @"^http://(?<hostname>[A-Z0-9-._]+):\d+$", "hostname");
						nodeNames.Add((hostname == null) ? text : hostname);
					} else {
						Console.Error.WriteLine(String.Format("Ignored node with empty uri: {0}", node.id));
					}
				}
			}
			return nodeNames;
		}
		
				[Test] 
		public void test06(){
					var payload = @"
					{
  ""commentText"": ""1"",
  ""userName"": ""testPlayer"",
  ""userAvatar"": """"
}
					";
		}
	}
}
