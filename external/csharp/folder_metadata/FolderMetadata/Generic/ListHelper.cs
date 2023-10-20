using System.Collections;
using System.Collections.Generic;

namespace HKS.FolderMetadata.Generic
{
	/// <summary>
	/// The ArrayHelper class contains some helpful methods for lists.
	/// </summary>
	public class ListHelper
	{
		/// <summary>
		/// Returns if a list is null or empty.
		/// </summary>
		/// <param name="array">The list to check.</param>
		/// <returns>Returns true if the list is null or has no elements, otherwise false.</returns>
		public static bool IsNullOrEmpty(IList list)
		{
			if (list == null)
			{
				return true;
			}

			return (list.Count == 0);
		}

		/// <summary>
		/// Joins a list to a semicolon+blank separated string.
		/// </summary>
		/// <param name="list">The list to convert.</param>
		/// <returns>Returns a string or null.</returns>
		public static string ToString(List<string> list)
		{
			if (IsNullOrEmpty(list))
			{
				return null;
			}

			return string.Join("; ", list.ToArray());
		}
	}
}
