using System;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTrayApp.Properties;
using System.Drawing;
using System.IO.Pipes;
using Utils;

namespace SystemTrayApp {
	class ContextMenus {
		bool isAboutLoaded = false;
		private PipeServer pipeServer;
		public ContextMenuStrip Create() {
			var menu = new ContextMenuStrip();
			pipeServer = new PipeServer("demo", PipeDirection.InOut);
			// TODO: do we really need two event handlers with two signatures ?
			pipeServer.MessageReceived += (Object s, MessageReceivedEventArgs o) => pipeServer.Send(o.Message);
			pipeServer.MessageReceived += OnMessageReceived;
			pipeServer.Start();
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

		void Exit_Click(object sender, EventArgs e){
			Application.Exit();
		}
		
		private void OnMessageReceived(Object s, MessageReceivedEventArgs o) { 
			if (o.Message.Contains("exit")) {
				Application.Exit();
			}
		}

	}
}