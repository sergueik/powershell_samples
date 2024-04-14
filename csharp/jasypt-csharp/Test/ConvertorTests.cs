using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Test {

	[TestFixture]
	public class ConvertorTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private String hexStr = null;
		private String str = null;
		private byte[] bytes = {};

		[SetUp]
		public void setUp() {
			verificationErrors.Clear();
			bytes = new byte[]{0x61,0x62,0x63,0x64,0x65,0x66,0x67,0x68};
			hexStr = "6162636461626364"; // 16
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void test1() {
			byte[] result = Convertor.HexStringToByteArray(hexStr);
			Assert.IsNotNull(result);
			Assert.AreEqual( 8, result.Length);
			Assert.AreEqual( 97, result[0]);
		}

		[Test]
		public void test2() {
			string result = Convertor.ByteArrayToString(bytes);
			Assert.IsNotNull(result);
			Assert.AreEqual( 8, result.Length);
			StringAssert.AreEqualIgnoringCase("ABCDEFGH", result);
		}

		[Test]
		public void test3() {
			string result = Convertor.ByteArrayToHexString(bytes);
			Assert.IsNotNull(result);
			Assert.AreEqual( 16, result.Length);
			StringAssert.AreEqualIgnoringCase("6162636465666768", result);
		}
		
		[Test]
		public void test4() {
			String result;
			result = Convertor.StringtoHexString("abcd");
			Assert.IsNotNull(result);
			Assert.AreEqual( 8, result.Length);
			StringAssert.AreEqualIgnoringCase("61626364", result);
			result = Convertor.StringtoHexString("абвг");
			Assert.IsNotNull(result);
			Assert.AreEqual( 12, result.Length);
			StringAssert.AreEqualIgnoringCase("430431432433", result);
		}
		
				[Test]
		public void test5() {
			String result1 = Convertor.ByteArrayToString(bytes);
			String result2 = Convertor.StringtoHexString(result1);
			String result3 = Convertor.ByteArrayToHexString(bytes);
			StringAssert.AreEqualIgnoringCase(result3, result2);
		}
	}
}
