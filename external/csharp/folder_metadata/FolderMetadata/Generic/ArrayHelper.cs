using System;

namespace HKS.FolderMetadata.Generic
{
	/// <summary>
	/// The ArrayHelper class contains some helpful methods for arrays.
	/// </summary>
	public static class ArrayHelper
	{
		/// <summary>
		/// Returns if an array is null or empty.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <returns>Returns true if the array is null or has no elements, otherwise false.</returns>
		public static bool IsNullOrEmpty(Array array)
		{
			return (array == null || array.Length == 0);
		}
	}
}
