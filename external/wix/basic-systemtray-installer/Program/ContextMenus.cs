using System;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTrayApp.Properties;
using System.Drawing;

namespace SystemTrayApp {
	class ContextMenus {
		bool isAboutLoaded = false;

		public ContextMenuStrip Create() {
			var menu = new ContextMenuStrip();
			ToolStripMenuItem item;

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

			item = new ToolStripMenuItem();
			item.Text = "Configure";
			item.Click += new EventHandler(Notepad_Click);
			item.Image = Resources.Gear;
			menu.Items.Add(item);


			menu.Items.Add(new ToolStripSeparator());

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			item.Image = Resources.Exit;
			menu.Items.Add(item);

			return menu;
		}

		void Explorer_Click(object sender, EventArgs e){
			Process.Start("explorer", null);
		}

		void About_Click(object sender, EventArgs e){
			if (!isAboutLoaded){
				isAboutLoaded = true;
				new AboutBox().ShowDialog();
				isAboutLoaded = false;
			}
		}

		void Notepad_Click(object sender, EventArgs e) {
			var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
			var process = new Process { 
				StartInfo = new ProcessStartInfo {
					FileName = "notepad.exe",
					Arguments = iniFilePath
				}
			};
			process.Start();
			process.WaitForExit();
		}

		void Exit_Click(object sender, EventArgs e){
			Application.Exit();
		}
	}
}