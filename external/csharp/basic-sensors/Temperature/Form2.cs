using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using OpenHardwareMonitor;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Management;


namespace Temperature
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(string content)
        {
            InitializeComponent();
            TimerInterval = content;  // PASS TIMER INTERVAL FOR DISPLAY
            
        }
        // private AboutBoxTools abt = new AboutBoxTools();
        // private DialogTools dt = new DialogTools();
        private string TimerInterval = String.Empty;
        private OHData odF2 = new OHData();
        private RamData rd = new RamData();
        private WMIBIOS b = new WMIBIOS();
         

        // SUPPORT FOR GETWINDOWSVERSION()
        #region
        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();
        // Detect 32 or 64 bit OS
        // MICROSOFT: Raymond Chen
        // http://blogs.msdn.com/oldnewthing/archive/2005/02/01/364563.aspx
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
             [In] IntPtr hProcess,
             [Out] out bool wow64Process
         );
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
        /// Helper method to check if the currently installed OS version name
        /// contains the given string
        public bool IsCurrentOSContains(string name)
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");

            return productName.Contains(name);
        }
        #endregion

        //  FOR CALCULATING DISPLAY RESOLUTION SCALEFACTOR
        #region
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        };
        #endregion

        // ALTERNATING LISTVIEW ROW COLORS
        // REV 07-30-2016 TO INCLUDE SUBITEMS
        public void SetAlternatingColors(ListView lv, Color c1, Color c2)
       {
            int x = 0;
          foreach (ListViewItem item in lv.Items)
          { 
              if (item.Index % 2 == 0)
              {
                  item.BackColor = c1;
                  for (x=0;x<item.SubItems.Count;x++)
                    {
                        item.SubItems[x].BackColor = c1;
                    }
              }
              else
              {
                  item.BackColor = c2;
                    for (x = 0; x < item.SubItems.Count; x++)
                    {
                        item.SubItems[x].BackColor = c2;
                    }
                }
          }     
        }
        // FORM CLOSE
        private void btnForm2Quit_Click(object sender, EventArgs e)
        {
            timerInfoForm.Stop();
            this.Close();
        }
        // FORM LOAD
        private void Form2_Load(object sender, EventArgs e)
        {
            timerInfoForm.Interval = 1000;
            UpdateDisplay();
            
            
        }
        // COPY LISTVIEW TO CLIPBOARD
        // CREDITS: http://dotnetref.blogspot.com/2007/06/copy-listview-to-clipboard.html
        public void CopyListViewToClipboard(ListView lv)
        {
            int pixelstochars = 5;
            string s = string.Empty;
            int[] pad = new int[3];
            pad[0] = (lv.Columns[0].Width)/pixelstochars;
            pad[1] = (lv.Columns[1].Width)/(pixelstochars-2);
            pad[2] = (lv.Columns[2].Width)/(pixelstochars+2);
            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < lv.Columns.Count; i++)
            {
                buffer.Append(lv.Columns[i].Text.PadRight(pad[i]));
                buffer.Append("\t");
                
            }
           
            buffer.Append("\n\n");

            for (int i = 0; i < lv.Items.Count; i++)
            {
                for (int j = 0; j < lv.Columns.Count; j++)
                {
                    // s = lv.Items[i].SubItems[j].Text.PadRight(pad[j]);
                    buffer.Append(lv.Items[i].SubItems[j].Text.PadRight(pad[j]));
                    buffer.Append("\t");
                }

                buffer.Append("\n");
            }

            Clipboard.SetText(buffer.ToString());
        }
        
        // UPDATE DISPLAY
        private void UpdateDisplay()
        {
            GetMemory(ref rd);
            if (TimerInterval != String.Empty)
            {
                lblTimerInterval.Text = TimerInterval;
            }
            else
            {
                lblTimerInterval.Text = "";
            }
            // OHData odF2 = new OHData();
            odF2.Update();
           

            listView1.Items.Clear();
            foreach (OHData.OHMitem o in odF2.DataList)
            {
                // FILL IN MEMORY DATA IF MISSING

                if (o.name == "Available Memory" && o.reading == "")
                {
                    o.reading = rd.FreeMemory;
                }
                if (o.name == "Generic Memory" && o.reading == "")
                {
                    o.reading = rd.TotalMemory;
                }
                if (o.name == "Used Memory" && o.reading == "")
                {
                    o.reading = rd.PercentUsed;
                }

                string[] r = new string[2];
                r[0] = o.name;
                r[1] = o.reading;
                listView1.Items.Add(o.type).SubItems.AddRange(r);

            }
            // ADD DISPLAY INFO 09-14-2017
            string srtype = "Display";
            string[] gs = new string[2];
            gs[0] = "Resolution";
            gs[1] = CurrentPrimaryDisplayResolution();
            listView1.Items.Add(srtype).SubItems.AddRange(gs);

            //WMIBIOS b = new WMIBIOS();
            b.Update();
            if (b.Name != String.Empty)
            {
                string type = String.Empty;
                string[] r = new string[2];
                type = "BIOS";
                r[0] = b.Name;
                r[1] = "";
                listView1.Items.Add(type).SubItems.AddRange(r);
                type = "Manufacturer";
                r[0] = b.Manufacturer;
                r[1] = "";
                listView1.Items.Add(type).SubItems.AddRange(r);
                type = "Version";
                r[0] = b.Version;
                r[1] = "";
                listView1.Items.Add(type).SubItems.AddRange(r);
                type = "Release Date";
                //r[0] = FormatDate( b.Date);
                r[0] = b.Date;
                r[1] = "";
                listView1.Items.Add(type).SubItems.AddRange(r);
            }
            // ADD OS VERSION INFO 01-09-2017
            // ADD MORE WINDOWS INFO 10-24-2017
            // ADD WINDOWS BUILD NUMBER 01-05-2019
            string type2 = "OS";
            string[] os = new string[2];
            os[0] = GetWindowsVersion();
            os[1] = "";
            listView1.Items.Add(type2).SubItems.AddRange(os);

            string type2a = "Version";
            string[] ver = new string[2];
            ver[0] = System.Environment.OSVersion.ToString();
            ver[1] = "";
            listView1.Items.Add(type2a).SubItems.AddRange(ver);

            string type2b = "Release";
            string[] rel = new string[2];
            rel[0] = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
            rel[1] = "";
            listView1.Items.Add(type2b).SubItems.AddRange(rel);

            string type2c = "Build";
            string[] bld = new string[2];
            bld[0] = GetWindowsBuildNumber();
            bld[1] = "";
            listView1.Items.Add(type2c).SubItems.AddRange(bld);



            // ADD PC NAME INFO 01-27-2017
            string type3 = "PC";
            string[] pc = new string[2];
            pc[0] = System.Environment.MachineName;
            pc[1] = "";
            listView1.Items.Add(type3).SubItems.AddRange(pc);

            foreach (ListViewItem LVI in this.listView1.Items)
            {
                LVI.ForeColor = Color.Blue;
                LVI.UseItemStyleForSubItems = false;
                LVI.SubItems[0].ForeColor = Color.Black;
                //LVI.SubItems[0].BackColor = Color.LightSteelBlue;
                LVI.SubItems[1].ForeColor = Color.Navy;
                //LVI.SubItems[1].BackColor = Color.LightSteelBlue;
                LVI.SubItems[2].ForeColor = Color.Blue;
                //LVI.SubItems[2].BackColor = Color.LightSteelBlue;
            }
            this.listView1.Columns[1].Width = -1;
            this.listView1.Columns[2].Width = listView1.Width - (listView1.Columns[0].Width + listView1.Columns[1].Width + 25);
            // OPTION - SetAlternatingColors(listView1, Color.White, Color.LightGoldenrodYellow);
            this.listView1.Refresh();
            return;

        }
        // LOCAL TIMER TICK
        private void timerInfoForm_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        // UPDATE BUTTON
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        /// <summary>
        /// Returns string containing current OS version, or unknown
        /// </summary>
        /// <returns></returns>
        private string GetWindowsVersion()
        {
            string result = "unknown";
            OperatingSystem osInfo = System.Environment.OSVersion;
            Version vs = osInfo.Version;
            if (osInfo.Platform == PlatformID.Win32Windows)
            {
                switch (vs.Minor)
                {
                    case 0:
                        result = "Windows 95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                        {
                            result = "Windows 98SE";
                        }
                        else
                        {
                            result = "Windows 98";
                        }
                        break;
                    case 90:
                        result = "Windows ME";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (osInfo.Platform == PlatformID.Win32NT)
                {
                    switch (vs.Major)
                    {
                        case 3:
                            result = "Windows NT 3.51";
                            break;
                        case 4:
                            result = "Windows NT 4.0";
                            break;
                        case 5:
                            if (vs.Minor == 0)
                            {
                                result = "Windows 2000";
                            }
                            else
                            {
                                result = "Windows XP";
                            }
                            break;
                        case 6:
                            if (vs.Minor == 0)
                            {
                                result = "Windows Vista";
                            }
                            else
                            {
                                if (vs.Minor == 1)
                                {
                                    result = "Windows 7";
                                    if (is64BitOperatingSystem)
                                    {
                                        result += " 64 Bit";
                                    }
                                    else
                                    {
                                        result += " 32 Bit";
                                    }
                                }
                                else
                                {
                                    if (vs.Minor == 2)
                                    {
                                        result = "Windows 8";
                                        if (IsCurrentOSContains("Windows 10"))
                                        {
                                            result = "Windows 10";
                                        }

                                        if (is64BitOperatingSystem)
                                        {
                                            result += " (64-bit) ";
                                        }
                                        else
                                        {
                                            result += " (32-bit)";
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }


            if (result != "unknown" && osInfo.ServicePack.ToString() != "")
            {
                result = result + " " + osInfo.ServicePack.ToString();
            }

            return result;


        }
        // GET FREE MEMORY
        private double FreeRAM()
        {
            double result;
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            result = (ramCounter.NextValue() / 1024); //.ToString("F1")+" GB";
            return result;
        }
        // GET TOTAL RAM
        private double TotalRAM()
        {
            // ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            double memory = 0;
            foreach (ManagementObject item in searcher.Get())
            {
                memory = double.Parse(item["TotalPhysicalMemory"].ToString());
            }
            memory = memory / 1048576;
            memory = memory / 1024;
            return memory; //.ToString("F1")+" GB";
        }
        // GET OVERALL MEMORY USAGE
        private void GetMemory(ref RamData r)
        {
            double total = TotalRAM();
            double free = FreeRAM();
            double used = (total-free)/total;
            r.FreeMemory = free.ToString("F1") + " GB";
            r.TotalMemory = total.ToString("F1") + " GB";
            r.PercentUsed = used.ToString("P1");
            return;
                
        }
        // GET CURRENT SCREEN RESOLUTION
        private string CurrentPrimaryDisplayResolution()
        {
            float screenWidth = Screen.PrimaryScreen.Bounds.Width;
            float screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // CORRECT FOR DPI SCALING
            // CREDIT : https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            screenHeight = (int)(screenHeight * ScreenScalingFactor);
            screenWidth = (int)(screenWidth * ScreenScalingFactor);

            string result = screenWidth.ToString()+ " x " + screenHeight.ToString();
            return result;
        }
        // GET WINDOWS BUILD NUMBER
        private string GetWindowsBuildNumber()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string part2 = registryKey.GetValue("UBR").ToString();
            string part1 = registryKey.GetValue("CurrentBuild").ToString();
            return part1 + "." + part2;
            
        }
       
    }
}
