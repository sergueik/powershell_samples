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
    public partial class GuidePt : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuidePt()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoPrinter.Checked)
            {
                controller.ChangeList.Add(new ServiceChange ("spooler",ServiceControllerStatus.Stopped,4));
            }
        }
    }
}

