using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Test {

	[TestFixture]
	public class ParseArgsTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private ParseArgs parseArgs;

		private static readonly string url = "http://www.google.com?q=123";
		private static readonly int number = 42;
		private static readonly string commandLine = String.Format("-url:{0} -number:{1} -debug -arg_with_space=\"a test\" -arg_with_repeated_spaces=\"another test  with    space\"", url, number);

		[SetUp]
		public void setUp() {
			verificationErrors.Clear();
			parseArgs = new ParseArgs(commandLine);
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test]
		public void test1() {
			String result = parseArgs.GetMacro("url");
			StringAssert.Contains(url, result);
			Assert.AreEqual(url, result);
		}

		[Test]
		public void test2() {
			var result = parseArgs.GetMacro("debug");
			Assert.IsNotNull(result);
			StringAssert.AreEqualIgnoringCase("TRUE", result);
			var debug = Boolean.Parse(parseArgs.GetMacro("debug"));
			Assert.AreEqual(true, debug);
		}

		[Test]
		public void test3() {
			var result = parseArgs.GetMacro("arg_with_space");
			Assert.IsNotNull(result);
			StringAssert.Contains(" ", result);
			Assert.AreEqual("a test", result);
		}

		[Test]
		public void test4() {
			var result = parseArgs.GetMacro("arg_with_repeated_spaces");
			Assert.IsNotNull(result);
			StringAssert.Contains("  ", result);
			Assert.AreEqual("another test  with    space", result);
		}

		[Test]
		public void test5() {
			var tokens = ParseArgs.splitTokens(commandLine);
			Assert.AreEqual(5, tokens.Length);

			Console.Error.WriteLine("Length: " + tokens.Length);
			StringAssert.Contains(" ", tokens[3]);
			StringAssert.Contains("  ", tokens[4]);
			StringAssert.IsMatch("[^ ]+" , tokens[0]);
			for (var cnt = 0; cnt != tokens.Length; cnt++)
				Console.Error.WriteLine("token: " + tokens[cnt]);
		}

	}
}
