using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Temperature
{
    public partial class ChartForm : Form
    {
        public ChartForm()
        {
            InitializeComponent();
        }
        public ChartForm(ref List<tempreading> Readings)
        {
            InitializeComponent();
            foreach (tempreading t in Readings)
            {
                tempreading tr = new tempreading();
                tr.Temperature = t.Temperature;
                tr.Dtime = t.Dtime;
                readings.Add(tr);
            }
            
        }
        public bool ClearHistory
        {
            get
            {
                return _clearhistory;
            }
        }
        private List<tempreading> readings = new List<tempreading>();
        private Series S = new Series();
        private double maxtemp = 100, mintemp = 50;
        private bool _clearhistory = false;
        
        // FORM LOAD
        private void ChartForm_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            CreateSeries();
            //S.LegendText = "CPU Temp";
            chart1.Series.Add(S);
            //chart1.Series[0].Color = Color.Red;
            chart1.Series[0].ToolTip = "T = #VALY";
            lblCountPoints.Text = S.Points.Count.ToString() + " Data Points - Log Started: " + globals.LogStartTime.ToLongTimeString();
            lblRThreshold.Text = "Red Alert Threshold    = " + globals.RedAlertThreshold.ToString() + " F";
            lblYThreshold.Text = "Yellow Alert Threshold = " + globals.YellowAlertThreshold.ToString() + " F";
        }
        // CLOSE BUTTON
        private void btnClose_Click(object sender, EventArgs e)
        {
            S.Dispose();
            this.Close();
        }
        // CLEAR HISTORY BUTTON
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            _clearhistory = true;
            this.Close();
        }

        // CREATE SERIES
        private void CreateSeries()
        {
            int x = 0;
            foreach (tempreading t in readings)
            {
               
                // DataPoint p = new DataPoint(Convert.ToSingle(t.Dtime.ToOADate()), t.Temperature);
                DataPoint p = new DataPoint(x, t.Temperature);
                double tx = Convert.ToDouble(t.Temperature);
                p.Color = Color.Lime;
                if (tx > globals.RedAlertThreshold)
                {
                    p.Color = Color.Red;
                }
                else
                {
                    if (tx > globals.YellowAlertThreshold)
                    {
                        p.Color = Color.Yellow;
                    }
                }
                S.Points.Add(p);
                if (t.Temperature < mintemp)
                {
                    mintemp = t.Temperature;
                }
                if (t.Temperature > maxtemp)
                {
                    maxtemp = t.Temperature;
                }
                x++;
            }
            S.ChartType = SeriesChartType.Line;
            S.MarkerStyle = MarkerStyle.Cross;
            S.IsValueShownAsLabel = false;      // IF ON, LABLES EACH POINT
            S.IsVisibleInLegend = false;        //TURN OFF SERIES NAME LABEL
            S.BorderWidth = 2;                  // LINE THICKNESS
            S.LabelBorderDashStyle = ChartDashStyle.Solid;
          
            //S.Color = Color.Red;
            chart1.ChartAreas[0].AxisY.Minimum = mintemp-10;
            chart1.ChartAreas[0].AxisY.Maximum = maxtemp+10; //10% above Maximuma
            chart1.ChartAreas[0].AxisY.Interval = 10;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Title = "Time";
            this.chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Near;
            this.chart1.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
            this.chart1.ChartAreas[0].AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            
            chart1.ChartAreas[0].AxisY.Title = "Deg F";
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "{D1}";
            chart1.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            chart1.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
            chart1.ChartAreas[0].BackColor = Color.DarkSlateBlue;
            
        }
    }
}
