using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace clipboard_helper
{
    class ClipboardListener
    {
        public string prevclipboardcontents;
        private static DelegateCheck m_DelegateCheck;
        private delegate void DelegateCheck();
        private System.Threading.Timer t = null;
        
        //clipboard
        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();

        // memory
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, int size);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalUnlock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalFree(IntPtr hMem);
        [DllImport("kernel32.dll")]
        public static extern UIntPtr GlobalSize(IntPtr hMem);
        public const uint GMEM_DDESHARE = 0x2000;
        public const uint GMEM_MOVEABLE = 0x2;

        public ArrayList list;

        public ClipboardListener()
        {
            prevclipboardcontents = "";
            m_DelegateCheck = new DelegateCheck(check_clipboard);

            System.Threading.TimerCallback timerDelegate_clipboard = new System.Threading.TimerCallback(this.callback_check);
            t = new System.Threading.Timer(timerDelegate_clipboard, null, 0, 1000);
            list = new ArrayList();
        }

        private void callback_check(object temp)
        {
            try {
                m_DelegateCheck();}
            catch{ }
        }

        public static string GetClipboardText()
        {
            IntPtr hInstance = IntPtr.Zero;            
            OpenClipboard(hInstance);
            IntPtr buffer = GetClipboardData(13);
            string text = System.Runtime.InteropServices.Marshal.PtrToStringUni(buffer);
            CloseClipboard();
            return text;
        }

        public static bool SetClipboardText(string text)
        {
            if (!OpenClipboard(IntPtr.Zero))
                return false;
            EmptyClipboard();                                   
            IntPtr alloc = GlobalAlloc(GMEM_MOVEABLE | GMEM_DDESHARE, (UIntPtr)(sizeof(char)*(text.Length+1)));
            IntPtr gLock = GlobalLock(alloc);

            if ((int)text.Length > 0)            
                Marshal.Copy(text.ToCharArray(), 0, gLock, text.Length);
            GlobalUnlock(alloc);
            SetClipboardData(13, alloc);
            
            CloseClipboard();
            return true;
        }
        private void check_clipboard()
        {
            string text = GetClipboardText();        
            
            if (text!=null && text.Length>0)
            {
                string currentclipboardcontents = "";
                try
                {
                    currentclipboardcontents = text.ToString();
                }
                catch { }
                if (prevclipboardcontents != currentclipboardcontents)
                {
                    prevclipboardcontents = currentclipboardcontents;
                    char[] b = prevclipboardcontents.ToCharArray();
                    list.Add(currentclipboardcontents);                    
                }
            }
        }
    }
}
