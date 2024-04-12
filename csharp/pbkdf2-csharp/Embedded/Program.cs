using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class /* ${utility_class} */ Program {
	private Rfc2898DeriveBytes deriveBytes;
	private string payload = "test";
	public string Payload { set { payload = value; } }
	private string saltString = null;
	public string SaltString { set { saltString = value; } }
	private string password = "password";
	public string Password { set { password = value; } }
	private byte[] salt = { };
	private byte[] iv = { };
	private bool debug = false;
	public bool Debug { set { debug = value; } }
	private bool strong = false;
	public  bool Strong{ set { strong = value; } }

	public string Encrypt(String payloadString, String passwordString, String saltString) {
		byte[] password = Encoding.UTF8.GetBytes(passwordString);
		byte[] payload = Encoding.UTF8.GetBytes(payloadString);
		byte[] result = { };
		byte[] data = { };

		if (String.IsNullOrEmpty(saltString)) {
			// Generating salt bytes
			salt = GetRandomBytes(16);
		} else {
			salt = HexStringToByteArray(saltString);
		}
		if (debug)
			Console.Error.WriteLine("salt: " + ByteArrayToHexString(salt));

		using (var memoryStream = new MemoryStream()) {
			using (var AES = Aes.Create()) {
				// AES sizes are in bits
				AES.KeySize = 256;
				AES.BlockSize = 128;
				// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.5
				// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha1?view=netframework-4.5
				// the Rfc2898DeriveBytes default uses HMACSHA1
				// but with .Net 4.6 one can override constructor 
				deriveBytes = (strong) ? new Rfc2898DeriveBytes(password, salt, 1000,  HashAlgorithmName.SHA512 ): new Rfc2898DeriveBytes(password, salt, 1000);
				AES.Key = deriveBytes.GetBytes(AES.KeySize / 8);
				if (debug)
					Console.Error.WriteLine("key: "  + ByteArrayToHexString(AES.Key));
				AES.IV = this.iv = deriveBytes.GetBytes(AES.BlockSize / 8);
				if (debug)
					Console.Error.WriteLine("iv: "  + ByteArrayToHexString(iv));
				AES.Mode = CipherMode.CBC;

				// Create an encryptor to encrypt the data
				// NOTE: probably passing key and iv aguments and through properties is the same
				ICryptoTransform encryptor = AES.CreateEncryptor(AES.Key, AES.IV);
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
					cryptoStream.Write(payload, 0, payload.Length);
					cryptoStream.Close();
				}
				data = memoryStream.ToArray();
			}
		}

		if (debug)
			Console.Error.WriteLine("data: " + ByteArrayToHexString(data));
		// Prepending salt and IV bytes to encrypted data
		// see also: https://github.com/giterlizzi/perl-Crypt-PBE/blob/master/lib/Crypt/PBE/PBES2.pm#L81
		result = new byte[data.Length + salt.Length + iv.Length];
		Array.Copy(salt, result, salt.Length);
		Array.Copy(iv, 0, result, salt.Length, iv.Length);
		Array.Copy(data, 0, result, salt.Length + iv.Length, data.Length);

		var encrypted = Convert.ToBase64String(result, 0, result.Length);
		if (debug)
			Console.Error.WriteLine("encrypted: " + encrypted);
		return encrypted;
	}

	public string Decrypt(String payloadString, String passwordString) {
		byte[] payload = Convert.FromBase64String(payloadString);
		byte[] password = Encoding.UTF8.GetBytes(passwordString);
		byte[] data = { };
		byte[] result = { };
		try {

			// salt and iv will be read from payload
			salt = new byte[16];
			iv = new byte[16];
			try {
				data = new byte[payload.Length - salt.Length - iv.Length];
			} catch (OverflowException e) {
				Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
				if (debug)
					throw e;
				return null;
			}
			Array.Copy(payload, salt, salt.Length);
			if (debug) 
				Console.Error.WriteLine("salt: " + ByteArrayToHexString(salt));
			// NOTE: read IV bytes prepended to encrypted data
			// see also: https://github.com/giterlizzi/perl-Crypt-PBE/blob/master/lib/Crypt/PBE/PBES2.pm#L93
			Array.Copy(payload, salt.Length, iv, 0, iv.Length);
			if (debug)
				Console.Error.WriteLine("iv: " + ByteArrayToHexString(iv));

			Array.Copy(payload, salt.Length + iv.Length, data, 0, data.Length);
			if (debug)
				Console.Error.WriteLine("data: " + ByteArrayToHexString(data));

			// NOTE: when arguments of process is invalid, throwing
			// System.Security.Cryptography.CryptographicException: Padding is invalid and cannot be removed.
			// System.Security.Cryptography.CryptographicException: Specified key is not a valid size for this algorithm.

			using (var memoryStream = new MemoryStream()) {
				using (var AES = Aes.Create()) {
					AES.KeySize = 256;
					AES.BlockSize = 128;
					deriveBytes = (strong) ? new Rfc2898DeriveBytes(password, salt, 1000,  HashAlgorithmName.SHA512 ): new Rfc2898DeriveBytes(password, salt, 1000);
					AES.Key = deriveBytes.GetBytes(AES.KeySize / 8);
					AES.IV = iv;
					AES.Mode = CipherMode.CBC;
					// Create a decryptor to decrypt the data
					// NOTE: probably passing key and iv aguments and through properties is the same
					ICryptoTransform decryptor = AES.CreateDecryptor(AES.Key, AES.IV);

					using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write)) {
						cryptoStream.Write(data, 0, data.Length);
						cryptoStream.Close();
					}
					result = memoryStream.ToArray();
				}
			}

			return Encoding.UTF8.GetString(result);
		} catch (CryptographicException e) {
			// Unhandled Exception: System.Security.Cryptography.CryptographicException: Bad Data.

			Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
			if (debug)
				throw e;
			return null;
		}
	}

	public static byte[] GetRandomBytes(int length) {
		var byteArray = new byte[length];
		RNGCryptoServiceProvider.Create().GetBytes(byteArray);
		return byteArray;
	}
		
	public static String ByteArrayToString(byte[] bytes) {
		return System.Text.Encoding.Default.GetString(bytes);
	}

	public static String ByteArrayToHexString(byte[] data) {
		var stringBuilder = new StringBuilder(data.Length * 2);
		foreach (byte b in data)
			stringBuilder.Append(b.ToString("X2"));
		//	stringBuilder.AppendFormat("{0:x2}", b);
		return stringBuilder.ToString();
	}
	public static byte[] HexStringToByteArray(String data) {
		int NumberChars = data.Length;
		byte [] hexByteArray = new byte[NumberChars / 2];
		for (int index = 0; index < NumberChars; index += 2)
			hexByteArray[index / 2] = Convert.ToByte(data.Substring(index, 2), 16);
		return hexByteArray;
	}
		
	public static String StringtoHexString(String data) {
		String hexString = String.Empty;
		foreach (char c in data) {
			int value = c;
			hexString += String.Format
                       ("{0:x2}", System.Convert.ToUInt32(value.ToString()));
		}
		return hexString;
	}

}
