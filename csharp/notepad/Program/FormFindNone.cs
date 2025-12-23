using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
    public partial class FormFindNone : Form {
        public FormFindNone(string text) {
            InitializeComponent();

            this.Icon = new Icon(FormMain.pathIconRed);

            findNoneLabel.Text = String.Format("Не удается найти: \"{0}\"", text);
        }

        private void findNoneButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
