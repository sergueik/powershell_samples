using System;
using System.Diagnostics;
using System.Windows.Forms;
using Program.Properties;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

using Utils;

namespace Program {

	class ContextMenus {
		bool isAboutLoaded = false;
		private Dictionary<string, string> machines = new Dictionary<String, String>();

		public ContextMenuStrip Create() {
			var menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			item = new ToolStripMenuItem();
			item.Text = "Explorer";
			item.Click += new EventHandler(Explorer_Click);
			item.Image = Resources.Explorer;
			menu.Items.Add(item);

			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			item.Image = Resources.About;
			menu.Items.Add(item);

			sep = new ToolStripSeparator();
			menu.Items.Add(sep);

			
			fillNodeData();
			
			foreach (var nodeKey in machines.Keys) {
				
				item = new ToolStripMenuItem();
				item.Text = GetNodeData(nodeKey);
				// item.Click += new System.EventHandler(Exit_Click);
				item.Image = Resources.Exit;
				string filename = "Resources/os_other.png";
				string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
				if (File.Exists(iconPath)) {
					using (Image image = Image.FromFile(iconPath)) {
						item.Image = new Bitmap(image);
					}
				} else
					item.Image = Resources.Exit;
									

				menu.Items.Add(item);
			}
			sep = new ToolStripSeparator();
			menu.Items.Add(sep);
			
			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			item.Image = Resources.Exit;
			menu.Items.Add(item);

			return menu;
		}

		void Explorer_Click(object sender, EventArgs e) {
			Process.Start("explorer", null);
		}

		void About_Click(object sender, EventArgs e) {
			if (!isAboutLoaded) {
				isAboutLoaded = true;
				new AboutBox().ShowDialog();
				isAboutLoaded = false;
			}
		}

		void Exit_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private string GetNodeData(string name) {
			string nodeValue;
			foreach (var nodeKey in machines.Keys) {
				var processRunner = new ProcessRunner();

				// NOTE: for debugging assigned to a plain string:
				// Tricky to navigate through dropdowns
				var arguments  = String.Format("{0} {1}", "showvminfo", nodeKey);
				var fileName = "VBoxManage.exe";
				var toolPath = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES%\Oracle\VirtualBox");

				processRunner.Run(String.Format(@"{0}\{1}", toolPath, fileName), arguments);
				// Debug.WriteLine(String.Format(@"{0}\{1}", toolPath, fileName));
				var info = "Guest OS:";
				var matchedLine = processRunner.StandardOutput.FindLast((line)=> line.IndexOf(info) == 0);
				if(matchedLine!= null) {
					var result = matchedLine.FindMatch(String.Format(@"{0}\s+(?<guest_os>[^ ].*)$", info));
					// Debug.WriteLine(String.Format("{0} \"{1}\"{2} \"{3}\"\n", "STDOUT:", String.Join(Environment.NewLine, processRunner.StandardOutput),"STDERR:", String.Join(Environment.NewLine, processRunner.StandardError)));
					Debug.WriteLine(String.Format("{0} {1}", info , result));
				} else
					Debug.WriteLine(String.Format("{0} {1}", info , "undefined"));
				nodeValue = machines[nodeKey];
				Console.Error.WriteLine(nodeKey + " = " + nodeValue);
				
			}
			if (machines.ContainsKey(name)) {
				nodeValue = machines[name];
			} else {
				nodeValue = String.Format("{0} is unknown", name);
			}
			return nodeValue;
		}

		private void fillNodeData() {
			//foreach (var node in nodes) {
			// machines.Add("{91047a20-5df0-4b68-b11d-1abd36738105}", "XPSP3");
			machines.Add("{3b5c8967-4a00-4bf5-a137-ce0c4a046900}","Windows 7");
			machines.Add("{f09db6f8-420b-4c64-9e22-0c2081c032d3}","Xubuntu 22.04");
			// the next one will not b found
			machines.Add("{7e261a39-d356-4eb1-a8ed-75675b149241}", "Xubuntu 22.04");
			// machines.Add("{0b64d785-4228-4357-83bc-2b6a436f81bf}", "Xubuntu VS Code");
			// machines.Add("{184f37d0-8529-474c-962d-6fd6781d9757}", "Windows 10 x64 ru");
			// machines.Add("{59c3df8a-e359-4211-8e7c-74ec5dd3e51d}", "default");
			// machines.Add("{55d01a4a-4656-480f-bccb-e6838f5df285}", "Windows 7");

			// Console.Error.WriteLine(sectionElement.Content);
			//}		
		}
	}
	
}
