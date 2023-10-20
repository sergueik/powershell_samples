using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.IO;
using System.Net;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Utils;
using Utils.Support;

namespace SeleniumClient {
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

		public string Environment {
			get { return environment; }
			set {
				environment = value;
				if (Visible) {
					SuspendLayout();
				}
				Text = String.Format("{0} status", environment);
				if (Visible) {
					ResumeLayout();
				}
			}
		}

		public Parser(String versionString) : this() {
			ShowVersion(versionString);
  			// the version label will not be visible in "Design" view unless uncommented code in InitializeComponent
		}

		public Parser() {
			status = true;
			InitializeComponent();
			popUpMenu = new ContextMenu();
			Clipboard.Clear();
			popUpMenu.MenuItems.Add("Copy To Clipboard", new EventHandler(PopUp_Clicked));
			SetUp();

		}

		private void countInventoryNodes() {
			inventoryNodes.Clear();
			var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
			var iniFile = IniFile.FromFile(iniFilePath);
			foreach (string s in iniFile[environment]["nodes"].Split(",".ToCharArray())) {
				inventoryNodes.Add(s); 
			}

			inventoryCount = inventoryNodes.Count;
		}

		private void countDiscoveredNodes() {
			discoveredNodes.Clear();
			for (int index = 0; index != dataSet.Tables["Hosts"].Rows.Count; index++) {
				// https://docs.microsoft.com/en-us/dotnet/api/system.data.datarow?view=netframework-4.5
				var hostname = dataSet.Tables["Hosts"].Rows[index]["hostname"].ToString();
				if (hostname != "")
					discoveredNodes.Add(hostname);
				// alternatively, use indexer
				// discoveredNodes.Add(dataSet.Tables["Hosts"].Rows[index][1].ToString());
			}
			discoveredNodeCount = discoveredNodes.Count;
		}


		Dictionary<String, String> nodeData = new Dictionary<String, String>();
		private void FillNodeData(IniFileSection data) {
			ReadOnlyCollection<string> nodes = data.GetKeys();
			foreach (var node in nodes) {
				if (! nodeData.ContainsKey(node)){
					nodeData.Add(node, data[node]);
					// Console.Error.WriteLine(sectionElement.Content);
				}
			}
		}

		private string GetNodeData(string name) {
			string nodeValue;
			foreach (var nodeKey in nodeData.Keys) {
				// NOTE: for debugging assigned to a plain string:
				// Tricky to navigate through dropdowns
				nodeValue = nodeData[nodeKey];
				Console.Error.WriteLine(nodeKey + " = " + nodeValue);
			}
			if (nodeData.ContainsKey(name)) {
				nodeValue = nodeData[name];
			} else {
				nodeValue = String.Format("{0} is unknown", name);
			}
			return nodeValue;
		}

		private void PopUp_Clicked(object sender, EventArgs e) {

			MenuItem miClicked = null; 

			if (sender is MenuItem)
				miClicked = (MenuItem)sender;
			else
				return;

			string item = miClicked.Text;

			if (item == "Copy To Clipboard") {
				countInventoryNodes();
				countDiscoveredNodes();
				inventoryNodes.RemoveAll(o => discoveredNodes.Contains(o));
				var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
				var iniFile = IniFile.FromFile(iniFilePath);
				var sections = iniFile.GetSectionNames();
				var sectionsList = new List<string>(sections);
				if (sectionsList.Contains("Nodes")) {
					IniFileSection nodesIniFileSection = iniFile["Nodes"];
					FillNodeData(nodesIniFileSection);
				}
				List<String> nodelabels = new List<string>();
				foreach (var nodeKey in inventoryNodes) {
					// NOTE: for debugging assigned to a plain string:
					// Tricky to navigate through dropdowns
					String nodeValue = GetNodeData(nodeKey);

					nodelabels.Add(nodeValue);
					Console.Error.WriteLine(nodeKey + " = " + nodeValue);
				}
				string strText = String.Join("\r\n", inventoryNodes.ToArray()) + "\r\n\r\n" +
				                 String.Join("\r\n", nodelabels.ToArray());
				try {
					Clipboard.SetDataObject(strText, true);
					/*
					 TODO: style the exception reporter message box 
					 */ 
				} catch (ExternalException exception) {
					MessageBox.Show(exception.ToString(), "Error Detected in Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} 
			Invalidate();
		}

		private bool CheckNodesStatus() {
			nodesStatus = discoveredNodeCount == inventoryCount;
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

		public void Start()
		{
			// NOTE: Selenium 3 Tray App uses an extra variable
			// var url = (String.IsNullOrEmpty(serviceUrl)) ? String.Format(serviceUrlTemplate, hub) : serviceUrl;
			// just overwrite serviceUrl
			if (String.IsNullOrEmpty(serviceUrl))
				serviceUrl = String.Format(serviceUrlTemplate, hub);
			// NOTE: Selenium 4.x redirects "/grid/console/" to "/ui/index.html#/"

			runningServer = ServerHealthCheck();
			if (runningServer) {
				ServerVersionCheck();
				if (validServerVersion)
					processDocument();
				// TODO: debug
				countInventoryNodes();
				countDiscoveredNodes();
				// too early ?
				lblImage.Visible = CheckNodesStatus() ? false : true;
			}
		}

		private void SetUp() {

			MakeDataSet();
			dataSet.Tables["Hosts"].Rows.Clear();
			dataGrid.SetDataBinding(dataSet, "Hosts");
			this.ContextMenu = popUpMenu;
		}

		public void ServerVersionCheck(){

			var uri = new Uri(serviceUrl);
			var version = gridStatusLoader.Selenium4Detected(uri);

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
			Grid4 root = null;
			var nodes = new List<String>();
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
