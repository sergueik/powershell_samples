using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServiceMaster
{
    public partial class Option : Form
    {
        public Option()
        {
            InitializeComponent();
            ConfigInit();
        }
        private void ConfigInit()
        {
            chbSaveLog.Checked = Properties.Settings.Default.isSaveLog;
            tbxDBAddr.Text =Application.StartupPath+"\\"+Properties.Settings.Default.DBfile;
            tbxLogAddr.Text = Application.StartupPath +"\\"+ Properties.Settings.Default.Log;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ConfigInit();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.isSaveLog = chbSaveLog.Checked;
            Properties.Settings.Default.DBfile = tbxDBAddr.Text;
            Properties.Settings.Default.Log = tbxLogAddr.Text;
        }
    }
}