using System;
using System.Windows.Forms;

namespace MyCustomActionProject
{
    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private readonly IntPtr _hwnd;
    }
}
