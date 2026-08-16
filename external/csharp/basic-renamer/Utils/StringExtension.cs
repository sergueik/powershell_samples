using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Utils {
	public static class StringExtension {
		// static char[] separator = new char[] {' ', '-',' '};
		static string separator = " - ";
		// static string ellipsis = "\u2026";
		static string ellipsis = "~";

		static string SqueezeToken(string value, int maxLength, string filler) {
			int available = maxLength - filler.Length;
			int left = (available + 1) / 2;
			int right = available / 2;

			return value.Substring(0, left) + filler + value.Substring(value.Length - right);
		}
		
		public static string Squeeze(this string text, int maxLength) {
			var tokens = text.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			tokens = Regex.Split(text, Regex.Escape(separator));
			var candidate = tokens.Select(( string value, int index) => new { value, index }).Where(x => x.value.Length > maxLength).OrderByDescending(x => x.value.Length).FirstOrDefault();

			if (candidate == null)
				return text;
			return string.Join(separator, tokens.Select((value, index) => index == candidate.index ? SqueezeToken(value, maxLength, ellipsis) : value));
		}
	}
}
