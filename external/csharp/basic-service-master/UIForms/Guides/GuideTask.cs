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
    public partial class GuideTask : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideTask()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoKnownTask.Checked)
            {
                controller.ChangeList.Add(new ServiceChange("Schedule", ServiceControllerStatus.Stopped, 3));
            }
        }
    }
}

