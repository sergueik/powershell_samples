using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Utils;

namespace Test
{
	[TestFixture]
	public class ExtensionTest
	{
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
		public static void Cleanup()
		{
		}

		[Test]
		public void test() {
			
			var date = DateTime.Today;

			var us = date.ToString("MM/dd/yyyy");
			var eu = date.ToString("dd/MM/yyyy");

			var dateDattern = @"(?<month>\d{2})/(?<day>\d{2})/(?<year>\d{4})";

			Assert.IsTrue(Regex.IsMatch(us, "^" + dateDattern + "$"));

			var dictionary = us.FindMatches(dateDattern);
			Assert.NotNull(dictionary);
			// novel NUnit 3.0 features - That, Does
			// Assert.That(dictionary.Keys, Does.Contain("month"));
			Assert.Contains("month", dictionary.Keys);
			Assert.NotNull(dictionary["month"]);

			Assert.AreEqual(date.Month.ToString("00"), dictionary["month"]);
			Assert.AreEqual(date.Day.ToString("00"), dictionary["day"]);
			Assert.AreEqual(date.Year.ToString(), dictionary["year"]);

			var resultRegex = new Regex("<([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var check = "<day>/<month>/<year>";
			var result = resultRegex.Replace(check, (Match match) => {
				string key = match.Groups[1].Value;
				// Return dictionary value resultif it exists, otherwise keep original match
				string value;
				return dictionary.TryGetValue(key, out value) ? value : match.Value;
			});
			Assert.AreEqual(eu, result);
			// one cannot pass a dictionary lookup directly like dictionary["$1"] inside Regex.Replace. $1 is a regex replacement token string, not a live variable or group value evaluated at runtime
			// System.Collections.Generic.KeyNotFoundException : The given key was not present in the dictionary
			Assert.Throws<KeyNotFoundException>(() => resultRegex.Replace(check, dictionary["$1"]));
			// the dictionary["$1"] doesn't invoke any regex machinery at all. It literally means: look up the string $1 in this dictionary.
		}

	}
}
