using System;
using System.Linq;

using Utils;

namespace TestUtils {

	public static class Ini {
		
		public static string readValue(this IniFile iniFile, string section, string key, string defaultValue) {
			var	value = defaultValue;
			try {
				value = iniFile[section][key];
				if (value == null)
					value = defaultValue;
			} catch (Exception) {
				// ignore
			}
			return value;
		}
		
		public static bool hasFlag(this uint val, Utils.STGM flag) {
			return (val & (uint)flag) == (uint)flag;
		}
	}

}

