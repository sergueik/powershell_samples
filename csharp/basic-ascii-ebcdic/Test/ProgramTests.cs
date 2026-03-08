using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Text.RegularExpressions;


using Utils;
using Program;

namespace Test {

	[TestFixture]
	public class ProgramTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private static String data;
		private static bool debug;

		// To ensure compatibility, encrypt test inputs via Java or Perl
		[SetUp]
		public void setUp() {
			data = "C8C5D3D3D6";
			debug = true;
			Program.Program.Debug = debug;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test]
		public void test() {
			var result = Program.Program.convertEBCDICToASCII(Convertor.HexStringToByteArray(data), "IBM037");
			Assert.IsNotNull(result);
			Assert.AreEqual("HELLO", Encoding.ASCII.GetString(result));
		}
	}
}