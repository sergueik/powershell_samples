using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

using Utils;

namespace Test {

	[TestFixture]
	public class DerivedBytesTest {
		private StringBuilder verificationErrors = new StringBuilder();
		private  const	String password = "password";
		private Rfc2898DeriveBytes deriveBytes;
		private  const	String salt = "fec4c9acd8c72cd9790ccfb953ba48f7";
		// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.5
		private byte[] key = {};
		private byte[] iv = {};
		private string keyhex = null;
		private string ivhex = null;

		[SetUp]
		public void setUp() {
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());

		}
		// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.5
		// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha512?view=netframework-4.5
		[Test]
		public void test1() {

			deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), 1000);

			keyhex  = Convertor.ByteArrayToHexString(deriveBytes.GetBytes(32));
			StringAssert.Contains("A60C9354B5C8EE41C16B6BD2CFBCE540345C49FD481F5D765053C67D45EC6AE2", keyhex);
			Console.Error.WriteLine("key(hex): " + keyhex);
			iv = deriveBytes.GetBytes(16);
			Assert.AreEqual( 0x10, iv[0]);

			ivhex = Convertor.ByteArrayToHexString(iv);
			Assert.IsNotNull(ivhex);
			Assert.AreEqual( 32, ivhex.Length);

            StringAssert.Contains("10D4EDCDC241350E176A747C23A38EAE", ivhex);
			Console.Error.WriteLine("iv(hex): " + ivhex);
		}

	}
}
