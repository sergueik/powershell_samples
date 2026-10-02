using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

namespace Utils
{
	public static class StringExtension
	{
		// static char[] separator = new char[] {' ', '-',' '};
		static string separator = " - ";
		// static string ellipsis = "\u2026";
		static string ellipsis = "~";

		static string SqueezeToken(string value, int maxLength, string filler)
		{
			int available = maxLength - filler.Length;
			int left = (available + 1) / 2;
			int right = available / 2;

			return value.Substring(0, left) + filler + value.Substring(value.Length - right);
		}
		
		public static string Squeeze(this string text, int maxLength) {
			var result = text; // shoul use copy ?
			while (result.Length > maxLength + 10) {
				var tokens = Regex.Split(result, Regex.Escape(separator));
			
				var candidate = tokens.Select(( string value, int index) => new { value, index }).OrderByDescending(x => x.value.Length).Take(2).ToList().Shuffle().FirstOrDefault(); 
				if (candidate != null){
					Debug.WriteLine(String.Format("candidate: {0} length : {1}",candidate, result.Length));
					result = string.Join(separator, tokens.Select((value, index) => index == candidate.index ? SqueezeToken(value, value.Length - 1, ellipsis) : value));

				}				
				
			}
			return result;
		}
	}
}
