using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LoginTest
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // Run the application with our context. It is splash, log and main forms
      Application.Run(new LoginTestContext());
    }
  }
}
