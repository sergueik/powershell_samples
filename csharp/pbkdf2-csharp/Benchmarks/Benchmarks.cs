using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Diagnostics;
using Utils;
// based on:
// https://github.com/talweiss1982/BenchmarkDotNetDemo/blob/master/BenchmarkDotNetDemo/IFooDemo.cs

namespace Benchmarks {

	public class PbkdfTest {

		private static String plainValue;
		private static String encryptedValue;
		private static String saltString;
		private static String password;
		private static bool debug = false;
		private static bool strong = true;
		private static AES aes;


		public PbkdfTest() {
			saltString = "fec4c9acd8c72cd9790ccfb953ba48f7";
			password = "password";
			debug = false;
			plainValue = "test";
			encryptedValue = "TNBKkqu9QHd8rjxVbw/JFLGLt+XVGTtmvjizv95EmoFpWs0+Vg9/kTgW1qkBGOvT";
			// Program.Program.Debug = debug;
			aes = new AES();
			aes.Strong = strong;
			aes.Debug = debug;

		}

		[Benchmark]
		// TODO: engage .net core syntax
        	// public string  Program() => program.Encrypt(plainValue, password, saltSting);
		public void DecryptIterate(int iterations)
		{
			String decrypted;
			for (int i = 0; i <= iterations; i++)
				decrypted = aes.Decrypt(encryptedValue, password);
			// Console.Error.WriteLine(decrypted );
			return;
		}

		public class Program {

			private static int iterations = 1000;
			public static void Main(string[] args) {

				var sw = new Stopwatch();
				sw.Start();
				var pbkdfTest = new PbkdfTest();
				if (args != null && args.Length > 0)
					iterations = int.Parse(args[0]);
				pbkdfTest.DecryptIterate(iterations);
				sw.Stop();
				Console.WriteLine("It took {0} ms to run {1} iterations.", sw.ElapsedMilliseconds, iterations);
			}
		}
	}
}
