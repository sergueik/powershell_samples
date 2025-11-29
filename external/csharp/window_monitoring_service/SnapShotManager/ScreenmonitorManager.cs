/*
 * Please leave this Copyright notice in your code if you use it
 * Written by Decebal Mihailescu [http://www.codeproject.com/script/articles/list_articles.asp?userid=634640]
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ScreenMonitorLib;
using System.IO;
using System.Management;
using System.Xml;

namespace SnapShotManager
{
    public partial class ScreenmonitorManager : Form
    {
        SnapShotDS.ApplicationsDataTable _apps;
        //        List<AppInfo> _apps;
        List<string> _files;
        string _folder;
        SnapShot _snp;

        public ScreenmonitorManager()
        {
            _folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _snp = new ScreenMonitorLib.SnapShot(_folder);
            InitializeComponent();
        }

        private void UpdateFileList(object sender, System.Data.DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                SnapShotDS.SnapShotRow rw = e.Row as SnapShotDS.SnapShotRow;
                string crtfile = rw.FileName.ToString();
                if (!_files.Exists(delegate(string val) { return string.CompareOrdinal(val, crtfile) == 0; }))
                    _files.Add(crtfile);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            this.Cursor = Cursors.AppStarting;
            Application.DoEvents();
            IntPtr sysMenuHandle = Win32API.GetSystemMenu(this.Handle, false);
            Win32API.AppendMenu(sysMenuHandle, Win32API.MF_SEPARATOR, new IntPtr(0), string.Empty);
            Win32API.AppendMenu(sysMenuHandle, Win32API.MF_STRING, new IntPtr(IDM_ABOUT), "About Screen Monitor...");
            System.ServiceProcess.ServiceController sc2 = new System.ServiceProcess.ServiceController("Screen Monitor", ".");

            if (sc2.Status == System.ServiceProcess.ServiceControllerStatus.Running)
            {
                sc2.Stop();
                sc2.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);//, TimeSpan.FromSeconds(30));
            }

            _snapShotDataTableBindingSource.DataSource = _snp.History;
            _files = new List<string>(_snp.History.Rows.Count);
            _snp.History.RowDeleting += new DataRowChangeEventHandler(UpdateFileList);
            _apps = new SnapShotDS.ApplicationsDataTable();
            _apps.CaseSensitive = false;
            foreach (SnapShotDS.SnapShotRow rw in _snp.History)
            {
                if (_apps.FindByProcessName(rw.ProcessName) == null)
                {
                    SnapShotDS.WndSettingsRow[] rws = _snp.WndSettings.Select(string.Format("{0} = '{1}'", _snp.WndSettings.ProcessNameColumn.ColumnName, rw.ProcessName)) as SnapShotDS.WndSettingsRow[];
                    if (rws.Length > 0)
                        _apps.AddApplicationsRow(rws[0].ClassName, null, rw.ProcessName);
                }
            }
            foreach (SnapShotDS.WndSettingsRow rw in _snp.WndSettings)
            {
                if (_apps.FindByProcessName(rw.ProcessName) == null)
                {
                    _apps.AddApplicationsRow(rw.ClassName, null, rw.ProcessName);
                }
            }

            _apps.AcceptChanges();
            _applicationsDataTableBindingSource.DataSource = _apps;
            _wndSettingsDataTableBindingSource.DataSource = _snp.WndSettings;
            string strval;
            if(GetKeyValue("interval", out strval))
                _interval.Text = strval;

            ManagementObject wmiService = null;

            _cbxStartupType.DataSource = Enum.GetNames(typeof(System.ServiceProcess.ServiceStartMode));

            try
            {

                //ConnectionOptions coOptions = new ConnectionOptions();

                //coOptions.Impersonation = ImpersonationLevel.Impersonate;
                //ManagementScope mgmtScope = new System.Management.ManagementScope(@"root\CIMV2", coOptions);
                //mgmtScope.Connect();

                // Query WMI for additional information about this service.

                //ManagementObject wmiService;
                wmiService = new ManagementObject("Win32_Service.Name='Screen Monitor'");
                wmiService.Get();


                object o = wmiService["StartMode"];//"Auto" or "Disabled"
                if (o.ToString() == "Auto")
                    _cbxStartupType.SelectedItem = "Automatic";
                else
                    _cbxStartupType.SelectedItem = o;


            }
            catch (System.Management.ManagementException)
            {

                MessageBox.Show(this, "The service might not be installed on this computer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (wmiService != null)
                    wmiService.Dispose();
                this.Cursor = Cursors.Arrow;

            }

        }

        private void snapShotDataTableBindingSource_PositionChanged(object sender, EventArgs e)
        {
            BindingSource bs = sender as BindingSource;

            DataRowView dr = bs.Current as DataRowView;
            if (dr != null)
            {
                SnapShotDS.SnapShotRow rw = dr.Row as SnapShotDS.SnapShotRow;
                string path = System.IO.Path.Combine(_folder, rw.FileName + ".jpg");
                if (File.Exists(path))
                {
                    if (_pictureBox.Image != null)
                        _pictureBox.Image.Dispose();
                    _pictureBox.Image = new Bitmap(path);
                }
            }
            else
            {
                if (_pictureBox.Image != null)
                    _pictureBox.Image.Dispose();
                _pictureBox.Image = null;
            }

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "Do you want to save changes before exiting?", "choose", MessageBoxButtons.YesNoCancel);
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            switch (res)
            {
                case DialogResult.Yes:
                    _snp.SaveHistory();

                    _files.ForEach(delegate(string name)
                    {
                        if (_snp.History.Select(string.Format("{0} = '{1}'", _snp.History.FileNameColumn.ColumnName, name)).Length == 0)
                        {
                            string path = System.IO.Path.Combine(_folder, name + ".jpg");
                            File.Delete(string.Format(path));
                        }
                    });

                    _snp.SaveSettings();
                    e.Cancel = false;
                    break;
                case DialogResult.No:
                    e.Cancel = false;
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
            base.OnClosing(e);
            if (!e.Cancel)
            {
                if (_interval.Text.Length > 0)
                {
                    try
                    {
                        uint n = uint.Parse(_interval.Text);
                        if (n > 0)
                        {
                            ChangeKey("interval", _interval.Text);
                        }

                    }
                    catch
                    {


                    }
                }

                ManagementObject wmiService = null;

                ManagementBaseObject InParam = null;

                try
                {

                    ConnectionOptions coOptions = new ConnectionOptions();

                    coOptions.Impersonation = ImpersonationLevel.Impersonate;
                    ManagementScope mgmtScope = new System.Management.ManagementScope(@"root\CIMV2", coOptions);
                    mgmtScope.Connect();

                    // Query WMI for additional information about this service.


                    wmiService = new ManagementObject("Win32_Service.Name='Screen Monitor'");
                    wmiService.Get();
                    object o = wmiService["StartMode"];//"Auto" or "Disabled"
                    InParam = wmiService.GetMethodParameters("ChangeStartMode");
                    string start = _cbxStartupType.Text;
                    InParam["StartMode"] = start;
                    ManagementBaseObject outParams = wmiService.InvokeMethod("ChangeStartMode", InParam, null);
                      uint ret = (uint)(outParams.Properties["ReturnValue"].Value);
                    if (ret != 0)
                        MessageBox.Show(this, "Error", string.Format("Failed to set the Start mode, error code: {0}", ret), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    //bad parent process
                    if (_ckStartService.Checked)
                    {
                        // Start service
                        outParams = wmiService.InvokeMethod("StartService", null, null);
                        ret = (uint)(outParams.Properties["ReturnValue"].Value);
                        if (ret != 0)
                            MessageBox.Show(this, "Error", string.Format("Failed to start the service with error code: {0}", ret), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }

                }
                catch (System.Management.ManagementException ex)
                {
                    //Console.WriteLine("Could not query for Win32_Service.Name = {0}", myServiceName);
                    MessageBox.Show(this, "The service might not be installed on this computer. or\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (InParam != null)
                        InParam.Dispose();
                    if (wmiService != null)
                        wmiService.Dispose();

                }

            }
            this.Cursor = Cursors.Arrow;
            Application.DoEvents();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            foreach (DataGridViewRow drv in _HistorydataGridView.SelectedRows)
            {
                _HistorydataGridView.Rows.Remove(drv);
            }
            this.Cursor = Cursors.Arrow;

        }


        private void _dataGridViewSettings_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }

        private void _tabCtrl_Selecting(object sender, TabControlCancelEventArgs e)
        {

            UIApps Apps = new UIApps(System.Diagnostics.Process.GetProcesses());
            Apps.ForEach(delegate(UIApp app)
            {
                if (_apps.FindByProcessName(app.ProcessName) == null)
                {
                    int len = _apps.Select(string.Format("{0} = '{1}'", _apps.ClassNameColumn.ColumnName, app.WindowClass)).Length;
                    if (len == 0)
                        _apps.AddApplicationsRow(app.WindowClass, app.Caption, app.ProcessName);
                }
            });

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;
            if (tsb == toolStripButton1)
            {
                _dataGridViewSettings.CommitEdit(DataGridViewDataErrorContexts.Commit);

                SnapShotDS.WndSettingsRow rw = _snp.WndSettings.NewWndSettingsRow();
                rw.ForegroundOnly = false;
                rw.GetIconicWnd = false;
                rw.ClientWindow = true;
                _snp.WndSettings.AddWndSettingsRow(rw);
                _wndSettingsDataTableBindingSource.CurrencyManager.Position = _snp.WndSettings.Rows.Count - 1;
                toolStripButton1.Enabled = false;
            }
        }


        private void _wndSettingsDataTableBindingSource_PositionChanged(object sender, EventArgs e)
        {
            if (_wndSettingsDataTableBindingSource.CurrencyManager.Position != _snp.WndSettings.Rows.Count - 1)
            {
                SnapShotDS.WndSettingsRow lastrow = _snp.WndSettings.Rows[_snp.WndSettings.Rows.Count - 1] as SnapShotDS.WndSettingsRow;
                if (lastrow.RowState == DataRowState.Added && lastrow.ClassName.Length == 0)
                {
                    _snp.WndSettings.Rows.Remove(lastrow);
                    toolStripButton1.Enabled = true;
                }

            }
        }

        private void _wndSettingsDataTableBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            int index = _wndSettingsDataTableBindingSource.CurrencyManager.Position;
            SnapShotDS.WndSettingsRow crtrow = _snp.WndSettings.Rows[index] as SnapShotDS.WndSettingsRow;
            if (_snp.WndSettings.Rows.Count > index && crtrow.ClassName.Length > 0)
            {
                SnapShotDS.ApplicationsRow app = _apps.FindByProcessName(crtrow.ProcessName);
                if (app != null)
                {
                    crtrow.ClassName = app.ClassName;
                    toolStripButton1.Enabled = true;
                }
            }



        }

        private void _dataGridViewSettings_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex >= 0 && e.RowIndex == _snp.WndSettings.Rows.Count - 1 && ProcessName.Index == e.ColumnIndex)
            {
                int index = _wndSettingsDataTableBindingSource.CurrencyManager.Position;
                SnapShotDS.WndSettingsRow crtrow = _snp.WndSettings.Rows[index] as SnapShotDS.WndSettingsRow;
                string proc = _dataGridViewSettings.CurrentRow.Cells[0].Value.ToString();
                if (_dataGridViewSettings.Rows.Count > index && proc.Length > 0)
                {
                    crtrow.ProcessName = proc;
                    SnapShotDS.ApplicationsRow app = _apps.FindByProcessName(crtrow.ProcessName);
                    if (app != null)
                    {
                        crtrow.ClassName = app.ClassName;
                    }
                }
            }
        }

        private void _cbxStartupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb == null)
                return;
            if (cb.SelectedIndex == 2)
            {
                _ckStartService.Checked = false;
                _ckStartService.Enabled = false;
            }
            else
                _ckStartService.Enabled = true;
        }

        private const Int32 IDM_ABOUT = 5000;
        internal delegate void AboutHandler();
        protected override void WndProc(ref Message m)
        {
            switch (m.WParam.ToInt32())
            {
                case IDM_ABOUT:
                    {
                        AboutHandler hnd = delegate()
                        {
                            AboutUtil.About dlg = new AboutUtil.About(this);
                            dlg.ShowDialog();
                            dlg.Dispose();
                        };
                        BeginInvoke(hnd);
                    }

                    break;
                default:
                    break;
            }
            base.WndProc(ref m);
        }



        private void _dataGridViewSettings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != getIconicWndDataGridViewCheckBoxColumn.Index && e.ColumnIndex != foregroundOnlyDataGridViewCheckBoxColumn.Index)
                return;
            DataGridViewCheckBoxCell ck = _dataGridViewSettings.CurrentRow.Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
            bool val = (bool)ck.EditingCellFormattedValue;
            if (e.ColumnIndex == getIconicWndDataGridViewCheckBoxColumn.Index && val)
            {
                ck = _dataGridViewSettings.CurrentRow.Cells[foregroundOnlyDataGridViewCheckBoxColumn.Index] as DataGridViewCheckBoxCell;
                ck.Value = false;
            }
            if (e.ColumnIndex == foregroundOnlyDataGridViewCheckBoxColumn.Index && val)
            {
                ck = _dataGridViewSettings.CurrentRow.Cells[getIconicWndDataGridViewCheckBoxColumn.Index] as DataGridViewCheckBoxCell;
                ck.Value = false;
            }
        }

        static bool ChangeKey(string strKey, string strValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //Change this to the location of your configuration file
            try
            {
                xmlDoc.Load("ScreenmonitorService.exe.config");
                //Change this if node is different
                XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

                //Check if the node exists before changing it
                foreach (XmlNode childNode in appSettingsNode.ChildNodes)
                {
                    if ((childNode.Attributes["key"].Value == strKey))
                    {
                        childNode.Attributes["value"].Value = strValue;
                    }
                }
                xmlDoc.Save("ScreenmonitorService.exe.config");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static bool GetKeyValue(string strKey, out string strValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //Change this to the location of your configuration file
            strValue = string.Empty;
            try
            {
                xmlDoc.Load("ScreenmonitorService.exe.config");
                //Change this if node is different
                XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

                //Check if the node exists before changing it
                foreach (XmlNode childNode in appSettingsNode.ChildNodes)
                {
                    if ((childNode.Attributes["key"].Value == strKey))
                    {
                         strValue = childNode.Attributes["value"].Value;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}