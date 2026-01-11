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
    public partial class GuideAVP : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideAVP()
        {
            InitializeComponent();
        }
        protected override void ChangeLstUpdate()
        {
            if (rbtNoAVP.Checked)
            {
                controller.ChangeList.Add(new ServiceChange("AVP",ServiceControllerStatus.Stopped, 4));
                controller.ChangeList.Add(new ServiceChange("klnagent",ServiceControllerStatus.Stopped, 4));
            }
        }
        public override bool detect()
        {
            return ServiceControl.isSvcExist("AVP");
        } 
        private void GuideAVP_Load(object sender, EventArgs e)
        {

        }
    }
}

