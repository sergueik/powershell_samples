using System;
using System.IO;
using System.Text;

namespace ASCII_EBCDIC_Converter
{
	internal class Convertor
	{
		public static void Convert(string inFile, string inputType, string outFile, bool crlf, int crlfLength,
			string codepage)
		{
			byte[] fileArray = File.ReadAllBytes(inFile);

			if (inputType.ToLower() == "ebcdic")
				fileArray = ConvertAsciiToEbcdic(fileArray, codepage);
			else if (inputType.ToLower() == "ascii")
				fileArray = ConvertEbcdicToAscii(fileArray, codepage);
			else
				throw new Exception(
					"Unable to process the conversion type. Please check the converto parameter in the config file.");

			using (var fileStream = new FileStream(outFile, FileMode.Create, FileAccess.Write)) {
				int byteCount = 0;
				int offset = 0;

				if (crlf) {
					var byteArray = new byte[crlfLength + Encoding.Default.GetBytes(Environment.NewLine).Length];
					Console.Error.WriteLine(String.Format("Saving {0} bytes to {1}", fileArray.Length, outFile));
					if (fileArray.Length > 3000) {
						foreach (byte b in fileArray) {
							byteArray[byteCount++] = b;
							if (byteCount % 3000 == 0) {
								foreach (byte _byte in Encoding.Default.GetBytes(Environment.NewLine))
									byteArray[byteCount++] = _byte;
								fileStream.Write(byteArray, 0, byteArray.Length);
								offset += byteCount;
								byteCount = 0;
							}
						}
					} else {
						Console.Error.WriteLine(String.Format("Saving {0} bytes to {1}", fileArray.Length, outFile));
						fileStream.Write(byteArray, 0, byteArray.Length);
 
					}
				}
				fileStream.Close();
			}
		}
/*
		public static string ConvertEbcdicToAscii(byte[] ebcdicBytes, string codepage) {
			// Get the EBCDIC encoding object for a specific code page (e.g., IBM037)
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);

			// Convert the EBCDIC bytes to a Unicode string
			string unicodeString = ebcdicEncoding.GetString(ebcdicBytes);

			// Convert the Unicode string to an ASCII byte array
			byte[] asciiBytes = Encoding.ASCII.GetBytes(unicodeString);

			// Convert the ASCII byte array back to an ASCII string
			return Encoding.ASCII.GetString(asciiBytes);
		}
		*/
		public static byte[]  ConvertEbcdicToAscii(byte[] ebcdicBytes, string codePage) {
			// Get the EBCDIC encoding object for a specific code page (e.g., IBM037)
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);

			// Convert the EBCDIC bytes to a Unicode string
			string unicodeString = ebcdicEncoding.GetString(ebcdicBytes);

			// Convert the Unicode string to an ASCII byte array
			byte[] asciiBytes = Encoding.ASCII.GetBytes(unicodeString);
	
			// Convert the ASCII byte array back to an ASCII string
			return asciiBytes;
			// return Encoding.ASCII.GetString(asciiBytes);
		}

		public static byte[] ConvertAsciiToEbcdic(string asciiString, string codePage)
		{
			// Get the ASCII encoding object and the target EBCDIC encoding object
			Encoding asciiEncoding = Encoding.ASCII;
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);

			// Convert the ASCII string to a byte array using the Encoding.Convert method
			byte[] asciiBytes = asciiEncoding.GetBytes(asciiString);
			byte[] ebcdicBytes = Encoding.Convert(asciiEncoding, ebcdicEncoding, asciiBytes);

			return ebcdicBytes;
		}

		public static byte[] ConvertAsciiToEbcdic(byte[] asciiBytes, string codePage)
		{
			// Get the ASCII encoding object and the target EBCDIC encoding object
			Encoding asciiEncoding = Encoding.ASCII;
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);

			// Convert the ASCII string to a byte array using the Encoding.Convert method
			byte[] ebcdicBytes = Encoding.Convert(asciiEncoding, ebcdicEncoding, asciiBytes);

			return ebcdicBytes;
		}
		/*
		#region public static byte[] ConvertAsciiToEbcdic(byte[] asciiData)

		public static byte[] ConvertAsciiToEbcdic(byte[] asciiData, string codepage)
		{
			Console.Error.WriteLine("ConvertAsciiToEbcdic");
			Encoding ascii = Encoding.ASCII;
			Encoding ebcdic = Encoding.GetEncoding(codepage);
			return Encoding.Convert(ascii, ebcdic, asciiData);
		}

		#endregion

		#region public static byte[] ConvertEbcdicToAscii(byte[] ebcdicData)

		public static byte[] ConvertEbcdicToAscii(byte[] ebcdicData, string codepage)
		{
			Console.Error.WriteLine("ConvertEbcdicToAscii");
			Encoding ascii = Encoding.ASCII;
			Encoding ebcdic = Encoding.GetEncoding(codepage);
			return Encoding.Convert(ebcdic, ascii, ebcdicData);
		}

		#endregion
		*/
	}
}