using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
namespace ServiceMaster
{
    public partial class OutputSvcList : Form
    {
        public OutputSvcList()
        {
            InitializeComponent();
            checkListBoxInit();
        }
        public OutputSvcList(ServiceController[] svcs)
        {
            InitializeComponent();
            checkListBoxInit(svcs);
        }
        public void checkListBoxInit()
        {
            chlbSvcs.Items.Clear();
            SvcList slist = new SvcList();
            slist.ShowDialog();
            if (slist.DialogResult == DialogResult.OK)
            {
                ServiceController[] svcs = slist.getServicesList();
                chlbSvcs.DisplayMember = "DisplayName";
                chlbSvcs.Items.AddRange(svcs);
            }
            else
            {
                Close();
            }
        }
        public void checkListBoxInit(ServiceController[] svcs)
        {
            chlbSvcs.DisplayMember = "DisplayName";
            chlbSvcs.Items.AddRange(svcs);
        }
        private void chbAll_CheckedChanged(object sender, EventArgs e)
        {
            int listCount = chlbSvcs.Items.Count;
            for (int i = 0; i < listCount; i++)
                chlbSvcs.SetItemChecked(i, chbAll.Checked);
        }
        private string getConfigFileName()
        {
            SaveFileDialog saveFD = new SaveFileDialog();
            saveFD.RestoreDirectory = true;
            saveFD.Filter = "ServiceMaster列表文件(*.sml)|*.sml|文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            saveFD.ShowDialog();
            if (saveFD.FileName.Equals(""))
            {
                saveFD.Dispose();
                return "";
            }
            else
                return saveFD.FileName;
        }
        private void btnOutput_Click(object sender, EventArgs e)
        {
            ServiceController[] svcs = new ServiceController[chlbSvcs.CheckedItems.Count];
            for (int i = 0; i < chlbSvcs.CheckedItems.Count; i++)
            {
                svcs[i] = (ServiceController)(chlbSvcs.CheckedItems[i]);
            }
            string filename = getConfigFileName();
            if (filename.Equals(""))
                MessageBox.Show("没有选择文件！");
            else
            {
                int svcNum = ServiceControl.outputConfig(svcs, filename);
                MessageBox.Show(svcNum + "个服务的配置信息已导出");
            }
        }
        private void btnOutputAll_Click(object sender, EventArgs e)
        {
            string filename = getConfigFileName();
            ServiceController[] svcs = new ServiceController[chlbSvcs.Items.Count];
            for (int i = 0; i < chlbSvcs.Items.Count; i++)
            {
                svcs[i] = (ServiceController)(chlbSvcs.Items[i]);
            }
            if (filename.Equals(""))
                MessageBox.Show("没有选择文件！");
            else
            {
                int svcNum=ServiceControl.outputConfig(svcs,filename);
                MessageBox.Show(svcNum + "个服务的配置信息已导出");
            }
                
        }
        public new void Close()
        {
            DialogResult = DialogResult.Cancel;
            Dispose();
        }
        private void OutputSvcList_Load(object sender, EventArgs e)
        {

        }
    }
}