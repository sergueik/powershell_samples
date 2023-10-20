using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Utils;

namespace SeleniumClient {
	
	// the form must be the first class in the file in order
	// the form resources to be compiled correctly by SharpDevelop,
	// all other classes has to be moved below
	
	public partial class Parser : Form {
		WebBrowser browser = new WebBrowser();
		private ContextMenu popUpMenu;
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

		public Parser() {
			status = true;
			InitializeComponent();
			popUpMenu = new ContextMenu();
			Clipboard.Clear();
			popUpMenu.MenuItems.Add("Copy To Clipboard", new EventHandler(PopUp_Clicked));
			SetUp();
		}

		public Parser(String versionString)
			: this() {
			ShowVersion(versionString);
			// the version label will not be visible in "Design" view unless uncommented code in InitializeComponent
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
				} catch (ExternalException exception) {
					MessageBox.Show(exception.ToString(), "Error Detected in Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} 
			Invalidate();
		}

		private void SetUp() {
			
			MakeDataSet();
			// System.Windows.Forms.DataGridView.SetDataBinding(object dataSource, string dataMember)			
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagrid.setdatabinding?view=netframework-4.5
			// NOTE: for  DataGridView the SetDataBinding method is not available
			// set the dataGridView's DataSource
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridview.datasource?view=netframework-4.5

			dataGrid.SetDataBinding(dataSet, "Hosts");
			// https://stackoverflow.com/questions/119506/virtual-member-call-in-a-constructor
			this.ContextMenu = popUpMenu;
		}

		// based on: https://github.com/sergueik/powershell_selenium/blob/master/csharp/protractor-net/Extensions/Extensions.cs
		private static string FindMatch(string text, string matchPattern, string matchTag) {
			result = null;
			regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			matches = regex.Matches(text);
			foreach (Match match in matches) {
				if (match.Length != 0) {
					foreach (Capture capture in match.Groups[matchTag].Captures) {
						if (result == null) {
							result = capture.ToString();
						}
					}
				}
			}
			return result;
		}

		public void Start() {
			browser.ScriptErrorsSuppressed = true;
			// browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(docCompleted);
			browser.AllowNavigation = true;
			//  can only run directly
			// browser.Navigate(String.Format("http://{0}:4444/grid/console/",hub));
			
			try {
				var url = (String.IsNullOrEmpty(serviceUrl)) ? String.Format(serviceUrlTemplate, hub) : serviceUrl;
				// NOTE: Selenium 4.x redirects "/grid/console/" to "/ui/index.html#/"
				
				var request = WebRequest.Create(url);
				using (var response = request.GetResponse()) {
					using (var content = response.GetResponseStream()) {
						using (var reader = new StreamReader(content)) {
							var strContent = reader.ReadToEnd();
							if (!browserReady) {
								browser.Navigate("about:blank");
								while (browser.ReadyState != WebBrowserReadyState.Complete) {
									Application.DoEvents();
									System.Threading.Thread.Sleep(5);
								}
								browserReady = true;
							}
							processDocument(strContent);
						}
					}
				}
			} catch (WebException e) {
				Trace.Assert(e != null);
				// show message box
				String text = String.Format("The host {0} appears down", hub);
				String caption = String.Format("{0} status", environment);
				
				
				//	string title;
				string instruction;
				string content;
				DialogResult result;
				DialogMessage.MsgIcons icon;
				DialogMessage.MsgButtons buttons;
			
				// repeat loading ini information
				// TODO: refactor
				var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
				IniFile iniFile = IniFile.FromFile(iniFilePath);
				// var environment = "Prod";
				var name = iniFile[environment]["name"];
				// title = "Hub is down";
				var sections = iniFile.GetSectionNames();

				var sectionsList = new List<string>(sections);
				if (sectionsList.Contains("Nodes")) {
					IniFileSection nodesIniFileSection = iniFile["Nodes"];
					FillNodeData(nodesIniFileSection);
				}

				string nodeValue;
				nodeValue = GetNodeData(name);
				Clipboard.Clear();
				instruction = String.Format("The {0} hub {1} is down", environment, name);
				buttons = DialogMessage.MsgButtons.OK;
				icon = DialogMessage.MsgIcons.Error;			
				content = "";
				string clipboardText = String.Format("{0}\r\n{1}", name, nodeValue);
				Mover.FindAndMoveMsgBox(100, 100, true, caption);
				result = DialogMessage.ShowMessage(caption, instruction, buttons, icon, content, false, clipboardText);
				switch (result) {

					case DialogResult.OK:
						Close();
						status = false;
						break;
					default:
						break;
				}
			}
			countInventoryNodes();
			countDiscoveredNodes();
			// too early ?
			lblImage.Visible = CheckNodesStatus() ? false : true;
		}

		void processDocument(String content) {
			// Error CS0518: Predefined type 'Microsoft.CSharp.RuntimeBinder.Binder' is not defined or imported
			// Error CS1969: One or more types required to compile a dynamic expression cannot be found.
			dynamic Doc = browser.Document.DomDocument;
			Doc.open();
			Doc.write(content);
			Doc.close();

			// var document_html = browser.Document.Body.InnerHtml;
			HtmlDocument doc = browser.Document;
			HtmlElement element = null;
			HtmlElement element2 = null;
			HtmlElementCollection elements = null;
			var nodes = new List<String>();
			var ids = new List<String>();
			int rowNum = 0;

			ids.Add("rightColumn"); // for older Selenium grid
			ids.Add("leftColumn");
			ids.Add("right-column");
			ids.Add("left-column");

			foreach (String id in ids) {
				element = doc.GetElementById(id);
				if (element == null) {
					continue;
				}
				var html = element.InnerHtml;

				elements = element.GetElementsByTagName("p");
				for (int cnt = 0; cnt != elements.Count; cnt++) {
					element2 = elements[cnt];

					if (element2.GetAttribute("classname") != null && element2.GetAttribute("classname").Contains("proxyid")) {
						String text = element2.InnerText;
						var hostname = FindMatch(text, @"^\s*id\s*:\s*http://(?<hostname>[A-Z0-9-._]+):\d+,.*$", "hostname");
						nodes.Add(hostname == null ? text : hostname);
					}
				}
			}
			nodes.Sort();
			int datarows = 0;
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
	}
}