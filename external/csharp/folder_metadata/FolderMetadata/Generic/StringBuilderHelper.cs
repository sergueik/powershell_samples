using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKS.FolderMetadata.Generic
{
	/// <summary>
	/// The StringBuilderHelper class contains some helpful methods for StringBuilders.
	/// </summary>
	public static class StringBuilderHelper
	{
		/// <summary>
		/// Returns if an StringBuilder is null or empty.
		/// </summary>
		/// <param name="array">The StringBuilder to check.</param>
		/// <returns>Returns true if the StringBuilder is null or has no content, otherwise false.</returns>
		public static bool IsNullOrEmpty(StringBuilder sb)
		{
			return (sb == null || sb.Length == 0);
		}
	}
}
