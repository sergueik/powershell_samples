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
		private Dictionary<string, Dictionary<string, string> > machines = new Dictionary<String, Dictionary<string, string> >();
		private string toolPath;
		// TODO: refactor
		public ContextMenuStrip Create(string toolPath) {
			this.toolPath = toolPath;
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

			foreach (var id in machines.Keys) {
				
				item = new ToolStripMenuItem();
				item.Text = getNodeData(id);
				// item.Click += new System.EventHandler(Exit_Click);
			
				// toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
				Image image = IconHelper.getImage(machines[id]["Guest OS"].ToLowerInvariant());
				item.Image = image != null ? image: Resources.QuestionMark; 
				item.Enabled = false;

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

		private string getNodeData(string id) {
			string nodeName = (machines.ContainsKey(id)) ?
				machines[id]["Name"] : String.Format("{0} is unknown", id);		
			return nodeName;
		}

		private void fillNodeData() {
			// machines.Add("{3b5c8967-4a00-4bf5-a137-ce0c4a046900}", new Dictionary<string, string>() { { "Name", "Windows 7" }, { "Guest OS", "unknown" } });
			machines.Add("{55d01a4a-4656-480f-bccb-e6838f5df285}", new Dictionary<string, string>() { { "Name", "Windows 7" }, { "Guest OS", "unknown" } });
			machines.Add("{f09db6f8-420b-4c64-9e22-0c2081c032d3}", new Dictionary<string, string>() { { "Name", "Xubuntu 22.04" }, { "Guest OS", "unknown" } });
			// machines.Add("{bb998aa9-6840-4bd7-b4b9-e6e2c28012a4}", new Dictionary<string, string>() { { "Name", "URU" }, { "Guest OS", "unknown" } });
			// some of the next one(s) may not be found
			machines.Add("{7e261a39-d356-4eb1-a8ed-75675b149241}", new Dictionary<string, string>() { { "Name", "Xubuntu 22.04" }, { "Guest OS", "unknown" } });
			// machines.Add("{97020c8c-542c-481e-86a3-f16bee525707}", new Dictionary<string, string>() { { "Name", "minikube" }, { "Guest OS", "unknown" } });
			// machines.Add("{55d01a4a-4656-480f-bccb-e6838f5df285}", new Dictionary<string, string>() { { "Name", "Windows 7" }, { "Guest OS", "unknown" } });
			// machines.Add("{aa7eaf83-18d1-4d7a-b20b-e98a9206c41b}", new Dictionary<string, string>() { { "Name", "default" }, { "Guest OS", "unknown" } });
			// Virtual Box box images
			machines.Add("{033248a8-18ef-4b52-8001-f9e7ebaaf3f7}", new Dictionary<string, string>() { { "Name", "Generic Alpine Virtualbox" }, { "Guest OS", "unknown" } });
			machines.Add("{a48d8b60-7e07-406e-9244-d58bbd530fed}", new Dictionary<string, string>() { { "Name", "Generic Centos Virtualbox" }, { "Guest OS", "unknown" } });

			foreach (var id in machines.Keys) {
				var processRunner = new ProcessRunner();

				// Tricky to navigate through dropdowns
				var arguments = String.Format("{0} {1}", "showvminfo", id);
				var fileName = "VBoxManage.exe";
				// var toolPath = Environment.ExpandEnvironmentVariables(@"%PROGRAMFILES%\Oracle\VirtualBox");

				Debug.WriteLine(String.Format(@"{0}\{1} {2}", toolPath, fileName, arguments));
				processRunner.Run(String.Format(@"{0}\{1}", toolPath, fileName), arguments);
				var key = "Guest OS";
				var value = "unknown";
				var matchedLine = processRunner.StandardOutput.FindLast((line) => line.IndexOf(String.Format("{0}:", key)) == 0);
				if (matchedLine != null) {
					value = matchedLine.FindMatch(String.Format(@"{0}:\s+(?<guest_os>[^ ].*)$", key));
					// Debug.WriteLine(String.Format("{0} \"{1}\"{2} \"{3}\"\n", "STDOUT:", String.Join(Environment.NewLine, processRunner.StandardOutput),"STDERR:", String.Join(Environment.NewLine, processRunner.StandardError)));
					Debug.WriteLine(String.Format("{0} {1}", id, value));
				} else
					Debug.WriteLine(String.Format("{0} {1}", id, "undefined"));
				machines[id][key] = value;
			}

		}
	}
	
}
