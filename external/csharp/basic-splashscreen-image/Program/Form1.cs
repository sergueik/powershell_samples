using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SplashScreenControl;

namespace WindowsApplication1
{

	public partial class Form1 : Form
	{
		public Form1()
		{
			//n.b. SplashScreen.BeginDisplay was called in Program.cs Main
			//as various checkpoints are reached you can inform the user about progress
			SplashScreen.SetCommentaryString("..constructing the Form");
			InitializeComponent();
		}
        
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			//this is where to call SplashScreen.EndDisplay
			SplashScreen.SetTitleString("OnShown Override.");
			SplashScreen.SetCommentaryString("..time to end the the display.");
			System.Threading.Thread.Sleep(2000);
			SplashScreen.EndDisplay();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			System.Threading.Thread.Sleep(1000);

			SplashScreen.SetCommentaryString("..loading this.");
			System.Threading.Thread.Sleep(1000);

			SplashScreen.SetCommentaryString("..loading that.");
			System.Threading.Thread.Sleep(2000);

			SplashScreen.SetCommentaryString("..loading the other.");
			System.Threading.Thread.Sleep(1000);
		}

		// display exit banner
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (splash2Checked) {
				//to display an exit banner call BeginDisplay in the OnFormClosing
				//and EndDisplay in OnFormClosed
				SplashScreen.SetTitleString("Application Close.");
				SplashScreen.SetCommentaryString("..shutting down");
				SplashScreen.BeginDisplay();
			}
			base.OnFormClosing(e);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
            			if (splash2Checked) {
			System.Threading.Thread.Sleep(2000);
			SplashScreen.EndDisplay();
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SplashScreen.SetTitleString("Displaying The SplashScreen Control");
			SplashScreen.SetCommentaryString("..timed countdown to EndDisplay.");
			SplashScreen.BeginDisplay();
			System.Threading.Thread.Sleep(1000);

			for (int n = 5; n >= 0; n--) {
				System.Threading.Thread.Sleep(1000);
				SplashScreen.SetCommentaryString(n.ToString() + " seconds.");
			}

			SplashScreen.EndDisplay();
		}
	}
}