using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServiceMaster
{
    public partial class Color16Dialog : Form
    {
        Color col;
        public Color16Dialog()
        {
            InitializeComponent();
        }

        private void Color16Dialog_Load(object sender, EventArgs e)
        {

        }
        public Color getColor()
        {
            return col;
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            col = ((RadioButton)(sender)).BackColor;
        }
    }
}