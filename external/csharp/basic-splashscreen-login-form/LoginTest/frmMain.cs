#region Using Statements

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LWork;

#endregion

namespace LoginTest
{
  /// <summary>
  /// The main form
  /// we can see here the name and the nikname of the logged user
  /// </summary>
  public partial class frmMain: Form
  {
    string strHello = String.Empty;

    public frmMain()
    {
      InitializeComponent();
      this.Paint += new PaintEventHandler(frmMain_Paint);
    }

    void frmMain_Paint( object sender, PaintEventArgs e )
    {
      Graphics grfx = e.Graphics;

      SizeF size = new SizeF();

      System.Drawing.Font fnt = new System.Drawing.Font(
      "Times New Roman", 35, System.Drawing.FontStyle.Bold);

      size = grfx.MeasureString(strHello, fnt);

      grfx.DrawString(strHello, fnt, Brushes.Red, (ClientSize.Width - (int)size.Width) / 2,
        (ClientSize.Height - (int)size.Height) / 2);

    }

    private void frmMain_Load( object sender, EventArgs e )
    {
      strHello = "Hello! " + LoginWork.User.Name + " aka (" + LoginWork.User.Nikname + ")";
    }
  }
}
