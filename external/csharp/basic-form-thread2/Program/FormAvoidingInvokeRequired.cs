using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UIThread35Desktop
{
    public partial class FormAvoidingInvokeRequired : Form
    {
        public FormAvoidingInvokeRequired()
        {
            InitializeComponent();
        }

        private void MultiParamsUnsafe(string text, int number, DateTime dateTime)
        {
            this.textBoxOut.Text = "string = " + text + Environment.NewLine +
                "number = " + number + Environment.NewLine + "datetime = " + dateTime;
        }

        #region No pattern, will crash if running thread != UI thread

        private void buttonSingleNone_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            SetTextUnsafe();
        }

        private void buttonMultiNone_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            new Thread(new ThreadStart( SetTextUnsafe)).Start();
        }

        private void SetTextUnsafe()
        {            
            try
            {                
                textBoxOut.Text = "No pattern was used. Will fail if multithread";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void buttonMultiNoneParams_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            new Thread(new ThreadStart(SetTextUnsafeParams)).Start();
        }

        private void SetTextUnsafeParams()
        {
            try
            {
                MultiParamsUnsafe("No pattern was used. Will fail if multithread", 2, DateTime.Now);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            } 
        }


        #endregion

        #region Standard pattern

        private void buttonSingleStandard_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            SetTextStandardPattern();
        }


        private void buttonMultiStandard_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            (new Thread(new ThreadStart(SetTextStandardPattern))).Start();
        }

        public delegate void DelegateStandardPattern();
        private void SetTextStandardPattern()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new DelegateStandardPattern(SetTextStandardPattern));
                return;
            }
            textBoxOut.Text = "Standard pattern was used. It works, but its really awfull";
        }

        private void buttonMultiStandardParams_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            (new Thread(delegate() { SetTextStandardPatternParams("StandardPattern with multiple params", 1, DateTime.Now); })).Start();
        }

        public delegate void DelegateStandardPatternParams(string text, int number, DateTime datetime);

        private void SetTextStandardPatternParams(string text, int number, DateTime datetime)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new DelegateStandardPatternParams(SetTextStandardPatternParams), text, number, datetime);
                return;
            }
            MultiParamsUnsafe(text, number, datetime);
        }

        #endregion

        #region UIThread pattern

        private void SingleUIThread_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            SetTextUIThreadPattern();
        }

        private void buttonMultiUIThread_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            (new Thread(delegate() { SetTextUIThreadPattern(); })).Start();
        }

        private void SetTextUIThreadPattern()
        {
            this.UIThread(delegate
            {
                textBoxOut.Text = "UIThread pattern was used";
            });
        }  

        private void buttonMultiUIThreadParams_Click(object sender, EventArgs e)
        {
            textBoxOut.Clear();
            (new Thread(delegate() { SetTextUIThreadPatternParams("UIThread pattern with multiple params", 4, DateTime.Now); })).Start();            
        }

        private void SetTextUIThreadPatternParams(string text, int number, DateTime dateTime)
        {
            this.UIThread(delegate
            {
                MultiParamsUnsafe(text, number, dateTime);
            });
        }






        #endregion


    











    }
}
