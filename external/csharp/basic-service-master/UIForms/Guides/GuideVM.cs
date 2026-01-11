using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;

namespace ServiceMaster.UIForms.Guides
{
    public partial class GuideVM : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideVM()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoVM.Checked)
            {
                controller.ChangeList.Add(new ServiceChange("VMAuthdService",ServiceControllerStatus.Stopped,3));
                controller.ChangeList.Add(new ServiceChange("VMnetDHCP",ServiceControllerStatus.Stopped,3));
                controller.ChangeList.Add(new ServiceChange("VMount2",ServiceControllerStatus.Stopped,3));
                controller.ChangeList.Add(new ServiceChange("VMware NAT Service",ServiceControllerStatus.Stopped,3));
                controller.ChangeList.Add(new ServiceChange("ufad-ws60",ServiceControllerStatus.Stopped,3)); 
            }
        }
        private void GuideVM_Enter(object sender, EventArgs e)
        {
            if (!ServiceControl.isSvcExist("VMware NAT Service"))
                this.controller.doNext();
        }
    }
}

