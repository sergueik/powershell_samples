using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace UIThread35Desktop
{
    static class ControlExtensions
    {
        /// <summary>
        /// Runs code in UI thread synchronously with BeginInvoke when required.
        /// </summary>
        /// <param name="code">the code, like "delegate { this.Text = "new text"; }"
        /// </param>
        static public void UIThread(this Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(code);
                return;
            }
            code.Invoke();
        }

        /// <summary>
        /// Runs code in UI thread synchronously with Invoke when required.
        /// </summary>
        /// <param name="code">the code, like "delegate { this.Text = "new text"; }"
        /// </param>
        static public void UIThreadInvoke(this Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
                return;
            }
            code.Invoke();
        }
    }
}
