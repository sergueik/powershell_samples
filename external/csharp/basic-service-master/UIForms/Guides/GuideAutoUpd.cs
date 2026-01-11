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
    public partial class GuideAutoUpd : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideAutoUpd()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoAutoUpdate.Checked)
            {
                this.controller.ChangeList.Add(new ServiceChange("wuauserv", ServiceControllerStatus.Stopped, 3));
            }
        }
    }
}

