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
    public partial class GuideTheme : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideTheme()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
                        
            if (rbtNoTheme.Checked)
            {
                controller.ChangeList.Add(new ServiceChange("Themes",ServiceControllerStatus.Stopped,4));
            }
        }
    }
}

