using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Program;


namespace Benchmarks {
	public class PbkdfTest {

		private static String plainValue;
		private static String encryptedValue;
		private static String saltString;
		private static String password;
		private static bool debug;

		public PbkdfTest() {
			saltString = "fec4c9acd8c72cd9790ccfb953ba48f7";
			password = "password";
			debug = true;
			plainValue = "test";
			encryptedValue = "/sTJrNjHLNl5DM+5U7pI98y1r5JDXiW2cfCLVjou2AcgYo+wgjLTzCGWuMA9dCC2";
			Program.Program.Debug = debug;
		}

		[Benchmark]
		// TODO: engage .net core syntax
        	// public string  Program() => program.Encrypt(plainValue, password, saltSting);
		public string Encrypt() {
			var value = Program.Program.Encrypt(plainValue, password, saltString);
			// read  salt and iv from payload
			var payload = Convert.FromBase64String(value);
			return null;
		}

		public class Example {
			public static void Main(string[] args) {
				var summary = BenchmarkRunner.Run(typeof(Example));
			}
		}
	}
}