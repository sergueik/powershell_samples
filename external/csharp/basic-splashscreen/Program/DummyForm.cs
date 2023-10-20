using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Example {
	public class DummyForm : Form {
		
        SplashScreenForm splashScreenForm = new SplashScreenForm();
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(465, 415);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += new System.EventHandler(this.MainForm_Load);
            ResumeLayout(false);

        }

        public DummyForm() {
            Hide();
            var splashthread = new Thread(new ThreadStart(SplashScreen.ShowSplashScreen));
            splashthread.IsBackground = true;
            splashthread.Start();
            InitializeComponent();
            Show();
            Activate();

        }

        private void MainForm_Load(object sender, EventArgs e) {
            SplashScreen.UdpateStatusText("Loading Items!!!");
            Thread.Sleep(1000);
            SplashScreen.UdpateStatusTextWithStatus("Success Message", TypeOfMessage.Success);
            Thread.Sleep(1000);
            SplashScreen.UdpateStatusTextWithStatus("Warning Message", TypeOfMessage.Warning);

            Thread.Sleep(1000);
            SplashScreen.UdpateStatusTextWithStatus("Error Message", TypeOfMessage.Error);
            Thread.Sleep(1000);
            SplashScreen.UdpateStatusText("Testing Default Message Color");
            Thread.Sleep(1000);
            SplashScreen.UdpateStatusText("Items Loaded..");
            Thread.Sleep(500);


            SplashScreen.CloseSplashScreen();
        }
    }
}
