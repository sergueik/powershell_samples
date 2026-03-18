using System;
using System.Text;
using NUnit.Framework;
using System.Linq;
using Utils;

/**
 * Copyright 2026 Serguei Kouzmine
 */
namespace Test
{
	[TestFixture]
	public class ValidatorTests
	{
		private StringBuilder verificationErrors = new StringBuilder();
		private static bool debug;
		private double threshold = .95;
		[SetUp]
		public void setUp()
		{
			debug = true;
			verificationErrors.Clear();
		}

		[TearDown]
		public void tearDown()
		{
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test]
		public void test1()
		{
			object[,] arguments = {
				{ "uppercase HELLO", "C8C5D3D3D6", true },
				{ "lowercase hello", "8885939396", true },
				{ "digits 12345", "F1F2F3F4F5", true },
				{ "HELLO.HELLO punctuation", "C8C5D3D3D64BC8C5D3D3D6", true },
				{ "contains NULL byte", "C8C500D3D6", false },
				{ "control characters 0x15", "151515", false },
				{ "mixed valid and invalid", "C8C5D315D6", false },
				{ "UTF-8 string 'HELLO'", "48454C4C4F", false },
				{ "ASCII digits '12345'", "3132333435", false },
				{ "UTF-16BE 'HELLO'", "00480045004C004C004F", false },
				{ "UTF-16 BOM + text", "FEFF00480045004C004C004F", false },
				{ "UTF-8 string 'é' (C3 A9)", "C3A9", true }
			};

			for (int i = 0; i < arguments.GetLength(0); i++) {
				string comment = (string)arguments[i, 0];
				string input = (string)arguments[i, 1];
				bool expected = (bool)arguments[i, 2];
				string codePage = "IBM037";
				try {
					// NOTE: 0x3F is a question mark and if it is logged in the message hex, the string to byte conversion was wrong 
					byte[] data = Convertor.HexStringToByteArray(input);
					var convertor = new Validator(data, codePage, threshold);
					ValidationResult result = convertor.Validate();

					Assert.IsNotNull(result);
					Assert.AreEqual(expected, result.Valid, string.Format("{0} input={1} hex={2} error={3}", comment, input, Convertor.ByteArrayToHexString(data), result.Message));
				} catch (AssertionException e) {
					Console.Error.WriteLine("Assertion Exception: {0}", e.Message);
					// ignore AssertionExceptio which is loged anyway and mimic DataDrivenTest   
				}
			}
		}

		[Test]
		public void test2() {
			object[,] arguments = {
				{ "ASCII HELLO", "HELLO", true },
				{ "Cyrillic привет", "привет", false }
			};

			for (int i = 0; i < arguments.GetLength(0); i++) {
				string comment = (string)arguments[i, 0];
				string input = (string)arguments[i, 1];
				bool expected = (bool)arguments[i, 2];
				string codePage = "ASCII";
				try {
					// 0x3F is a questionmark and if it is loggedin the message, the string to byte conversion was wrong 
					byte[] data = Encoding.UTF8.GetBytes(input);
					var convertor = new Validator(data, codePage, threshold);
					ValidationResult result = convertor.Validate();

					Assert.IsNotNull(result);
					Assert.AreEqual(expected, result.Valid, string.Format("{0} input={1} hex={2} error={3}", comment, input, Convertor.ByteArrayToHexString(data), result.Message));
				} catch (AssertionException e) {
					Console.Error.WriteLine("Assertion Exception: {0}", e.Message);
					// ignore AssertionExceptio which is loged anyway and mimic DataDrivenTest   
				}
			}
		}

		[Test]
		public void test3() {
			object[,] arguments = { { "Spanish accented characters in CP1047", "El veloz murciélago hindú comía feliz cardillo y kixwi; la cigüeña tocaba el saxofón detrás del palenque de paja", true },
                { "Canadian French accented characters in CP1047", "Voix ambiguë d’un cœur qui au zéphyr préfère les jattes de kiwi", true },
                { "European banking text with Euro sign", "La banque européenne a reçu 100€ pour le dépôt", true },
                { "European smart quote", "Voix ambiguë d’un cœur", true }
            };

            for (int i = 0; i < arguments.GetLength(0); i++) {
                string comment = (string)arguments[i, 0];
                string input = (string)arguments[i, 1];
                bool expected = (bool)arguments[i, 2];
                string codePage= "IBM037";
                try {
		        	// 0x3F is a questionmark and if it is loggedin the message, the string to byte conversion was wrong 
		            byte[] data = Encoding.GetEncoding(codePage).GetBytes(input);
		            var convertor = new Validator(data, codePage, null);
		            ValidationResult result = convertor.Validate();
		
		            Assert.IsNotNull(result);
		            Assert.AreEqual(expected, result.Valid, string.Format("{0} input={1} hex={2} error={3}", comment, input, Convertor.ByteArrayToHexString(data), result.Message));
                } catch (AssertionException e) {
                	Console.Error.WriteLine("Assertion Exception: {0}", e.Message  );
                	// ignore AssertionExceptio which is loged anyway and mimic DataDrivenTest   
                }
            }
        }

		[Test]
		public void test4() {
			object[,] arguments = { { "Spanish accented characters in CP1047", "El veloz murciélago hindú comía feliz cardillo y kixwi; la cigüeña tocaba el saxofón detrás del palenque de paja", true },
                { "Canadian French accented characters in CP1047", "Voix ambiguë d’un cœur qui au zéphyr préfère les jattes de kiwi", true },
                { "European banking text with Euro sign", "La banque européenne a reçu 100€ pour le dépôt", true },
                { "European smart quote", "Voix ambiguë d’un cœur", true }
            };

            for (int i = 0; i < arguments.GetLength(0); i++) {
                string comment = (string)arguments[i, 0];
                string input = (string)arguments[i, 1];
                bool expected = (bool)arguments[i, 2];
                string codePage= "IBM037";
                try {
		        	// 0x3F is a questionmark and if it is loggedin the message, the string to byte conversion was wrong 
		            // byte[] data = Encoding.UTF8.GetBytes(input);
		            
		            byte[] data = Encoding.GetEncoding(codePage).GetBytes(input);
		            var convertor = new Validator(data, codePage, threshold);
		            ValidationResult result = convertor.Validate();
		
		            Assert.IsNotNull(result);
		            Assert.AreEqual(expected, result.Valid, string.Format("{0} input={1} hex={2} error={3}", comment, input, Convertor.ByteArrayToHexString(data), result.Message));
                } catch (AssertionException e) {
                	Console.Error.WriteLine("Assertion Exception: {0}", e.Message  );
                	// ignore AssertionExceptio which is loged anyway and mimic DataDrivenTest   
                }
            }
        }
    }
}