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
    public partial class GuideCptBws : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideCptBws()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if(rbtNoComputerBrowser.Checked)
                controller.ChangeList.Add(new ServiceChange ("Browser",ServiceControllerStatus.Stopped,3));
        }
    }
}

