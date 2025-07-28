using System;
using NUnit.Framework;
using Utils;


namespace Tests {
	[TestFixture]
	public class MarkdownConvertorTests {
		private  MarkdownConvertor helper = new MarkdownConvertor();
		private string payload;
		private string result;

		[Test]
		public void test1() {
			payload = "**Hello world!**";

			result = helper.convert(payload);
			Console.Error.WriteLine("HTML " + result);
			Assert.IsNotNull(result);
			StringAssert.Contains("<strong>Hello world!</strong>", result);
		}

		[Test]
		public void test2() {
			payload = @"|         |            |  
|----------------------|---------|
| Hello world!     | Hello world!        | Hello world!     |  
| 1    | 2        | 3     |  
";

			result = helper.convert(payload);
			Console.Error.WriteLine("HTML " + result);
			Assert.IsNotNull(result);
			StringAssert.Contains("<td>Hello world!</td>", result);
		}
		[Test]
		public void test3() {
			payload = @"```
			Hello world!
```";

			result = helper.convert(payload);
			Console.Error.WriteLine("HTML " + result);
			Assert.IsNotNull(result);
			StringAssert.Contains(@"<pre><code>", result);
		}

	}
}
