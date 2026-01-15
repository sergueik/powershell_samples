using System;
using System.Text;

namespace ASCII_EBCIDIC_Converter {
	internal class Program {

		public static string ConvertEbcdicToAscii(byte[] ebcdicBytes, int codePage) {
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);
			string unicodeString = ebcdicEncoding.GetString(ebcdicBytes);
			byte[] asciiBytes = Encoding.ASCII.GetBytes(unicodeString);
			return Encoding.ASCII.GetString(asciiBytes);
		}

		public static byte[] ConvertAsciiToEbcdic(string asciiString, int codePage) {
			Encoding asciiEncoding = Encoding.ASCII;
			Encoding ebcdicEncoding = Encoding.GetEncoding(codePage);
			byte[] asciiBytes = asciiEncoding.GetBytes(asciiString);
			byte[] ebcdicBytes = Encoding.Convert(asciiEncoding, ebcdicEncoding, asciiBytes);
			return ebcdicBytes;
		}

		public static void Main(string[] args) {

			string originalAscii = "0123456789abcdefghijklmnopqrstuvwxyz";
			int ibm037CodePage = 37; // Standard US EBCDIC code page

			byte[] ebcdicData = ConvertAsciiToEbcdic(originalAscii, ibm037CodePage);
			// Console.WriteLine("EBCDIC bytes (hex): " + BitConverter.ToString(ebcdicData));
			// https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter?view=netframework-4.5
			Console.WriteLine("EBCDIC bytes (hex): " + BitConverter.ToString(ebcdicData).Replace("-", string.Empty));

			string convertedAscii = ConvertEbcdicToAscii(ebcdicData, ibm037CodePage);
			Console.WriteLine(String.Format("Converted back to ASCII: {0}", convertedAscii));

		}
	}
}