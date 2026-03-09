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

		public static String byteArrayToString(byte[] bytes) {
			return System.Text.Encoding.Default.GetString(bytes);
		}

		public static String byteArrayToHexString(byte[] data) {
			var stringBuilder = new StringBuilder(data.Length * 2);
			foreach (byte b in data)
				stringBuilder.Append(b.ToString("X2"));
			return stringBuilder.ToString();
		}

		public static byte[] hexStringToByteArray(String data) {
			data = Regex.Replace(data, "[^0-9A-Fa-f]", "");
			int numberChars = data.Length;

			if ((numberChars & 1) != 0)
				throw new ArgumentException("Odd-length hex string");

			byte[] hexByteArray = new byte[numberChars / 2];

			for (int index = 0; index < numberChars; index += 2)
				hexByteArray[index / 2] =
					Convert.ToByte(data.Substring(index, 2), 16);

			return hexByteArray;
		}

		public static String StringtoHexString(String data) {
			String hexString = String.Empty;

			foreach (char c in data) {
				int value = c;
				hexString += String.Format(
					"{0:x2}",
					System.Convert.ToUInt32(value.ToString()));
			}

			return hexString;
		}

		private static readonly Predicate<int> isAsciiChar =
			delegate(int charCode) {
				return charCode <= 0x7F;
			};

		public static Predicate<int> getPredicate(string codePage) {
			switch (codePage) {
					case "IBM037": return isEbcdicChar;
					case "ASCII": return isAsciiChar;
					case "UTF-8":return null;
					default: return null;
			}
		}
		private static readonly Predicate<int> isEbcdicChar =
			delegate(int charCode) {
					return
		// space
		charCode == 0x40 ||
		// digits
		(charCode >= 0xF0 && charCode <= 0xF9) ||
		// uppercase letters
		(charCode >= 0xC1 && charCode <= 0xC9) || (charCode >= 0xD1 && charCode <= 0xD9)
		|| (charCode >= 0xE2 && charCode <= 0xE9) ||
		// lowercase letters
		(charCode >= 0x81 && charCode <= 0x89) || (charCode >= 0x91 && charCode <= 0x99)
		|| (charCode >= 0xA2 && charCode <= 0xA9) ||
		// basic punctuation
		(charCode >= 0x4A && charCode <= 0x6F) ||
		// generic fallback bytes for Western European characters
	    // NOTE: these represent accented letters or symbols outside ASCII,
		charCode == 0x45 ||charCode == 0xCE || charCode == 0xE9 || charCode == 0xD3 || charCode == 0xC7;
		};
		
		private static String decodeUTF8(byte[] data) {
			var utf8 = new UTF8Encoding(false, true);
			return utf8.GetString(data);
		}

		public static String decodeEBCDIC(byte[] data) {

			Encoding enc = Encoding.GetEncoding( "IBM037", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
			return enc.GetString(data);
		}

		public static ValidationResult validateCore( byte[] data, String codePage, Func<byte[], String> decoder, Predicate<int> rangeValidator, double? threshold) {

			bool status = true;
			String message = null;
			int validCount = 0;

			if (decoder != null) {
				try {
					decoder(data);
				} catch (Exception e) {

					return new ValidationResult( false, String.Format( "failed to decode in code page {0}: {1}", codePage, e.Message));
				}
			}

			if (rangeValidator == null)
				return new ValidationResult(true, null);

			for (int position = 0; position < data.Length; position++) {

				int charCode = data[position] & 0xFF;

				if (charCode == 0) {

					status = false;

					message = String.Format(
						"null character in code page {0} at position {1}",
						codePage,
						position);
				}

				bool valid = rangeValidator(charCode);

				if (valid)
					validCount++;

				if (!valid && threshold == null) {
					status = false;
					message = String.Format( "invalid code page {0} character 0x{1:X2} at position {2}", codePage, charCode, position);
				}
			}

			if (threshold != null) {
				double ratio = (double)validCount / data.Length;
				if (ratio < threshold.Value) {
					status = false;
					message = String.Format( "valid byte ratio {0:F2} below threshold {1:F2} for code page {2}", ratio, threshold.Value, codePage);
				}
			}

			return new ValidationResult(status, message);
		}

		public static ValidationResult validateUTF8(byte[] data) {

			return validateCore( data, "UTF-8", decodeUTF8, null, null);
		}

		public static ValidationResult validateASCII(byte[] data) {

			return validateCore( data, "US-ASCII", null, isAsciiChar, null);
		}

		public static ValidationResult validateASCII(byte[] data, double threshold) {

			return validateCore( data, "US-ASCII", null, isAsciiChar, threshold);
		}

		public static ValidationResult validateEBCDIC(byte[] data) {

			return validateCore( data, "IBM037", decodeEBCDIC, isEbcdicChar, null);
		}

		public static ValidationResult validateEBCDIC(byte[] data, double threshold) {

			return validateCore( data, "IBM037", decodeEBCDIC, isEbcdicChar, threshold);
		}

		// validator registry
		private static readonly Dictionary<string, Func<byte[], ValidationResult>> validatorMap =
			new Dictionary<string, Func<byte[], ValidationResult>>(StringComparer.OrdinalIgnoreCase) {

				{ "ascii", validateASCII },
				{ "us-ascii", validateASCII },
				{ "utf8", validateUTF8 },
				{ "utf-8", validateUTF8 },
				{ "ebcdic", validateEBCDIC },
				{ "ibm037", validateEBCDIC },
				{ "cp037", validateEBCDIC }
			};

		public static ValidationResult validate(byte[] data, string charMap) {

			if (String.IsNullOrEmpty(charMap))
				charMap = "ascii";

			Func<byte[], ValidationResult> validator = null;

			if (validatorMap.TryGetValue(charMap, out validator))
				return validator(data);

			return validateEBCDIC(data);
		}
	}
}
