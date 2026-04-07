using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Program {

	public partial class Form1 : Form {

		public Form1() {
			InitializeComponent();
		}


		[STAThread]
		static void Main() {
			Application.Run(new Form1());
		}

		void Label1Click(object sender, EventArgs e) {
	
		}

	}
}
