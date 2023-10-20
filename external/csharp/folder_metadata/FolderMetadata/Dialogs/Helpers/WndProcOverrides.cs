using System;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs.Helpers
{
	/// <summary>
	/// The WndProcOverrides class contains methods that are used when overriding WndProc in a Windows form.
	/// </summary>
	public static class WndProcOverrides
	{
		private const int WM_UPDATEUISTATE = 0x0128;
		private const int UISF_HIDEACCEL = 0x2;
		private const int UIS_CLEAR = 0x2;

		/// <summary>
		/// Force display of mnemonics (underscored letters or numbers in buttons or labels).
		/// </summary>
		/// <param name="m"></param>
		public static void ShowMnemonics(ref Message m)
		{
			if (m.Msg == WM_UPDATEUISTATE)
			{
				m.WParam = (IntPtr)((UISF_HIDEACCEL & 0x0000FFFF) | (UIS_CLEAR << 16));
			}
		}
	}
}
