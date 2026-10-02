using System;
using System.Linq;

using NUnit.Framework;

using Utils;

namespace Test {
	[TestFixture]
	public class StringExtensionTest{
		const string text = "A1 - What Shall We Do With the Drunken Sailor - Dschinghis Khan";
		[Test]
		public void test() {
			int min = 30;
			int max = 50;
			foreach (int size in Enumerable.Range(min, max - min + 1).Reverse()) {
				var result =  text.Squeeze(size);
				Console.WriteLine(String.Format("{0} -> result: {1} / {2} chars", size, result, result.Length));
			}
		}
	}
}
