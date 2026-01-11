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
    public partial class GuideFinish : ServiceMaster.UIForms.Guides.GuideBase
    {
        public GuideFinish()
        {
            InitializeComponent();
        }
        public void setData()
        {
            foreach (ServiceChange svcChange in controller.ChangeList)
            {
                ListViewItem lvi = new ListViewItem();
                string name = new ServiceController(svcChange.SvcName).DisplayName;
                lvi.Text=name;
                lvi.SubItems.Add(ServiceControl.statusTranslanter(svcChange.SvcStatus));
                lvi.SubItems.Add(ServiceControl.getStartType(svcChange.StartTypeID));
                lvi.SubItems.Add("Œ¥¥¶¿Ì");
                this.listView1.Items.Add(lvi);
            }
        }
    }
}

