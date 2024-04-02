#Copyright (c) 2023,2024 Serguei Kouzmine
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



param(
  [string]$properties = '',
  [string]$name = '',
  [string]$keyfile = '',
  [String]$value ='information',
  [string]$password = 'secred',
  [string]$operation = 'encrypt',
  [switch]$help,
  [switch]$debug
)

$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $debug.ToBool()
if ([bool]$PSBoundParameters['help'].IsPresent) {
write-host @'
Usage: jasypt.ps1 [-operation decrypt] -value [PAYLOAD] -password [PASSWORD] [-debug]  [-name NAME] [-keyfile KEYFILE] [-properties PROPERTIESFILE]

PBE encryption helper using PKCSKeyGenerator derived helper class, compatible with Jasypt
Param:
   operation - whether to encrypt (default) or decrypt the value
   value - payload. NOTE: for decrypt needs to be base64 encoded
   password - symmetric password
   debug - print additional information during processing

Example:

  .\jasypt.ps1 -value information -password secret
  LH4ONhWdAwWUbUb7wJdSHo3/Xv+LrfFl
  
  .\jasypt.ps1 -value 'LH4ONhWdAwWUbUb7wJdSHo3/Xv+LrfFl' -password secret -operation decrypt
  information

  .\jasypt.ps1 -key 'x\key.txt' -properties 'application.properties' -name 'name' -operation decrypt


'@
}
[boolean] $file_args = $false
[boolean] $file_args_valid = $false
if ($name -ne '') {
  $file_args = $true
  if ($keyfile -ne '') {
    $k = resolve-path $keyfile -erroraction silentlycontinue
    if ( -not ($k -eq $null)){
      if ($properties -ne '') {
        $p = resolve-path $properties -erroraction silentlycontinue
        if ( -not ($p -eq $null)){
          $file_args_valid = $true
        }
      }
    }
  }
}
if ($file_args) {
  if (-not ($file_args_valid)) {
    write-error 'invalid args'
    exit
  }
  # NOTE: do not get-content from the specicied text file "raw"
  # but split and trim all trailing whitespace from the first line
  # NOTE: without the index [0] one can drop parenthesis in the
  # "leading comma unary operator in expression mode" assignment 
  # still producing an array
  $key_content = (,(get-content -path $k.path))[0]
  # alternative:
  # $key_content = @(get-content -path $k.path.path))[0]
  $password = $key_content -replace ' *$', ''
  write-host ('password: {0}' -f $password)

  $config_line_regexp  = ('{0} *[:=] *(.*$)' -f $name)
  $matched_line_object = select-string -pattern $config_line_regexp -path $p.Path
  if (-not ($matched_line_object)) {
    write-error 'invalid args'
    exit
  }
  # NOTE: fragile
  $value_data = $matched_line_object.Matches[0].Captures[0].Groups[1].Value
  $value_data = $matched_line_object.Matches[0].Groups[1].Captures[0].Value
  # write-host ('value_data: {0}' -f $value_data)
  $matched_value_object = select-string -pattern 'ENC\(([^)]*)\)' -inputobject $value_data
  $value = $matched_value_object.Matches[0].Captures[0].Groups[1].Value
  write-host ('value: {0}' -f $value)
} else {
  # use provided value and password
  write-host ('value: {0}' -f $value)
  write-host ('password: {0}' -f $password)  
}
$helperclass = 'Example.Program'
add-type -language CSharp -typedefinition @'

using System;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Text;
using System.Linq;
using System.IO;
using System.Security.Cryptography;

namespace Example
{

	public class Program {
		private bool debug;
		public bool Debug { set { debug = value;} }
		public void decrypt(String value, String password, int obtentionIterations, int segments)
		{
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
				return;
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
				return;
			}
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
			} catch (CryptographicException e) {
				// Unhandled Exception: System.Security.Cryptography.CryptographicException: Bad Data.
				Console.Error.WriteLine("Error: check your inputs - expected a valid base64 encoded string");
				if (debug)
					throw e;
				return;
			}

		}

		public void encrypt(String value, String password, String saltString, int obtentionIterations, int segments)
		{
			byte[] salt = { };
			
			if (String.IsNullOrEmpty(saltString)) {
				Random random = new Random();
				salt = new Byte[8];
				random.NextBytes(salt);
			} else {
				salt = Convertor.StringToByteArray(saltString);
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

		}
	}

}
	public class ParseArgs
	{

		private bool _DEBUG = false;
		public bool DEBUG {
			get { return _DEBUG; }
			set { _DEBUG = value; }
		}
		private StringDictionary _MACROS;

		private StringDictionary AllMacros {
			get { return _MACROS; }
		}

		private bool DefinedMacro(string sMacro)
		{
			return _MACROS.ContainsKey(sMacro);
		}

		public string GetMacro(string sMacro)
		{

			return (DefinedMacro(sMacro)) ?
			_MACROS[sMacro] : String.Empty;
		}

		public string SetMacro(string sMacro, string sValue)
		{
			_MACROS[sMacro] = sValue;
			return _MACROS[sMacro];
		}

		public ParseArgs(string sLine)
		{

			_MACROS = new StringDictionary();

			string s = @"(\s|^)(?<token>(/|-{1,2})(\S+))";
			var r = new Regex(s, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
			MatchCollection m = null;
			try {
				m = r.Matches(sLine);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			if (m != null) {

				for (int i = 0; i < m.Count; i++) {
					string sToken = m[i].Groups["token"].Value.ToString();
					// Console.WriteLine("{0}", sToken);
					ParseSwithExpression(sToken);
				}
			}
			return;
		}

		private void ParseSwithExpression(string sToken)
		{
			string s = @"(/|\-{1,2})(?<macro>[a-z0-9_\-\\\@\$\#]+)([\=\:](?<value>[\:\/a-z0-9_\.\,\\\-\+\@\$\#\=]+))*";

			var r = new Regex(s, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
			MatchCollection m = r.Matches(sToken);

			if (m != null) {
				for (int i = 0; i < m.Count; i++) {
					string sMacro = m[i].Groups["macro"].Value.ToString();

					string sValue = m[i].Groups["value"].Value;
					if (sValue == "")
						sValue = "true";
					SetMacro(sMacro, sValue);
					if (DEBUG)
						Console.WriteLine("{0} = \"{1}\"", sMacro, GetMacro(sMacro));
				}
			}
			return;
		}

	}

public class Convertor
{
 
	public static string ByteArrayToString(byte[] bytes)
	{
		return System.Text.Encoding.Default.GetString(bytes);
	}

	public static string ByteArrayToHexString(byte[] bytes)
	{
		var hex = new StringBuilder(bytes.Length * 2);
		foreach (byte b in bytes)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}
	public static byte[] StringToByteArray(String hex)
	{
		int NumberChars = hex.Length;
		var bytes = new byte[NumberChars / 2];
		for (int index = 0; index < NumberChars; index += 2)
			bytes[index / 2] = Convert.ToByte(hex.Substring(index, 2), 16);
		return bytes;
	}
}
public class PKCSKeyGenerator
{
	byte[] key = new byte[8], iv = new byte[8];
	DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        
	public bool Debug { get; set; }

	public byte[] Key { get { return key; } }
	public byte[] IV { get { return iv; } }
	public ICryptoTransform Encryptor { get { return des.CreateEncryptor(key, iv); } }
	public ICryptoTransform Decryptor { get { return des.CreateDecryptor(key, iv); } }
	public PKCSKeyGenerator()
	{
	}
	public PKCSKeyGenerator(String password, byte[] salt, 
		int obtentionIterations, int segments)
	{
		Generate(password, salt, obtentionIterations, segments);
	}
        
	public void Generate(String password, byte[] salt, 
		int obtentionIterations, int segments)
	{
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
		// ----  Now hash consecutively for obtentionIterations times ------
                
		for (int i = 0; i < obtentionIterations; i++)
			data = md5.ComputeHash(data);
		// kept for compatibility with Perl version in debug mode
		var DK = new byte[16];
		Array.Copy(data, 0, DK, 0, 16);
		if (Debug)
			Console.Error.WriteLine("DK: " + Convertor.ByteArrayToHexString(data));
		Array.Copy(data, 0, key, 0, 8);
		Array.Copy(data, 8, iv, 0, 8);
            
		return;
	}
	
}
'@ -referencedassemblies 'System.Security.dll','System.Data.dll'
[int]$obtentionIterations = 1000
[int]$segments = 1
$o = new-object Example.Program
$o.Debug = $debug
if ($operation -eq 'decrypt') {
   $o.decrypt($value, $password, $obtentionIterations, $segments)
} else {
   $o.encrypt($value, $password, '', $obtentionIterations, $segments)
}
