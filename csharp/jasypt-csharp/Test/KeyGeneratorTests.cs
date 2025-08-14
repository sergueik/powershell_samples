using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Test {

	[TestFixture]
	public class KeyGeneratorTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private PKCSKeyGenerator keyGenerator;
		private const String key = "29395b04f92c9d2a";
		private const String iv = "57e85c0eb06566bb";
		private const String saltString = "fd2b12742f751d0b";
		private const String password = "secret";
		private static bool debug = true;
		private const int obtentionIterations = 1000;
		private const int segments = 1;
		private byte[] salt;

		[SetUp]
		public void setUp() {
			verificationErrors.Clear();
			keyGenerator = new PKCSKeyGenerator();
			keyGenerator.Debug = debug;
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void test() {
			salt = Convertor.HexStringToByteArray(saltString);
			keyGenerator.Generate(password, salt, obtentionIterations, segments);
			String result = Convertor.ByteArrayToHexString(keyGenerator.Key);
			Assert.IsNotNull(result);
			Assert.AreEqual(16, result.Length);
			// TODO: make ignoring case only in Linux test
			StringAssert.Contains(key.ToUpper(), result);
			StringAssert.AreEqualIgnoringCase(key, result);
			result = Convertor.ByteArrayToHexString(keyGenerator.IV);
			Assert.IsNotNull(result);
			Assert.AreEqual(16, result.Length);
			StringAssert.Contains(iv.ToUpper(), result);
			StringAssert.AreEqualIgnoringCase(iv, result);
		}
	}
}
