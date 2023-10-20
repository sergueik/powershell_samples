using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Net;


using Utils;
using Utils.Support;

namespace Client {
	// TODO: "Design":
	// ICSharpCode.FormsDesigner.FormsDesignerLoadException:
	// System.Reflection.AmbiguousMatchException: Ambiguous match found.
	// http://www.java2s.com/Tutorial/CSharp/0460__GUI-Windows-Forms/Formpopupmenu.htm
	public partial class Parser : Form {
		private ContextMenu popUpMenu;

		private readonly ClientConfiguration clientConfiguration = GenericProxies.defaultConfiguration;
		private readonly GridStatusLoader gridStatusLoader = new GridStatusLoader();
		
		private static Boolean validServerVersion = false;
		private static Boolean runningServer = false;

		private String appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
		
		[STAThread]
		static void Main() {
			var versionString = "1.2.3";
			Parser parser = new Parser(versionString);
			parser.ServiceUrl = "http://localhost:4444/status";
			// Selenium 3.9.1 - wrong JSON schem
			// parser.ServiceUrl = "http://localhost:4444/status?version=3";
			// Ancient Legacy Selenium 3.x - no JSON status page
			// parser.ServiceUrl = "http://localhost:8080//resources/static/page.html";
			// NOTE: in this layout one cannot perform call parser.Start() 
			Application.Run(parser);
		}

		public Parser(String versionString) : this() {
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

		public void Start() {
			runningServer = ServerHealthCheck();
			if (runningServer) {
			ServerVersionCheck();
			if (validServerVersion) 
				processDocument();
			}
		}
		private void SetUp() {

			MakeDataSet();
			dataSet.Tables["Hosts"].Rows.Clear();
			dataGrid.SetDataBinding(dataSet, "Hosts");
		}

		public void ServerVersionCheck(){
			
			var uri = new Uri(serviceUrl);
			var version = gridStatusLoader.Selenium4Detected(uri);
			
			// Testing only - read Selenium Hub version from file loaded from application directory
			// var source = "grid3.json";
			// source = "grid4.json";
			// version = gridStatusLoader.Selenium4Detected(new FileStream(String.Format(@"{0}\{1}", appPath, source), System.IO.FileMode.Open));
			if (!version.StartsWith("4.")) {
				validServerVersion = false;
				const string caption = "Unsupported Selenium Version";
				var instruction = String.Format("Selenium Hub Verson {0}", String.IsNullOrEmpty(version) ? "unknown" : version );
				Mover.FindAndMoveMsgBox(100, 100, true, caption);
				dialogResult = DialogMessage.ShowMessage(caption, instruction, DialogMessage.MsgButtons.OK, DialogMessage.MsgIcons.Error, "", false, null);
			} else {
				validServerVersion = true;
			}
		}

		void processDocument() {
			var root = GridStatusLoader.GetMock<Grid4>(new FileStream(String.Format(@"{0}\{1}", appPath, "grid4.json"), System.IO.FileMode.Open));
			var nodes = new List<String>();
			nodes = gridStatusLoader.processDocument(root);
			try {
				root = GridStatusLoader.SynchronousGetServiceUrl<Grid4>(serviceUrl);
				nodes = gridStatusLoader.processDocument(root);
				// MessageBox.Show(String.Format("process {0} nodes", nodes.Count));
			} catch (Exception ex) {
				MessageBox.Show("Failed to call the service - " + ex.Message);
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
			
		public Boolean ServerHealthCheck() {
			Boolean result = false;
			try {
				var status = GenericProxies.RestHead(serviceUrl);
				if (status == 200)
					result = true;
			} catch (WebException e) {
								
				string name = "";
				string nodeValue = "";
				string content = "";
				
				icon = DialogMessage.MsgIcons.Error;			
				buttons = DialogMessage.MsgButtons.OK;

				string hub = "?";
				string environment = "?";
				String text = String.Format("The host {0} appears down", environment);
				String caption = String.Format("{0} status", environment);
				string instruction = String.Format("The {0} hub {1} is down", environment, hub);
				string clipboardText = String.Format("{0}\r\n{1}", name, nodeValue);
				Mover.FindAndMoveMsgBox(100, 100, true, caption);
				dialogResult = DialogMessage.ShowMessage(caption, instruction, buttons, icon, content, false, clipboardText);
			}
			return result;
		}
	}
}
