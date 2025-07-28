using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using System.Collections;
using System.Reflection;
using Utils;


namespace Tests {
	[TestFixture]
	public class MarkdownConvertorTests {
		private  MarkdownConvertor helper = new MarkdownConvertor();
		private string payload;
		private string result;

		[Test]
		public void test() {
			payload = "**Hello world!**";

			result = helper.convert(payload);
			Console.Error.WriteLine("HTML " + result);
			Assert.IsNotNull(result);
			StringAssert.Contains("<strong>Hello world!</strong>", result);
		}
	}
}
