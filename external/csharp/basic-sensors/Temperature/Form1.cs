using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Temperature
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            bw.DoWork += new DoWorkEventHandler(bwDoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwCompleted);
            bw.WorkerSupportsCancellation = true;

        }
        // POSITION WINDOW IN RLQ
        protected override void OnLoad(EventArgs e)
        {
            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
            base.OnLoad(e);
        }
        // REPOSITION WINDOW AS NEEDED
        private void ResetWindow()
        {
            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
        }
        // LOCAL VARIABLES AND CLASSES
        private static string CurrentTemp = String.Empty;
        private static bool TempUpdated = false;
        private string HighestTempAsString = String.Empty;
        private OHData od = new OHData();
        private List<tempreading> Temps = new List<tempreading>();
        
        // THREADING
        private BackgroundWorker bw = new BackgroundWorker();
        private void bwDoWork(object sender, DoWorkEventArgs e)
        {
            
            BackgroundWorker worker = sender as BackgroundWorker;
            TempUpdated = false;
            CurrentTemp = CPUTempFromOH();
 
        }
        private void bwCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                TempUpdated = true;
            }
            else
            {
                TempUpdated = false;
            }
        }

        // FORM LOAD
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettingsFile();
            timer1.Interval = 60000; // Update rate 1/min
            timer1.Start();
            updateToolStripMenuItem_Click(sender, e);
            if (globals.LogTemps)
            {
                pbLog.Visible = true;
                globals.LogStartTime = DateTime.Now;
            }
            else
            {
                pbLog.Visible = false;
            }
            
        }
        // FORM CLICKED
        private void MainForm_Click(object sender, EventArgs e)
        {
            Point p = new Point(this.Left, this.Bottom- (this.Height));
            this.contextMenuStrip1.Show(p);
        }
        // CONVERT TEMP STRING TO AN INTEGER VALUE
        private int GetNumericTemp(string source)
        {
            int x;
            char ch;
            int result = 0;
            string output = String.Empty;
            for (x=0;x<source.Length;x++)
            {
                ch = source[x];
                if (ch != '.')
                {
                    output += ch;
                    continue;
                }
                break;
            }
            try
            {
               result = Convert.ToInt16(output);
               return result;
            }
            catch
            {
                result = 0;
                return result;
            }
           
            

        }
        // TIMER TICK
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
            string s;
            if (TempUpdated)
            {
                s = CurrentTemp;
            }
            else
            {
                return;
            }
            UpdateTempLabel(s);
            if (globals.LogTemps)
            {
                tempreading tr = new tempreading();
                tr.Dtime = DateTime.Now;
                tr.Temperature = Convert.ToDouble(s.Split(' ')[0]);
                Temps.Add(tr);
            }
        }
        // UPDATE TEMPERATURE LABEL
        private void UpdateTempLabel(string temp)
        {
            int newtemp = GetNumericTemp(temp);
            int oldtemp = GetNumericTemp(HighestTempAsString);
            if (newtemp >= oldtemp)
            {
                HighestTempAsString = temp;
            }
            label1.ForeColor = Color.Lime;
            if (newtemp > globals.RedAlertThreshold)
            {
                label1.ForeColor = Color.Red;
            }
            else
            {
                if (newtemp > globals.YellowAlertThreshold)
                {
                    label1.ForeColor = Color.Yellow;
                }
            }
            
            label1.Text = temp;

            this.Text = label1.Text;
            this.maxTempToolStripMenuItem.Text = "Max Temp = " + HighestTempAsString;
            UpdateLogIcon();
        }
        // EXTRACT CPU TEMP FROM OHData INSTANCE
        // REV 10-30-2017 Averages all available CPU Temperatures, both Core and Package
        private string CPUTempFromOH()
        {
            
            List<string> templist = new List<string>();
            string result = String.Empty;
            od.Update();
            int count = od.DataList.Count();
            int x;
            float temp = 0;
            // Build List of Temps
            for (x=0;x<count;x++)
            {
                if ((od.DataList[x].type == "Temperature: ") && (od.DataList[x].name.Contains("CPU")))
                {
                    result = od.DataList[x].reading.Split(' ')[0];
                    templist.Add(result);
                }
            }
            // Convert them back into floats and average
            foreach (string s in templist)
            {
                temp += (float)Convert.ToDouble(s);
            }
            temp = temp / templist.Count();
            //Convert numeric average back into string
            result = temp.ToString("F1") + " F";
            return result;
            
        }
        // UNUSED
        private string CPUfanRPM()
        {
            string result = String.Empty;
            
            return result;

        }
        // MENU QUIT
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            this.Close();
        }
        // INFO MENU ITEM
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //timer1.Stop();
            // DEBUGGING 04062015
       
            Form2 f2 = new Form2((this.timer1.Interval/1000).ToString()+" sec");
            f2.Show();
            //timer1.Start();

        }
        // UPDATE MENU ITEM
        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s;
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
            label1.Text = ""; // BLANK TO SHOW ITS UPDATING
            s = CPUTempFromOH();
            UpdateTempLabel(s);

        }
        // 60 SECOND MENU ITEM
        private void secToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 60000;
            secToolStripMenuItem.Checked = true;
            secToolStripMenuItem1.Checked = false;
            secToolStripMenuItem2.Checked = false;
            
        }
        // 30 SECOND TIMER INTERVAL
        private void secToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 30000;
            secToolStripMenuItem1.Checked = true;
            secToolStripMenuItem.Checked = false;
            secToolStripMenuItem2.Checked = false;
        }
        // 1 SECOND TIMER INTERVAL
        private void secToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.timer1.Interval = 1000;
            secToolStripMenuItem2.Checked = true;
            secToolStripMenuItem1.Checked = false;
            secToolStripMenuItem.Checked = false;
            
        }
        // for future developement BIOS ID
        // UNUSED - For debugging
        private List<string> BiosInfo()
        {
            
            
            List<string> results = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"\\.\root\cimv2","SELECT * FROM Win32_BIOS");
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject o in collection )
            {
                results.Add(Convert.ToString(o.GetPropertyValue("Name")));
                results.Add(Convert.ToString(o.GetPropertyValue("ReleaseDate")));
                results.Add(Convert.ToString(o.GetPropertyValue("Caption")));
                results.Add(Convert.ToString(o.GetPropertyValue("Description")));
                results.Add(Convert.ToString(o.GetPropertyValue("Manufacturer")));
                results.Add(Convert.ToString(o.GetPropertyValue("SMBIOSBIOSVersion")));

            }
            searcher.Dispose();
            return results;

        }
        // RESET WINDOW MENU ITEM
        private void resetWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetWindow();
            return;
        }
        // OPTIONS CONTEXT MENU
        private void optionsToolStripMenuItem_Click(object sender,EventArgs e)
        {
            SetOpacity handler = SetWindowOpacity;
            SettingsForm SF = new SettingsForm(globals.YellowAlertThreshold,globals.RedAlertThreshold,globals.WindowOpacity,handler, globals.LogTemps);
            SF.ShowDialog(this);
            if (SF.Apply)
            {
                globals.WindowOpacity = SF.OpacityValue;
                globals.YellowAlertThreshold = SF.YellowAlertvalue;
                globals.RedAlertThreshold = SF.RedAlertValue;
                if (globals.LogTemps && !SF.LogTemps)       // LOG WAS ON, NOW OFF
                {
                    Temps.Clear();  //Logging turned off, clear data
                }
                else
                {
                    if (SF.LogTemps && !globals.LogTemps)
                    {
                        Temps.Clear();                          // should be clear already
                        globals.LogStartTime = DateTime.Now;    //reset start time
                    }
                }
                globals.LogTemps = SF.LogTemps;
                SaveSettingsFile();
                
            }
            SetWindowOpacity(globals.WindowOpacity);
            
            UpdateLogIcon();
            if (SF.Apply)
            {
                updateToolStripMenuItem_Click(sender, EventArgs.Empty);
            }
            SF.Dispose();
           
        }
        // SETWINDOWOPACITY HANDLER
        private void SetWindowOpacity(double wopacity)
        {
            this.Opacity = wopacity;
        }
        // SAVE SETTINGS FILE
        private bool SaveSettingsFile()
        {
            
            settings s = new settings();
            s.yellowalert = globals.YellowAlertThreshold;
            s.redalert = globals.RedAlertThreshold;
            s.opacity = globals.WindowOpacity;
            s.logtemps = globals.LogTemps;
           
            string filename = globals.SettingsFilePath + "\\" + globals.SettingsFileName;
            bool result = false;
            BinaryFormatter Formatter = new BinaryFormatter();
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            try
            {

                using (BinaryWriter writer = new BinaryWriter(fs))
                {

                    // HEADER
                    writer.Write(globals.SettingsFileHeader);

                    // REPORTS
                    Formatter.Serialize(fs, s);
                    result = true;
                    //DT.NotifyDialog(this, "Saving User Settings...");
                }
            }
            catch (Exception Ex)
            {
                // EHT.GeneralExceptionHandler("Error Saving Settings File", "Save Settings", false, Ex);
                // ADD EXCEPTION NOTIFIER HERE IF DESIRED

            }
            finally
            {
                fs.Close();
                Formatter = null;
            }
            return result;
        }
        // LOAD SETTINGS FILE
        private bool LoadSettingsFile()
        {
            bool result = false;
            globals.SettingsFilePath = Directory.GetCurrentDirectory();
            string filename = globals.SettingsFilePath + "\\" + globals.SettingsFileName;
            if (File.Exists(filename))
            {
                settings s = new settings();
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                BinaryFormatter Formatter = new BinaryFormatter();
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    try
                    {
                        // READER HEADER
                        string Header = reader.ReadString();
                        if (Header == globals.SettingsFileHeader) /* check for correct header string */
                        {

                            // READ DATAFILE


                           
                            s = (settings)Formatter.Deserialize(fs);
                            globals.YellowAlertThreshold = s.yellowalert;
                            globals.RedAlertThreshold = s.redalert;
                            globals.WindowOpacity = s.opacity;
                            globals.LogTemps = s.logtemps;
                                    //SAVE ORIGINATING DIRECTORY FOR APP AND SETTINGS FILE
                            result = true;



                        }
                        else
                        {
                            // EHT.GeneralExceptionHandler("Incorrect File Format", "Load Settings File", true, BadFileFormat);  // REV 09-18-18 FAIL IF CFG FILE INCORRECT
                            result = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //EHT.GeneralExceptionHandler("Unable to Open Settings File", "Load Settings File", false, ex);
                        result = false;

                    }
                    finally
                    {
                        fs.Close();
                        Formatter = null;
                    }
                }
            }
            return result;
        }
        // GRAPH CONTEXT MENU ITEM
        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            ChartForm CF = new ChartForm(ref Temps);
            CF.ShowDialog(this);
            if (CF.ClearHistory)
            {
                Temps.Clear();
                globals.LogStartTime = DateTime.Now;    //Update start time after clear
            }
            CF.Dispose();
            //timer1_Tick(sender, EventArgs.Empty);
            timer1.Start();
        }
        // UPDATE LOG TEMPS ICON ON WINDOWS
        private void UpdateLogIcon()
        {
            if (globals.LogTemps)
            {
                pbLog.Visible = true;
                graphToolStripMenuItem.Enabled = true;
            }
            else
            {
                pbLog.Visible = false;
                graphToolStripMenuItem.Enabled = false;
            }
        }
    }
}
