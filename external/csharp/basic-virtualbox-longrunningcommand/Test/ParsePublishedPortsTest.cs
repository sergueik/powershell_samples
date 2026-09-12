using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Utils;

namespace Test {

	[TestFixture]
	public class  ParsePublishedPortsTest {
		
		private string result = null;
		// private NameValueCollection appSettings;
		private Regex regex = new Regex("%(?<token>[A-Z0-9_]+)%");
		private HashSet<string> resolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, string> values = new Dictionary<string, string>();
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}
		private string data = null;
		[SetUp]
		public void SetUp() {

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
		public void test() {
			
			var tokens = data.Replace("\r", "").Replace("\n", " ").Split(new char[]{','});
					foreach (var token in tokens) {

			var publishedPortPattern = @"\s*(?<host_address>\d{1,3}\.\d{1,3}\.\d{1,3}\.?(?:\d{1,3})?):(?<host_port>\d{2,6})->(?<container_port>\d{2,6})/(?<protocol>(?:tcp|udp))\s*";

			// publishedPortPattern = @"(?<host_address>0.0.0.0):(?<host_port>8443)->(?<container_port>8443)/(?<protocol>tcp) *";
			// System.ArgumentException : parsing "^(?<host address>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/(?<host port>\d{2,4})->(?<container port>\d{2,4})/(?<protocol>(?:tcp|udp))$" -
//			Invalid group name: Group names must begin with a word character. - 
			Assert.IsTrue(Regex.IsMatch(token, "^" + publishedPortPattern + "$"),String.Format("\"{0}\" is not match", token));

			var dictionary = token.FindMatches(publishedPortPattern);
			Assert.NotNull(dictionary);
			// novel NUnit 3.0 features - That, Does
			// Assert.That(dictionary.Keys, Does.Contain("month"));
			Assert.Contains("host_address", dictionary.Keys);
			Assert.NotNull(dictionary["host_address"]);
			/*
			Assert.AreEqual(date.Month.ToString("00"), dictionary["month"]);
			Assert.AreEqual(date.Day.ToString("00"), dictionary["day"]);
			Assert.AreEqual(date.Year.ToString(), dictionary["year"]);

			var resultRegex = new Regex("<([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var check = "<day>/<month>/<year>";
			var result = resultRegex.Replace(check, (Match match) => {
				string key = match.Groups[1].Value;
				string value;
				return dictionary.TryGetValue(key, out value) ? value : match.Value;
			});
			Assert.AreEqual(eu, result);
			Console.WriteLine(dictionary.PrettyPrint());
			// one cannot pass a dictionary lookup directly like dictionary["$1"] inside Regex.Replace. $1 is a regex replacement token string, not a live variable or group value evaluated at runtime
			// System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary
			Assert.Throws<KeyNotFoundException>(() => resultRegex.Replace(check, dictionary["$1"]));
			// the dictionary["$1"] doesn't invoke any regex machinery at all. It literally means: look up the string $1 in this dictionary.
			*/
			}
		}
	}
	class PublishedPort {
		public string hostAddress { get; set; }
		public string hostPort { get; set; }
		public string containerPort { get; set; }
		public string protocol { get; set; }		
	}
}
