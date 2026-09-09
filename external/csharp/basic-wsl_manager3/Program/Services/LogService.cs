using System;
using System.Drawing;
using System.Windows.Forms;

namespace WslManagerFramework.Services {
	public class LogService {
		private RichTextBox richTextBox;

		public LogService(RichTextBox richTextBox) {
			this.richTextBox = richTextBox;
		}

		public void AddLog(string message, Color? color = null) {
			if (richTextBox.InvokeRequired) {
				richTextBox.Invoke(new Action(() => AddLog(message, color)));
				return;
			}

			var timestamp = DateTime.Now.ToString("HH:mm:ss");
			var logMessage = String.Format("[{0}] {1}\n", timestamp, message);
            
			richTextBox.SelectionStart = richTextBox.TextLength;
			richTextBox.SelectionLength = 0;
			richTextBox.SelectionColor = color ?? Color.White;
			richTextBox.AppendText(logMessage);
			richTextBox.ScrollToCaret();
		}

		public void SafeAddLog(string message, Color? color = null) {
			if (richTextBox.InvokeRequired) {
				try {
					richTextBox.Invoke(new Action(() => SafeAddLog(message, color)));
				} catch (ObjectDisposedException) {
					return;
				} catch (InvalidOperationException) {
					return;
				}
				return;
			}

			try {
				var timestamp = DateTime.Now.ToString("HH:mm:ss");
				var logMessage = String.Format("[{0}] {1}\n", timestamp, message);
                
				richTextBox.SelectionStart = richTextBox.TextLength;
				richTextBox.SelectionLength = 0;
				richTextBox.SelectionColor = color ?? Color.White;
				richTextBox.AppendText(logMessage);
				richTextBox.ScrollToCaret();
			} catch (ObjectDisposedException) {
				// Ignore
			}
		}
	}
}
