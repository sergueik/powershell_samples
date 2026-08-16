using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Test {
	[TestFixture]
	public class StringExtensionTest{
		[Test]
		public void test() {
			String text = "A1 - Love Isn't Easy (But It Sure Is Hard Enough) - ABBA";
			foreach (int size in Enumerable.Range(5, 20)) {
				Console.WriteLine(String.Format("result: {0}", text.Squeeze(size)));
			}
		}
	}
}
