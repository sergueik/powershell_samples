using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
	[TestFixture]
	public class BasicTests {
		private HtmlNode element  = null;
		private IEnumerable<HtmlNode> elements = null;
		private HtmlDocument htmlDocument = null;
		private const string filename = "ch09s33.html";

		[SetUp]
		public void  setup() {
			htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(File.ReadAllText(filename));
		}

		[Test]
		public void test1() {
			Console.Error.WriteLine("{0}", htmlDocument.DocumentElement.InnerText); 
		}

		[Test]
		public void test2() {
			foreach (var element in htmlDocument.GetElementsByTagName("code"))
				printElement(element);
		}

		[Test]
		public void test3() {
			var doc = new HtmlDocument();
			doc.LoadHtml(File.ReadAllText("ch09s33.html"));
			foreach (var element in doc.GetElementsByTagName("table")) {
				foreach (var element2 in element.QuerySelectorAll("code[class]")) {
					Assert.NotNull(element2); 
					printElement(element2);
				}
			}
		}

		[Test]
		public void test4() {
			elements = htmlDocument.GetElementsByTagName("table");
			Console.WriteLine(String.Format("elements type: \"{0}\"", elements.GetType()));
			// Utils.HtmlNode+<GetElementsByTagName>d__0

			int cnt = 0;
			var enumerator = elements.GetEnumerator();
			while (enumerator.MoveNext()) {
				cnt++;
			}
			Console.WriteLine(String.Format("elements count: \"{0}\"", cnt));
			Assert.Greater(cnt, 0, "expect at least one element");
			try {
				enumerator.Reset();
			} catch (NotSupportedException) {
				// ignore:
				// System.NotSupportedException : 
				// Specified method is not supported. 
			}
			elements.GetEnumerator().MoveNext();
			element = elements.GetEnumerator().Current;
			Assert.Null(element); 

			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element); 
			printElement(element);
			
			// Type and identifier are both required in a foreach statement
			foreach (var element1 in element.QuerySelectorAll("code[class]")) {
				printElement(element1);
			}
		}
		
		[Test]
		public void test5() {
			elements = htmlDocument.GetElementsByTagName("table");
			var enumerator = elements.GetEnumerator();
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element); 
			
			// A local variable named 'element1' cannot be declared in this scope because it would give a different meaning to 'element1', which is already used in a 'parent or current' scope to denote something else (CS0136)
			
			foreach (var element1 in element.QuerySelectorAll("span.bold")) {
				foreach (var element2 in element1.QuerySelectorAll("strong")) {
					printElement(element2);
				}
			}
		}
	
		[Test]
		public void test6() {
			IEnumerator<HtmlNode> enumerator = null;
			elements = htmlDocument.GetElementsByTagName("table");
			enumerator = elements.GetEnumerator();
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element); 

			elements = element.QuerySelectorAll("#idp13365488");			
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element); 

			elements = element.QuerySelectorAll("table[summary=\"PC speaker configuration options\"]");			
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element); 

			elements = htmlDocument.DocumentElement.Children[0].QuerySelectorAll("table");
			enumerator = elements.GetEnumerator();
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element);
			
			elements = htmlDocument.QuerySelectorAll("table[summary]");
			enumerator = elements.GetEnumerator();
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element);
	
			elements = element.QuerySelectorAll("table[summary=\"PC speaker configuration options\"] .computeroutput");			
			enumerator.MoveNext();
			element = enumerator.Current;
			Assert.NotNull(element);
			printElement(element);

		}

		[Test]
		public void test7() {
			Console.WriteLine(String.Format("DocumentElement Children: {0} \"{1}\"",
			                                htmlDocument.DocumentElement.Children.Count,
			                                htmlDocument.DocumentElement.Children[0].TagName
			                               ));
		}

		private void printElement(HtmlNode element) {
			Console.WriteLine(String.Format("element tag name: \"{0}\" " + 
			                                " innerText: \"{1}\"  " +  
			                                " innerText: \"{2}\"  " +  
			                                "class: \"{3}\"", element.TagName, 
			                                element.Children[0].InnerText,
											element.InnerText,			                                
			                                element.GetAttribute("class")));
		}
	}
}

