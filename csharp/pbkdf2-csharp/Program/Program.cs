using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using Utils;

namespace Program {
	public class Program {
		
		private static String value = "test";
		private static String saltString = null;
		private static String password = "password";
		private static bool debug = false;
		public static bool Debug { set { debug = value; } }
		private static bool strong = false;
		private static String operation = "encrypt";
		private static byte[] passwordBytes = { };
		// private static byte[] result = null;

		public static void Main(string[] args) {

			// https://learn.microsoft.com/en-us/dotnet/api/system.environment.commandline?view=netframework-4.5
			var parseArgs = new ParseArgs(System.Environment.CommandLine);
			// NOTE: have to set debug with value true, switch arguments are not supported
			
			if (parseArgs.GetMacro("debug") != String.Empty)
				debug = true;
			if (parseArgs.GetMacro("strong") != String.Empty)
				strong = true;

			if (parseArgs.GetMacro("version") != String.Empty) {
				var versionObj = Assembly.GetExecutingAssembly().GetName().Version;
				Console.Error.WriteLine("version: " + versionObj.ToString());
				Environment.Exit(0);

				// https://stackoverflow.com/questions/12977924/how-do-i-properly-exit-a-c-sharp-application 
				// Application.Exit();
			}

			if (parseArgs.GetMacro("value") != String.Empty)
				value = parseArgs.GetMacro("value");

			if (parseArgs.GetMacro("password") != String.Empty)
				password = parseArgs.GetMacro("password");

			if (parseArgs.GetMacro("operation") != String.Empty)
				operation = parseArgs.GetMacro("operation");

			if (parseArgs.GetMacro("salt") != String.Empty)
				saltString = parseArgs.GetMacro("salt");

			if (debug) {
				Console.Error.WriteLine(String.Format("debug: " + parseArgs.GetMacro("debug")));
				Console.Error.WriteLine(String.Format("password: {0}\nvalue: {1}\nuse SHA512: {2}", password, value,strong));
			}
			if (operation.Equals("encrypt"))
				Program.Encrypt(value, password, saltString);
			if (operation.Equals("decrypt"))
				Program.Decrypt(value, password);
		}
	
		public static string Decrypt(String payloadString, String passwordString) {
			var aes = new AES();
			aes.Strong = strong;
			aes.Debug = debug;
			var decrypted = aes.Decrypt(payloadString, passwordString);        
			Console.Error.WriteLine((debug) ? "decrypted: " + decrypted : decrypted);
			return decrypted;
		}

		public static string Encrypt(String payloadString, String passwordString, String saltString) {
			var aes = new AES();
			aes.Strong = strong;
			aes.Debug = debug;
			var encrypted = aes.Encrypt(payloadString, passwordString, saltString);
			Console.Error.WriteLine((debug) ? "encrypted: " + encrypted : encrypted);
			return encrypted;
		}
	}
}
