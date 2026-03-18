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
		public void setUp(){
			debug = true;
			Program.Program.Debug = debug;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown(){
			Assert.AreEqual("", verificationErrors.ToString());
		}

		public void validate1(string data, bool status, string comment) {
			var result = Convertor.validateEBCDIC(Convertor.hexStringToByteArray(data));
			Assert.IsNotNull(result);
			Assert.AreEqual(status, result.Valid, comment);
		}

		// converts string to byte array argument on the fly
		public void validate2(string input, bool status, string comment) {
			string hex = String.Concat( input.Select(ch => ((int)ch).ToString("X2")));
			byte[] data = Convertor.hexStringToByteArray(hex);

			var result = Convertor.validateASCII(data, 0.96);

			Assert.IsNotNull(result);
			Assert.AreEqual(status, result.Valid, String.Format("{0} input={1} hex={2}", comment, input, hex));
		}

		// converts string to byte array argument on the fly, alternative take
		public void validate3(string input, bool status, string comment) {
			string hex = String.Join("", input.ToCharArray().Select(c => ((int)c).ToString("X2")));
			byte[] data = Convertor.hexStringToByteArray(hex);
			var result = Convertor.validateASCII(data, 0.96);

			Assert.IsNotNull(result);
			Assert.AreEqual(status, result.Valid, String.Format("{0} input={1} hex={2}", comment, input, hex));
		}
		
		public void validate4(string input, string codePage, bool status, string comment) { 
			string hex = String.Concat( input.Select(ch => ((int)ch).ToString("X2")));
			byte[] data = Convertor.hexStringToByteArray(hex);

			// validate encoded string
			ValidationResult result = Convertor.validateCore(data, codePage, Convertor.getDecoder(codePage), Convertor.getPredicate(codePage), .90);

			Assert.IsNotNull(result);
			Assert.AreEqual(status, result.Valid, String.Format("{0} input={1} hex={2} error={3}", comment, input, hex,  result.Message));
		}

		public void validate5(string input, string codePage, bool status, string comment){
		    byte[] data = Encoding.GetEncoding(codePage).GetBytes(input);
		    string hex = BitConverter.ToString(data).Replace("-", "");
		    ValidationResult result = Convertor.validateCore(data, codePage, Convertor.getDecoder(codePage), Convertor.getPredicate(codePage), .90);
		    Assert.IsNotNull(result);
		    Assert.AreEqual(status, result.Valid, String.Format("{0} input={1} hex={2} error={3}", comment, input, hex, result.Message));
		}

		[Test]
		// NOTE: TestName is not supported prior to Nunit 3.x
		// [TestName("EBCDIC validation")]
		public void test1() {
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

				validate1(data, result, string.Format("{0} data={1}", comment, data));
			}
		}

		[Test]
		// NOTE: TestName is not supported prior to Nunit 3.x
		// [TestName("ASCII validation - clear text test data")]
		public void test2() {

			object[,] arguments = {
				{ "ASCII HELLO", "HELLO", true },
				{ "Cyrillic привет", "привет", false }
			};

			for (int i = 0; i < arguments.GetLength(0); i++) {

				string comment = (string)arguments[i, 0];
				string data = (string)arguments[i, 1];
				bool result = (bool)arguments[i, 2];

				validate2(data, result,
					String.Format("{0} data={1}", comment, data));
				validate3(data, result,
					String.Format("{0} data={1}", comment, data));
			}
		}
		
		[Test]
		// NOTE: TestName is not supported prior to Nunit 3.x
		// [TestName("EBCDIC validation")]
		public void test3() {
			object[,] arguments = {
				{ "Spanish accented characters in CP1047", "El veloz murciélago hindú comía feliz cardillo y kixwi; la cigüeña tocaba el saxofón detrás del palenque de paja", true },
				{ "Canadian French accented characters in CP1047", "Voix ambiguë d’un cœur qui au zéphyr préfère les jattes de kiwi", true },
				// A typographic quote ’ or Euro sign € are not present in CP1047.
				// however they get replaced and tests pass
				 { "European banking text with Euro sign", "La banque européenne a reçu 100€ pour le dépôt", true },
				 { "European smart quote", "Voix ambiguë d’un cœur", true}

			};

			for (int i = 0; i < arguments.GetLength(0); i++) {

				string comment = (string)arguments[i, 0];
				string data = (string)arguments[i, 1];
				string codePage = "IBM037";
				bool result = (bool)arguments[i, 2];

				validate5(data, codePage, result, String.Format("{0} data={1}", comment, data));
			}
		}
		
	}
}
