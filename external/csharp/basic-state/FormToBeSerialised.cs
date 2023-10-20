using System;
using System.Diagnostics;
using System.Windows.Forms;
using FormSerialisation;

namespace SerialisationTest {
  public partial class FormToBeSerialised : Form {
    public FormToBeSerialised() {
      InitializeComponent();
    }

    private void chkHappy_CheckedChanged(object sender, EventArgs e) {
      grpHappy.Visible = chkHappy.Checked;
    }

    private void btnOK_Click(object sender, EventArgs e) {
      if(chkSerialise.Checked) {
        FormSerialisor.Serialise(this, Application.StartupPath + @"\serialise.xml");
      }
      this.Hide();
    }

    private void FormToBeSerialised_Load(object sender, EventArgs e) {
      FormSerialisor.Deserialise(this, Application.StartupPath + @"\serialise.xml");
    }

  }
}
