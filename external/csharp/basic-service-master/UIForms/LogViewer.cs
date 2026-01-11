using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ServiceMaster
{
    public partial class LogViewer : Form
    {
        public LogViewer()
        {
            InitializeComponent();
            loadLog();
        }
        private void loadLog()
        {
            FileStream fs=null;
            StreamReader sr = null; ;
            try
            {

                fs = new FileStream(Application.StartupPath + "\\EVENTLOG.LOG", FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("日志文件不存在！！");
               // this.Dispose();
                return;
            }
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                string[] n={": "};
                string[] str = s.Split(n, StringSplitOptions.RemoveEmptyEntries);
                tbxLog.Text += "["+str[0]+"]--[";
                tbxLog.Text += str[1]+"]\r\n";
                s = sr.ReadLine();
                tbxLog.Text += LogProcess(s)+"\r\n";
            }
            sr.Close();
            fs.Close();
        }
        private string LogProcess(string s)
        {
            string[] spt={": "," as "};
            string[] LogInfo=s.Split(spt,3,StringSplitOptions.RemoveEmptyEntries);
            string info="";
            if (LogInfo[0].Equals("setStartType"))
            {
                info += "改变服务" + LogInfo[1] + "的启动方式";
            }
            else if (LogInfo[0].Equals("OutputFile"))
            {
                info += "输出配置文件" + LogInfo[1];
            }
            else
            {
                info = s;
            }
            return info;
        }
        private void LogViewer_Load(object sender, EventArgs e)
        {

        }

        private void tbxLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}