using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EnumListCtrls
{
	public static class EnumGlobal
	{
		/// <summary>
		/// Finds the parent.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="child">The child.</param>
		/// <returns></returns>
		public static T FindParent<T>(DependencyObject child) where T : DependencyObject
		{
			//get parent item
			DependencyObject parentObject = VisualTreeHelper.GetParent(child);

			//we've reached the end of the tree
			if (parentObject == null) return null;

			//check if the parent matches the type we're looking for
			T parent = parentObject as T;

			// return or continue search
			return (parent != null) ? parent : FindParent<T>(parentObject);
		}
	}
}
