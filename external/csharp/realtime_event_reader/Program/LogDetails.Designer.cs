using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace RealTimeEventLogReader {
	partial class LogDetails {
		private IContainer components = null;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			rtbDetailXML = new RichTextBox();
			SuspendLayout();
			// 
			// rtbDetailXML
			// 
			rtbDetailXML.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
			rtbDetailXML.Location = new Point(13, 13);
			rtbDetailXML.Name = "rtbDetailXML";
			rtbDetailXML.ReadOnly = true;
			rtbDetailXML.Size = new Size(662, 436);
			rtbDetailXML.TabIndex = 0;
			rtbDetailXML.Text = "";
			// 
			// LogDetails
			// 
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(687, 461);
			Controls.Add(rtbDetailXML);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Name = "LogDetails";
			StartPosition = FormStartPosition.CenterParent;
			Text = "LogDetails";
			ResumeLayout(false);

		}

		private RichTextBox rtbDetailXML;
	}
}
