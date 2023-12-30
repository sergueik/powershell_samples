using System;
using System.Collections.Generic;


// http://www.java2s.com/Code/CSharp/Collections-Data-Structure/DictionaryPrettyPrint.htm
namespace ScriptServices {
	public static class DictionaryExtensions {
		public static string PrettyPrint<Key, Val>(this IDictionary<Key, Val> data) {
			if (data == null)
				return "";
			string result = "[";
			ICollection<Key> keys = data.Keys;
			int i = 0;
			foreach (Key key in keys) {
				result += key.ToString() + "=" + data[key].ToString();
				if (i++ < keys.Count - 1) {
					result += ", ";
				}
			}
			return result + "]";
		}

	}
}
