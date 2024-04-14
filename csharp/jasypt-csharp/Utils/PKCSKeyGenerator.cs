//*************************************************************************
// modifications Copyright 2023 Serguei Kouzmine
// based on:
//
// PKCSKeyGenerator.cs
// Derive key material using PKCS #1 v1.5 algorithm with MD5 hash
//
// Portions Copyright (C) 2005.  Michel I. Gallant
// Portions copyright 2006 Richard Smith
// Adapted from http://www.jensign.com/JavaScience/dotnet/DeriveKeyM/index.html
//
//*************************************************************************
//
//  DeriveKeyM.cs
//
//  Derive a key from a pswd and Salt using MD5 and PKCS #5 v1.5 approach
//   see also:   http://www.openssl.org/docs/crypto/EVP_BytesToKey.html
//   see also:   http://java.sun.com/j2se/1.5.0/docs/guide/security/jce/JCERefGuide.html#PBE
//
//**************************************************************************

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
namespace Utils {
	public class PKCSKeyGenerator {
		byte[] key = new byte[8], iv = new byte[8];
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        // see also: https://codereview.stackexchange.com/questions/154017/aes256-implementation
        private AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
		public bool Debug { get; set; }

		public byte[] Key { get { return key; } }
		public byte[] IV { get { return iv; } }
		public ICryptoTransform Encryptor { get { return des.CreateEncryptor(key, iv); } }
		public ICryptoTransform Decryptor { get { return des.CreateDecryptor(key, iv); } }
		public ICryptoTransform EncryptorAes { get { return aes.CreateEncryptor(key, iv); } }
		public ICryptoTransform DecryptorAes { get { return aes.CreateDecryptor(key, iv); } }
		public PKCSKeyGenerator() { }
		public PKCSKeyGenerator(String password, byte[] salt, 
			int obtentionIterations, int segments) {
			Generate(password, salt, obtentionIterations, segments);
		}
        
		public void Generate(String password, byte[] salt, 
			int obtentionIterations, int segments) {
			
			
			int HASHLENGTH = 16;    //MD5 bytes
			// to store concatenated Mi hashed results
			var keymaterial = new byte[HASHLENGTH * segments]; 
			
            
			// --- get secret password bytes ----
			var passwordBytes = Encoding.UTF8.GetBytes(password);
            
			// --- concatenate salt and pswd bytes into fixed data array ---
			byte[] data = new byte[passwordBytes.Length + salt.Length];
			// copy the psassword bytes
			Array.Copy(passwordBytes, data, passwordBytes.Length);
			// concatenate the salt bytes			
			Array.Copy(salt, 0, data, passwordBytes.Length, salt.Length);
           		// NOTE: segment loop removed: only support one segment for now
			// ---- do multi-hashing and concatenate results  D1, D2 ...  
			// into keymaterial bytes ----	
			MD5 md5 = new MD5CryptoServiceProvider();
			// SHA512 sha512 = new SHA512CryptoServiceProvider ();
			// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha1?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha512?view=netframework-4.5
			// https://stackoverflow.com/questions/15470738/java-messagedigest-class-in-c-sharp
			
			// ----  Now hash consecutively for obtentionIterations times ------
                
			for (int i = 0; i < obtentionIterations; i++)
				data = md5.ComputeHash(data);
			// kept for compatibility with Perl version in debug mode
			/*
			byte[] data_aes = new byte[passwordBytes.Length + salt.Length];
			byte[] data_aes_hash = new byte[passwordBytes.Length + salt.Length];
			for (int i = 0; i < obtentionIterations; i++){
						
				// TODO: concat data_aes and password 
					data_aes_hash  = sha512.ComputeHash(data_aes);
					data_aes = Xor(data_aes_hash , data_aes);
			}
			*/
			var DK = new byte[16];
			Array.Copy(data, 0, DK, 0, 16);
			if (Debug)
				Console.Error.WriteLine("DK: " + Convertor.ByteArrayToHexString(data));
			Array.Copy(data, 0, key, 0, 8);
			Array.Copy(data, 8, iv, 0, 8);
        
			return;
		}
		
		// origin: http://www.java2s.com/example/csharp/system/xor-two-byte-array.html 
		public static byte[] Xor(byte[] left, byte[] right)
        {
            byte[] val = new byte[left.Length];
            for (int i = 0; i < left.Length; i++)
                val[i] = (byte)(left[i] ^ right[i]);
            return val;
        }
	}
	
}
