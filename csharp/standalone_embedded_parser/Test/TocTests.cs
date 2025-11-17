using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using System.Collections.Generic;
using NUnit.Framework;

using Utils;

namespace Tests {
	[TestFixture]
	public class TocTests {
		
		private HtmlNode element  = null;
		private IEnumerable<HtmlNode> elements = null;
		private HtmlDocument htmlDocument = null;
		private const string filename = "toc.hhc";
		private string name = null;
		private string local  = null;


		[Test]
		public void test1() {
			htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(File.ReadAllText(filename));

			foreach (var element in htmlDocument.GetElementsByTagName("OBJECT")) {
				foreach (var element2 in element.QuerySelectorAll("param[name=\"Name\"]")) {
					Assert.NotNull(element2); 
					printElement(element2);
				}
				foreach (var element2 in element.QuerySelectorAll("param[name=\"Local\"]")) {
					Assert.NotNull(element2); 
					printElement(element2);
				}
			}
		}

		[Test]
		public void test2() {
			htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(File.ReadAllText(filename));
			
			foreach (var element in htmlDocument.GetElementsByTagName("OBJECT")) {
			var elements1  =  element.QuerySelectorAll("param[name=\"Name\"]");
			var elements2  =  element.QuerySelectorAll("param[name=\"Local\"]");
			int cnt1 = 0;
			var enumerator1 = elements1.GetEnumerator();
			while (enumerator1.MoveNext())
				cnt1++;
			int cnt2 = 0;
			var enumerator2 = elements2.GetEnumerator();
			while (enumerator2.MoveNext())
				cnt2++;
					Assert.AreEqual(cnt2, cnt1); 
			}
		}
		[Test]
		public void test3() {
			string payload = File.ReadAllText(filename);
			var tokens = parseToc(payload);
			Assert.IsNotNull(tokens, "The toclist list should not be null");
            Assert.IsNotEmpty(tokens, "The toclist list should not be empty");

            foreach (var entry in tokens) {
                Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
                Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
            }

		}
		private void printElement(HtmlNode element) {
			Console.WriteLine(String.Format("element name: \"{0}\" " +  
			                                " value: \"{1}\"" , 
			                                element.GetAttribute("name"),
			                                element.GetAttribute("value")
							));
		}

		public List<TocEntry> parseToc(String payload) {
			var result = new List<TocEntry>();
			
            // Extract <OBJECT><PARAM name="Name"><PARAM name="Local">

			htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(payload);

			foreach (var element1 in htmlDocument.GetElementsByTagName("OBJECT")) {
				foreach (var element2 in element1.QuerySelectorAll("param[name=\"Name\"]")) {
					name =  element2.GetAttribute("value");
				}
				foreach (var element2 in element1.QuerySelectorAll("param[name=\"Local\"]")) {
					local = element2.GetAttribute("value"); 
				}
				result.Add(new TocEntry {
                    Name = name,
                    Local = local
                });

			}

			return result;			
		}

	}
	
	public class TocEntry {
		public string Name { get; set; }
		public string Local { get; set; }
	}
}

