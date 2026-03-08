/**
 * Copyright 2026 Serguei Kouzmine
 */
 
using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

// see also:
// https://www.ibm.com/docs/en/zos-connect/3.0.0?topic=properties-coded-character-set-identifiers

namespace Test {

	[TestFixture]
	public class ConvertorTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private static bool debug;

		// To ensure compatibility, encrypt test inputs via Java or Perl
		[SetUp]
		public void setUp() {
			debug = true;
			Program.Program.Debug = debug;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown() {
			Assert.AreEqual("", verificationErrors.ToString());
		}


		
		public void validate(string data, bool status, string comment) {
			var result = Convertor.validateEBCDIC(Convertor.HexStringToByteArray(data));
			Assert.IsNotNull(result);
			Assert.AreEqual(status, result.Valid, comment);
		}
		
		[Test]
		public void test()  {
			object[,] arguments = {
				{ "uppercase HELLO", "C8C5D3D3D6", true },
				{ "lowercase hello", "8885939396", true },
				{ "digits 12345", "F1F2F3F4F5", true },
				{ "HELLO.HELLO punctuation", "C8C5D3D3D64BC8C5D3D3D6", true },

				{ "contains NULL byte", "C8C500D3D6", false },
				{ "control characters 0x15", "151515", false },
				{ "mixed valid and invalid", "C8C5D315D6", false },

				// encoding confusion cases
				{ "UTF-8 string 'HELLO'", "48454C4C4F", false },
				{ "ASCII digits '12345'", "3132333435", false },
				{ "UTF-16BE 'HELLO'", "00480045004C004C004F", false },
				{ "UTF-16 BOM + text", "FEFF00480045004C004C004F", false },

				// overlap case
				{ "UTF-8 string 'é' (C3 A9)", "C3A9", true }
			};

			for (int i = 0; i < arguments.GetLength(0); i++) {
				string comment = (string)arguments[i, 0];
				string data = (string)arguments[i, 1];
				bool result = (bool)arguments[i, 2];

				validate(data, result, string.Format("{0} data={1}", comment, data) );
			}
		}
	}
}
