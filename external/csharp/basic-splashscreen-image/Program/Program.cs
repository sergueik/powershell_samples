using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SplashScreenControl;


namespace WindowsApplication1 {
    static class Program {
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            
            SplashScreen.Instance.Font = new System.Drawing.Font( "Verdana", 11.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );

            SplashScreen.SetBackgroundImage( WindowsApplication1.Resources.splashbg );
            SplashScreen.SetTitleString( "SplashScreen Demo" );
            SplashScreen.BeginDisplay();
            
            Application.Run( new Form1() );
        }
    }
}
