using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
    public partial class FormAbout : Form
    {
        public FormAbout() {
            InitializeComponent();

            this.Icon = new Icon(FormMain.pathIconBlue);
        }
    }
}
