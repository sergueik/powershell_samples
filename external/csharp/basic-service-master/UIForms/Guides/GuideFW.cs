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
    public partial class GuideFW : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideFW()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoFW.Checked)
            {
                this.controller.ChangeList.Add(
                    new ServiceChange("SharedAccess", ServiceControllerStatus.Stopped, 4));
            }
        }
    }
}