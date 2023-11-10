using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication2 {
    public partial class DBIns : Form {
        public DBIns() {
            InitializeComponent();
            panel2.Enabled = false;
            panel2.Visible = false;
            btnBack.Enabled = false;
        }
        /// <summary>
        /// subscribe to the event, so we can display the executed sql statements. 
        /// </summary>
        /// <param name="m"></param>
        public void Subscribe(NpgsqlRtns m)
        {
            m.Tick += new NpgsqlRtns.TickHandler(HeardIt);
        }
        /// <summary>
        /// display the executed sql statements
        /// </summary>
        /// <param name="m">the class that holds the sql statement that was executed</param>
        /// <param name="e">the event argument</param>
        private void HeardIt(NpgsqlRtns m, EventArgs e)
        {
            textBox1.AppendText(m.Result + "\r\n");
            Thread.Sleep(10);
            for (int i = 0; i < 1000; i++)
            {
                Application.DoEvents();
            }
            Thread.Sleep(10);
        }
        /// <summary>
        /// click the back button. determine what to do. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (panel1.Enabled != true)
            {
                panel2.Enabled = false;
                panel2.Visible = false;
                panel1.Enabled = true;
                panel1.Visible = true;
                btnNext.Text = "Next ->";   // change the button to read next. 
                btnBack.Enabled = false;
            }
        }
        /// <summary>
        /// click the next button. determine what to do. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            // if panel 1 is enabled and a next button was rpessed we
            // want to display panel2 and allow the user to rpess the start button. 
            if (panel1.Enabled == true)
            {
                // Check to see if the data was entered before we proceeed. 
                if (txtServer.Text.Length == 0)
                {
                    errorProvider1.SetError(txtServer, "Please enter a valid server name");
                    return;
                }
                // Check to see if the data was entered before we proceeed. 
                if (txtDBName.Text.Length == 0)
                {
                    errorProvider1.SetError(txtDBName, "Please enter a valid database name");
                    return;
                }
                // Check to see if the data was entered before we proceeed. 
                if (txtUser.Text.Length == 0)
                {
                    errorProvider1.SetError(txtUser, "Please enter a valid User name");
                    return;
                }
                // Check to see if the data was entered before we proceeed. 
                if (txtPassword.Text.Length == 0)
                {
                    errorProvider1.SetError(txtPassword, "Please enter a valid password");
                    return;
                }
                panel1.Enabled = false;
                panel1.Visible = false;
                panel2.Enabled = true;
                panel2.Visible = true;
            
                
                btnNext.Text = "Start";
                btnBack.Enabled = true;
                panel2.Visible = true;
                textBox1.TabIndex = 1;
                textBox1.Focus();
                btnBack.TabIndex = 2;
                btnNext.TabIndex = 3;
            }
            else
            {
                NpgsqlRtns cl1;
                bool    bCreateDB;
                string strDBName;
                string strServer;
                string strUser;
                string strPassword;

                btnBack.Enabled = false;
                btnNext.Enabled = false;
                cl1 = new NpgsqlRtns();
                bCreateDB = chkBoxCreateDB.Checked;
                strDBName = txtDBName.Text;
                strServer = txtServer.Text;
                strUser = txtUser.Text;
                strPassword = txtPassword.Text;
                Subscribe(cl1);
                cl1.StartConv(bCreateDB, strDBName, strServer, strUser, strPassword);
            }
        }

    }
}
