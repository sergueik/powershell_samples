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
    public partial class GuideBase : Form
    {
        protected GuideControl controller;

        public GuideControl Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        public GuideBase()
        {
            controller = null;
            InitializeComponent();
        }

        protected void GuideBase_Load(object sender, EventArgs e)
        {

        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            ChangeLstUpdate();
            this.controller.doNext();
        }

        protected virtual void ChangeLstUpdate()
        {
 
        }

        public void setFinish()
        {
            this.btnNext.Text = "Íê³É";
        }

        private new void Close()
        {
            controller.closeAll();
        }

        public virtual bool detect()
        {
            return true;
        }
    }
}