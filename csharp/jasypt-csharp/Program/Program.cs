using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using Utils;

namespace Program {
	public class Program {
		private static String value = "test";
		private static String saltString = null;
		// "fd2b12742f751d0b";
		private static String password = "secret";
		private static bool debug = false;
		public static bool Debug { set { debug = value; } }
		private static String operation = "encrypt";
		private static int obtentionIterations = 1000;
		const int segments = 1;
		// number of 16-byte segments to create. 1 to mimic Java behaviour.
		
		public static void Main(string[] args) {
			
			// https://learn.microsoft.com/en-us/dotnet/api/system.environment.commandline?view=netframework-4.5
			var parseArgs = new ParseArgs(System.Environment.CommandLine);
			// NOTE: have to set debug with value true, switch arguments are not supported

			if (parseArgs.GetMacro("debug") != String.Empty)
				debug = true;
				// debug = Boolean.Parse(parseArgs.GetMacro("debug"));

			if (parseArgs.GetMacro("value") != String.Empty)
				value = parseArgs.GetMacro("value");

			if (parseArgs.GetMacro("version") != String.Empty) {
				var versionObj = Assembly.GetExecutingAssembly().GetName().Version;
				Console.Error.WriteLine("version: " + versionObj.ToString());
				Environment.Exit(0);

				// https://stackoverflow.com/questions/12977924/how-do-i-properly-exit-a-c-sharp-application 
				// Application.Exit();
			}
			if (parseArgs.GetMacro("password") != String.Empty)
				password = parseArgs.GetMacro("password");

			if (parseArgs.GetMacro("operation") != String.Empty)
				operation = parseArgs.GetMacro("operation");

			if (parseArgs.GetMacro("salt") != String.Empty)
				saltString = parseArgs.GetMacro("salt");

			if (parseArgs.GetMacro("iterations") != String.Empty)
				obtentionIterations = int.Parse(parseArgs.GetMacro("iterations"));
			if (debug)
				Console.Error.WriteLine(String.Format("password: {0}\nvalue: {1}", password, value));
			if (operation.Equals("encrypt"))
				Program.Encrypt(value, password, saltString, obtentionIterations, segments);
			if (operation.Equals("decrypt"))
				Program.Decrypt(value, password, obtentionIterations, segments);
		}

		public static string Decrypt(String value, String password, int obtentionIterations, int segments) {
			byte[] payload = { };
			if (debug)
				Console.Error.WriteLine("value: " + value);
			try {
				payload = Convert.FromBase64String(value);
			} catch (FormatException e) {
				// Unhandled Exception: System.FormatException: Invalid length for a Base-64 char array or string.
					// echo '/SsSdC91HQoudY3jRwuF3TLeizUKLM' | base64 -d -
				// base64: invalid input
				Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
				if (debug)
					throw e;
				return null;
			}
			if (debug)
				Console.Error.WriteLine("payload: " + Convertor.ByteArrayToString(payload));
			byte[] salt = new byte[8];
				byte[] cipherDataBytes = { };
				try {
					cipherDataBytes = new byte[payload.Length - salt.Length];
				} catch (OverflowException e) {
					Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
					if (debug)
						throw e;
					return null;
				}
				// public static void System.Array.Copy(Array sourceArray, Array destinationArray, int length);
				Array.Copy(payload, salt, salt.Length);
				// public static void System.Array.Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length); 
				Array.Copy(payload, salt.Length, cipherDataBytes, 0, cipherDataBytes.Length);
			if (debug) {
				Console.Error.WriteLine("salt: " + Convertor.ByteArrayToString(salt));
				Console.Error.WriteLine("salt: " + Convertor.ByteArrayToHexString(salt));
			}
			var kp = new PKCSKeyGenerator();
			kp.Debug = debug;
			kp.Generate(password, salt, obtentionIterations, segments);
			ICryptoTransform crypt = kp.Decryptor;

			var memoryStream = new MemoryStream();

			var cryptoStream = new CryptoStream(memoryStream, crypt, CryptoStreamMode.Write);

			cryptoStream.Write(cipherDataBytes, 0, cipherDataBytes.Length);
			
			try {
				cryptoStream.FlushFinalBlock();
				byte[] result = memoryStream.ToArray();
				var decrypted = Convertor.ByteArrayToString(result);        
				Console.Error.WriteLine((debug) ? "decrypted: " + decrypted : decrypted);
				return decrypted;
			} catch (CryptographicException e) {
				// Unhandled Exception: System.Security.Cryptography.CryptographicException: Bad Data.
				Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
				if (debug)
					throw e;
				return null;
			}

		}

		public static string Encrypt(String value, String password, String saltString, int obtentionIterations, int segments) {
			byte[] salt = { };
			
			if (String.IsNullOrEmpty(saltString)) {
				Random random = new Random();
				salt = new Byte[8];
				random.NextBytes(salt);
			} else {
				salt = Convertor.HexStringToByteArray(saltString);
			}
			

			var kp = new PKCSKeyGenerator();
			kp.Debug = debug;
			kp.Generate(password, salt, obtentionIterations, segments);
			ICryptoTransform crypt = kp.Encryptor;
			
			var memoryStream = new MemoryStream();

			var cryptoStream = new CryptoStream(memoryStream, crypt, CryptoStreamMode.Write);

			byte[] plainBytes = Encoding.ASCII.GetBytes(value);

			cryptoStream.Write(plainBytes, 0, plainBytes.Length);

			cryptoStream.FlushFinalBlock();

			byte[] cipherBytes = memoryStream.ToArray();

			memoryStream.Close();
			cryptoStream.Close();

			byte[] result = new byte[cipherBytes.Length + salt.Length];
			if (debug) {
				Console.Error.WriteLine("salt: " + Convertor.ByteArrayToString(salt));
				Console.Error.WriteLine("salt: " + Convertor.ByteArrayToHexString(salt));
			}
			Array.Copy(salt, result, salt.Length);
			Array.Copy(cipherBytes, 0, result, salt.Length, cipherBytes.Length);
			if (debug)
				Console.Error.WriteLine("result: " + Convertor.ByteArrayToString(result));

			var encrypted = Convert.ToBase64String(result, 0, result.Length);
			Console.Error.WriteLine((debug) ? "encrypted: " + encrypted : encrypted);
			return encrypted;
		}
	}
}
