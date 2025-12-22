using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace clipboard_helper
{
    public class GetOneDownWindow
    {
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern int GetWindowModuleFileName(int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(int hWnd);

        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        public ArrayList wndArray = new ArrayList(); //array of windows

        public IntPtr GetThatWindow()
        {
            EnumWindowsProc ewp = new EnumWindowsProc(EnumWindow);
            EnumWindows(ewp, 0);
            return ((IntPtr)wndArray[1]);
        }

        private bool EnumWindow(int hWnd, int lParam)
        {
            if (!IsWindowVisible(hWnd))
                return (true);

            StringBuilder module = new StringBuilder(256);

            GetWindowModuleFileName(hWnd, module, 256);
            string test = module.ToString();

            wndArray.Add((IntPtr)hWnd);

            return (true);
        }
    }
}
