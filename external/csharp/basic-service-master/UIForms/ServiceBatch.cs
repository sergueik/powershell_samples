using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;

namespace ServiceMaster
{
    public partial class ServiceBatch : Form
    {
        ServiceController[] svcs;
        string status;
        string startType;
        int col;
        public ServiceBatch()
        {
            InitializeComponent();
            startType = "";
            status = "";
        }
        public ServiceBatch(ServiceController[] ser)
        {
            InitializeComponent();
            startType = "";
            status = "";
            svcListFill(ser);
            svcs = ser;
        }
        public new void Close()
        {
            DialogResult = DialogResult.Cancel;
        }
        private void svcSelect_Click(object sender, EventArgs e)
        {
            SvcList svcL = new SvcList();
            svcL.ShowDialog();
            if (svcL.DialogResult != DialogResult.Cancel)
            {
                svcs = svcL.getServicesList();
                svcListFill(svcs);
            }
        }
        private void svcListFill(ServiceController[] svcs)
        {
            ListViewItem lvi = new ListViewItem();
            ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
            foreach (ServiceController sc in svcs)
            {
                lvi = new ListViewItem();
                lvi.Text = sc.DisplayName;
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = sc.Status.ToString();
                lvi.SubItems.Add(lvsi);
                lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = ServiceControl.getStartType(sc.ServiceName);
                lvi.SubItems.Add(lvsi);
                listView1.Items.Add(lvi);
            }
 
        }
        private void btnChangeColor_Click(object sender, EventArgs e)
        {
            Color16Dialog cd = new Color16Dialog();
            cd.ShowDialog();
            Color Coldia = cd.getColor();
            lblColor.ForeColor = Coldia;
            ConsoleColor conColor = getColorNum(Coldia);
            col = (int)(conColor);
        }
        public ConsoleColor getColorNum(Color SysCol)
        {
            switch (SysCol.Name)
            {
                case "Black":
                    return ConsoleColor.Black;
                case "Blue":
                    return ConsoleColor.Blue;
                case "Cyan":
                    return ConsoleColor.Cyan;
                case "DarkBlue":
                    return ConsoleColor.DarkBlue;
                case "DarkCyan":
                    return ConsoleColor.DarkCyan;
                case "DarkGray":
                    return ConsoleColor.DarkGray;
                case "DarkMagenta":
                    return ConsoleColor.DarkMagenta;
                case "DarkGreen":
                    return ConsoleColor.DarkGreen;
                case "DarkRed":
                    return ConsoleColor.DarkRed;
                case "Yellow":
                    return ConsoleColor.Yellow;
                case "Red":
                    return ConsoleColor.Red;
                case "Magenta":
                    return ConsoleColor.Magenta;
                case "Green":
                    return ConsoleColor.Green;
                case "Gray":
                    return ConsoleColor.Gray;
                default:
                    return ConsoleColor.White;
            }
        }

        private void chbStart_CheckedChanged(object sender, EventArgs e)
        {
            rbtnAuto.Enabled = chbStart.Checked;
            rbtnDemand.Enabled = chbStart.Checked;
            rbtnDisable.Enabled = chbStart.Checked;
        }

        private void chbStatus_CheckedChanged(object sender, EventArgs e)
        {
            rbtnStart.Enabled = chbStatus.Checked;
            rbtnStop.Enabled = chbStatus.Checked;
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfD = new SaveFileDialog();
            sfD.Filter = "批处理文件(*.bat)|*.bat";
            sfD.ShowDialog();
            string fileName = sfD.FileName;
            if (fileName.Equals(""))
                sfD.Dispose();
            else
            {
                Boolean success = ServiceControl.outputBatch(svcs, col, status, startType, chbFileHead.Checked, chbFileEnd.Checked, fileName);
            }
        }

        private void rbtnDemand_CheckedChanged(object sender, EventArgs e)
        {
            startType = "demand";
        }

        private void rbtnDisable_CheckedChanged(object sender, EventArgs e)
        {
            startType = "disabled";
        }

        private void rbtnAuto_CheckedChanged(object sender, EventArgs e)
        {
            startType = "auto";
        }

        private void rbtnStop_CheckedChanged(object sender, EventArgs e)
        {
            status = "stop";
        }

        private void rbtnStart_CheckedChanged(object sender, EventArgs e)
        {
            status = "start";
        }
    }
}