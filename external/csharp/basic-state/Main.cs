using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerialisationTest {
  public partial class Main : Form {
    public Main() {
      InitializeComponent();
    }

    private void btnOpen_Click(object sender, EventArgs e) {
      FormToBeSerialised c = new FormToBeSerialised();
      c.ShowDialog();
    }

  }
}
