using System;
using System.Collections.Generic;


namespace TestUtils {
	public static class ListExtensions {
		public static void Shuffle<T>(this IList<T> values) {
			for (int count = values.Count - 1; count > 0; count--) {
				int randomIndex = new Random().Next(count + 1);

				T value = values[count];
				values[count] = values[randomIndex];
				values[randomIndex] = value;
			}
		}

	}
}

