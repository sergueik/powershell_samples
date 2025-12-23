using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
    public partial class FormFind : Form {
        public RichTextBox mainTextField;
        public int startIndex = 0;

        public FormFind(ref RichTextBox richTextBox) {
            InitializeComponent();

            this.Icon = new Icon(FormMain.pathIconBlue);

            mainTextField = richTextBox;
            startIndex = mainTextField.SelectionStart;
        }

        private void findButtonFindMore_Click(object sender, EventArgs e) {
            if (findTextField.Text.Length == 0) {
                findLabel2.Text = "Введите текст";
                return;
            }

            string source;
            string dest;

            if (findCheckRegister.Checked) {
                source = mainTextField.Text;
                dest = findTextField.Text;
            } else {
                source = mainTextField.Text.ToLower();
                dest = findTextField.Text.ToLower();
            }

            find(source, dest, findGroupBoxRadioButtonDown.Checked);
        }

        private void findButtonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void find(string source, string dest, bool toDown) {
            if (source.Contains(dest)) {
                int endIndex = source.Length - dest.Length;

                if (toDown) {
                    for (int i = startIndex; i < endIndex; i++) {
                        if (source.Substring(i, dest.Length) == dest) {
                            mainTextField.Select(i, dest.Length);
                            startIndex = i + 1;
                            break;
                        }
                    }
                } else {
                    for (int i = startIndex; i > 0; i--) {
                        if (source.Substring(i, dest.Length) == dest) {
                            mainTextField.Select(i, dest.Length);
                            startIndex = i - 1;
                            break;
                        }
                    }
                }
            } else {
                FormFindNone formFindNone = new FormFindNone(findTextField.Text);
                formFindNone.Show();
            }
        } 
    }
}