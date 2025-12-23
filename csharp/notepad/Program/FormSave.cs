using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
    public partial class FormSave : Form {
        public FormSave(string filename) {
            InitializeComponent();

            this.Icon = new Icon(FormMain.pathIconRed);
            this.Text = FormMain.title;
            saveLabel.Text += String.Format("\"{0}\"?", filename);
        }

        private void saveButtonSave_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void saveButtonDontSave_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.No;
            this.Close();
        }

        private void saveButtonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
