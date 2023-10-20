using System;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using System.Diagnostics;
using SeleniumClient.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
// 'System.Array' does not contain a definition for 'Contains'
// and no extension method 'Contains' accepting a first argument
// of type 'System.Array' could be found
using System.Linq;
using Utils;

namespace SeleniumClient {
	
	public class ProcessIcon : IDisposable {
		// TODO: The name 'CharSet' does not exist in the current context (CS0103)
		// [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
		// extern static bool DestroyIcon(IntPtr handle);

		private NameValueCollection appSettings;
		// NOTE: wrong class? - seeing error in SharpDevelop UI designer (ignoring in this revision):
		// Failed to load designer. Check the source code for syntax errors and check if all references are available.
		// ICSharpCode.FormsDesigner.FormsDesignerLoadException: System.ComponentModel.Design.Serialization.CodeDomSerializerException:
		// The variable 'versionString' is either undeclared or was never assigned.

		NotifyIcon notifyIcon;
		private string versionString = "1.2.3";
		private string serviceUrl = "";
		private string serviceUrlTemplate = @"http://{0}:4444/grid/console/";
		public ProcessIcon() {
			appSettings = ConfigurationManager.AppSettings;

			if (appSettings.AllKeys.Contains("Version")) {
				versionString = appSettings["Version"];
			}
			;
			if (String.IsNullOrEmpty(versionString)) {
				// read version from the assembly info
				var versionObj = Assembly.GetExecutingAssembly().GetName().Version;
				versionString = String.Format("{0}.{1}.{2}", versionObj.Major, versionObj.Minor, versionObj.Build);
			}
			// Get the service url template
			serviceUrlTemplate = ConfigurationManager.AppSettings["ServiceUrlTemplate"];
			// Get the service url
			serviceUrl = (appSettings.AllKeys.Contains("ServiceUrl")) ? 
				ConfigurationManager.AppSettings["ServiceUrl"] : "";
			notifyIcon = new NotifyIcon();
		}

		public void Display() {
			// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.NotifyIcon.icon?view=netframework-4.5
			// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.icon?view=netframework-4.5s
			// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=netframework-4.5
			
				
			const string iconBase64 = "iVBORw0KGgoAAAANSUhEUgAAAD8AAABBCAYAAABmd3xuAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAUISURBVHhe7VltcxNVFPbP+QKIKKAItLS8VBheHWR8GTqjDgj1AzBUHVCcQSrOCO0HLTgqfuITH5RhwAGl1pLdvJGwSdskbbK7SR7PububtMvNDG13Y7e5Z+aZbdLkJs+5z3nuOZvn0MGhyHdqKPKdGoq8LGKbnl9RML464zJrhiIvC9kCUYYi7wtFXhayBaIMRd4XirwsZAtEGYq8LxR5WcgWiDLaT/7NF1y8iNhmAl8bzxFk7wkJ7SW/ZRX0HesRf38v4gP9SF84i+TgSegfHUH87e3Qtq11E9CeJLSVvLbjNRhXvkE5qcOczMOeKcEqTMPM51B4cA/pi19AP7ANWteatqigvTvfvQbp08dQ1h+hZlZQr1absC3YpSKm/7iF5IeHEdu6OvQEhE9+bi1TjWs9ryB95hhmx/9GzbZRr9VQr9cd0N8100Tx7m0kjh6gBKx6er0AES55Iq1RjWvdLzcTwNeu1Uif+hizj8YpAVaTvAtWxdStm9D6Xm++LwSER55dnGo8f+M6MpfOQ+tdR4b3kvM/VsD2V50ETPxDkicFEGkOkYBaFfb0FBKnj1PiqP79aweE8Mizs/cfROVxCuaTDDJDX0LfvclJAO8mH3PsAedOwSKi8+TPu18pI//bT9DeekO+fgAIjzwRSw2egD2ZEztrkbsbw0PQ9mwm4pQA3v2d65H97mvY5PhMnqORAMtCaewB9CN98vUDQGjkuc5Tn38Ke2rSIUPkzEyaEvAt9L1bBLJD51FJJcjtHdnPA3kBewKrR7Z+EAiRPO382U9o5/PivV4CLEqGMXoF2auXYBpZUd9zg18nrkR+ZmIM+gf7pOsHgdBr3sw+nr+jdKbbxQJhGjU+33213oBlonDnd2gHe+TrB4DwyHNNk8GV/ryDGtWvR8oL/2MvvOe55rnhiR/e2TRJ2ecsAeGRZ1CTItw8Z7Te4VagcqhSx5e/NiL8oXFMBohwybu7b4wO07lNxuee588M9gjyhdzoVcSp5+dSClIB4ZJnULOjH+pF/pcfxXEn+nhWwbMqgV7HHpGjBGp9G51jMqAEhE+exlONhhR931ZkaGqbGX8o5Mw9vEgAqYE9QTx2jzyOuVd+XZUMMvfrNafpYQVIP2thaAN5F7RbPNTE392DFM3xkzdvoHj/Lgr3bsP4+Qdkvr+IcjzmmKOrCo7G1fWAHHlAfH93ICbYPvIM/rLc1tIX5xsXPN9zj8/zu5j2Bk+irE1QAkgFvON+8DFJ3SB7gL6/a8km2F7yfoidc8HK2LUBaeoKedgRCZD5Aj1nGU9IAcOilJZSAv8veT84GaQIVkAlnXTG3RYJYAUY10fEaSLUtIgSWF7kGawAGn8TA/2o0CzQqgS4OxR9gDf5LUIBy488gxNACuDZoBz7t2GCHPOSIBQw5fQB7AGsANl6LbA8yTO8EvhsALPCBJ++4+MkwDHB7OULiJFpLkT+y5c8gxWwayOZ4ICrAIkJ8gkwU4IxcpnIr1tB5BmeAgZPCBMULbKXALrWymUUafpL9h9aQbKfC1ZAz1oywaOUgISY+Ii9uNFZenhf/AiymHt90SDP4AT0Oo3Q7NhfsPKG2PHGLe4FyN1DdMgz3BJIHn9PzAnxd/rcLm/hxBnRIs/gBNCgJH7XW+KIGz3yAUKR94UiLwvZAlGGIu8LRV4WsgWiDEXeF4q8LGQLRBmKvC9aku+EUOQ7NRT5To0OJg/8B2e5AyQxI1LwAAAAAElFTkSuQmCC";
			var iconBytes = Convert.FromBase64String(iconBase64);
			var iconStream = new MemoryStream(iconBytes, 0, iconBytes.Length);
			iconStream.Write(iconBytes, 0, iconBytes.Length);
			var iconImage = Image.FromStream(iconStream, true);
			var iconBitmap = new Bitmap(iconStream);			
			IntPtr hicon = iconBitmap.GetHicon();
			Icon icon = Icon.FromHandle(hicon);

			notifyIcon.Icon = icon; // Icon.FromHandle(Properties.Resources.selenium.GetHicon());
			notifyIcon.Icon = Icon.FromHandle(Properties.Resources.selenium.GetHicon());
			notifyIcon.Text = "System Tray Selenium Grid Status Checker";
			
			notifyIcon.BalloonTipText = "polls status of Selenium Grid";
			notifyIcon.BalloonTipTitle = "System Tray Selenium Grid Status Checker";

			notifyIcon.Visible = true;
			var contextMenus = new ContextMenus();
			contextMenus.VersionString = versionString;
			contextMenus.ServiceUrlTemplate = serviceUrlTemplate;
			contextMenus.ServiceUrl = serviceUrl;
			var contextMenuStrip = contextMenus.Create();
			notifyIcon.ContextMenuStrip = contextMenuStrip;
		}

		public void Dispose() {
			notifyIcon.Dispose();
		}

		// TODO: the icon does not disappear instantly on exit, only after mouse hover
		// method protection level prevents from calling
		// notifyIcon.Dispose( disposing )

		public void DisplayBallonMessage(string message, int delay) {
			if (!string.IsNullOrEmpty(message)) {
				notifyIcon.BalloonTipText = message;
			} else {
				notifyIcon.BalloonTipText = "polls status of Selenium Grid";
				notifyIcon.BalloonTipTitle = "System Tray Selenium Grid Status Checker";
			}
			notifyIcon.ShowBalloonTip(delay);
		}

	}

	
	class ContextMenus {
		
		private string versionString = "1.2.3";
		private string serviceUrlTemplate = @"http://{0}:4444/grid/console/";
		public string VersionString {
			set {
				versionString = value;
			}
		}

		public string ServiceUrlTemplate {
			get { return serviceUrlTemplate; }
			set {
				serviceUrlTemplate = value;
			}
		}
		private string serviceUrl = "";
		public string ServiceUrl {
			get { return serviceUrl; }
			set {
				serviceUrl = value;
			}
		}
		
		// show one dialog at a time
		bool isFormDisplayed = false;
		ContextMenuStrip menu;

		public ContextMenuStrip Create() {
			var iniFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\config.ini";
			menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			
			var iniFile = IniFile.FromFile(iniFilePath);
			var sections = iniFile.GetSectionNames();
			var environments = iniFile["Environments"]["values"];
			if (environments != null) {
				foreach (String environment in environments.Split(new char[] {','})) {
					// String hostname = iniFileNative.ReadValue(environment, "hub");
					var hostname = iniFile[environment]["hub"];
					var nodes = new List<string>();
					
					foreach (string s in iniFile[environment]["nodes"].Split(",".ToCharArray())) {
						nodes.Add(s); 
					}
					// NOTE: there is no close() in iniFile class 
					item = new ToolStripMenuItem();
					item.Text = String.Format("{0} status", environment);
					var data = new Dictionary<String, String>();
					data.Add("hub", hostname);
					data.Add("environment", environment);
					item.Tag = data;
					item.Click += new EventHandler(Process_Click);
					item.Image = Resources.search;
					menu.Items.Add(item);
				}

			}
			menu.Items.Add(new ToolStripSeparator());

			item = new ToolStripMenuItem();
			item.Text = "Configure";
			item.Click += new EventHandler(Notepad_Click);
			item.Image = Resources.gear;
			menu.Items.Add(item);

			menu.Items.Add(new ToolStripSeparator());


			item = new ToolStripMenuItem();
			item.Text = "exit";
			item.Click += (object sender, EventArgs e) => Application.Exit();
			item.Image = Resources.exit;
			menu.Items.Add(item);
			return menu;
		}
		
		void Process_Click(object sender, EventArgs eventArgs) {
			var item = (ToolStripMenuItem)sender;
			var items = item.GetCurrentParent().Items;
			var data = (Dictionary<String, String>)item.Tag;
			String hub = data["hub"];
			String environment = data["environment"];
			if (!isFormDisplayed) {
				var parser = new Parser(versionString);
				parser.Hub = hub;
				if (serviceUrl != "")
					parser.ServiceUrl = serviceUrl;
				parser.ServiceUrlTemplate = serviceUrlTemplate;
				parser.Environment = environment;
				// NOTE: no need to set explicilty - keep for later refactoring
				parser.VersionString = versionString;
				parser.Start();
				if (parser.Status) {
					// https://stackoverflow.com/questions/13405714/is-versus-try-cast-with-null-check
					foreach (var item1 in items) {
						var item2 = item1 as ToolStripMenuItem;
						if (item2 == null) {
							// must be "separator"
							continue;
						}
						if (item.Equals(item2)) {
							item.BackColor = Color.LightGray;
						}
						item2.Enabled = false;
					}
					isFormDisplayed = true;
					parser.ShowDialog();
					// handle closing
					foreach (var item1 in items) {
						var item2 = item1 as ToolStripMenuItem;
						if (item2 == null) {
							// must be "separator"
							continue;
						}
						if (item.Equals(item2)) {
							item.BackColor = Color.White;
						}
						item2.Enabled = true;
					}
					isFormDisplayed = false;
				}
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

	}
}
