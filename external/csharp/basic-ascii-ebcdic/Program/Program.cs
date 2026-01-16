using System;
using System.Text;
using System.Reflection;
using System.IO;
using Utils;

namespace Program {
	internal class Program {

		public static byte[] ConvertEbcdicToAscii(byte[] bytes, string codePage) {
			return Encoding.ASCII.GetBytes(Encoding.GetEncoding(codePage).GetString(bytes));
		}

		public static byte[] ConvertAsciiToEbcdic(byte[] bytes, string codePage) {
			return Encoding.Convert(Encoding.ASCII, Encoding.GetEncoding(codePage), bytes);;
		}
		private static bool debug = false;
		public static bool Debug { set { debug = value; } }
		private static string operation = "encode";
		private static string inputfile = null;
		private static string outputfile = null;
		private static string data = "0123456789abcdefghijklmnopqrstuvwxyz";
		private static byte[] ebcdicBytes = {};
		private static byte[] asciiBytes = {};
		private static byte[] bytes = {};
 	
		private static string codePage = "IBM037"; // Standard US EBCDIC code page
	
		
		public static void Main(string[] args) {
			var parseArgs = new ParseArgs(System.Environment.CommandLine);
			// NOTE: have to set debug with value true, switch arguments are not supported
			
			if (parseArgs.GetMacro("debug") != String.Empty)
				debug = true;
			if (parseArgs.GetMacro("operation") != String.Empty)
				operation = parseArgs.GetMacro("operation");

			if (parseArgs.GetMacro("version") != String.Empty) {
				var versionObj = Assembly.GetExecutingAssembly().GetName().Version;
				Console.Error.WriteLine("version: " + versionObj.ToString());
				Environment.Exit(0);

				// https://stackoverflow.com/questions/12977924/how-do-i-properly-exit-a-c-sharp-application 
				// Application.Exit();
			}
			if (parseArgs.GetMacro("data") != String.Empty)
				data = parseArgs.GetMacro("data");

			if (parseArgs.GetMacro("outputfile") != String.Empty)
				outputfile = parseArgs.GetMacro("outputfile");

			if (parseArgs.GetMacro("inputfile") != String.Empty){
				inputfile = parseArgs.GetMacro("inputfile");
				bytes = File.ReadAllBytes(inputfile);
			}

			if (operation.Equals("encode")) {
			
				ebcdicBytes = ConvertAsciiToEbcdic(Encoding.ASCII.GetBytes(data), codePage);
				// https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter?view=netframework-4.5
				// Console.WriteLine("EBCDIC bytes (hex): " + BitConverter.ToString(ebcdicBytes).Replace("-", string.Empty));
				Console.WriteLine("EBCDIC bytes (hex): " + Utils.Convertor.ByteArrayToHexString(ebcdicBytes));
			}
			
			if (operation.Equals("decode")) {
				ebcdicBytes = Utils.Convertor.HexStringToByteArray(data);
				asciiBytes = ConvertEbcdicToAscii(ebcdicBytes, codePage);
				Console.WriteLine(String.Format("Converted back to ASCII: {0}", Encoding.ASCII.GetString(asciiBytes)));
			}
		}
	}
}