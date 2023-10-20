using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using Utils;
using Program;
using System.Text.RegularExpressions;

namespace Test {

	[TestFixture]
	public class ProgramTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private static String plainValue;
		private static String encryptedValue;
		private static String badEncryptedValue;
		private static String saltString;
		private static String password;
		private static bool debug;
		private static String operation;
		private static int obtentionIterations;
		const int segments = 1;

		// To ensure compatibility, encrypt test inputs via Java or Perl
		[SetUp]
		public void setUp() {
			saltString = "fd2b12742f751d0b";
			password = "apple";
			operation = "encrypt";
			obtentionIterations = 1000;
			debug = false;
			plainValue = "message";
			encryptedValue = "/SsSdC91HQvvrC2pBmOrNQ==";
			Program.Program.Debug = debug;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void encryptTest1() {
			String value = Program.Program.Encrypt(plainValue, password, saltString, obtentionIterations, segments);
			StringAssert.Contains(encryptedValue, value);
		}

		[Test] 
		public void encryptTest2() {
			String value = Program.Program.Encrypt(plainValue, password, saltString, obtentionIterations, segments);
			byte[] salt = Utils.Convertor.HexStringToByteArray(saltString);
			// remove the padding
			var regex = new Regex("=+$");

			var head = regex.Replace(Convert.ToBase64String(salt), "");
			// remove the very last character in the salt base64 string before padding
			// NOTE:
			// Expected: String starting with "/SsSdC91HQs"
			// but was:  "/SsSdC91HQvvrC2pBmOrNQ=="
			// NOTE the last letter before the padding part
			head = head.Substring(0, head.Length - 1);
			StringAssert.StartsWith(head, value);
		}

		[Test]
		public void decryptTest1() {
			String value = Program.Program.Decrypt(encryptedValue, password, obtentionIterations, segments);
			StringAssert.IsMatch(plainValue, value);

		}

		// [Ignore]
		// https://docs.nunit.org/articles/nunit/writing-tests/assertions/classic-assertions/Assert.Throws.html
		[Test]
		public void inputTest1() {
			Assert.Throws<System.FormatException>(
				() => {
					badEncryptedValue = "something";
					Program.Program.Debug = true;
					Program.Program.Decrypt(badEncryptedValue, password, obtentionIterations, segments);
				});
		}
	
		[Test]
		public void inputTest2() {
			Assert.Throws<System.OverflowException>(
				() => {
					badEncryptedValue = "test";
					Program.Program.Debug = true;
					Program.Program.Decrypt(badEncryptedValue, password, obtentionIterations, segments);
				});
		}
	}
}
