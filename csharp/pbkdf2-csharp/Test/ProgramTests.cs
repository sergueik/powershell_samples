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
		private static String plainValue;
		private static String encryptedValue;
		private static String saltString;
		private static String password;
		private static bool debug;

		// To ensure compatibility, encrypt test inputs via Java or Perl
		[SetUp]
		public void setUp() {
			saltString = "fec4c9acd8c72cd9790ccfb953ba48f7";
			password = "password";
			debug = true;
			plainValue = "test";
			encryptedValue = "/sTJrNjHLNl5DM+5U7pI98y1r5JDXiW2cfCLVjou2AcgYo+wgjLTzCGWuMA9dCC2";
			Program.Program.Debug = debug;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void encryptTest1() {
			String value = Program.Program.Encrypt(plainValue, password, saltString );
			StringAssert.Contains(encryptedValue, value);
		}

		[Test]
		public void encryptTest2() {
			String value = Program.Program.Encrypt(plainValue, password, saltString);
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
			String value = Program.Program.Decrypt(encryptedValue, password);
			StringAssert.IsMatch(plainValue, value);

		}

		[Test]
		public void encryptTest3() {

			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Utils.Convertor.HexStringToByteArray(saltString), 1000);
			// ignore the key in this test 
			deriveBytes.GetBytes(32);

			var iv1 = deriveBytes.GetBytes(16);
			Console.Error.WriteLine("IV (computed): "  + Convertor.ByteArrayToHexString(iv1));

			String value = Program.Program.Encrypt(plainValue, password, saltString);
			// read  salt and iv from payload
			var payload = Convert.FromBase64String(value);
			var salt = new byte[16];
			Array.Copy(payload, salt, 16);
			Console.Error.WriteLine("Salt (from payload): "  + Convertor.ByteArrayToHexString(salt));
			var iv2 = new byte[16];
			Array.Copy(payload, 16, iv2, 0, 16);
			Console.Error.WriteLine("IV (from payload): "  + Convertor.ByteArrayToHexString(iv2));
			StringAssert.AreEqualIgnoringCase(saltString, Convertor.ByteArrayToHexString(salt));
			StringAssert.AreEqualIgnoringCase(Utils.Convertor.ByteArrayToHexString(iv1), Convertor.ByteArrayToHexString(iv2));
		}

	}
}