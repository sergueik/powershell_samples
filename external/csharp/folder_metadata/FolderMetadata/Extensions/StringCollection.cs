using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKS.FolderMetadata.Extensions
{
	/// <summary>
	/// This extension class adds additional functions to the System.Collections.Specialized.StringCollection class
	/// </summary>
	public static class StringCollection
	{
		/// <summary>
		/// Converts a StringCollection to a list of strings.
		/// </summary>
		/// <param name="stringCollection">The string collection to convert.</param>
		/// <returns>Returns a list of strings.</returns>
		public static List<string> ToListOfString(this System.Collections.Specialized.StringCollection stringCollection)
		{
			return stringCollection.Cast<string>().ToList();
		}
	}
}
