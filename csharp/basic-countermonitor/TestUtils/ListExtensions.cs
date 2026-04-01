using System;
using System.Collections.Generic;


namespace TestUtils {
	public static class ListExtensions {
		public static void Shuffle<T>(this IList<T> list) {
			for (int i = list.Count - 1; i > 0; i--) {
				int j = new Random().Next(i + 1);

				T temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}
		}

	}
}

