using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test {

	[TestFixture]
	public class  ParsePublishedPortsTest {
		
		private string data = null;
		private HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}
		[SetUp]
		public void SetUp() {
			results = new HashSet<string>();
data = @"
		0.0.0.0:8443->8443/tcp,
 [::]:8443->8443/tcp,
 0.0.0.0:8080->8080/tcp,
 [::]:8080->8080/tcp
		";
		// expect to find IPv4 published ports:		
		// 8443->8443/tcp		
		// 8081->8080/tcp
		// and have the IPv6 entries are deliberately excluded
		// List<PublishedPort> ports = DockerHelper.ParsePublishedPorts(container.Ports);
		// 0.0.0.0 : 8081 -> 8080 / tcp
		//      │       │
		//      │       └── container port
		//      └────────── published host port
		}

		[TestFixtureTearDown]
		public static void Cleanup() {
		}

		[Test]
		public void test1() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.string.split?view=netframework-4.5#system-string-split(system-char())
			var tokens = data.Replace("\r", "").Replace("\n", " ").Split(new char[]{ ',' });
			foreach (var token in tokens) {

				var publishedPortPattern = @"(?<host_address>(?:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}|\[::\])):(?<host_port>\d{2,6})->(?<container_port>\d{2,6})/(?<protocol>(?:tcp|udp))";

				// NOTE: publishedPortPattern = @"(?<host_address>0.0.0.0):(?<host_port>8443)->(?<container_port>8443)/(?<protocol>tcp) *";
				// System.ArgumentException : parsing "^(?<host address>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/(?<host port>\d{2,4})->(?<container port>\d{2,4})/(?<protocol>(?:tcp|udp))$" -
				// Invalid group name: Group names must begin with a word character. - 
				Assert.IsTrue(Regex.IsMatch(token.Trim(), "^" + publishedPortPattern + "$"), String.Format("\"{0}\" is not match", token));

				var dictionary = token.FindMatches(publishedPortPattern);
				Assert.NotNull(dictionary);
				// novel NUnit 3.0 features - That, Does
				// Assert.That(dictionary.Keys, Does.Contain("host_addres"));
				Assert.Contains("host_address", dictionary.Keys);
				Assert.NotNull(dictionary["host_address"]);
				var publishedPort = new PublishedPort();
				publishedPort.hostAddress = dictionary["host_address"];
				publishedPort.hostPort = dictionary["host_port"];

				publishedPort.containerPort = dictionary["container_port"];
				publishedPort.protocol = dictionary["protocol"];
				results.Add(publishedPort.hostPort);
			
			}
			// NOTE: Argument 2: cannot convert from 'System.Collections.Generic.HashSet<string>' to 'System.Collections.ICollection' (CS1503)
			// Assert.Contains("8080", results);
			Assert.IsTrue(results.Contains("8080"));
			Assert.IsTrue(results.Contains("8443"));
			Debug.WriteLine(String.Format("Results: {0}", String.Join(",", results.ToList())));
		}
	}
	class PublishedPort {
		public string hostAddress { get; set; }
		public string hostPort { get; set; }
		public string containerPort { get; set; }
		public string protocol { get; set; }		
	}
}
