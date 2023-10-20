using System;
using System.Collections.Generic;
using NUnit.Framework;
using MIL.Html;

namespace UnitTests {
	[TestFixture]
	public class Tests {
		private string payload;
		[TestFixtureSetUp]
		public void SetUp() {
			payload = @"
			<?xml version=""1.0""?>
<html>
  <head>
    <script src=""//ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js"">
</script>
    <script src=""/grid/resources/org/openqa/grid/images/console-beta.js"">
</script>
    <link href=""/grid/resources/org/openqa/grid/images/console-beta.css"" rel=""stylesheet"" type=""text/css""/>
    <link href=""/grid/resources/org/openqa/grid/images/favicon.ico"" rel=""icon"" type=""image/x-icon""/>
    <title>
Grid Console</title>
    <style>
.busy { opacity : 0.4;filter: alpha(opacity=40);}</style>
  </head>
  <body>
    <div id=""main_content"">
      <div id=""header"">
        <h1>
          <a href=""/grid/console"">
Selenium</a>
        </h1>
        <h2>
Grid Console v.2.53.0</h2>
        <div>
          <a id=""helplink"" target=""_blank"" href=""https://github.com/SeleniumHQ/selenium/wiki/Grid2"">
Help</a>
        </div>
      </div>
      <div id=""leftColumn"">
        <div class=""proxy"">
          <p class=""proxyname"">
DefaultRemoteProxy (version : 2.53.0)<p class=""proxyid"">
id : http://SERGUEIK53:5555, OS : WIN8_1</p>
<div class=""tabs""><ul><li class=""tab"" type=""browsers""><a title=""test slots"" href=""#"">
Browsers</a></li><li class=""tab"" type=""config""><a title=""node configuration"" href=""#"">
Configuration</a></li></ul></div>
</p>
        </div>
        <div class=""proxy"">
          <p class=""proxyname"">
DefaultRemoteProxy (version : 2.53.0)<p class=""proxyid"">
id : http://SERGUEIK53:5556, OS : WIN8_1</p>
<div class=""tabs""><ul><li class=""tab"" type=""browsers""><a title=""test slots"" href=""#"">
Browsers</a></li><li class=""tab"" type=""config""><a title=""node configuration"" href=""#"">
Configuration</a></li></ul></div>
</p>
        </div>
      </div>
      <div id=""rightColumn"">
        <div class=""proxy"">
          <p class=""proxyname"">
DefaultRemoteProxy (version : 2.53.0)<p class=""proxyid"">
id : http://SERGUEIK53:5557, OS : WIN8_1</p>
<div class=""tabs""><ul><li class=""tab"" type=""browsers""><a title=""test slots"" href=""#"">
Browsers</a></li><li class=""tab"" type=""config""><a title=""node configuration"" href=""#"">
Configuration</a></li></ul></div>
</p>
        </div>
        <div class=""proxy"">
          <p class=""proxyname"">
DefaultRemoteProxy (version : 2.53.0)<p class=""proxyid"">
id : http://SERGUEIK53:5558, OS : WIN8_1</p>
<div class=""tabs""><ul><li class=""tab"" type=""browsers""><a title=""test slots"" href=""#"">
Browsers</a></li><li class=""tab"" type=""config""><a title=""node configuration"" href=""#"">
Configuration</a></li></ul></div>
</p>
        </div>
      </div>
    </div>
    <div class=""clearfix"">
</div>
    <div>
      <ul>
</ul>
    </div>
    <a href=""?config=true"">view config</a>
  </body>
</html>
";

		}
		
		[Test]
		public void test01() {
			
			var document = MIL.Html.HtmlDocument.Create(payload, false);
			Assert.IsNotNull(document);
			string value = document.HTML;
			
			Assert.IsNotNull(value);
			var nodes = document.Nodes;

			Assert.IsNotNull(nodes);
			Assert.AreEqual(2, nodes.Count);
			StringAssert.AreEqualIgnoringCase(@"<?xml version=""1.0"" ?>", nodes[0].ToString());
			Assert.AreEqual(true, nodes[1].IsRoot);
			var nodes2 = nodes.FindByName("html");
			Assert.AreEqual(1, nodes2.Count);
			nodes2 = nodes.FindByName("p");
			Assert.AreEqual(8, nodes2.Count);
			nodes2 = nodes.FindByAttributeNameValue("class", "proxyid");
			Assert.AreEqual(4, nodes2.Count);
			
			Assert.IsTrue(nodes2[0].IsParent);
			var node2 = nodes2[0].FirstChild;
			Assert.IsTrue(node2.IsText());
			StringAssert.AreEqualIgnoringCase("id : http://SERGUEIK53:5555, OS : WIN8_1", node2.ToString());
			StringAssert.AreEqualIgnoringCase(@"<p class=""proxyid"">id : http://SERGUEIK53:5555, OS : WIN8_1</p>", nodes2[0].HTML);
			
			var element = (MIL.Html.HtmlElement)nodes2[0];
			StringAssert.AreEqualIgnoringCase("id : http://SERGUEIK53:5555, OS : WIN8_1", element.Text);
		}

		// NOTE: temporarily made MIL.Html.Parser public - this does not give one much - all relevant methods are provided by MIL.Html.HtmlDocument
		[Test]
		public void test02() {
			var parser = new HtmlParser();
			var nodes = parser.Parse(payload);
			Assert.IsNotNull(nodes);
			// NOTE: whitespace is not removed and counts as extra node
			Assert.AreEqual(5, nodes.Count);
			Assert.AreEqual(true, nodes[1].IsRoot);

			int cnt = 0;
			foreach (var node in nodes) {
				Console.Error.WriteLine("node {0}:\n{1}", cnt, node.ToString());
				cnt++;
			}
			var nodes2 = nodes.FindByName("html");
			Assert.AreEqual(1, nodes2.Count);
			nodes2 = nodes.FindByName("p");
			Assert.AreEqual(8, nodes2.Count);

			nodes2 = nodes.FindByAttributeNameValue("class", "proxyid");
			Assert.AreEqual(4, nodes2.Count);
			
			Assert.IsTrue(nodes2[0].IsParent);
			StringAssert.AreEqualIgnoringCase(@"<p class=""proxyid"">" + "\r\n" + "id : http://SERGUEIK53:5555, OS : WIN8_1</p>", nodes2[0].HTML);
			var node2 = nodes2[0].FirstChild;
			Assert.IsTrue(node2.IsText());
			StringAssert.AreEqualIgnoringCase("\r\nid : http://SERGUEIK53:5555, OS : WIN8_1", node2.ToString());

			var element = (MIL.Html.HtmlElement)nodes2[0];
			StringAssert.AreEqualIgnoringCase("\r\nid : http://SERGUEIK53:5555, OS : WIN8_1", element.Text);
		}

		// NOTE:
		// combining the FineByName with FindByAttributeNameValue does not appears to work right:
		[Test]
		public void test03() {
			int cnt = 0;
			var document = MIL.Html.HtmlDocument.Create(payload, false);
			var nodes = document.Nodes;
			var nodes2 = nodes.FindByName("p");
			foreach (var node in nodes2) {
				var element = (MIL.Html.HtmlElement)node;
				Console.Error.WriteLine("node {0}: class = {1}\n{2}", cnt, element.Attributes.FindByName("class"), element.HTML);
				cnt++;
			}
			var nodes3 = nodes2.FindByAttributeNameValue("class", "proxyid");
			cnt = 0;
			foreach (var node in nodes3) {
				var element = (MIL.Html.HtmlElement)node;
				Console.Error.WriteLine("node {0}: class = {1}\n{2}", cnt, element.Attributes.FindByName("class"), element.HTML);
				cnt++;
			}
			// NOTE: getting 8 nodes, instead of 4
			// actually the nodes are simply nested
			Assert.Throws<AssertionException>(
				() => {
					Assert.AreEqual(4, nodes3.Count);
				});
			
		}

	}
}
