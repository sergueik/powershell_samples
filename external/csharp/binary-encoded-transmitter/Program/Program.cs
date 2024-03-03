﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Compression;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using CommandLine;

namespace Program {
	public class Options
	{
		[Option('i', "inputs", Required = true, HelpText = "Specifies the input file(s) to process, you can use also a wildcard pattern or specify multiple files separted by space")]
		public String Inputs { get; set; }

		[Option('o', "outputfolder", Required = false, HelpText = "Specify the output folder where all the powershell scripts will be generated")]
		public String OutputFolder { get; set; }

		[Option('b', "base64", Required = false, HelpText = "Specify the base64 file format for the powershell script(s)")]
		public bool Base64 { get; set; }

		[Option('d', "decimal", Required = false, HelpText = "Specify the decimal file format for the powershell script(s)")]
		public bool Decimal { get; set; }

		[Option('c', "compress", Required = false, HelpText = "Specify to compress the input file(s) with gzip compression")]
		public bool Compress { get; set; }

		[Option('h', "hash", Required = false, HelpText = "Specify to add a SHA256 hash as check on file(s) integrity in the powershell script(s)")]
		public bool Hash { get; set; }

		[Option('s', "single", Required = false, HelpText = "Specify to create just a single script file for all input files")]
		public bool SingleFile { get; set; }

		[Option('p', "password", Required = false, HelpText = "Specify the password used to encrypt data with AES")]
		public String Password { get; set; }

		[Option('r', "recurse", Required = false, HelpText = "Specify to perform recursive search on all input file(s)")]
		public bool Recurse { get; set; }
	}


	class Program
	{
		const int KEYSIZE = 256;

		public static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => CreateScript(o));
		}



		static string ComputeSha256Hash(byte[] bytes)
		{
			using (SHA256 sha256Hash = SHA256.Create()) {
				return BitConverter.ToString(sha256Hash.ComputeHash(bytes)).Replace("-", String.Empty);
			}

		}


		public static byte[] EncryptBytes(byte[] input, string password)
		{
			var pbkdf2DerivedBytes = new Rfc2898DeriveBytes(password, 16, 2000);

			using (var AES = Aes.Create()) {
				AES.KeySize = KEYSIZE;
				AES.Key = pbkdf2DerivedBytes.GetBytes(KEYSIZE / 8);
				AES.Mode = CipherMode.CBC;
				AES.Padding = PaddingMode.PKCS7;

				using (var memoryStream = new MemoryStream()) {
					var cryptoStream = new CryptoStream(memoryStream, AES.CreateEncryptor(), CryptoStreamMode.Write);

					memoryStream.Write(pbkdf2DerivedBytes.Salt, 0, 16);  // 16 bytes of SALT for PBKDF2 derivation function, must not be encrypted
					memoryStream.Write(AES.IV, 0, 16);  // IV is always 128 bits for AES, must not be encrypted
					cryptoStream.Write(input, 0, input.Length);
					cryptoStream.FlushFinalBlock();

					// uncomment this line to debug encryption
					// Console.WriteLine(String.format("Password {0} Salt {1} IV {2} Key {3} Input {4} ActualPosition {5}",password, BitConverter.ToString(pbkdf2DerivedBytes.Salt)),BitConverter.ToString(AES.IV),BitConverter.ToString(AES.Key), BitConverter.ToString(input),memoryStream.Length ));

					return memoryStream.ToArray();
				}
			}
		}


		public static byte[] CopyBytesToStream(byte[] bytes, bool fromStream, Func<Stream, Stream> streamCallback)
		{

			var inputMemoryStream = new MemoryStream(bytes);
			var outputMemoryStream = new MemoryStream();

			var stream = streamCallback(fromStream ? inputMemoryStream : outputMemoryStream);

			if (fromStream)
				stream.CopyTo(outputMemoryStream);
			else {
				inputMemoryStream.CopyTo(stream);
				stream.Flush();
			}

			return outputMemoryStream.ToArray();
		}



		private static StringBuilder CreateScriptHeader(Options o)
		{
			var script = new StringBuilder();

			if (o.Compress || !String.IsNullOrEmpty(o.Password)) {
				script.AppendLine(@"
function copyBytesToStream  {
    [OutputType([byte[]])]
    Param (
        [Parameter(Mandatory=$true)] [byte[]] $bytes,
        [Parameter(Mandatory=$true)] [System.Boolean] $fromStream,
        [Parameter(Mandatory=$true)] [ScriptBlock] $streamCallback)

    $InputMemoryStream = New-Object System.IO.MemoryStream @(,$bytes)
    $OutputMemoryStream = New-Object System.IO.MemoryStream

    $stream = (Invoke-Command $streamCallback -ArgumentList $(if ($fromStream) { $InputMemoryStream } else { $OutputMemoryStream }))

    if ($fromStream) {
        $stream.CopyTo($OutputMemoryStream)
    }
    else {
        $InputMemoryStream.CopyTo($stream)
        $stream.Flush()
    }

    $result = $OutputMemoryStream.ToArray()

    ,$result
}");
			}

			if (!o.Base64 && !o.Decimal) {
				script.AppendLine(@"
function StringToByteArray  {
    [OutputType([byte[]])]
    Param ([Parameter(Mandatory=$true)] [System.String] $hexstring)

    [byte[]] $bytes = New-Object Byte[] ($hexstring.Length/2)
    for ($i=0; $i -lt $hexstring.Length;$i+=2) {
        $bytes[$i/2] = [System.Byte]::Parse($hexstring.Substring($i,2),[System.Globalization.NumberStyles]::HexNumber)
    }
    ,$bytes
    }

");
			}




			if (!String.IsNullOrEmpty(o.Password)) {
				// uncomment these lines and put them in the decryptBytes function below (row "$Dec = $AES.CreateDecryptor()") to troubleshoot encryption
				//Write - Host ""Password $password""
				//Write - Host ""KEY: $([System.BitConverter]::ToString($AES.Key))""
				//Write - Host ""IV: $([System.BitConverter]::ToString($AES.IV))""
				//Write - Host ""EncryptedData: $([System.BitConverter]::ToString($EncryptedData))""

				// uncomment these lines and put them in the decryptBytes function below (row ",$result") to troubleshoot encryption
				//Write - Host ""DecryptedData: $([System.BitConverter]::ToString($result))""

				script.Append(String.Format(@"function decryptBytes {{
    [OutputType([byte[]])]
    Param (
		[parameter(Mandatory=$true)] [System.Byte[]] $bytes,
		[parameter(Mandatory=$true)] [System.String] $password
	) 

    # Split IV and encrypted data
    $PBKDF2Salt = New-Object Byte[] 16
    $IV = New-Object Byte[] 16
    $EncryptedData = New-Object Byte[] ($bytes.Length-32)
    
    [System.Array]::Copy($bytes, 0, $PBKDF2Salt, 0, 16)
    [System.Array]::Copy($bytes, 16, $IV, 0, 16)
    [System.Array]::Copy($bytes, 32, $EncryptedData, 0, $bytes.Length-32)

	# Generate PBKDF2 from Salt and Password
	$PBKDF2 = New-Object System.Security.Cryptography.Rfc2898DeriveBytes($password, $PBKDF2Salt, 2000)

	# Setup our decryptor
	$AES = [Security.Cryptography.Aes]::Create()
	$AES.KeySize = {KEYSIZE}
	$AES.Key = $PBKDF2.GetBytes({KEYSIZE / 8})
	$AES.IV = $IV
	$AES.Mode = [System.Security.Cryptography.CipherMode]::CBC
	$AES.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7

	$Dec = $AES.CreateDecryptor()

    [byte[]] $result = copyBytesToStream $EncryptedData $true {{ param ($EncryptedStream) New-Object System.Security.Cryptography.CryptoStream($EncryptedStream, $Dec, [System.Security.Cryptography.CryptoStreamMode] 'Read') }} 

	,$result
}}

"));
			}

			var decryptCode = String.IsNullOrEmpty(o.Password) ? String.Empty : "\t\t$bytes = $(decryptBytes $bytes $password)";

			var decompressCodeMultiRow = @"
        if ($decompress) {
            $bytes = copyBytesToStream $bytes $true { param ($EncryptedStream) New-Object System.IO.Compression.DeflateStream($EncryptedStream, [System.IO.Compression.CompressionMode ] 'Decompress') } 
        }
";
			var decompressCode = o.Compress ? decompressCodeMultiRow : String.Empty;


			var hashCodeMultiRow = @"
        if (![System.String]::IsNullOrEmpty($hash)) {
            $actualHash = (Get-FileHash -Path $file -Algorithm Sha256).Hash
            if ($actualHash -ne $hash) {
                Write-Error ""Integrity check failed on $file expected $hash actual $actualHash!""
            }
        }
";
			var hashCode = o.Hash ? hashCodeMultiRow : String.Empty;


			script.Append(String.Format(@"function createFile  {{
	param (
		[parameter(Mandatory=$true)] [String] $file,
		[parameter(Mandatory=$true)] [byte[]] $bytes,
		[parameter(Mandatory=$false)] [String] $password,
        [Parameter(Mandatory=$false)] [String] $hash,
        [Parameter(Mandatory=$false)] [System.Boolean] $decompress=$false)
	
		$null = New-Item -ItemType Directory -Path (Split-Path $file) -Force
{decryptCode}
{decompressCode}
		if ($global:core) {{ Set-Content -Path $file -Value $bytes -AsByteStream -Force }} else {{ Set-Content -Path $file -Value $bytes -Encoding Byte -Force }}

{hashCode}
        Write-Host ""Created file $file Length $($bytes.Length)""
	}}

"));



			script.Append(String.Format("function createFiles  {{\n\tparam ([parameter(Mandatory={0})] [String] $password)\n\n", (String.IsNullOrEmpty(o.Password) ? "$false" : "$true")));
			script.Append("\t$setContentHelp = (help Set-Content) | Out-String\n\tif ($setContentHelp.Contains(\"AsByteStream\")) { $global:core = $true } else { $global:core = $false }\n\n");

			return script;
		}



		public static void CreateScript(Options o)
		{
			if (String.IsNullOrEmpty(o.OutputFolder))
				o.OutputFolder = Directory.GetCurrentDirectory();
			else
				Directory.CreateDirectory(o.OutputFolder);

			StringBuilder script = CreateScriptHeader(o);

			var outputFile = Path.Combine(o.OutputFolder, "SingleScript.ps1");

			foreach (var input in o.Inputs.Split(new Char []{','})) {
				var actualCompress = false;

				var path = Path.GetDirectoryName(input);
				foreach (var file in Directory.GetFiles(!String.IsNullOrEmpty(path) ? path : ".", Path.GetFileName(input), o.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {

					if (!o.SingleFile) {
						script = CreateScriptHeader(o);

						outputFile = Path.Combine(o.OutputFolder, String.Format("{0}_script.ps1", Path.GetFileName(file).Replace(".", "_")));
					}

					// TODO:
					Console.Write(String.Format("Scripting file {0} {1}", file, (!o.SingleFile ? String.Format("into {1}...", outputFile) : "String.Empty")));

					var inputBytes = File.ReadAllBytes(file);
					var hash = ComputeSha256Hash(inputBytes);

					if (o.Compress) {
						var compressedFileBytes = CopyBytesToStream(inputBytes, false, encryptedStream => new DeflateStream(encryptedStream, CompressionMode.Compress));

						if (compressedFileBytes.Length > 0 && compressedFileBytes.Length < inputBytes.Length) {
							inputBytes = compressedFileBytes;
							actualCompress = true;
						} else
							Console.Write("compression is useless, disabling it...");
					}

					var bytes = String.IsNullOrEmpty(o.Password) ? inputBytes : EncryptBytes(inputBytes, o.Password);

					if (o.Base64) {
						script.Append(String.Format(@"\t[byte[]] $bytes = [Convert]::FromBase64String('{0}')", Convert.ToBase64String(bytes)));
					} else {
						script.Append(o.Decimal ? "\t[byte[]] $bytes = " : "\t[byte[]] $bytes = (StringToByteArray '");

						foreach (var b in bytes) {
							if (o.Decimal)
								script.Append(String.Format("{0},", b.ToString("D")));
							else
								script.Append(String.Format("{0}", b.ToString("X2")));
						}

						if (!o.Decimal)
							script.Append("')");
						else
							script.Length--;
					}
					// TODO:
					script.Append(String.Format("\n\tcreateFile '{0}' $bytes $password {1} {2}\n\n", file, (o.Hash ? String.Format(@"'{0}'", hash) : "''"), (o.Compress ? String.Format(@"${}", actualCompress) : "$false")));

					if (!o.SingleFile) {
						script.Append(String.Format("}}\n\ncreateFiles '{0}'\n", o.Password));

						var outputScript = script.ToString();
						File.WriteAllText(outputFile, outputScript);
						Console.WriteLine(String.Format(@"length {0}KB.", Math.Round(outputScript.Length / 1024.0)));
					} else
						Console.WriteLine("");
				}
			}

			if (o.SingleFile) {
				script.Append(String.Format("}}\n\ncreateFiles '{0}'\n", o.Password));

				var outputScript = script.ToString();
				File.WriteAllText(outputFile, outputScript);

				Console.WriteLine(String.Format("Created single script file {0} length {1}KB.", outputFile, Math.Round(outputScript.Length / 1024.0)));
			}
		}

	}
}