using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Test {

	[TestFixture]
	public class ConvertorTests {
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
		public void validateTest(){
			var result = Convertor.validateEBCDIC(Convertor.HexStringToByteArray(data));
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Valid);
		}
	}
}
