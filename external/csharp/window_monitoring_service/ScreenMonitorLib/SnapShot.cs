/*
 * Please leave this Copyright notice in your code if you use it
 * Written by Decebal Mihailescu [http://www.codeproject.com/script/articles/list_articles.asp?userid=634640]
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace ScreenMonitorLib
{
    public class SnapShot
    {
        public static bool SaveSnapShot()
        {
            try
            {
            Bitmap bitmap = MakeSnapshot(Win32API.GetDesktopWindow(), false, Win32API.WindowShowStyle.Restore);
            if (bitmap == null)
            {
                return false;
            }
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }
         static System.Drawing.Bitmap MakeSnapshot(IntPtr hWnd, bool isClient, Win32API.WindowShowStyle nCmdShow)
        {
            if (hWnd == IntPtr.Zero || !Win32API.IsWindow(hWnd) || !Win32API.IsWindowVisible(hWnd))
                return null;
            if (Win32API.IsIconic(hWnd))
                Win32API.ShowWindow(hWnd, nCmdShow);//show it

            System.Drawing.Bitmap image = null;
            RECT appRect = new RECT(); ;
            Win32API.GetWindowRect(hWnd, out appRect);

            image = new System.Drawing.Bitmap(appRect.Width, appRect.Height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image);

            //paint control onto graphics using provided options  
            IntPtr hDC = graphics.GetHdc();
            IntPtr hdcTo = IntPtr.Zero;
            IntPtr hBitmap = IntPtr.Zero;
            try
            {

                Win32API.PrintWindow(hWnd, hDC, 0);//Win32API.PW_CLIENTONLY);
                if (!isClient)
                    return image;
                RECT clientRect;
                bool res = Win32API.GetClientRect(hWnd, out clientRect);
                Point lt = new Point(clientRect.Left, clientRect.Top);
                Win32API.ClientToScreen(hWnd, ref lt);

                hdcTo = Win32API.CreateCompatibleDC(hDC);
                hBitmap = Win32API.CreateCompatibleBitmap(hDC, clientRect.Width, clientRect.Height);

                //  validate...
                if (hBitmap != IntPtr.Zero)
                {
                    // copy...
                    int x = lt.X - appRect.Left;
                    int y = lt.Y - appRect.Top;
                    IntPtr hLocalBitmap = Win32API.SelectObject(hdcTo, hBitmap);

                    Win32API.BitBlt(hdcTo, 0, 0, clientRect.Width, clientRect.Height, hDC, x, y, Win32API.SRCCOPY);
                    //  create bitmap for window image...
                    image.Dispose();
                    image = System.Drawing.Image.FromHbitmap(hBitmap);
                }

            }

            finally
            {
                if (hBitmap != IntPtr.Zero)
                    Win32API.DeleteObject(hBitmap);
                if (hdcTo != IntPtr.Zero)
                    Win32API.DeleteDC(hdcTo);
                graphics.ReleaseHdc(hDC);

            }

            return image;

        }
    }
}
