using System;

using System.Text;
using System.Linq;

namespace Utils {
	public class Helper {
		private static bool debug = false;
		public static bool Debug { set { debug = value; } }
		private static bool strong = false;
		public static bool Strong { set { strong = value; } }
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

