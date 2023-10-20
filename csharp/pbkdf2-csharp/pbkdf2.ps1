#Copyright (c) 2023 Serguei Kouzmine
#
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.

param (
  [string]$value = 'test',
  [string]$password = 'password',
  [string]$operation = 'encrypt',
  [string]$salt = $null,
  [switch]$strong,
  [switch]$debug
)

$utility_class = 'WinAPI_AES'

if (-not ($utility_class -as [type])) {

Add-Type -TypeDefinition @"
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class ${utility_class} {
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
"@
}
$o = new-object $utility_class
# TODO: refactor
$o.Payload = $value
$o.Password = $password
$o.Strong = [bool]$psboundparameters['strong'].ispresent
$o.Debug = [bool]$psboundparameters['debug'].ispresent
if ($operation -eq 'encrypt'){
  [string]$result = $o.Encrypt($value,$password,$null)
} else {
  [string]$result = $o.Decrypt($value,$password)
}


write-output $result
<#
. .\pbkdf2.ps1 -value 'hello,world' -password secret -operation encrypt -debug
salt: AEA1E8F59EEF08149E665AA77A6AF564
key: 13CE7AF3964CBE18069380FA7E6715DC5C50F64FAF0D9EECC2429D2C7CB8AD34
iv: 601A5A3472C5D59F28190C796A89CBD2
data: 14CFEE457B13275A68BBA43A90C5B936
encrypted: rqHo9Z7vCBSeZlqnemr1ZGAaWjRyxdWfKBkMeWqJy9IUz+5FexMnWmi7pDqQxbk2
rqHo9Z7vCBSeZlqnemr1ZGAaWjRyxdWfKBkMeWqJy9IUz+5FexMnWmi7pDqQxbk2



. .\pbkdf2.ps1 -value 'rqHo9Z7vCBSeZlqnemr1ZGAaWjRyxdWfKBkMeWqJy9IUz+5FexMnWmi7pDqQxbk2' -password secret -operation decrypt -debug
salt: 0DF1E2394A8646D3BDFEB36D2C0E1CA5
iv: E76ABC681494B5BB10300DAC6FD993D4
data: B8541C1AB9B2D4407E371D4C2A32BEC1
hello,world


. .\pbkdf2.ps1 -value 'hello, world of AES' -password secret -operation encrypt -strong -debug

salt: A5A1AC6E23D5DDAB29743B4CD57292C3
key: F981D05072DB283C0BF3288CA5502E4BF6F5388798D537EFA581212B96B6D5ED
iv: 5DB2A8E3EFD369E074957768D45310E3
data: 1A4C3E78E2D22FECDF7DB34CB9C112C6C2FED8EE74A108CC06B88A8638B221CF
encrypted: paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==
paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==

. .\pbkdf2.ps1 -value 'paGsbiPV3aspdDtM1XKSw12yqOPv02ngdJV3aNRTEOMaTD544tIv7N99s0y5wRLGwv7Y7nShCMwGuIqGOLIhzw==' -password secret -operation decrypt -strong -debug
salt: A5A1AC6E23D5DDAB29743B4CD57292C3
iv: 5DB2A8E3EFD369E074957768D45310E3
data: 1A4C3E78E2D22FECDF7DB34CB9C112C6C2FED8EE74A108CC06B88A8638B221CF
hello, world of AES

#>