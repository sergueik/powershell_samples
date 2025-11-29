using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;
namespace ScreenmonitorService
{
    enum SystemCategory : short
    {
        None = 0,
        Devices,
        Disk,
        Printers,
        Services,
        Shell,
        SystemEvent,
        Network

    }
    public partial class ScreenMonitor : ServiceBase
    {
        //private System.Diagnostics.EventLog _eventLog;
        public ScreenMonitor()
        {
            InitializeComponent();
            //this.EventLog.Source = "Screen Monitor";
            //this.EventLog.Log = EventLog.Source;

            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            //this.CanHandlePowerEvent = true;
            //this.CanHandleSessionChangeEvent = true;
            //this.CanPauseAndContinue = true;
            //this.CanShutdown = true;
            //this.CanStop = true;

            //if (!System.Diagnostics.EventLog.SourceExists(EventLog.Source))
            //    System.Diagnostics.EventLog.CreateEventSource(
            //       EventLog.Source, "Application");

            //System.Diagnostics.EventLog _eventLog =
            //   new System.Diagnostics.EventLog();
            //_eventLog.Source = "ExistingSourceString";
            //_eventLog.WriteEntry("TestEntry2");


        }

#if(DEBUG)
        public void StartDebugService()
        {
            //start debugger if needed
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                System.Diagnostics.Debugger.Launch();
            }
            //System.Diagnostics.Debugger.Log(1, "info", "starting with the debugger attached");
            //using (RegistryKey ckey = Registry.LocalMachine.OpenSubKey(
            //    string.Format(@"SYSTEM\CurrentControlSet\Services\{0}", this.ServiceName), true))
            //{
            //    // Good to always do error checking!
            //    if (ckey != null)
            //    {
            //        // Ok now lets make sure the "Type" value is there, 
            //        //and then do our bitwise operation on it.
            //        if (ckey.GetValue("Type") != null)
            //        {
            //            ckey.SetValue("Type", ((int)ckey.GetValue("Type") | 256));
            //        }
            //    }
            //}
            OnStart(Environment.GetCommandLineArgs());
        }
#endif
        protected override void OnStart(string[] args)
        {
            //EventLog.WriteEntry("plain string entry");
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Warning", EventLogEntryType.Warning);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Warning event id =1, categ id = 0", EventLogEntryType.Warning, 1, 0);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Warning event id =1, categ id = 1", EventLogEntryType.Warning, 1, 1);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Warning event id =1, categ id = 2", EventLogEntryType.Warning, 1, 2);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Warning event id =1, categ id = 3", EventLogEntryType.Warning, 1, 3);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.Error event id =2, categ id = 4", EventLogEntryType.Error, 1, 4);
            //EventLog.WriteEntry(this.ServiceName, " 'service name = this.ServiceName' +plain string entry + EventLogEntryType.error event id =3, categ id = 5", EventLogEntryType.Error, 1, 5);
            //EventLog.WriteEntry("other source", " 'service name = other source' +plain string entry + EventLogEntryType.SuccessAudit", EventLogEntryType.SuccessAudit);

            //for (SystemCategory i = SystemCategory.None; i <= SystemCategory.Network; i++)
            //{
            //    EventLog.WriteEntry("test source id", " service name = 'test source id' + EventLogEntryType.Information event id = " + i.ToString() + ", categ id = " + i.ToString(), EventLogEntryType.Information, Convert.ToInt32(i), Convert.ToInt16(i));
            //}
            if(ScreenMonitorLib.SnapShot.SaveSnapShot())
                EventLog.WriteEntry(this.ServiceName, "Got the image", EventLogEntryType.Information, 1, Convert.ToInt16(SystemCategory.Shell));
            else
                EventLog.WriteEntry(this.ServiceName, "did not get the image", EventLogEntryType.Error, 1, Convert.ToInt16(SystemCategory.Shell));

        }

        /// <summary>
        /// OnStop: Put your stop code here
        /// - Stop threads, set final data, etc.
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
        }

        /// <summary>
        /// OnPause: Put your pause code here
        /// - Pause working threads, etc.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// OnContinue: Put your continue code here
        /// - Un-pause working threads, etc.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();
        }

        /// <summary>
        /// OnShutdown(): Called when the System is shutting down
        /// - Put code here when you need special handling
        ///   of code that deals with a system shutdown, such
        ///   as saving special data before shutdown.
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        /// <summary>
        /// OnCustomCommand(): If you need to send a command to your
        ///   service without the need for Remoting or Sockets, use
        ///   this method to do custom methods.
        /// </summary>
        /// <param name="command">Arbitrary Integer between 128 & 256</param>
        protected override void OnCustomCommand(int command)
        {
            //  A custom command can be sent to a service by using this method:
            //#  int command = 128; //Some Arbitrary number between 128 & 256
            //#  ServiceController sc = new ServiceController("NameOfService");
            //#  sc.ExecuteCommand(command);

            base.OnCustomCommand(command);
        }

        /// <summary>
        /// OnPowerEvent(): Useful for detecting power status changes,
        ///   such as going into Suspend mode or Low Battery for laptops.
        /// </summary>
        /// <param name="powerStatus">The Power Broadcase Status (BatteryLow, Suspend, etc.)</param>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        /// <summary>
        /// OnSessionChange(): To handle a change event from a Terminal Server session.
        ///   Useful if you need to determine when a user logs in remotely or logs off,
        ///   or when someone logs into the console.
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);
        }
    }
}
