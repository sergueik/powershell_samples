
/**
 * Copyright 2026 Serguei Kouzmine
 */

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

// see also:
// https://www.ibm.com/docs/en/zos-connect/3.0.0?topic=properties-coded-character-set-identifiers
namespace Test {

	[TestFixture]
	public class ProgramTests {
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

		public void convert(string data, string expected) {
			var result = Program.Program.convertEBCDICToASCII(Convertor.hexStringToByteArray(data), "IBM037");
			Assert.IsNotNull(result);
			Assert.AreEqual(expected, Encoding.ASCII.GetString(result), "Failed for input: " + data);
		}

		[Test]
		public void test() {
			string[,] arguments = {
				{ "C8C5D3D3D6", "HELLO" },
				{ "E6D6D9D3C4", "WORLD" },
				{ "F1F2F3F4F5", "12345" }
			};

			for (int cnt = 0; cnt < arguments.GetLength(0); cnt++) {
				var data = arguments[cnt, 0];
				var result = arguments[cnt, 1];
				convert(data, result);
			}
		}
	}
}