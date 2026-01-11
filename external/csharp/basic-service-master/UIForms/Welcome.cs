using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace ServiceMaster
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
            testDbFile();
        }
        private void testDbFile()
        {
            if (!File.Exists(Application.StartupPath +"\\"+ Properties.Settings.Default.DBfile))
                MessageBox.Show("找不到数据库文件！\n" + Application.StartupPath + "\\" + Properties.Settings.Default.DBfile, "数据库文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void Welcome_Load(object sender, EventArgs e)
        {
            VersionInfo.Text="Ver "+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            timer1.Enabled = true;
            timer1.Start(); 
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 20;
            transUI();
        }
        public void transUI()
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.02;
            }
            else
            {
                this.timer1.Enabled = false;
                this.Visible = false;
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}