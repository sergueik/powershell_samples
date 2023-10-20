using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;

namespace Client {
	// TODO: "Design": 
	// ICSharpCode.FormsDesigner.FormsDesignerLoadException: 
	// System.Reflection.AmbiguousMatchException: Ambiguous match found.
	// http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/Formpopupmenu.htm
	public class Parser: Form  {
		private ContextMenu popUpMenu;
		private Label lblImage;
		private const string wrenchiconBase64 = "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";
		private static bool nodesStatus = false;
		private DataGrid dataGrid;
		private DataGridTableStyle dataGridTableStyle;
		private DataGridTextBoxColumn textCol;
		private DataSet dataSet;
		private Button refreshButton;
		private Label versionLabel;
		private string versionString = "1.2.3";
		public string VersionString {
			set {
				versionString = value;
			}
		}

		private static string result = null;
		WebBrowser browser = new WebBrowser();
		private const String columnName = "hostname";
		private IContainer components = null;

		private void ShowVersion(string value){
			VersionString = value;
			versionLabel.Text = String.Format("Version: {0}",versionString);
		}

		[STAThread]
		static void Main() {
			var versionString = "1.2.3";
			Application.Run(new Parser(versionString));
		}
    protected override void OnPaintBackground(PaintEventArgs e) { /* Ignore */ }
		public Parser (String versionString): this() {
			       InitializeComponent();

        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        this.BackColor = Color.Transparent;
        this.TransparencyKey = Color.Transparent; // I had to add this to get it to work.

			ShowVersion(versionString);
			// the version label will not be visible in "Design" view unless uncommented code in InitializeComponent
		}

		public Parser() {
			InitializeComponent();
			SetUp();
		}

		private void PopUp_Clicked(object sender, EventArgs e) {

			string[] data = { "host1", "host2",  "host4", "host5", "host6" };
			var nodes1 = new List<string>();
			foreach (string s in data) {
				nodes1.Add(s); 
			}

			MenuItem miClicked = null; 

			if (sender is MenuItem)
				miClicked = (MenuItem)sender;
			else
				return;

			string item = miClicked.Text;

			var nodes2 = new List<string>();
			if (item == "Copy To Clipboard") { 
				for (int index = 0; index != dataSet.Tables["Hosts"].Rows.Count; index++) {
					// https://docs.microsoft.com/en-us/dotnet/api/system.data.datarow?view=netframework-4.5
					nodes2.Add(dataSet.Tables["Hosts"].Rows[index]["hostname"].ToString());
					// alternatively, use indexer
					// nodes2.Add(dataSet.Tables["Hosts"].Rows[index][1].ToString());
				}
				// TODO:lamda expression can be simplified to method group
				nodes1.RemoveAll(o => nodes2.Contains(o));
				string strText = String.Join("\r\n", nodes1.ToArray());
				Clipboard.SetDataObject(strText, true);
			} 
			Invalidate();
		}


		private void InitializeComponent() {
			dataGrid = new DataGrid();
			popUpMenu = new ContextMenu();
			Clipboard.Clear();
			popUpMenu.MenuItems.Add("Copy To Clipboard", new EventHandler(PopUp_Clicked));
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
				lblImage.Visible = CheckNodesStatus() ? true: false;
			};
			versionLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
			versionLabel.Location = new System.Drawing.Point(236, 399);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new System.Drawing.Size(105, 23);
			versionLabel.TabIndex = 0;
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
			((ISupportInitialize)(dataGrid)).EndInit();

			this.ContextMenu = popUpMenu;
			lblImage = new Label();
			lblImage.Parent = dataGrid;
			lblImage.Location = new Point(4, 4);
			lblImage.Image = LoadBase64(wrenchiconBase64);
			lblImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lblImage.Size = new Size(18, 18);
			lblImage.Visible = CheckNodesStatus() ? true: false;
			ResumeLayout(false);

		}

		private bool CheckNodesStatus() {
			nodesStatus = !nodesStatus;
			return nodesStatus;
		}

		public static Image LoadBase64(string base64) {
			byte[] bytes = Convert.FromBase64String(base64);
			Image image;
			using (var memoryStream = new MemoryStream(bytes)) {
				image = Image.FromStream(memoryStream);
			}
			return image;
		}

		private void SetUp() {
			MakeDataSet();
			dataSet.Tables["Hosts"].Rows.Clear();
			processDocument();
			dataGrid.SetDataBinding(dataSet, "Hosts");
		}

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		void processDocument( ) {
			var nodes = new List<String>();

			foreach (string s in (new String[] { "host1", "host3", "host5", "host7" })) {
				nodes.Add(s); 
			}

			var ids = new List<String>();
			nodes.Sort();
			int datarows = 0;
			int rowNum = 0;
			foreach (String text in nodes) {
				datarows++;
				Console.Error.WriteLine(text);
				// database table column name
				if (dataSet.Tables["Hosts"].Rows.Count < datarows) {
					dataSet.Tables["Hosts"].Rows.Add(new Object[]{ rowNum, text });
				} else {
					dataSet.Tables["Hosts"].Rows[rowNum][columnName] = text;
				}
				rowNum++;
			}
		}

		private void MakeDataSet() {
			dataSet = new DataSet("DataSet");

			var dataTable = new DataTable("Hosts");

			// Create two columns, and add them to the first table.
			var cHostId = new DataColumn("HostId", typeof(int));
			var chostname = new DataColumn("hostname");
			dataTable.Columns.Add(cHostId);
			dataTable.Columns.Add(chostname);

			// Add the tables to the DataSet.
			dataSet.Tables.Add(dataTable);

			DataRow newRow1;

			for (int i = 1; i < 5; i++) {
				newRow1 = dataTable.NewRow();
				newRow1["HostId"] = i;
				// Add the row to the Hosts table.
				dataTable.Rows.Add(newRow1);
			}
			dataTable.Rows[0]["hostname"] = "host1";
		}
	}
}
