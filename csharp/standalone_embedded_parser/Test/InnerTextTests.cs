using System;
using System.Linq;
using NUnit.Framework;

using Utils; 

namespace Tests {
	[TestFixture]
    public class InnerTextTests {

		HtmlNode node;
		
        [Test]
        public void test1() {
            node = loadSingleNode("<div>Hello</div>", "div");
            Assert.AreEqual("Hello", Helper.extractInnerText(node));
        }

        [Test]
        public void test2() {
            node = loadSingleNode("<div>   Hello   </div>", "div");
            Assert.AreEqual("Hello", Helper.extractInnerText(node));
        }

        [Test]
        public void test3(){
            node = loadSingleNode("<div><!-- hi -->Hello</div>", "div");
            Assert.AreEqual("Hello", Helper.extractInnerText(node));
        }

        [Test]
        public void test4(){
            node = loadSingleNode("<!--x--><span>Hello</span>", "span");
            Assert.AreEqual("Hello", Helper.extractInnerText(node));
        }

        [Test]
        public void test5(){
            node = loadSingleNode("<td><strong>Value</strong></td>", "td");
            Assert.AreEqual("Value", Helper.extractInnerText(node));
        }
        
        
        // [Ignore]
        //  Expected: "Deep"
  		// But was:  <string.Empty>
        [Test]
        public void test6( ){
            node = loadSingleNode("<div><span><b>Deep</b></span></div>", "div");
            Assert.AreEqual("Deep", Helper.extractInnerText(node));
        }

        [Test]
        public void test7() {
            node = loadSingleNode("<div>aaa <span>bbb</span></div>", "div");
            Assert.AreEqual("bbb", Helper.extractInnerText(node));
        }

        [Test]
        public void test8() {
            node = loadSingleNode("<p><em>Inner</em> text outside</p>", "p");
            Assert.AreEqual("Inner", Helper.extractInnerText(node));
        }

        [Test]
        public void test9() {
            node = loadSingleNode("<div></div>", "div");
            Assert.AreEqual("", Helper.extractInnerText(node));
        }

        [Test]
        public void test10() {
         	node = loadSingleNode("Hello", "*");
            Assert.AreEqual("Hello", Helper.extractInnerText(node));
        }

       private HtmlNode loadSingleNode(string html, string selector = "*") {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var results = doc.QuerySelectorAll(selector).ToList();
            Assert.Greater(results.Count, 0, "Expected at least one node for selector " + selector);

            return results[0];
        }
	}
}

