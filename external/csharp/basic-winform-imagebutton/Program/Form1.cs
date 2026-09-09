using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace Program {
	public partial class Form1 : Form {
		private bool hideAlerts = false;
		public Form1() {
			InitializeComponent();
		}

		private void imageButton13_Click(object sender, EventArgs e) {
			hideAlerts = !hideAlerts;
			if (hideAlerts) {
				imageButton13.NormalImage = Utils.Properties.Resources.CCheckedNormal;
				imageButton13.HoverImage = Utils.Properties.Resources.CCheckedHover;
				imageButton13.DownImage = Utils.Properties.Resources.CCheckedDown;
			} else {
				imageButton13.NormalImage = Utils.Properties.Resources.CUncheckedNormal;
				imageButton13.HoverImage = Utils.Properties.Resources.CUncheckedHover;
				imageButton13.DownImage = Utils.Properties.Resources.CUncheckedDown;
			}
		}


		private void imageButton_Click(object sender, EventArgs e) {
			Controls.Add(toolbarPanel);
			var button = (ImageButton) sender;
			MessageBox.Show(button.Tag.ToString());
		}
		
	}
}
