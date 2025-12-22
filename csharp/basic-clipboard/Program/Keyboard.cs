using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;

namespace clipboard_helper
{
    public class KeyboardHook
    {
        ClipboardListener cl = new ClipboardListener();
        public KeyboardHook()
        {            
            hook();
        }
        ~KeyboardHook()
        {
            unhook();
        }
        //public event 
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        IntPtr hhook = IntPtr.Zero;
        private delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
        private keyboardHookProc m_keyboardHookProc;

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public bool hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            m_keyboardHookProc = new keyboardHookProc(hookProc);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, m_keyboardHookProc, hInstance, 0);
            return (hhook.ToInt32() != 0);
        }
        private void unhook()
        {
            UnhookWindowsHookEx(hhook);
        }

        const Keys MY_LCTRL = Keys.LControlKey;
        const Keys MY_RCTRL = Keys.RControlKey;
        const Keys MY_LSHIFT = Keys.LShiftKey;
        const Keys MY_RSHIFT = Keys.RShiftKey;
        const Keys MY_V = Keys.V;

        bool ctrl_pressed = false;
        bool shft_pressed = false;
        bool v_pressed = false;
        bool sth_else = false;

        bool busy=false;

        public void ShowWindow()
        {
            Form1 frm = new Form1();
            frm.populate(ref cl.list);
            int x = System.Windows.Forms.Control.MousePosition.X;
            int y = System.Windows.Forms.Control.MousePosition.Y;

            if (x + frm.Width > Screen.PrimaryScreen.Bounds.Width)
                x = Screen.PrimaryScreen.Bounds.Width - frm.Width;
            if (y + frm.Height > Screen.PrimaryScreen.Bounds.Height)
                y = Screen.PrimaryScreen.Bounds.Height - frm.Height;

            frm.Location = new Point(x, y);
            frm.ShowDialog();
            string text = frm.forward_to_clipboard;
            if (text.Length != 0)
            {
                //put the chosen item as last added - so it would be on top of the list
                cl.list.Remove(text);
                cl.list.Add(text);
                cl.prevclipboardcontents = text;
                ClipboardListener.SetClipboardText(text);
                frm.ForwardDataToClipboard();
            }
        }
        private int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (busy)
                return 0;
            else
                busy = true;

            if (code >= 0)
                {                
                    Keys key = (Keys)lParam.vkCode;
                    if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                    {
                        switch (key)
                        {
                            case MY_LCTRL:
                            case MY_RCTRL:
                                ctrl_pressed = true;
                                break;
                            case MY_LSHIFT:
                            case MY_RSHIFT:
                                shft_pressed = true;
                                break;
                            case MY_V:
                                v_pressed = true;
                                break;                        
                            default:
                                sth_else = true;
                                break;
                        }                    
                    }
                    if (wParam == WM_KEYUP || wParam == WM_SYSKEYDOWN)
                    {
                        switch (key)
                        {
                            case MY_LCTRL:
                            case MY_RCTRL:
                                if(ctrl_pressed)
                                    ctrl_pressed = false;
                                break;
                            case MY_LSHIFT:
                            case MY_RSHIFT:
                                if(shft_pressed)
                                    shft_pressed = false;
                                break;
                            case MY_V:
                                if(v_pressed )
                                    v_pressed = false;
                                break;
                            default:
                                sth_else = true;
                                break;
                        }
                    }
                    if (sth_else)
                    {
                        ctrl_pressed = false;
                        v_pressed = false;
                        shft_pressed = false;
                        sth_else = false;
                    }

                    if (ctrl_pressed && v_pressed && shft_pressed)
                    {                        
                        ctrl_pressed = false;
                        v_pressed = false;
                        shft_pressed = false;

                        ShowWindow();
                        busy = false;
                        return 0;
                    }
                }
                busy = false;
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }
   }
}