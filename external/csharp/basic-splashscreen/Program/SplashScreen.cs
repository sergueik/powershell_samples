using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Example {

    public enum TypeOfMessage {
        Success,
        Warning,
        Error,
    }

    public static class SplashScreen {
		
        static SplashScreenForm splashScreenForm = null;
        public static void ShowSplashScreen() {
            if (splashScreenForm == null) {
                splashScreenForm = new SplashScreenForm();
                splashScreenForm.ShowSplashScreen();
            }
        }

        public static void CloseSplashScreen() {
            if (splashScreenForm != null) {
                splashScreenForm.CloseSplashScreen();
                splashScreenForm = null;
            }
        }

        public static void UdpateStatusText(string Text) {
            if (splashScreenForm != null)
                splashScreenForm.UdpateStatusText(Text);
        }
        
        public static void UdpateStatusTextWithStatus(string Text,TypeOfMessage typeOfMessage) {            
            if (splashScreenForm != null)
                splashScreenForm.UdpateStatusTextWithStatus(Text, typeOfMessage);
        }
    }

}
