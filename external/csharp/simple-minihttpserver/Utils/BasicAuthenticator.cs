using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Runtime.Serialization;

namespace MiniHttpd
{
	public class BasicAuthenticator : IAuthenticator
	{
		public void AddUser(string username, string password)
		{
			entries.Add(username, md5.ComputeHash(encoding.GetBytes(password)));
		}

		public void AddUserByHash(string username, byte[] hash)
		{
			entries.Add(username, hash);
		}

		public void AddUserByHash(string username, string hash)
		{
			entries.Add(username, HexToBytes(hash));
		}

		public void RemoveUser(string username)
		{
			entries.Remove(username);
		}

		public void ChangePassword(string username, string current, string newPassword)
		{
			if (!Authenticate(username, current))
				throw new System.Security.SecurityException("User or password is incorrect.");

			entries[username] = md5.ComputeHash(encoding.GetBytes(newPassword));
		}

		public ICollection Users {
			get {
				return entries.Keys;
			}
		}

		public string GetPasswordHash(string username)
		{
			return BytesToHex(entries[username] as byte[]);
		}

		string BytesToHex(byte[] bytes)
		{
			System.Text.StringBuilder sb = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
				sb.Append(b.ToString("X2"));
			
			return sb.ToString();
		}

		byte[] HexToBytes(string hex)
		{
			if (hex.Length % 2 != 0)
				throw new ArgumentException("String must have an even length", "hex");

			byte[] bytes = new byte[hex.Length / 2];
			for (int i = 0; i < bytes.Length; i++) {
				byte b = (byte)(GetNibble(hex[i * 2]) << 4);
				b &= GetNibble(hex[i * 2 + 1]);
			}

			return bytes;
		}

		byte GetNibble(char b)
		{
			if (b >= '0' && b <= '9')
				return (byte)(b - '0');
			else if (b >= 'A' && b <= 'F')
				return (byte)(b - 'A');
			else if (b >= 'a' && b <= 'f')
				return (byte)(b - 'a');
			else
				throw new FormatException();
		}

		public bool Exists(string username)
		{
			return entries[username] != null;
		}

		Hashtable entries = new Hashtable();

		[NonSerialized]
		Encoding encoding = new UTF8Encoding(false, false);

		[NonSerialized]
		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

		static bool BytesEquals(byte[] array1, byte[] array2)
		{
			if (array1.Length != array2.Length)
				return false;

			for (int i = 0; i < array1.Length; i++)
				if (array1[i] != array2[i])
					return false;

			return true;
		}


		public bool Authenticate(string username, string password)
		{
			if (username == null)
				return false;
			if (password == null)
				return false;

			if (!entries.ContainsKey(username))
				return false;

			if (BytesEquals(entries[username] as byte[], md5.ComputeHash(encoding.GetBytes(password))))
				return true;
			else
				return false;
		}

	}
}
