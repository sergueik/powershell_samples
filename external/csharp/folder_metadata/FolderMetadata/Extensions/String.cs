using HKS.FolderMetadata.Generic;
using System;
using System.Collections.Generic;

namespace HKS.FolderMetadata.Extensions
{
	/// <summary>
	/// This extension class adds additional functions to the System.String class
	/// </summary>
	public static class String
	{
		/// <summary>
		/// Checks if a string is empty.
		/// </summary>
		/// <param name="s">The string to check</param>
		/// <returns>Returns true if the string is empty, otherwise false.</returns>
		public static bool IsEmpty(this string s)
		{
			return (s.Length == 0);
		}

		/// <summary>
		/// Checks if a string equals any of the specified values
		/// </summary>
		/// <param name="s">The string to check</param>
		/// <param name="comparisonType">The comparison method to compare the strings</param>
		/// <param name="values">The values to check against</param>
		/// <returns>Returns true if the string matches any of the specified values. If the string is null or empty or does not match, it returns false.</returns>
		public static bool IsAnyValue(this string s, StringComparison comparisonType, params string[] values)
		{
			if (ArrayHelper.IsNullOrEmpty(values))
			{
				return false;
			}

			foreach (string value in values)
			{
				if (string.Equals(s, value, comparisonType))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if a string equals any of the specified values ignoring the case using the invariant culture.
		/// </summary>
		/// <param name="s">The string to check</param>
		/// <param name="values">The values to check against</param>
		/// <returns>Returns true if the string matches any of the specified values. If the string is null or empty or does not match, it returns false.</returns>
		public static bool IsAnyValue(this string s, params string[] values)
		{
			return s.IsAnyValue(StringComparison.InvariantCultureIgnoreCase, values);
		}

		/// <summary>
		/// Converts a semicolon separated string into a list of strings.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>Returns a list or null.</returns>
		public static List<string> ToList(this string s)
		{
			string[] entries = s.Split(';');
			List<string> retVal = new List<string>();
			int length = entries.Length;
			for (int i = 0; i < length; i++)
			{
				string entry = entries[i];
				if (string.IsNullOrEmpty(entry))
				{
					continue;
				}

				entry = entry.Trim();
				if (entry.IsEmpty())
				{
					continue;
				}

				retVal.Add(entry);
			}

			if (retVal.Count == 0)
			{
				return null;
			}

			return retVal;
		}
	}
}
