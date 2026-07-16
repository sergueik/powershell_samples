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
			// name fragment based lookup
			// support the icons copied from Virtualbox resources directory in github
			KeyValuePair<string, string>[] iconLookup = {
				new KeyValuePair<string, string>("windows", "os_win_other.png"),
				new KeyValuePair<string, string>("microsoft", "os_win_other.png"),

				new KeyValuePair<string, string>("ubuntu", "os_ubuntu.png"),
				new KeyValuePair<string, string>("debian", "os_debian.png"),
				new KeyValuePair<string, string>("red hat", "os_redhat.png"),
				new KeyValuePair<string, string>("redhat", "os_redhat.png"),
				new KeyValuePair<string, string>("rhel", "os_redhat.png"),
				new KeyValuePair<string, string>("fedora", "os_fedora.png"),
				new KeyValuePair<string, string>("arch", "os_archlinux.png"),
				new KeyValuePair<string, string>("opensuse", "os_opensuse.png"),
				new KeyValuePair<string, string>("suse", "os_opensuse.png"),
				new KeyValuePair<string, string>("gentoo", "os_gentoo.png"),
				new KeyValuePair<string, string>("mandriva", "os_mandriva.png"),
				new KeyValuePair<string, string>("oracle linux", "os_oracle.png"),

				new KeyValuePair<string, string>("freebsd", "os_freebsd.png"),
				new KeyValuePair<string, string>("netbsd", "os_netbsd.png"),
				new KeyValuePair<string, string>("openbsd", "os_openbsd.png"),

				new KeyValuePair<string, string>("macos", "os_macosx.png"),
				new KeyValuePair<string, string>("mac os", "os_macosx.png"),
				new KeyValuePair<string, string>("os x", "os_macosx.png"),

				new KeyValuePair<string, string>("solaris", "os_solaris.png"),
				new KeyValuePair<string, string>("qnx", "os_qnx.png"),

				// Keep generic Linux last.
				new KeyValuePair<string, string>("linux", "os_linux.png")
			};

			foreach (var id in machines.Keys) {
				
				item = new ToolStripMenuItem();
				item.Text = getNodeData(id);
				// item.Click += new System.EventHandler(Exit_Click);
				item.Image = Resources.Exit;
			
				//
				string os = machines[id]["Guest OS"].ToLowerInvariant();

				string filename = "Resources/os_other.png";
				Debug.WriteLine(String.Format("Determine icon for {0}", os));
				foreach (KeyValuePair<string, string> keyValuePair  in iconLookup) {
					if (os.Contains(keyValuePair.Key)) {
						filename = "Resources/" + keyValuePair.Value;
						break;
					}
				}
				string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

				if (File.Exists(iconPath)) {
					using (Image image = Image.FromFile(iconPath)) {
						item.Image = new Bitmap(image);
						item.Enabled = false;
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

		private string getNodeData(string id) {
			string nodeName = (machines.ContainsKey(id)) ?
				machines[id]["Name"] : String.Format("{0} is unknown", id);		
			return nodeName;
		}

		private void fillNodeData() {
			machines.Add("{3b5c8967-4a00-4bf5-a137-ce0c4a046900}", new Dictionary<string, string>() { { "Name", "Windows 7" }, { "Guest OS", "unknown" } });
			machines.Add("{f09db6f8-420b-4c64-9e22-0c2081c032d3}", new Dictionary<string, string>() { { "Name", "Xubuntu 22.04" }, { "Guest OS", "unknown" } });
			machines.Add("{bb998aa9-6840-4bd7-b4b9-e6e2c28012a4}", new Dictionary<string, string>() { { "Name", "URU" }, { "Guest OS", "unknown" } });
			// some of the next one(s) may not b found
			machines.Add("{7e261a39-d356-4eb1-a8ed-75675b149241}", new Dictionary<string, string>() { { "Name", "Xubuntu 22.04" }, { "Guest OS", "unknown" } });
			machines.Add("{97020c8c-542c-481e-86a3-f16bee525707}", new Dictionary<string, string>() { { "Name", "minikube" }, { "Guest OS", "unknown" } });
			machines.Add("{55d01a4a-4656-480f-bccb-e6838f5df285}", new Dictionary<string, string>() { { "Name", "Windows 7" }, { "Guest OS", "unknown" } });
			machines.Add("{aa7eaf83-18d1-4d7a-b20b-e98a9206c41b}", new Dictionary<string, string>() { { "Name", "default" }, { "Guest OS", "unknown" } });

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
