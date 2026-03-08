using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/**
 * Copyright 2024,2026 Serguei Kouzmine
 */

namespace Utils {
	public class Convertor {

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
			// deal with dash or whitespace formatted hex strings 
			data = Regex.Replace(data, "[^0-9A-Fa-f]", "");
			int NumberChars = data.Length;
			if ((NumberChars & 1) != 0) {
				throw new ArgumentException("Odd-length hex string");
			}
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

		public static ValidationResult validateUTF8(byte[] data) {
			bool status = false;
			string message = null;
		    try {
		        var utf8 = new UTF8Encoding(false, true); // throw on invalid bytes
		        string decoded = utf8.GetString(data);
		        status = true;
		    } catch (DecoderFallbackException e) {
		        message = String.Format("invalid UTF-8: {0}",e.Message);
		    }
	        return new ValidationResult(status, message);
		}
		
		public static ValidationResult validateASCII(byte[] data) {
			bool status = true;
			string message = null;
		    for (int cnt = 0; cnt < data.Length; cnt++) {
		        if (data[cnt] > 127) {
					status = false;
		            message = String.Format("invalid US-ASCII character 0x{0:X2} at offset {1}", data[cnt] ,cnt);
		        }
		    }
		    return new ValidationResult(status, message);
		}
		
		// EBCDIC charcode range validator
		public static ValidationResult validateEBCDIC(byte[] data) {
		    bool status = true;
		    string message = null;

			for (int cnt = 0; cnt < data.Length; cnt++) {
			    int charCode = data[cnt] & 0xFF;

			    if (charCode == 0) {
			        status = false;
			        message = String.Format("null character on {0}",cnt);
			    }
				// EBCDIC picture range probing
				// EBCDIC isn’t contiguous like ASCII				
			    bool valid =
			        charCode == 0x40 ||                     // space
			        (charCode >= 0xF0 && charCode <= 0xF9) || // digits
			        (charCode >= 0xC1 && charCode <= 0xC9) || // uppercase
			        (charCode >= 0xD1 && charCode <= 0xD9) ||
			        (charCode >= 0xE2 && charCode <= 0xE9) ||
			        (charCode >= 0x81 && charCode <= 0x89) || // lowercase
			        (charCode >= 0x91 && charCode <= 0x99) ||
			        (charCode >= 0xA2 && charCode <= 0xA9) ||
			        (charCode >= 0x4A && charCode <= 0x6F);  // punctuation window

			    if (!valid) {
			        status = false;
			        message = String.Format("invalid EBCDIC character 0x{0:X2} on {1}", charCode, cnt);
			    }
			}
		    return new ValidationResult(status, message);
		}

		private static readonly Dictionary<string, Func<byte[], ValidationResult>> validatorMap =
		    new Dictionary<string, Func<byte[], ValidationResult>>(StringComparer.OrdinalIgnoreCase) {
		    { "ascii", validateASCII },
		    { "us-ascii", validateASCII },
		    { "utf-8", validateUTF8 },
		    { "utf8", validateUTF8 },
		    { "ebcdic", validateEBCDIC },
		    { "IBM037", validateEBCDIC },
		    { "cp037", validateEBCDIC }
		};

		public static ValidationResult Validate(byte[] data, string charMap){
			if (string.IsNullOrEmpty(charMap))
				charMap = "ascii";
			Func<byte[], ValidationResult> validator = null;
			if (validatorMap.TryGetValue(charMap, out validator)) {
				return validator(data);
			} else
				return validateEBCDIC(data);
		}
	}
}

