using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Management;

//////////////////////////////////////////////////////////
//														//
// Author:		Max Methot (maxmethot@gmail.com)		//
// Date	 :		August 27th 2013						//
// Description:	With this program, you will be able to	//
//				print to any local or network printers	//
//				even if the printer is not the default	//
//				printer on the system. You must first	//
//				select one of the local or network		//
//				printers in the list and click the		//
//				Print button to send the job to the		//
//				selected printer.						//
//														//
//////////////////////////////////////////////////////////

namespace PrintToPrinter
{
    public partial class FormPrint : Form
    {
		/// <summary>
		/// Used to associate the name of the printer to print to.
		/// </summary>
        private string printerName;
        private bool defaultNetwork = false;

        public FormPrint()
        {
            InitializeComponent();

			//Before knowing if any printers are available, let's simply diable the print button, just in case.
			btnPrint.Enabled = false;

			//Let's load our list of printers
            LoadPrinters();

            if (defaultNetwork)
                radioNetworkPrinters.Checked = true;
            else
                radioLocalPrinters.Checked = true;
        }

		/// <summary>
		/// Sets a list of printers into the local and network comboboxes by finding them on the system.
		/// </summary>
        private void LoadPrinters()
        {
			//Let's clear our comboboxes to start fresh.
			cboNetworkPrinters.Items.Clear();
			cboLocalPrinters.Items.Clear();

            //We use the ObjectQuery to get the list of configured printers
            System.Management.ObjectQuery oquery = new System.Management.ObjectQuery("SELECT * FROM Win32_Printer");
            System.Management.ManagementObjectSearcher mosearcher = new System.Management.ManagementObjectSearcher(oquery);
            System.Management.ManagementObjectCollection moc = mosearcher.Get();

			//If at least one printer is found, we add it to a specific list and enable the Print button.
			if (moc != null && moc.Count > 0)
			{
				//For each printer found, we put it either in the local or network combobox, accordingly.
				foreach (ManagementObject mo in moc)
				{
                    bool network = false;
                    string[] ipAddress = mo["PortName"].ToString().Split('.');

					//It's a network printer.
                    if (mo["PortName"] != null && ipAddress.Length == 4)
					{
						cboNetworkPrinters.Items.Add(mo["Name"].ToString());
                        network = true;
					}
					//It's a local printer.
					else
					{
						cboLocalPrinters.Items.Add(mo["Name"].ToString());
					}

					//If the printer is found to be the default one, we select it.
					if ((bool)mo["Default"])
					{
                        if (network) {
                            cboNetworkPrinters.SelectedItem = mo["Name"].ToString();
                            defaultNetwork = true;
                        }
                        else {
						    cboLocalPrinters.SelectedItem = mo["Name"].ToString();
                            defaultNetwork = false;
                        }

                        lblNameValue.Text = mo["Name"].ToString();
                        lblPortValue.Text = mo["PortName"].ToString();
                        lblDriverValue.Text = mo["DriverName"].ToString();
                        lblDeviceIDValue.Text = mo["DeviceID"].ToString();
                        lblSharedValue.Text = mo["Shared"].ToString();
					}
				}

				//Now that we know at least one printer exists, we can enable the print button.
				btnPrint.Enabled = true;
			}
        }

		/// <summary>
		/// Event used to send the print job to the printer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
			//If the local printer radio button is checked, we try to associate the selected local printer.
            if (radioLocalPrinters.Checked)
                printerName = cboLocalPrinters.SelectedItem.ToString();
			//If the network printer radio button is checked, we try to associate the selected network printer.
            else
                printerName = cboNetworkPrinters.SelectedItem.ToString();

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printerName;

			//Here, we try to see if the printer really exists. Otherwise, it's no use to print there.
            if (printerSettings.IsValid)
            {
                printDoc.PrinterSettings = printerSettings;

				//Let's print!
				try
				{
					printDoc.Print();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
            }
        }

		/// <summary>
		/// Event used to build the data to be sent to the printer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
			//We are simply going to build a string saying "This is a test".
            using (Graphics g = e.Graphics)
            {
				// Let's create a StringFormat object with each line of text, and the block of text centered on the page.
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;

				using (Font font1 = new Font("Arial", 11))
				{
					RectangleF rectF1 = new RectangleF(15, 85, 160, 15);
					e.Graphics.DrawRectangle(Pens.White, Rectangle.Round(rectF1));
					g.DrawString("This is a test", font1, Brushes.Black, rectF1, stringFormat);
				}
            }
        }

		/// <summary>
		/// Event triggered as we select the local printers radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioLocalPrinters_CheckedChanged(object sender, EventArgs e)
        {
			//If the local printers radio button is checked, we want to deselect the network printers radio button and disable the network printers combobox.
            if (radioLocalPrinters.Checked)
            {
                radioNetworkPrinters.Checked = false;
                cboNetworkPrinters.Enabled = false;
                cboLocalPrinters.Enabled = true;
            }
            else
            {
                radioNetworkPrinters.Checked = true;
                cboNetworkPrinters.Enabled = true;
                cboLocalPrinters.Enabled = false;
            }
        }

		/// <summary>
		/// Event triggered as we select the network printers radio button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void radioNetworkPrinters_CheckedChanged(object sender, EventArgs e)
        {
			//If the network printers radio button is checked, we want to deselect the local printers radio button and disable the local printers combobox.
            if (radioNetworkPrinters.Checked)
            {
                radioLocalPrinters.Checked = false;
                cboNetworkPrinters.Enabled = true;
                cboLocalPrinters.Enabled = false;
            }
            else
            {
                radioLocalPrinters.Checked = true;
                cboNetworkPrinters.Enabled = false;
                cboLocalPrinters.Enabled = true;
            }
        }

		/// <summary>
		/// Event used when exiting the program.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
			//Bye bye!
            Environment.Exit(0);
        }

		/// <summary>
		/// Event triggered when we click the refresh button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
			//Let's reload the printers in the lists.
            LoadPrinters();

            if (defaultNetwork)
                radioNetworkPrinters.Checked = true;
            else
                radioLocalPrinters.Checked = true;
        }

        private void cboLocalPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblNameValue.Text = "";
            lblPortValue.Text = "";
            lblDriverValue.Text = "";
            lblDeviceIDValue.Text = "";
            lblSharedValue.Text = "";

            System.Management.ObjectQuery oquery = new System.Management.ObjectQuery("SELECT * FROM Win32_Printer WHERE Name = '" + ((String)cboLocalPrinters.SelectedItem).Replace("\\", "\\\\") + "'");
            System.Management.ManagementObjectSearcher mosearcher = new System.Management.ManagementObjectSearcher(oquery);
            System.Management.ManagementObjectCollection moc = mosearcher.Get();

            foreach (ManagementObject mo in moc)
            {
                lblNameValue.Text = mo["Name"].ToString();
                lblPortValue.Text = mo["PortName"].ToString();
                lblDriverValue.Text = mo["DriverName"].ToString();
                lblDeviceIDValue.Text = mo["DeviceID"].ToString();
                lblSharedValue.Text = mo["Shared"].ToString();
            }
        }
    }
}
