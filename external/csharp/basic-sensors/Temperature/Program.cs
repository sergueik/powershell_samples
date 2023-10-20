using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;

namespace Temperature
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // EMBEDDED DLL HANDLER tested OK 01-15-2014
            // Must run in Program Class (where exception occurs
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.Run(new MainForm());
        }
        // EMBEDDED DLL LOADER 
        // VERSION 2.0 01-15-2014 derives resourcename from args and application namespace
        // assumes resource is a DLL
        // this should load any missing DLL that is properly embedded
        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string appname = Application.ProductName + "."; // gets Application Namespace
            string[] dll = args.Name.ToString().Split(','); // separates args.Name string
            string resourcename = appname + dll[0] + ".dll"; // element [0] contains the missing resource name
            Assembly MyAssembly = Assembly.GetExecutingAssembly();
            Stream AssemblyStream = MyAssembly.GetManifestResourceStream(resourcename);
            byte[] raw = new byte[AssemblyStream.Length];
            AssemblyStream.Read(raw, 0, raw.Length);
            return Assembly.Load(raw);
        }
    }
    public delegate void SetOpacity(double opacity);

    public class RamData
    {
        public string TotalMemory;
        public string FreeMemory;
        public string PercentUsed;
    }
    /// <summary>
    /// Wrapper For OpenHarwareMonitor.dll
    /// </summary>
    public class OHData
    {
        // DATA ACCESSOR
        public List<OHMitem> DataList
        {
            get
            {
                return ReportItems;
            }
            set
            {

            }

        }
        
        // UPDATE METHOD
        public void Update()
        {
            UpdateOHM();
        }

        // for report compilation
        public class OHMitem
        {
            public OHMitem()
            {
            }
            public string name
            {
                get
                {
                    return Name;
                }
                set
                {
                    Name = value;
                }
            }
            public string type
            {
                get
                {
                    return OHMType;
                }
                set
                {
                    OHMType = value;
                }
            }
            public string reading
            {
                get
                {
                    return OHMValue;
                }
                set
                {
                    OHMValue = value;
                }
            }

            private string Name = String.Empty;
            private string OHMType = String.Empty;
            private string OHMValue = String.Empty;

        }
        // for report compilation
        private List<OHMitem> ReportItems = new List<OHMitem>();
        
        // ADDS ITEMS TO REPORT
        private void AddReportItem(string ARIName, string ARIType, string ARIValue)
        {
            // int readingwidth = 26;

            // REV 10-27-2017 Exclude Data & Level entries missing values
            // Further work required regarding additional data from OHMonitor
            // Don't remove incomplete Memory Entries since these are updated automatically in Form2.cs
            
            if ((ARIType == "Data" && ARIValue == "" && !ARIName.Contains("Memory")) || (ARIType == "Level" && ARIValue == ""))
            {
                return;
            }

            // END REV

            OHMitem ARItem = new OHMitem();
            ARItem.name = ARIName;
            ARItem.type = ARIType + ": ";
            ARItem.reading = ARIValue;
            if (ARIType == "GpuAti")
            {
                ARItem.type = "Graphics Card";
            }

            if (ARIType == "Temperature")
            {
                try
                {
                    double temp = Convert.ToDouble(ARIValue);
                   // 01-26-2017 ARItem.reading = ((((9.0 / 5.0) * temp) + 32).ToString("000.0") + " F");
                    ARItem.reading = ((((9.0 / 5.0) * temp) + 32).ToString("F1") + " F");
                }
                catch
                {
                    
                    return;
                }
            }
            if (ARIType == "Clock")
            {
                try
                {
                    double temp = Convert.ToDouble(ARIValue);
                    if (temp < 1000)
                    {
                        ARItem.reading = (temp.ToString("F1") + " MHZ");
                    }
                    else
                    {
                        temp = temp / 1000;
                        ARItem.reading = (temp.ToString("F1") + " GHZ");
                    }
                }
                catch
                {
                    return;
                }

            }
            if (ARIType == "Control" || ARIType == "Load")
            {
                try
                {
                    double temp = Convert.ToDouble(ARIValue);
                    ARItem.name = ARIName;
                    ARItem.reading = (temp.ToString("F1") + " %"); // REV 10-30-2017 F0->F1
                }
                catch
                {
                    return;
                }
            }
            if (ARIType == "Voltage")
            {
                try
                {
                    double temp = Convert.ToDouble(ARIValue);
                    ARItem.name = ARIName;
                    ARItem.reading = (temp.ToString("F1") + " V");
                }
                catch
                {
                    return;
                }
            }
            // 07-28-2016 Added This item
            if (ARIType == "Fan")
            {
                try
                {
                    double rpm = Convert.ToDouble(ARIValue);
                    ARItem.name = ARIName;
                    ARItem.reading = (rpm.ToString("F0") + " RPM");
                }
                catch
                {
                    return;
                }
            }
            // 01-27-2016 Added Item
            if (ARIType == "Power")
            {
                try
                {
                    double watts = Convert.ToDouble(ARIValue);
                    ARItem.name = ARIName;
                    ARItem.reading = (watts.ToString("F1") + " W");
                }
                catch
                {
                    return;
                }
            }



            ReportItems.Add(ARItem);
        }
        // LOCAL INSTANCE OHM
        private OpenHardwareMonitor.Hardware.Computer computerHardware = new OpenHardwareMonitor.Hardware.Computer();
        // UPDATE OHM DATA
        private void UpdateOHM()
        {
            
            string s = String.Empty;
            string name = string.Empty;
            string type = string.Empty;
            string value = string.Empty;
            int x, y, z, yy,zz;
            int hardwareCount;
            int subcount;
            int sensorcount;
            int moresubhardwarecount;
            int moresensorcount;
            ReportItems.Clear();
            computerHardware.MainboardEnabled = true;
            computerHardware.FanControllerEnabled = true;
            computerHardware.CPUEnabled = true;
            computerHardware.GPUEnabled = true;
            computerHardware.RAMEnabled = true;
            computerHardware.HDDEnabled = true;
            computerHardware.Open();
            hardwareCount = computerHardware.Hardware.Count();
            for (x = 0; x < hardwareCount; x++)
            {
                name = computerHardware.Hardware[x].Name;
                type = computerHardware.Hardware[x].HardwareType.ToString();
                value = ""; // no value for non-sensors;
                AddReportItem(name, type, value);
                subcount = computerHardware.Hardware[x].SubHardware.Count();

                // ADDED 07-28-2016
                // NEED Update to view Subhardware
                for (y = 0; y < subcount; y++)
                {
                    computerHardware.Hardware[x].SubHardware[y].Update();
                    if (computerHardware.Hardware[x].SubHardware[y].SubHardware.Count() > 0)
                    {
                        yy = computerHardware.Hardware[x].SubHardware[y].SubHardware.Count();
                        for (zz=0;zz<yy;zz++)
                        {
                            computerHardware.Hardware[x].SubHardware[y].SubHardware[zz].Update();

                        }
                    }

                }
               
                if (subcount > 0)
                {
                    for (y = 0; y < subcount; y++)
                    {
                        sensorcount = computerHardware.Hardware[x].SubHardware[y].Sensors.Count();
                        // REV 08-06-2016
                        moresubhardwarecount = computerHardware.Hardware[x].SubHardware[y].SubHardware.Count();
                        // END REV
                        name = computerHardware.Hardware[x].SubHardware[y].Name;
                        type = computerHardware.Hardware[x].SubHardware[y].HardwareType.ToString();
                        value = "";
                        AddReportItem(name, type, value);

                        if (sensorcount > 0)
                        {
                            
                            for (z = 0; z < sensorcount; z++)
                            {
                                
                                name = computerHardware.Hardware[x].SubHardware[y].Sensors[z].Name;
                                type = computerHardware.Hardware[x].SubHardware[y].Sensors[z].SensorType.ToString();
                                value = computerHardware.Hardware[x].SubHardware[y].Sensors[z].Value.ToString();
                                AddReportItem(name, type, value);
                                
                            }
                        }
                        // REV 08-06-2016
                        for (yy=0;yy < moresubhardwarecount; yy++)
                        {
                            computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Update();
                            moresensorcount = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Sensors.Count();
                            name = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Name;
                            type = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].HardwareType.ToString();
                            value = "";
                            AddReportItem(name, type, value);

                            if (sensorcount > 0)
                            {

                                for (zz = 0; zz < sensorcount; zz++)
                                {

                                    name = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Sensors[zz].Name;
                                    type = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Sensors[zz].SensorType.ToString();
                                    value = computerHardware.Hardware[x].SubHardware[y].SubHardware[yy].Sensors[zz].Value.ToString();
                                    AddReportItem(name, type, value);

                                }
                            }

                        }
                        // END REV
                    }
                    
                }
                
                sensorcount = computerHardware.Hardware[x].Sensors.Count();
                
                if (sensorcount > 0)
                {
                    for (z = 0; z < sensorcount; z++)
                    {
                        
                        name = computerHardware.Hardware[x].Sensors[z].Name;
                        type = computerHardware.Hardware[x].Sensors[z].SensorType.ToString();
                        value = computerHardware.Hardware[x].Sensors[z].Value.ToString();
                        AddReportItem(name, type, value);
                        
                    }
                }

            }
          
           computerHardware.Close();
        }
    }
    /// <summary>
    /// Wrapper for BIOS Information from Win_32BIOS WMI
    /// </summary>
    public class WMIBIOS
    {
        public string Name
        {
            get
            {
                return name;
            }
           
        }
        public string Manufacturer
        {
            get
            {
                return manufacturer;
            }
        }
        public string Date
        {
            get
            {
                return FormatDate(date);
            }

        }
        public string Version
        {
            get
            {
                return version;
            }

        }
        public void Update()
        {
            update();
        }
        private string name = String.Empty;
        private string manufacturer = String.Empty;
        private string date = String.Empty;
        private string version = String.Empty;
        // Get BIOS Data using WMI
        private void update()
        {
            
            
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"\\.\root\cimv2", "SELECT * FROM Win32_BIOS");
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject o in collection)
            {
                try
                {
                    name = Convert.ToString(o.GetPropertyValue("Name"));
                }
                catch
                {
                    name = String.Empty;
                }
                try
                {
                    date = Convert.ToString(o.GetPropertyValue("ReleaseDate"));
                }
                catch
                {
                    date = String.Empty;
                }
                try
                {
                    manufacturer = Convert.ToString(o.GetPropertyValue("Manufacturer"));
                }
                catch
                {
                    manufacturer = String.Empty;
                }
                try
                {
                    version = Convert.ToString(o.GetPropertyValue("SMBIOSBIOSVersion"));
                }
                catch
                {
                    version = String.Empty;
                }

            }
            searcher.Dispose();
            return;
        }
        // FORMAT DATE FROM WIN_32 BIOS INTO USABLE FORM
        private string FormatDate(string rawdata)
        {
            string result = String.Empty;
            string year = String.Empty;
            string month = String.Empty;
            string day = String.Empty;
            try
            {
                year = rawdata.Substring(0, 4);
                month = rawdata.Substring(4, 2);
                day = rawdata.Substring(6, 2);
            }
            catch
            {
                return result;
            }
            result = month + "-" + day + "-" + year;
            return result;


        }

    }
    public class tempreading
    {
        public DateTime Dtime;
        public double Temperature;
    }
    /// <summary>
    /// GLOBAL VARIABLES
    /// </summary>
    public class globals
    {
        public static int YellowAlertThreshold = 120;
        public static int RedAlertThreshold = 150;
        public static Double WindowOpacity = 0.75;
        public const int DefaultYellowAlertThreshold = 120;
        public const int DefaultRedAlertThreshold = 150;
        public const double DefaultWindowOpacity = 0.75;
        public static bool LogTemps = false;
        public const bool DefaultLogTemps = false;
        public static string SettingsFileHeader = "TEMPERATURE EXE SETTINGS VER 1.0";
        public static string SettingsFileName = "tempappsettings.cfg";
        public static string SettingsFilePath = String.Empty;
        public static DateTime LogStartTime;
    }
    [Serializable]
    public class settings
    {
        public int yellowalert;
        public int redalert;
        public double opacity;
        public bool logtemps;
    }


}
        
    


        
    
