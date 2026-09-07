using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace WslManagerFramework {
    partial class Form1 {
        private IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent() {
            this.components = new Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 450);
            this.Text = "Form1";
            // all controls placed in Form1.cs
        }
    }
}

