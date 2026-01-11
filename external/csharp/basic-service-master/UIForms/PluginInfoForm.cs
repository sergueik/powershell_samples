using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ServiceMaster
{
    public partial class PluginInfoForm : Form
    {
        public PluginInfoForm()
        {
            InitializeComponent();
            pluginInfoInit();
        }

        private void PluginInfoForm_Load(object sender, EventArgs e)
        {

        }
        private void pluginInfoInit()
        {
            listView1.Items.Clear();
            IPluginInfo[] infos = PluginCheck.getAllPluginsInfo();
            foreach (IPluginInfo info in infos)
            {
                if (info != null)
                {
                    string[] pluginInfoStr = new string[3];
                    pluginInfoStr[0] = info.Name;
                    pluginInfoStr[1] = info.Version;
                    pluginInfoStr[2] = PluginCheck.getPluginType(info);
                    ListViewItem lvi = new ListViewItem(pluginInfoStr);
                    listView1.Items.Add(lvi);
                }
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}