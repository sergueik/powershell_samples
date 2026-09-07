using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Program {
	public partial class Form1 : Form {
		private bool hideAlerts = false;
		public Form1() {
			InitializeComponent();
		}

		private void imageButton13_Click(object sender, EventArgs e) {
			hideAlerts = !hideAlerts;
			if (hideAlerts) {
				imageButton13.NormalImage = Properties.Resources.CCheckedNormal;
				imageButton13.HoverImage = Properties.Resources.CCheckedHover;
				imageButton13.DownImage = Properties.Resources.CCheckedDown;
			} else {
				imageButton13.NormalImage = Properties.Resources.CUncheckedNormal;
				imageButton13.HoverImage = Properties.Resources.CUncheckedHover;
				imageButton13.DownImage = Properties.Resources.CUncheckedDown;
			}
		}

		private void imageButton1_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("New");
		}

		private void imageButton2_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Reload");
		}
		
		private void imageButton4_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Start");
		}

		private void imageButton5_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Stop");
		}

		private void imageButton6_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Shell");
		}

		private void imageButton7_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Configure");
		}
		private void imageButton8_Click(object sender, EventArgs e) {
			if (!hideAlerts)
				MessageBox.Show("Recycle");
		}
	}
}
