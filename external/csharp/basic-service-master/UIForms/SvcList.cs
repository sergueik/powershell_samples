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
    public partial class SvcList : Form
    {
        private ServiceController[] selectSvcs;          
        public SvcList()
        {
            InitializeComponent();
            checkListBoxInit();
        }
        public ServiceController[] getServicesList()
        {
            return selectSvcs;
        }
        public void checkListBoxInit()
        {
            chlbSvcs.Items.Clear();
            ServiceController[] svcs = ServiceController.GetServices();
            chlbSvcs.DisplayMember = "DisplayName";
            chlbSvcs.Items.AddRange(svcs);
        }
        private void chbAll_CheckedChanged(object sender, EventArgs e)
        {
            int listCount = chlbSvcs.Items.Count;
            for (int i = 0; i < listCount; i++)
                chlbSvcs.SetItemChecked(i, chbAll.Checked);
        }
        private void btnOutput_Click(object sender, EventArgs e)
        {
            selectSvcs = new ServiceController[chlbSvcs.CheckedItems.Count];
            for (int i = 0; i < chlbSvcs.CheckedItems.Count; i++)
            {
                selectSvcs[i] = (ServiceController)(chlbSvcs.CheckedItems[i]);
            }
            DialogResult = DialogResult.OK;
        }
        public new void Close()
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}