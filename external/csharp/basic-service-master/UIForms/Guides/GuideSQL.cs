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
    public partial class GuideSQL : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideSQL()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoSQL.Checked)
            {
                controller.ChangeList.Add(new ServiceChange("SQLWriter",ServiceControllerStatus.Stopped,3));
                controller.ChangeList.Add(new ServiceChange("SQLBrowser",ServiceControllerStatus.Stopped,3));
            }
        }

        private void GuideSQL_Enter(object sender, EventArgs e)
        {
            if (!ServiceControl.isSvcExist("SQLWriter"))
                this.controller.doNext();
        }
    }
}

