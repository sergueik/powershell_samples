using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SimpleThreadSafeCall
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(StartCalculation));
            thread.Start();
        }

        public void StartCalculation()
        {
            button1.SafeInvoke(d => d.Enabled = false);

            for (int i = 0; i <= 100; i++)
            {                
                Thread.Sleep(100);
                string textForLabel = (i) + "%";

                lblProcent.SafeInvoke(d => d.Text = textForLabel);
                progressBar1.SafeInvoke(d => d.Value = i);
                string labelText = lblProcent.SafeInvoke(d => d.Text);
                this.SafeInvoke(d => d.SetText("test", "test1"));
            }
            button1.SafeInvoke(d => d.Enabled = true);
        }

        public void SetText(string test1, string test2)
        {

        }
    }
}
