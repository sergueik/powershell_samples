using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WslManagerFramework.Services
{
	public class TrayService
	{
		private NotifyIcon _notifyIcon;
		private ContextMenuStrip _trayContextMenu;
		private Form _parentForm;
		private LogService _logService;

		public TrayService(Form parentForm, LogService logService)
		{
			_parentForm = parentForm;
			_logService = logService;
			InitializeTrayComponents();
		}

		private void InitializeTrayComponents()
		{
			_notifyIcon = new NotifyIcon();
			_trayContextMenu = new ContextMenuStrip();
            
			var iconPath = System.IO.Path.Combine(
				                        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
				                        "tax.ico");
                
			if (System.IO.File.Exists(iconPath)) {
				_notifyIcon.Icon = new Icon(iconPath);
			} else {
				_notifyIcon.Icon = SystemIcons.Information;
			}
            
			_notifyIcon.Text = "WSL Manager";
			_notifyIcon.Visible = true;
			_notifyIcon.DoubleClick += (_, __) => ShowWindow();
            
			_trayContextMenu.BackColor = Color.FromArgb(45, 45, 48);
			_trayContextMenu.ForeColor = Color.White;
			_notifyIcon.ContextMenuStrip = _trayContextMenu;
		}

		public void UpdateTrayMenu()
		{
			_trayContextMenu.Items.Clear();

			try {
				var distros = WslService.ListDistros();
                
				if (distros.Length > 0) {
					foreach (var distro in distros) {
						var status = WslService.GetDistroStatus(distro);
						var distroItem = new ToolStripMenuItem(distro);
						distroItem.BackColor = Color.FromArgb(45, 45, 48);
						distroItem.ForeColor = Color.White;

						if (status.ToLower() == "running") {
							var stopItem = new ToolStripMenuItem("停止");
							stopItem.BackColor = Color.FromArgb(45, 45, 48);
							stopItem.ForeColor = Color.LightCoral;
							stopItem.Click += async (_, __) => {
								try {
									await WslService.StopWslAsync(distro);
									UpdateTrayMenu();
								} catch (Exception ex) {
									if (_logService != null)
										_logService.SafeAddLog(String.Format("WSL停止エラー: {0}", ex.Message), Color.LightCoral);
								}
							};
							distroItem.DropDownItems.Add(stopItem);
						} else {
							var startItem = new ToolStripMenuItem("バックグラウンド起動");
							startItem.BackColor = Color.FromArgb(45, 45, 48);
							startItem.ForeColor = Color.LightGreen;
							startItem.Click += async (_, __) => {
								try {
									await WslService.LaunchWslBackgroundAsync(distro);
									UpdateTrayMenu();
									if (_logService != null)
										_logService.SafeAddLog(String.Format("{0} をバックグラウンドで起動しました。", distro), Color.LightGreen);
								} catch (Exception ex) {
									if (_logService != null)
										_logService.SafeAddLog(String.Format("バックグラウンド起動エラー: {0}", ex.Message), Color.LightCoral);
								}
							};
							distroItem.DropDownItems.Add(startItem);
						}

						var cmdItem = new ToolStripMenuItem("cmdで開く");
						cmdItem.BackColor = Color.FromArgb(45, 45, 48);
						cmdItem.ForeColor = Color.White;
						cmdItem.Click += (_, __) => {
							try {
								WslService.LaunchInCmd(distro);
							} catch (Exception ex) {
								if (_logService != null)
									_logService.SafeAddLog(String.Format("cmd起動エラー: {0}", ex.Message), Color.LightCoral);
							}
						};
						distroItem.DropDownItems.Add(cmdItem);

						var directItem = new ToolStripMenuItem("直接WSL起動");
						directItem.BackColor = Color.FromArgb(45, 45, 48);
						directItem.ForeColor = Color.White;
						directItem.Click += (_, __) => {
							try {
								WslService.LaunchWslDirect(distro);
							} catch (Exception ex) {
								if (_logService != null)
									_logService.SafeAddLog(String.Format("直接起動エラー: {0}", ex.Message), Color.LightCoral);
							}
						};
						distroItem.DropDownItems.Add(directItem);

						var statusText = status.ToLower() == "running" ? "Running" : "Stopped";
						var statusColor = status.ToLower() == "running" 
                            ? Color.LightGreen 
                            : Color.LightCoral;
						distroItem.Text = String.Format("{0} ({1})", distro, statusText);
						distroItem.ForeColor = statusColor;

						_trayContextMenu.Items.Add(distroItem);
					}

					_trayContextMenu.Items.Add(new ToolStripSeparator());
				}
			} catch (Exception ex) {
				if (_logService != null)
					_logService.SafeAddLog(String.Format("タスクトレイメニュー更新エラー: {0}", ex.Message), Color.LightCoral);
			}

			// 固定メニュー項目
			var showItem = new ToolStripMenuItem("ウィンドウを表示");
			showItem.BackColor = Color.FromArgb(45, 45, 48);
			showItem.ForeColor = Color.White;
			showItem.Click += (_, __) => ShowWindow();
			_trayContextMenu.Items.Add(showItem);

			var refreshItem = new ToolStripMenuItem("更新");
			refreshItem.BackColor = Color.FromArgb(45, 45, 48);
			refreshItem.ForeColor = Color.White;
			refreshItem.Click += (_, __) => UpdateTrayMenu();
			_trayContextMenu.Items.Add(refreshItem);

			var exitItem = new ToolStripMenuItem("終了");
			exitItem.BackColor = Color.FromArgb(45, 45, 48);
			exitItem.ForeColor = Color.White;
			exitItem.Click += (_, __) => {
				_notifyIcon.Visible = false;
				Application.Exit();
			};
			_trayContextMenu.Items.Add(exitItem);
		}

		private void ShowWindow()
		{
			_parentForm.Show();
			_parentForm.WindowState = FormWindowState.Normal;
			_parentForm.ShowInTaskbar = true;
			_parentForm.BringToFront();
		}

		public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
		{
			_notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
		}

		public void Dispose()
		{
			if (_notifyIcon != null)
				_notifyIcon.Dispose();

			if (_trayContextMenu != null)
				_trayContextMenu.Dispose();
		}
	}
}