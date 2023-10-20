using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HKS.FolderMetadata.Dialogs.Helpers
{
	/// <summary>
	/// Retrieves Icons from a resource file (e.g. EXE or DLL).
	/// </summary>
	public class ResGetter
	{
		public enum IconSize : uint
		{
			Large,
			Small
		};

		[DllImport("Shell32", CharSet = CharSet.Auto)]
		private static extern int ExtractIconEx( string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);

		[DllImport("User32.dll")]
		public static extern int DestroyIcon(IntPtr hIcon);

		private readonly string FULL_PATH;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fullPath">The full path to the resource file.</param>
		public ResGetter(string fullPath)
		{
			this.FULL_PATH = fullPath;
		}

		public string FullPath
		{ get { return FULL_PATH; } }

		/// <summary>
		/// Returns an icon from the resource file.
		/// </summary>
		/// <param name="index">The icon index.</param>
		/// <param name="size">The desired icon size.</param>
		/// <returns></returns>
		public Icon GetIcon(int index, IconSize size = IconSize.Small)
		{
			Icon retVal = null;
			int readIconCount = 0;
			IntPtr[] phiconLarge = new IntPtr[] { IntPtr.Zero };
			IntPtr[] phiconSmall = new IntPtr[] { IntPtr.Zero };

			try
			{
				readIconCount = ExtractIconEx(FULL_PATH, index, phiconLarge, phiconSmall, 1);
				if (readIconCount > 0)
				{
					switch (size)
					{
						case IconSize.Large:
							retVal = (Icon)Icon.FromHandle(phiconLarge[0]).Clone();
							break;
						case IconSize.Small:
							retVal = (Icon)Icon.FromHandle(phiconSmall[0]).Clone();
							break;
						default:
							break;
					}
				}
			}
			catch
			{ }
			finally
			{
				foreach (IntPtr ptr in phiconLarge)
				{
					if (ptr != IntPtr.Zero)
					{
						DestroyIcon(ptr);
					}
				}

				foreach (IntPtr ptr in phiconSmall)
				{
					if (ptr != IntPtr.Zero)
					{
						DestroyIcon(ptr);
					}
				}
			}

			return retVal;
		}

		/// <summary>
		/// Sets the icon of a form to the icon retrieved from the resource file.
		/// </summary>
		/// <param name="form">The form to receive the icon.</param>
		/// <param name="index">The index of the icon.</param>
		/// <param name="iconSize">The desired icon size. Default is Large.</param>
		/// <returns></returns>
		public bool SetFormIcon(System.Windows.Forms.Form form, int index, IconSize iconSize = IconSize.Large)
		{
			try
			{
				Icon icon = GetIcon(index, iconSize);
				if (icon != null)
				{
					form.Icon = icon;
				}
			}
			catch
			{ }

			return false;
		}

		/// <summary>
		/// Returns a ResGetter instance for Windows shell32.dll resource file.
		/// </summary>
		/// <returns>Returns a ResGetter instance or null.</returns>
		public static ResGetter GetShell32Getter()
		{
			return GetSystem32ResGetter("shell32.dll");
		}

		/// <summary>
		/// Returns a ResGetter instance for the specified Windows resource file (in System32 folder).
		/// </summary>
		/// <param name="resourceFileName">The file name</param>
		/// <returns>Returns a ResGetter instance or null.</returns>
		public static ResGetter GetSystem32ResGetter(string resourceFileName)
		{
			string dllPath = System.Environment.GetFolderPath(Environment.SpecialFolder.System);
			if (string.IsNullOrEmpty(dllPath))
			{
				return null;
			}

			dllPath = System.IO.Path.Combine(dllPath, resourceFileName);
			if (!System.IO.File.Exists(dllPath))
			{
				return null;
			}

			return new ResGetter(dllPath);
		}
	}
}
