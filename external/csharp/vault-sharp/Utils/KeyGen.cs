using System;
using System.Security.Cryptography;
using System.Text;

// Origin: https://www.codeproject.com/Articles/7546/A-CBC-Stream-Cipher-in-C-With-wrappers-for-two-ope
namespace Utils
{
	public sealed class KeyGen
	{
		private KeyGen()
		{}

		public static byte[] DeriveKey(string password, int keySize, byte[] salt)
		{
			return DeriveKey(password, keySize, salt, 1024);
		}

		//  derived key from string password and salt
		//  based on PBKDF1 (PKCS #5 v1.5)
		//  see http://www.faqs.org/rfcs/rfc2898.html
		public static byte[] DeriveKey(string password, int keySize, byte[] salt, int obtentionIterations = 1000 ) {
			System.Security.Cryptography.SHA256 sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider ();
			if (keySize > 32)
				keySize = 32;
			// byte[] data = Convertor.StringToByteArray(password);
			byte[] data = Encoding.UTF8.GetBytes(password);
			if (salt != null) {
				var temp = new byte[data.Length + salt.Length];
				Array.Copy(data, 0, temp, 0, data.Length);
				Array.Copy(salt, 0, temp, data.Length, salt.Length);
				data = temp;
			}
			if (obtentionIterations <= 0)
				obtentionIterations = 1;
			for (int i = 0; i < obtentionIterations; i++) {
				data = sha256.ComputeHash(data);
				// data = sha256.MessageSHA256(data);
			}
			var key = new byte[keySize];
			Array.Copy(data, 0, key, 0, keySize);
			return key;
		}


	}
}

