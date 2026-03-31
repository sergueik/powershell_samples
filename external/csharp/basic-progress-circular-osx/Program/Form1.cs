using System;
using System.Drawing;
using System.Windows.Forms;
using Utils; // reference to your Utils.dll or project

namespace Program
{
    public partial class Form1 : Form
    {
        private CircularProgressControl cbc1;
        private CircularProgressControl cbc2;
        private Button button1;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(170, 140);
            this.Text = "OS X Progress Control";
            this.BackColor = Color.LightGray;

            cbc1 = new CircularProgressControl
            {
                Location = new Point(10, 20),
                MinimumSize = new Size(56, 56),
                Size = new Size(56, 56),
                TickColor = Color.DarkBlue,
                Clockwise = true,
                StartAngle = 270
            };

            cbc2 = new CircularProgressControl
            {
                Location = new Point(10, 80),
                MinimumSize = new Size(56, 56),
                Size = new Size(56, 56),
                TickColor = Color.Yellow,
                Clockwise = false,
                StartAngle = 270
            };

            button1 = new Button
            {
                Location = new Point(70, 80),
                Size = new Size(75, 23),
                Text = "Start"
            };
            button1.Click += Button1_Click;

            this.Controls.Add(cbc1);
            this.Controls.Add(cbc2);
            this.Controls.Add(button1);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                button1.Text = "Stop";
                cbc1.Start();
                cbc2.Start();
            }
            else
            {
                button1.Text = "Start";
                cbc1.Stop();
                cbc2.Stop();
            }
        }
    }
}
