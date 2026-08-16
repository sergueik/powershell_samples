using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class ExtensionTest {
		// NOTE: unused
		private const string name = @"(01) Artist Title";
		private const string matchPattern = @"\\((?<index>[AB0-9][0-9]+)\\)\\s+(?<artist>[^ ].+[^ ])\\s+(?<title>[^ ].+)";
		private const string newname = @"<index> - <title> - <artist>";
		private TestContext testContextInstance;

		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestFixtureTearDown]
		public static void Cleanup() { }

		[Test]
		public void test() {
			var date = "2026-Aug 18";
			var dateDattern = @"\d{4}\-(?<month>\w{3})";
			var dateDatternReg = new Regex(dateDattern);
			Assert.IsTrue(dateDatternReg.IsMatch(date));
			String month = date.FindMatch(dateDattern);
			Console.WriteLine(String.Format("month:{0}", month));
			var dictionary = date.FindMatches(dateDattern);
			Assert.NotNull(dictionary);
			Assert.Contains("month", dictionary.Keys);
			Assert.NotNull(dictionary["month"]);
			var resultRegex = new Regex("<([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var check = "the month is <month>";
			Console.WriteLine(String.Format("result:{0}", resultRegex.Replace(check, (Match match) => {
				string key = match.Groups[1].Value;
				// Return dictionary value if it exists, otherwise keep original match
				string value;
				return dictionary.TryGetValue(key, out value) ? value : match.Value;
			})));
			// one cannot pass a dictionary lookup directly like dictionary["$1"] inside Regex.Replace. $1 is a regex replacement token string, not a live variable or group value evaluated at runtime
			// System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary
			Assert.Throws<KeyNotFoundException>(() => resultRegex.Replace(check, dictionary["$1"]));
		}

	}
}
