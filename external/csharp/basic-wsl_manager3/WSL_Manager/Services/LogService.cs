using System;
using System.Drawing;
using System.Windows.Forms;

namespace WslManagerFramework.Services
{
    public class LogService
    {
        private RichTextBox _logBox;

        public LogService(RichTextBox logBox)
        {
            _logBox = logBox;
        }

        public void AddLog(string message, Color? color = null)
        {
            if (_logBox.InvokeRequired)
            {
                _logBox.Invoke(new Action(() => AddLog(message, color)));
                return;
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logMessage = String.Format("[{0}] {1}\n",timestamp,message);
            
            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.SelectionLength = 0;
            _logBox.SelectionColor = color ?? Color.White;
            _logBox.AppendText(logMessage);
            _logBox.ScrollToCaret();
        }

        public void SafeAddLog(string message, Color? color = null)
        {
            if (_logBox.InvokeRequired)
            {
                try
                {
                    _logBox.Invoke(new Action(() => SafeAddLog(message, color)));
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                return;
            }

            try
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var logMessage = String.Format("[{0}] {1}\n",timestamp,message);
                
                _logBox.SelectionStart = _logBox.TextLength;
                _logBox.SelectionLength = 0;
                _logBox.SelectionColor = color ?? Color.White;
                _logBox.AppendText(logMessage);
                _logBox.ScrollToCaret();
            }
            catch (ObjectDisposedException)
            {
                // コントロールが破棄されている場合は無視
            }
        }
    }
}