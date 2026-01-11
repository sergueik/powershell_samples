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
    public partial class GuideLDM : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideLDM()
        {
            InitializeComponent();
        }
                
        protected override void ChangeLstUpdate()
        {
            if (rbtNoDM.Checked)
                controller.ChangeList.Add(new ServiceChange("dmserver", ServiceControllerStatus.Stopped, 3));
        }
    }
}

