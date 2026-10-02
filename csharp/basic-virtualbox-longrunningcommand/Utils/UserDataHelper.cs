using System;
using System.Runtime.InteropServices;

using System.IO;
using System.Text;
using System.Threading;

namespace Utils {
	// https://www.pinvoke.net/default.aspx/user32/setwindowlongptr.html
	// https://www.pinvoke.net/default.aspx/user32/getwindowlongptr.html

	// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongptra
	// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlongptra
	public static class UserDataHelper {
		// see also:
		// https://devblogs.microsoft.com/oldnewthing/20260629-00/?p=112484
		// https://stackoverflow.com/questions/866692/if-you-add-extra-data-space-to-a-dialogbox-class-be-accessed-by-getwindowlongptr
	    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
	    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);
	    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
	    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

	    // Dialog specific offsets for Get/SetWindowLongPtr
	   //  https://www.pinvoke.net/default.aspx/Constants/GWL%20-%20GetWindowLong.html
	    public static class DialogWindowIndices {
	        public static readonly int DWLP_MSGRESULT = 0;
	        public static readonly int DWLP_DLGPROC = 4;
	        public static readonly int DWLP_USER = 8; // Often offset 8 or negative depending on usage
	    }

	    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
	    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

	    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
	    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

	    // Safe wrapper method for 32-bit and 64-bit compatibility
	    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex) {
	        if (IntPtr.Size == 4) {
	            return new IntPtr(GetWindowLong32(hWnd, nIndex));
	        } else {
	            return GetWindowLongPtr64(hWnd, nIndex);
	        }
	    }

	    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
	        if (IntPtr.Size == 8) {
	            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
	        } else {
	            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
	        }
	    }
	}
}

/*

### Usage During Dialog Creation
IntPtr dialogHandle = ...; // your dialog HWND
IntPtr userData = Utils.GetWindowLongPtr(dialogHandle, NativeMethods.DialogWindowIndices.DWLP_USER);

When working with dialog boxes (such as during `WM_INITDIALOG`), you typically call this function to store class pointers or set user data like `DWL_USER` (or `DWLP_USER`):

```csharp
// Example constant for user dialog extra memory
const int DWLP_USER = moglieIndex ?? -21; // or -8 depending on standard definitions

// Inside your dialog message handler / hook
IntPtr myDataPtr = GCHandle.ToIntPtr(gch);
Utils.SetWindowLongPtr(hwndDlg, DWLP_USER, myDataPtr);
*/


