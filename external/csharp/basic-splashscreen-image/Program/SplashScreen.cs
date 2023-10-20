using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;

namespace SplashScreenControl {
	public sealed class SplashScreen : Control {
		private static SplashScreen instance = null;
		private static object lockObject = new object();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static public extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public extern static IntPtr SetParent(IntPtr hChild, IntPtr hParent);
		[DllImport("user32.dll", EntryPoint = "SetWindowPos", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		// https://learn.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles		
		private const int WS_EX_TOPMOST = unchecked((int)0x00000008L);
		private const int WS_EX_TOOLWINDOW = unchecked((int)0x00000080);
		// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
		private const uint SWP_NOSIZE = 0x0001;
		private const uint SWP_NOMOVE = 0x0002;
		private const int HWND_TOPMOST = -1;

		private string titleString;
		private string commentaryString;
		private StringFormat stringFormat;
		private int m_nTextOffsetY;
		private int m_nTextOffsetX;
		private int m_nLeading;

		private SplashScreen() {
			TitleString = string.Empty;
			CommentaryString = string.Empty;

			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
			stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

			m_nTextOffsetY = 0;
			m_nTextOffsetX = 45;
			m_nLeading = 6;

			Visible = false;
			Width = 420;
			Height = 320;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}

		protected override void OnHandleCreated(EventArgs e) {
			if (Handle != IntPtr.Zero) {
				IntPtr hWndDeskTop = GetDesktopWindow();
				SetParent(Handle, hWndDeskTop);
			}

			base.OnHandleCreated(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			if (Bounds.Width > 0 && Bounds.Height > 0 && Visible) {
				try {
					var rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
					Graphics g = e.Graphics;
					g.SetClip(e.ClipRectangle);
					if (BackgroundImage == null) {
						var solidBrush = new SolidBrush(BackColor);
						g.FillRectangle(solidBrush, rect);
						solidBrush.Dispose();
					} else {
						g.DrawImage(BackgroundImage, rect, 0, 0, BackgroundImage.Width, BackgroundImage.Height, GraphicsUnit.Pixel);
					}
				} catch (Exception exception) {
					var stackFrame = new System.Diagnostics.StackFrame(true);
					Console.Error.WriteLine("\nException: {0}, \n\t{1}, \n\t{2}, \n\t{3}\n", this.GetType().ToString(), stackFrame.GetMethod().ToString(), stackFrame.GetFileLineNumber(), exception.Message);
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e) {
			if (Bounds.Width > 0 && Bounds.Height > 0 && Visible) {
				try {
					Graphics g = e.Graphics;
					g.SetClip(e.ClipRectangle);
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

					float nClientHeight = ClientRectangle.Height;
					//start the text two thirds down:
					m_nTextOffsetY = Convert.ToInt32(Math.Ceiling(((nClientHeight / 3) * 2))) + m_nLeading;

					if (TitleString != string.Empty) {
						var fontTitle = new Font(Font, FontStyle.Bold);
						SizeF sizeF = g.MeasureString(TitleString, fontTitle, ClientRectangle.Width, stringFormat);
						m_nTextOffsetY += Convert.ToInt32(Math.Ceiling(sizeF.Height));
						var rectangleF = new RectangleF(ClientRectangle.Left + m_nTextOffsetX, ClientRectangle.Top + m_nTextOffsetY, sizeF.Width, sizeF.Height);
						var brushFont = new SolidBrush(ForeColor);
						g.DrawString(TitleString, fontTitle, brushFont, rectangleF, stringFormat);
						brushFont.Dispose();
						fontTitle.Dispose();

						m_nTextOffsetY += m_nLeading;
					}

					if (CommentaryString != string.Empty) {
						SizeF sizeF = g.MeasureString(CommentaryString, Font, ClientRectangle.Width, stringFormat);
						m_nTextOffsetY += Convert.ToInt32(Math.Ceiling(sizeF.Height));
						var rectangleF = new RectangleF(ClientRectangle.Left + m_nTextOffsetX, ClientRectangle.Top + m_nTextOffsetY, sizeF.Width, sizeF.Height);
						var brushFont = new SolidBrush(ForeColor);
						g.DrawString(CommentaryString, Font, brushFont, rectangleF, stringFormat);
						brushFont.Dispose();
					}
				} catch (Exception exception) {
					var stackFrame = new System.Diagnostics.StackFrame(true);
					Console.Error.WriteLine("\nException: {0}, \n\t{1}, \n\t{2}, \n\t{3}\n", this.GetType().ToString(), stackFrame.GetMethod().ToString(), stackFrame.GetFileLineNumber(), exception.Message);
				}
			}
		}

		protected override void OnBackgroundImageChanged(EventArgs e) {
			base.OnBackgroundImageChanged(e);

			if (BackgroundImage != null) {
				Width = BackgroundImage.Width;
				Height = BackgroundImage.Height;
				RePosition();
			}

			Refresh();
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
				cp.Parent = IntPtr.Zero;
				return cp;
			}
		}

		private void RePosition() {
			Rectangle screenArea = SystemInformation.WorkingArea;
			int nX = (screenArea.Width - Width) / 2;
			int nY = (screenArea.Height - Height) / 2;
			Location = new Point(nX, nY);
		}

		public static void BeginDisplay() {
			Instance.RePosition();
			Instance.Visible = true;
			if (!Instance.Created) {
				Instance.CreateControl();
			}
			SetWindowPos(Instance.Handle, (System.IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
			Instance.Refresh();
		}

        public static void EndDisplay() {
			Instance.Visible = false;
		}

		public static void SetTitleString(string title) {
			Instance.TitleString = title;
		}

		public static void SetCommentaryString(string commentary) {
			Instance.CommentaryString = commentary;
		}

		public static void SetBackgroundImage(Image image) {
			Instance.BackgroundImage = image;
		}

		public static SplashScreen Instance {
			get {
				lock (lockObject) {
					if (instance == null || instance.IsDisposed) {
						instance = new SplashScreen();
					}
					return instance;
				}
			}
		}

		private string CommentaryString {
			get {
				return commentaryString;
			}
			set {
				commentaryString = value;
				Refresh();
			}
		}

		private string TitleString {
			get {
				return titleString;
			}
			set {
				titleString = value;
				Refresh();
			}
		}
	}
}
