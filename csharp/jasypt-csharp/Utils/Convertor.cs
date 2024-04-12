using System;
using System.IO;
using System.Text;

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
			int NumberChars = data.Length;
			var result = new byte[NumberChars / 2];
			for (int index = 0; index < NumberChars; index += 2)
				result[index / 2] = Convert.ToByte(data.Substring(index, 2), 16);
			return result;
		}

		public static byte[] StringToByteArray(String data) {
			var result = new byte[data.Length];
			for(int i = 0; i < data.Length; i++)
			{
				result[i] = (byte)data[i];
			}
			return result;
		}
		
		public static String StringtoHexString(String data) {
			String result = String.Empty;
			foreach (char c in data) {
				int value = c;
				result += String.Format
                       ("{0:x2}", System.Convert.ToUInt32(value.ToString()));
			}
			return result;
		}
	}
}

