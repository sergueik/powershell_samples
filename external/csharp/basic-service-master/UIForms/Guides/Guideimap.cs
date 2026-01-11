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
    public partial class Guideimap : ServiceMaster.UIForms.Guides.GuideBase
    {
        public Guideimap()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNero.Checked || NoDRW.Checked)
            {

                controller.ChangeList.Add(new ServiceChange("ImapiService",ServiceControllerStatus.Stopped, 4));
            }
        }
    }
}

