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
    public partial class Form1 : Form//,IDisposable
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32", EntryPoint = "VkKeyScan")]
        public static extern short VkKeyScan(byte cChar_Renamed);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(int hWnd);
        [DllImport("user32.dll")]
        private static extern int SetFocus(int hWnd);

        public const int WM_PASTE = 0x0302;

        public const byte VK_CONTROL = 0x11;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        private static int LoWord(int number)
        {
            return number & 0xffff;
        }                
        ArrayList list;
        public string forward_to_clipboard="";
        
        private int remove_item_index;

        public Form1()
        {
            InitializeComponent();
        }
        public void populate(ref ArrayList entries)
        {
            list = entries;
            for (int i = entries.Count-1; i>=0; i--) // put the recent items on top
            {
                clip_entries.Items.Add(entries[i]);
            }
        }
        private void clip_entries_MouseClick(object sender, MouseEventArgs e)
        {
            int index;
            index = clip_entries.IndexFromPoint(e.X,e.Y);
            if(index>=0 && index<clip_entries.Items.Count)
            {                
                forward_to_clipboard = (string)clip_entries.Items[index];
                this.Close();
            }
        }
        private void usunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(remove_item_index>=0 && remove_item_index <clip_entries.Items.Count)
                clip_entries.Items.RemoveAt(remove_item_index);
        }       
        private void clip_entries_MouseHover(object sender, EventArgs e)
        {
            string text = "";
            Point p = clip_entries.PointToClient(Form1.MousePosition);
            int index = clip_entries.IndexFromPoint(p);
            if (index >= 0 && index < clip_entries.Items.Count)
                text = clip_entries.Items[index].ToString();
            else
                text = "Clipboard Little Helper";
            toolTip1.SetToolTip(clip_entries, text);
        }
        private void clip_entries_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenu1.Show();
                Point p = clip_entries.PointToClient(Control.MousePosition);
                remove_item_index = clip_entries.IndexFromPoint(p);
            }
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1.0f;
            forward_to_clipboard = (string)"";
        }
        private void Form1_Deactivate(object sender, EventArgs e)
        {
            //this.Opacity = 0.4f;            
            this.Close();
        }
        public void ForwardDataToClipboard()
        {            
            int outsideApphwnd;
            GetOneDownWindow wnd = new GetOneDownWindow();

            outsideApphwnd = (int)wnd.GetThatWindow(); //wnd.wndArray[1];

            SetForegroundWindow(outsideApphwnd);
            SetFocus(outsideApphwnd);
            keybd_event(VK_CONTROL, 0, 0, 0);
            keybd_event((byte)VkKeyScan((byte)'v'), 0, 0, 0);
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
            keybd_event((byte)VkKeyScan((byte)'v'), 0, KEYEVENTF_KEYUP, 0);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            list.Clear();
            for (int i = clip_entries.Items.Count-1; i >= 0; i--) // put back begining with the oldest
            {
                list.Add(clip_entries.Items[i]);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void clip_entries_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}