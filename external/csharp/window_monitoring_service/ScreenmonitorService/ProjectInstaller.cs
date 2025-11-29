using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;

namespace ScreenmonitorService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();


        }

        private void ServiceMonitorInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (RegistryKey ckey = Registry.LocalMachine.OpenSubKey(
                string.Format(@"SYSTEM\CurrentControlSet\Services\{0}", ServiceMonitorInstaller.ServiceName), true))
            {
                // Good to always do error checking!
                if (ckey != null)
                {
                    // Ok now lets make sure the "Type" value is there, 
                    //and then do our bitwise operation on it.
                    if (ckey.GetValue("Type") != null)
                    {
                        ckey.SetValue("Type", ((int)ckey.GetValue("Type") | 256));
                    }
                }
            }
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}