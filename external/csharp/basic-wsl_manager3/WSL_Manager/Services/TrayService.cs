using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WslManagerFramework.Services {
	public class TrayService {
		private NotifyIcon notifyIcon;
		private ContextMenuStrip contextMenuStrip;
		private Form form;
		private LogService logService;

		public TrayService(Form form, LogService logService) {
			this.form = form;
			this.logService = logService;
			InitializeTrayComponents();
		}

		private void InitializeTrayComponents() {
			notifyIcon = new NotifyIcon();
			contextMenuStrip = new ContextMenuStrip();
            
			var iconPath = System.IO.Path.Combine(
				               System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
				               "tax.ico");
                
			if (System.IO.File.Exists(iconPath)) {
				notifyIcon.Icon = new Icon(iconPath);
			} else {
				notifyIcon.Icon = SystemIcons.Information;
			}
            
			notifyIcon.Text = "WSL Manager";
			notifyIcon.Visible = true;
			notifyIcon.DoubleClick += (_, __) => ShowWindow();
            
			contextMenuStrip.BackColor = Color.FromArgb(45, 45, 48);
			contextMenuStrip.ForeColor = Color.White;
			notifyIcon.ContextMenuStrip = contextMenuStrip;
		}

		public void UpdateTrayMenu() {
			contextMenuStrip.Items.Clear();

			try {
				var distros = WslService.ListDistros();
                
				if (distros.Length > 0) {
					foreach (var distro in distros) {
						var status = WslService.GetDistroStatus(distro);
						var distroItem = new ToolStripMenuItem(distro);
						distroItem.BackColor = Color.FromArgb(45, 45, 48);
						distroItem.ForeColor = Color.White;

						if (status.ToLower() == "running") {
							var stopItem = new ToolStripMenuItem("Stop");
							stopItem.BackColor = Color.FromArgb(45, 45, 48);
							stopItem.ForeColor = Color.LightCoral;
							stopItem.Click += async (_, __) => {
								try {
									await WslService.StopWslAsync(distro);
									UpdateTrayMenu();
								} catch (Exception ex) {
									if (logService != null)
										logService.SafeAddLog(String.Format("WSL Stop error: {0}", ex.Message), Color.LightCoral);
								}
							};
							distroItem.DropDownItems.Add(stopItem);
						} else {
							var startItem = new ToolStripMenuItem("Launch in Background");
							startItem.BackColor = Color.FromArgb(45, 45, 48);
							startItem.ForeColor = Color.LightGreen;
							startItem.Click += async (_, __) => {
								try {
									await WslService.LaunchWslBackgroundAsync(distro);
									UpdateTrayMenu();
									if (logService != null)
										logService.SafeAddLog(String.Format("{0} was launched in the background", distro), Color.LightGreen);
								} catch (Exception ex) {
									if (logService != null)
										logService.SafeAddLog(String.Format("Background launch error: {0}", ex.Message), Color.LightCoral);
								}
							};
							distroItem.DropDownItems.Add(startItem);
						}

						var cmdItem = new ToolStripMenuItem("Open with cmd");
						cmdItem.BackColor = Color.FromArgb(45, 45, 48);
						cmdItem.ForeColor = Color.White;
						cmdItem.Click += (_, __) => {
							try {
								WslService.LaunchInCmd(distro);
							} catch (Exception ex) {
								if (logService != null)
									logService.SafeAddLog(String.Format("Startup error: {0}", ex.Message), Color.LightCoral);
							}
						};
						distroItem.DropDownItems.Add(cmdItem);

						var directItem = new ToolStripMenuItem("Launch");
						directItem.BackColor = Color.FromArgb(45, 45, 48);
						directItem.ForeColor = Color.White;
						directItem.Click += new EventHandler((object source, EventArgs args) => {
							try {
								WslService.LaunchWslDirect(distro);
							} catch (Exception ex) {
								if (logService != null)
									logService.SafeAddLog(String.Format("Launch Error: {0}", ex.Message), Color.LightCoral);
							}
						                             });
						distroItem.DropDownItems.Add(directItem);

						var statusText = status.ToLower() == "running" ? "Running" : "Stopped";
						var statusColor = status.ToLower() == "running" 
                            ? Color.LightGreen 
                            : Color.LightCoral;
						distroItem.Text = String.Format("{0} ({1})", distro, statusText);
						distroItem.ForeColor = statusColor;

						contextMenuStrip.Items.Add(distroItem);
					}

					contextMenuStrip.Items.Add(new ToolStripSeparator());
				}
			} catch (Exception ex) {
				if (logService != null)
					logService.SafeAddLog(String.Format("Tray menu update error: {0}", ex.Message), Color.LightCoral);
			}

			var showItem = new ToolStripMenuItem("Show window");
			showItem.BackColor = Color.FromArgb(45, 45, 48);
			showItem.ForeColor = Color.White;
			showItem.Click += delegate(object sender, EventArgs e) {
				ShowWindow();
			};
			contextMenuStrip.Items.Add(showItem);

			var refreshItem = new ToolStripMenuItem("Update");
			refreshItem.BackColor = Color.FromArgb(45, 45, 48);
			refreshItem.ForeColor = Color.White;
			refreshItem.Click += delegate {
				UpdateTrayMenu();
			};
			contextMenuStrip.Items.Add(refreshItem);

			var exitItem = new ToolStripMenuItem("End");
			exitItem.BackColor = Color.FromArgb(45, 45, 48);
			exitItem.ForeColor = Color.White;
			exitItem.Click += delegate {
				notifyIcon.Visible = false;
				Application.Exit();
			};
			contextMenuStrip.Items.Add(exitItem);
		}

		private void ShowWindow() {
			form.Show();
			form.WindowState = FormWindowState.Normal;
			form.ShowInTaskbar = true;
			form.BringToFront();
		}

		public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
		{
			notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
		}

		public void Dispose() {
			if (notifyIcon != null)
				notifyIcon.Dispose();

			if (contextMenuStrip != null)
				contextMenuStrip.Dispose();
		}
	}
}
