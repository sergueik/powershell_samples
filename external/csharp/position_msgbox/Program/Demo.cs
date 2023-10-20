using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace MessageBoxExDemo {
	public class MainClass {
		// http://www.java2s.com/Code/CSharp/GUI-Windows-Form/YesNoCancel.htm
		public static void Main() {
			String text = "Do you want to create a new file?";
			String caption = "Caption";
			FindAndMoveMsgBox(100, 100, true, caption);
			switch (MessageBox.Show(text, caption,
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation)) {

				case DialogResult.OK:
					break;
				default:
					break;
			}
		}
		[DllImport("user32.dll")]
		static extern IntPtr FindWindow(IntPtr classname, string title);

		[DllImport("user32.dll")]
		static extern void MoveWindow(IntPtr hwnd, int X, int Y, 
			int nWidth, int nHeight, bool rePaint);

		[DllImport("user32.dll")]
		static extern bool GetWindowRect(IntPtr hwnd, out Rectangle rect);

              
 		
		static void FindAndMoveMsgBox(int x, int y, bool repaint, string title)
		{
			Thread thr = new Thread(() => { // create a new thread
				IntPtr msgBox = IntPtr.Zero;
				// while there's no MessageBox, FindWindow returns IntPtr.Zero
				while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero)
					;
				// after the while loop, msgBox is the handle of your MessageBox
				Rectangle r = new Rectangle();
				GetWindowRect(msgBox, out r); // Gets the rectangle of the message box
				MoveWindow(msgBox /* handle of the message box */, x, y, 
					r.Width - r.X /* width of originally message box */, 
					r.Height - r.Y /* height of originally message box */, 
					repaint /* if true, the message box repaints */);
			});
			thr.Start(); // starts the thread
		}
		
	}

}
