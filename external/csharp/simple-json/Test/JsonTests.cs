using System;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Tests
{
	[TestFixture]
	public class JsonTests
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
        ""availability"": ""DOWN"",
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

		int cnt = 0;
		SimpleJSON.JSONNode childNode = null;
		SimpleJSON.JSONNode foundNode = null;
		// NOTE: do not initilize
		SimpleJSON.JSONNode.KeyEnumerator keys;
		IEnumerator<SimpleJSON.JSONNode> children = null;
		int position = 0;
		String key;
		ArrayList items = new ArrayList();
		SimpleJSON.JSONNode root;
		SimpleJSON.JSONNode value;
		SimpleJSON.JSONNode node;
		SimpleJSON.JSONArray nodes;
		
		[TestFixtureSetUp]
		public void SetUp() {
			// root = SimpleJSON.JSONNode.Parse(payload);
			root = SimpleJSON.JSONNode.Parse(payload).AsObject;
			Assert.NotNull(root);

		}

		[Test]
		public void test1() {
			Assert.NotNull(root);
			Console.Error.WriteLine(root.ToString());
		}

		[Test]
		public void test2(){
			keys = root.Keys.GetEnumerator();
			Assert.NotNull(keys);
			key = keys.Current;
			Console.Error.WriteLine("Keys: " + keys.ToString());
			cnt = 0;
			position = 0;
			while (keys.MoveNext()) {
				key = keys.Current;
				Assert.NotNull(key);
				Console.Error.WriteLine(String.Format("Key [{0}]: {1}", cnt, key.ToString()));			
				if (key.Equals("value")) {
					position = cnt;
				}
				cnt++;
			}
			
			children = root.Children.GetEnumerator();
			cnt = 0;
			childNode = null;
			foundNode = null;
			while (children.MoveNext()) {
				childNode = children.Current;
				Assert.NotNull(childNode);
				Console.Error.WriteLine(String.Format("Child [{0}]: {1}", cnt, childNode.ToString()));
				if (cnt == position) {
					foundNode = childNode;
				}
				cnt++;				
			}
			Assert.NotNull(foundNode);
			keys = foundNode.Keys.GetEnumerator();
			Assert.NotNull(keys);
			
			Console.Error.WriteLine("Keys: " + keys.ToString());
			cnt = 0;
			position = 0;
			while (keys.MoveNext()) {
				key = keys.Current;
				Assert.NotNull(key);
				Console.Error.WriteLine(String.Format("Key [{0}]: {1}", cnt, key.ToString()));			
				if (key.Equals("nodes")) {
					position = cnt;
				}
				cnt++;
			}			
		}

		[Test]
		public void test3() {
			items = new ArrayList();
			foreach (var item in root.Keys.GetEnumerator()) { 
				items.Add(item);
			}
			// root.Keys
			Assert.Contains("value", items); 
			value = root["value"];
			Assert.NotNull(value);
			items = new ArrayList();
			foreach (var item in value.Keys.GetEnumerator()) { 
				items.Add(item);
			}
			// value.Keys
			Assert.Contains("nodes", items); 
			nodes = value["nodes"].AsArray;
			Assert.AreEqual(5, nodes.Count);

			node = nodes[0];
			items = new ArrayList();
			foreach (var item in node.Keys.GetEnumerator()) { 
				items.Add(item);
			}
			// node.Keys
			Assert.Contains("availability", items);
			Assert.IsTrue(node["availability"].IsString);
			Assert.AreEqual(@"""UP""", node["availability"].ToString());
			Assert.IsTrue(new Regex("(UP|DOWN)", RegexOptions.IgnoreCase).IsMatch(node["availability"].ToString()));			
		}

		[Test]
		public void test4() {
			nodes = root["value"].AsObject["nodes"].AsArray;
			var total_nodes_up_count = 0;
			for (cnt = 0; cnt != nodes.Count; cnt++) {
				childNode = nodes[cnt];
				Console.Error.WriteLine(String.Format("node {0} is {1}", childNode["uri"], childNode["availability"].ToString()));
				if (childNode["availability"].ToString().ToUpper().Contains("UP")) {
					foundNode = childNode;
					total_nodes_up_count++;
				}
				
			}
			Assert.AreEqual(3, total_nodes_up_count);
		}
	}
}
