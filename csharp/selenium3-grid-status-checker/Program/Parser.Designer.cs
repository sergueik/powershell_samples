using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;

using Utils;

namespace SeleniumClient {
	
	// the form must be the first class in the file in order 
	// the form resources to be compiled correctly by SharpDevelop,
	// all other classes has to be moved below
	
	public partial class Parser : Form {

		private DataGrid dataGrid;
		private DataGridTableStyle dataGridTableStyle;
		private DataGridTextBoxColumn textCol;
		private DataSet dataSet;
		private Button refreshButton;
		private Label versionLabel;
		private Label lblImage;
		private const string wrenchiconBase64 = "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";

		private static int discoveredNodeCount;
		private static int inventoryCount;
		private static List<string> inventoryNodes = new List<string>();
		private static List<string> discoveredNodes = new List<string>();
		static bool nodesStatus = true;
		
		private static string result = null;
		private Boolean  browserReady = false;
		private static Regex regex;
		private static MatchCollection matches;
		private const string columnName = "hostname";
		private IContainer components = null;
		private string environment = null;
		private string versionString = "1.2.3";
		public string VersionString {
			set {
				versionString = value;
			}
		}
		private void ShowVersion(string value){
			VersionString = value;
			versionLabel.Text = String.Format("Version: {0}",versionString);
		}


		private Boolean status;
		public Boolean Status {
			get { return status; }
		}

		private string serviceUrlTemplate = @"http://{0}:4444/grid/console/";
		public string ServiceUrlTemplate {
			get { return serviceUrlTemplate; }
			set {
				serviceUrlTemplate = value;
			}
		}

		private string serviceUrl = "";
		public string ServiceUrl {
			get { return serviceUrl; }
			set {
				serviceUrl = value;
			}
		}

		private String hub;
		public string Hub {
			get { return hub; }
			set {
				hub = value;
			}
		}

		private void InitializeComponent() {
			dataGrid = new DataGrid();
			dataGridTableStyle = new DataGridTableStyle();			
			textCol = new DataGridTextBoxColumn();
			refreshButton = new Button();
			versionLabel = new Label();
			((ISupportInitialize)(dataGrid)).BeginInit();
			SuspendLayout();
			dataGrid.DataMember = "";
			dataGrid.HeaderForeColor = SystemColors.ControlText;
			dataGrid.Location = new Point(8, 8);
			dataGrid.Margin = new Padding(4);
			dataGrid.Name = "dataGrid";
			dataGrid.Size = new Size(333, 382);
			dataGrid.TabIndex = 1;
			dataGrid.TableStyles.AddRange(new DataGridTableStyle[] {
			dataGridTableStyle});
			dataGridTableStyle.AlternatingBackColor = Color.LightGray;
			dataGridTableStyle.DataGrid = dataGrid;
			dataGridTableStyle.GridColumnStyles.AddRange(new DataGridColumnStyle[] {
			textCol});
			dataGridTableStyle.HeaderForeColor = SystemColors.ControlText;
			dataGridTableStyle.MappingName = "Hosts";
			textCol.Format = "";
			textCol.FormatInfo = null;
			textCol.HeaderText = "hostname";
			textCol.MappingName = "hostname";
			textCol.Width = 300;
			refreshButton.Location = new Point(8, 399);
			refreshButton.Name = "refreshButton";
			refreshButton.Size = new Size(75, 23);
			refreshButton.TabIndex = 0;
			refreshButton.Text = "Refresh";
			refreshButton.Click += (object sender, EventArgs e) => {
				dataSet.Tables["Hosts"].Rows.Clear();
				Start();
			};
			versionLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			versionLabel.Location = new System.Drawing.Point(236, 399);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new System.Drawing.Size(105, 23);
			// uncomment the following lines to let the version label be shown in "Design" view
			// with dummy data
			// NOTE: without assigning the versionString property explicitly 
			// the "Design" view will throw
			// System.ComponentModel.Design.Serialization.CodeDomSerializerException
			// The variable 'versionString' is either undeclared or was never assigned.

			versionString = "1.0.0";
			versionLabel.Text = String.Format("Version: {0}",versionString);

			AutoScaleDimensions = new SizeF(8F, 16F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(348, 429);
			Controls.Add(dataGrid);
			Controls.Add(refreshButton);
			Controls.Add(versionLabel);
			Name = "Parser";

			lblImage = new Label();
			lblImage.Parent = dataGrid;
			lblImage.Location = new Point(4, 4);
			lblImage.Image = LoadBase64(wrenchiconBase64);
			lblImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lblImage.Size = new Size(18, 18);

			((ISupportInitialize)(dataGrid)).EndInit();
			ResumeLayout(false);
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

	}
}
