using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utils;

namespace Program {

	public class Launcher {

		private	static Dictionary<String, String> nodeData = new Dictionary<String, String>();
		// TODO: intermittent "The type or namespace name 'STAThread' could not be found"
		// when quickly converting project type and introducing the launcher
		[STAThread]
		public static void Main() {
			
			string title;
			string instruction;
			string content;
			DialogResult result;
			DialogMessage.MsgIcons icon;
			DialogMessage.MsgButtons buttons;
			
			//
			var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
			IniFile iniFile = IniFile.FromFile(iniFilePath);

			var environment = "Prod";
			var name = iniFile[environment]["name"];
			title = "Hub is down";
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
			DialogMessage.ShowMessage(title, instruction, buttons, icon, content, true, clipboardText);
			title = "Question";
			instruction = "Want to learn how to write your own message box?";
			buttons = DialogMessage.MsgButtons.YesNo;
			icon = DialogMessage.MsgIcons.Question;			
			content = "In this project " + "\r\n" + "we will learn the logic necessary" + "\r\n" +
			"to write your own dialog message box in Windows";
			result = DialogMessage.ShowMessage(title, instruction, buttons, icon, content);
			// DialogResult of the button clicked by user

			// Show the stock standard Windows MessageBox with the user selection
			if (buttons == DialogMessage.MsgButtons.YesNo)
			if (result == DialogResult.Yes)
				MessageBox.Show("Yes was selected");
			else
				MessageBox.Show("No was selected");
		}

		private static void FillNodeData(IniFileSection data) {
			ReadOnlyCollection<string> nodes = data.GetKeys();
			foreach (var node in nodes) {
				nodeData.Add(node, data[node]);
				// Console.Error.WriteLine(sectionElement.Content);
			}		
		}
		private static string GetNodeData(string name) {
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
	}	
}