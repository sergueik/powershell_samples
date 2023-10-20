using System;
using System.Configuration;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExampleApplication {
	[TestFixture]
	public class ConfigTest {

		[Test]
		public void test01() {
			var configSection = ConfigurationManager.GetSection("FizzBuzz") as UrlsSection;
			if (configSection == null)
				Console.WriteLine("Failed to load FizzBuzz Section.");
			else {
				Console.WriteLine("The urls collection {0} of app.config:", configSection.Name);
				foreach (UrlConfigElement url in configSection.Urls) {
					Console.WriteLine("  Name={0} URL={1} Port={2}", url.Name, url.Url, url.Port);
					if (url.Content != null && url.Content.Value != null)
						Console.WriteLine(@" Content: ""{0}""", url.Content.Value
						);
				}
			}
			var tags = new List<string> { "sql1", "sql2", "sql3" };
			foreach (string tag in tags) {

				var stats = ConfigurationManager.GetSection(tag) as SqlElement;
				if (stats == null)
					Console.WriteLine(@"Failed to load sql Section ""{0}""", tag);
				else if (stats.Name != "")
					Console.WriteLine(@"The sql Section """ + tag + @""" name: """ + stats.Name + @" ""query"" element value:" + "\n" + stats.Query.Value + "\n");
			}
		}
	}
}
