using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace LoginTest
{
  /// <summary>
  /// This class contains logic and data the LoginTest application
  /// </summary>
  public class LoginTestContext: ApplicationContext
  {

    #region Private fields
    // here we can declare the all forms application and manage it directly
    //(show, close, set as MainForm and so on) 
    private frmLogIn fLogin;
    private frmMain fMain;

    #endregion

    #region Initialization

    public LoginTestContext()
    {
      CreateSplashForm();

      CreateLoginForm();

    }

    #endregion

    #region Private Methods

    /// <summary>
    ///  The Splash form 
    ///  initialization, show and close
    /// </summary>
    private static void CreateSplashForm()
    {

      Form fSplash = new Form();
      fSplash.BackgroundImage = System.Drawing.Image.FromFile(@"backg.bmp");

      fSplash.BackgroundImageLayout = ImageLayout.Center;
      fSplash.FormBorderStyle = FormBorderStyle.None;
      fSplash.StartPosition = FormStartPosition.CenterScreen;
      fSplash.TopMost = true;

      fSplash.TransparencyKey = System.Drawing.Color.White;// it sets transparency for the background of image

      // Set the splash form size and we are shure the image fit to the form
      fSplash.Width = (int)fSplash.BackgroundImage.PhysicalDimension.Width;
      fSplash.Height = (int)fSplash.BackgroundImage.PhysicalDimension.Height;

      fSplash.Show();
      System.Threading.Thread.Sleep(2000);
      fSplash.Close();

    }

    /// <summary>
    /// The Login form
    /// initialization and show
    /// </summary>
    private void CreateLoginForm()
    {

      fLogin = new frmLogIn();
      fLogin.Closed += new EventHandler(fLogin_Closed);
      this.MainForm = fLogin;
      fLogin.Show();

    }

    #endregion

    #region Events

    /// <summary>
    /// If the login procedure done successfully
    /// we'll see the Main Form
    /// else the application will close 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void fLogin_Closed( object sender, EventArgs e )
    {
      if (LWork.LoginWork.Logged) //if the user is logged
      {
        fMain = new frmMain();
        this.MainForm = fMain; //set the main message loop applicaton in this form
        fMain.Show();
      }
      else
      {
        ExitThread();
      }

    }

    #endregion
  }
}
