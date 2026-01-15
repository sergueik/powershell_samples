using System;
using System.Text;

namespace ASCII_EBCIDIC_Converter {
	internal class Program {

		public static byte[] ConvertEbcdicToAscii(byte[] ebcdicBytes, string codePage) {
			byte[] asciiBytes = Encoding.ASCII.GetBytes(Encoding.GetEncoding(codePage).GetString(ebcdicBytes));
			return asciiBytes;
		}

		public static byte[] ConvertAsciiToEbcdic(byte[] asciiBytes, string codePage) {
			byte[] ebcdicBytes = Encoding.Convert(Encoding.ASCII, Encoding.GetEncoding(codePage), asciiBytes);
			return ebcdicBytes;
		}

		public static void Main(string[] args) {

			string data = "0123456789abcdefghijklmnopqrstuvwxyz";
			const string codePage = "IBM037"; // Standard US EBCDIC code page

			byte[] ebcdicBytes = ConvertAsciiToEbcdic(Encoding.ASCII.GetBytes(data), codePage);
			// https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter?view=netframework-4.5
			Console.WriteLine("EBCDIC bytes (hex): " + BitConverter.ToString(ebcdicBytes).Replace("-", string.Empty));

			byte[] asciiBytes = ConvertEbcdicToAscii(ebcdicBytes, codePage);
			Console.WriteLine(String.Format("Converted back to ASCII: {0}", Encoding.ASCII.GetString(asciiBytes)));
			}
	}
}