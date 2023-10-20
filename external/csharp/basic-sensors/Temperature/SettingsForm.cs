using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Temperature
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        // OVERLOAD 1
        public SettingsForm(int YellowAlert,int RedAlert,Double Opacity, SetOpacity s,bool LogTemperatures)
        {
            InitializeComponent();
            _yellowalert = YellowAlert;
            _redalert = RedAlert;
            _opacity = Opacity;
            SO = s;
            _logtemps = LogTemperatures;
        }
        public int YellowAlertvalue
        {
            get
            {
                return _yellowalert;
            }
        }
        public int RedAlertValue
        {
            get
            {
                return _redalert;
            }
        }
        public double OpacityValue
        {
            get
            {
                return _opacity;
            }
        }
        public bool Apply
        {
            get
            {
                return _applyclicked;
            }
        }
        public bool LogTemps
        {
            get
            {
                return _logtemps;
            }
        }
        private int _yellowalert;
        private int _redalert;
        private double _opacity = 1;
        private bool _logtemps;
        private SetOpacity SO;
        private bool _applyclicked = false;
        // TRACK BAR SCROLL HANDLER
        private void trbTransparancy_Scroll(object sender, EventArgs e)
        {
            this.lblTrackBarCurrentValue.Text = trbTransparancy.Value.ToString();
            _opacity = Convert.ToDouble(trbTransparancy.Value);     // NOTE: DON'T DIVIDE VALUE/100. VALUE IS AN INT AND INT/100 = 0. ONLY DIVIDE THE DOUBLE BY 100.
            _opacity = _opacity / 100;                              // DIVIDE HERE NOT ABOVE
            return;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // FORM LOAD
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.nudRedAlert.Value = _redalert;
            this.nudYellowAlert.Value = _yellowalert;
            this.trbTransparancy.Value = Convert.ToInt16(_opacity * 100);
            this.lblTrackBarCurrentValue.Text = trbTransparancy.Value.ToString();
            this.cbLogTemps.Checked = _logtemps;
            

        }
        // PREVIEW BUTTON HANDLER
        private void btnPreview_Click(object sender, EventArgs e)
        {
            SO(_opacity);   // DELEGATE CALL-BACK MAIN WINDOW
        }
        // APPLY BUTTON HANDLER
        private void btnApply_Click(object sender, EventArgs e)
        {
            _applyclicked = true;
            _yellowalert =(int) nudYellowAlert.Value;
            _redalert = (int)nudRedAlert.Value;
            this.Close();
        }
        // RESTORE DEFAULT VALUES BUTTON HANDLER
        private void btnRestoreDef_Click(object sender, EventArgs e)
        {
            this.nudRedAlert.Value = globals.DefaultRedAlertThreshold;
            this.nudYellowAlert.Value = globals.DefaultYellowAlertThreshold;
            this._opacity = globals.DefaultWindowOpacity;
            this.trbTransparancy.Value = Convert.ToInt16(_opacity * 100);
            this.lblTrackBarCurrentValue.Text = trbTransparancy.Value.ToString();
            this.cbLogTemps.Checked = globals.DefaultLogTemps;
        }
        // CB LOG TEMPS CHECK CHANGED HANDLER
        private void cbLogTemps_CheckedChanged(object sender, EventArgs e)
        {
            _logtemps = cbLogTemps.Checked;
           
        }
    }
}
