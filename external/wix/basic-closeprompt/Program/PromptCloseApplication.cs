using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Program {
    public class PromptCloseApplication : IDisposable {
        private readonly string productName;
        private readonly string processName;
        private readonly string  displayName;
        private System.Threading.Timer timer;
        private Form form;
        private IntPtr mainWindowHanle;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public PromptCloseApplication(string productName, string processName, string displayName) {
           this.productName = productName;
            this.processName = processName;
            this.displayName = displayName;
        }

        public bool Prompt() {
            if (IsRunning(processName)) {
                form = new ClosePromptForm(String.Format(@"Please close running instances of ""{0}"" before running ""{1}"" setup.",  displayName,  productName));
                mainWindowHanle = FindWindow(null,  productName + " Setup");
                if (mainWindowHanle == IntPtr.Zero)
                    mainWindowHanle = FindWindow("#32770",  productName);

                timer = new System.Threading.Timer(TimerElapsed, form, 200, 200);

                return ShowDialog();
            }
            return true;
        }

        bool ShowDialog() {
            if (form.ShowDialog(new WindowWrapper(mainWindowHanle)) == DialogResult.OK)
                return !IsRunning(processName) || ShowDialog();
            return false;
        }

        private void TimerElapsed(object sender) {
            if (form == null || IsRunning(processName) || !form.Visible)
                return;
            form.DialogResult = DialogResult.OK;
            form.Close();
        }

        static bool IsRunning(string processName) {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        public void Dispose() {
            if (timer != null)
                timer.Dispose();
            if (form != null && form.Visible)
                form.Close();
        }
    }
}
