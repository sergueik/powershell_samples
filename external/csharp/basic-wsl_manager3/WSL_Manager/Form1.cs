using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WSL_Manager.Services;
using WslManagerFramework.Models;
using WslManagerFramework.Services;
using WslManagerFramework.UI;

namespace WslManagerFramework
{
	public partial class Form1 : Form
	{
		private readonly TabControl _tabControl = new TabControl();
		private readonly FlowLayoutPanel _panel = new FlowLayoutPanel();
		private readonly Button _refresh = new Button();
		private readonly Label _status = new Label();
		private readonly ToolTip _tip = new ToolTip();
		private readonly TextBox _searchBox = new TextBox();
		private readonly Label _searchLabel = new Label();
		private readonly RichTextBox _logBox = new RichTextBox();
		private string[] _allDistros = new string[0];
		private readonly Dictionary<string, DistroRow> _distroRows = new Dictionary<string, DistroRow>();
		private readonly DescriptionStore _store = new DescriptionStore("WslLauncher");
		// 保存先: %AppData%\WslLauncher\descriptions.json
		private readonly FavoriteStore _fav = new FavoriteStore("WslLauncher");
		// %AppData%\WslLauncher\favorites.json

		private CheckBox _runAtStartupCheckBox;
		private bool _isApplyingUi;
		// 反映中フラグ（イベント再入防止）



		// サービス
		private LogService _logService;
		private TrayService _trayService;
        
		// 設定関連
		private AppSettings _settings;
		private CheckBox _minimizeToTrayCheckBox;
        
		// インストール関連
		private FlowLayoutPanel _installPanel = new FlowLayoutPanel();
		private Button _refreshAvailable = new Button();
		private Label _installStatus = new Label();
		private List<AvailableDistro> _availableDistros = new List<AvailableDistro>();
		private Dictionary<string, InstallProgress> _activeInstalls = new Dictionary<string, InstallProgress>();

		public Form1()
		{
			InitializeComponent();

			Text = "WSL Manager";
			Width = 800;
			Height = 500;

			// 設定を読み込み
			_settings = AppSettings.Load();

			// サービス初期化（_logBoxが初期化される前なのでここでは初期化しない）

			// ウィンドウアイコンの設定
			var iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "tax.ico");
			if (System.IO.File.Exists(iconPath)) {
				Icon = new Icon(iconPath);
			}

			// ダークテーマの設定
			BackColor = Color.FromArgb(45, 45, 48);
			ForeColor = Color.White;

			InitializeTabs();
		}

		private void InitializeTabs()
		{
			// TabControl の設定
			_tabControl.Dock = DockStyle.Fill;
			_tabControl.BackColor = Color.FromArgb(45, 45, 48);
			_tabControl.ForeColor = Color.White;
			Controls.Add(_tabControl);

			// メインタブ
			var mainTab = new TabPage("コンソール");
			mainTab.BackColor = Color.FromArgb(45, 45, 48);
			mainTab.ForeColor = Color.White;
			_tabControl.TabPages.Add(mainTab);

			// インストールタブ
			var installTab = new TabPage("インストール");
			installTab.BackColor = Color.FromArgb(45, 45, 48);
			installTab.ForeColor = Color.White;
			_tabControl.TabPages.Add(installTab);

			// 設定タブ
			var settingsTab = new TabPage("設定");
			settingsTab.BackColor = Color.FromArgb(45, 45, 48);
			settingsTab.ForeColor = Color.White;
			_tabControl.TabPages.Add(settingsTab);

			InitializeMainTab(mainTab);
			InitializeInstallTab(installTab);
			InitializeSettingsTab(settingsTab);
            
			// デフォルトでメインタブを選択
			_tabControl.SelectedIndex = 0;
            
			// サービス初期化（_logBoxが初期化された後）
			_logService = new LogService(_logBox);
			_trayService = new TrayService(this, _logService);
            
			// ウィンドウイベントの設定
			WindowState = FormWindowState.Normal;
			ShowInTaskbar = true;
			Resize += Form1_Resize;
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;

			// フォーム表示時に初回読み込み（１回だけ実行）
			Shown += OnFormShown;
		}

		private void InitializeMainTab(TabPage mainTab)
		{
			// 上部バー
			_refresh.Text = "↺";
			_refresh.Size = new Size(35, 30);
			_refresh.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
			_refresh.BackColor = Color.FromArgb(63, 63, 70);
			_refresh.ForeColor = Color.White;
			_refresh.FlatStyle = FlatStyle.Flat;
			_refresh.FlatAppearance.BorderColor = Color.FromArgb(104, 104, 104);
			_refresh.Click += (_, __) => Reload();
			_tip.SetToolTip(_refresh, "ディストリリストを手動更新");

			_searchLabel.Text = "検索:";
			_searchLabel.AutoSize = true;
			_searchLabel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			_searchLabel.ForeColor = Color.White;
			_searchLabel.Margin = new Padding(8, 12, 4, 8);

			_searchBox.Width = 200;
			_searchBox.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			_searchBox.BackColor = Color.FromArgb(62, 62, 66);
			_searchBox.ForeColor = Color.White;
			_searchBox.BorderStyle = BorderStyle.FixedSingle;
			_searchBox.Margin = new Padding(4, 8, 8, 8);
			_searchBox.TextChanged += (_, __) => FilterDistros();

			_status.AutoSize = true;
			_status.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			_status.ForeColor = Color.FromArgb(204, 204, 204);
			_status.Margin = new Padding(8);

			var top = new FlowLayoutPanel {
				Dock = DockStyle.Top,
				Height = 48,
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(8),
				AutoSize = true,
				BackColor = Color.FromArgb(45, 45, 48)
			};
			top.Controls.Add(_refresh);
			top.Controls.Add(_searchLabel);
			top.Controls.Add(_searchBox);
			top.Controls.Add(_status);

			// 一覧エリア
			_panel.Dock = DockStyle.Fill;
			_panel.FlowDirection = FlowDirection.TopDown;
			_panel.WrapContents = false;
			_panel.AutoScroll = true;
			_panel.Padding = new Padding(8);
			_panel.BackColor = Color.FromArgb(37, 37, 38);

			// ログエリア
			_logBox.Dock = DockStyle.Bottom;
			_logBox.Height = 90;
			_logBox.ReadOnly = true;
			_logBox.BackColor = Color.FromArgb(30, 30, 30);
			_logBox.ForeColor = Color.White;
			_logBox.Font = new Font("Consolas", 9F, FontStyle.Regular);
			_logBox.BorderStyle = BorderStyle.FixedSingle;
			_logBox.ScrollBars = RichTextBoxScrollBars.Vertical;

			mainTab.Controls.Add(_panel);
			mainTab.Controls.Add(_logBox);
			mainTab.Controls.Add(top);
		}

		private void InitializeInstallTab(TabPage installTab)
		{
			// 上部バー
			_refreshAvailable.Text = "↺";
			_refreshAvailable.Size = new Size(35, 30);
			_refreshAvailable.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
			_refreshAvailable.BackColor = Color.FromArgb(63, 63, 70);
			_refreshAvailable.ForeColor = Color.White;
			_refreshAvailable.FlatStyle = FlatStyle.Flat;
			_refreshAvailable.FlatAppearance.BorderColor = Color.FromArgb(104, 104, 104);
			_refreshAvailable.Click += (_, __) => RefreshAvailableDistros();
			_tip.SetToolTip(_refreshAvailable, "利用可能なディストリ一覧を更新");

			_installStatus.AutoSize = true;
			_installStatus.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			_installStatus.ForeColor = Color.FromArgb(204, 204, 204);
			_installStatus.Margin = new Padding(8);
			_installStatus.Text = "利用可能なディストリ一覧";

			var installTop = new FlowLayoutPanel {
				Dock = DockStyle.Top,
				Height = 48,
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(8),
				AutoSize = true,
				BackColor = Color.FromArgb(45, 45, 48)
			};
			installTop.Controls.Add(_refreshAvailable);
			installTop.Controls.Add(_installStatus);

			// インストール可能ディストリ一覧エリア
			_installPanel.Dock = DockStyle.Fill;
			_installPanel.FlowDirection = FlowDirection.TopDown;
			_installPanel.WrapContents = false;
			_installPanel.AutoScroll = true;
			_installPanel.Padding = new Padding(8);
			_installPanel.BackColor = Color.FromArgb(37, 37, 38);

			installTab.Controls.Add(_installPanel);
			installTab.Controls.Add(installTop);

			// 初期データを設定
			InitializeAvailableDistros();
			DisplayAvailableDistros();
		}

		private void InitializeSettingsTab(TabPage settingsTab)
		{
			var settingsPanel = new Panel {
				Dock = DockStyle.Fill,
				Padding = new Padding(20),
				BackColor = Color.FromArgb(37, 37, 38)
			};

			// タスクトレイ最小化設定
			var minimizeLabel = new Label {
				Text = "ウィンドウ設定",
				Location = new Point(20, 20),
				Size = new Size(200, 25),
				Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent
			};

			_minimizeToTrayCheckBox = new CheckBox {
				Text = "最小化時にタスクトレイに格納する",
				Location = new Point(20, 60),
				Size = new Size(300, 25),
				Font = new Font("Microsoft Sans Serif", 10F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Checked = _settings.MinimizeToTray
			};
			_minimizeToTrayCheckBox.CheckedChanged += OnMinimizeToTrayChanged;

			_runAtStartupCheckBox = new CheckBox {
				Text = "Windows サインイン時に自動起動する",
				Location = new Point(20, 80),
				Size = new Size(350, 25),
				Font = new Font("Microsoft Sans Serif", 10F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Checked = _settings.RunAtStartup
			};
			_runAtStartupCheckBox.CheckedChanged += OnRunAtStartupChanged;

			var saveButton = new Button {
				Text = "設定を保存",
				Location = new Point(20, 120),
				Size = new Size(120, 35),
				Font = new Font("Microsoft Sans Serif", 10F),
				BackColor = Color.FromArgb(0, 122, 204),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat
			};
			saveButton.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			saveButton.Click += OnSaveSettingsClick;

			var resetButton = new Button {
				Text = "初期設定に戻す",
				Location = new Point(150, 120),
				Size = new Size(120, 35),
				Font = new Font("Microsoft Sans Serif", 10F),
				BackColor = Color.FromArgb(217, 83, 79),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat
			};
			resetButton.FlatAppearance.BorderColor = Color.FromArgb(217, 83, 79);
			resetButton.Click += OnResetSettingsClick;

			settingsPanel.Controls.Add(minimizeLabel);
			settingsPanel.Controls.Add(_minimizeToTrayCheckBox);

			settingsPanel.Controls.Add(_runAtStartupCheckBox);

			settingsPanel.Controls.Add(saveButton);
			settingsPanel.Controls.Add(resetButton);

			settingsTab.Controls.Add(settingsPanel);
		}

		private void OnRunAtStartupChanged(object sender, EventArgs e)
		{
			if (_isApplyingUi)
				return; // プログラム側でチェックをいじった時は無視

			try {
				// レジストリへ反映（必要なら引数を追加: "--minimized" 等）
				StartupRegistrar.SetEnabled(_runAtStartupCheckBox.Checked /*, args: "--minimized"*/);

				// 設定モデルへ反映＆保存
				_settings.RunAtStartup = _runAtStartupCheckBox.Checked;
				_settings.Save();
			} catch (Exception ex) {
				MessageBox.Show(String.Format("自動起動の更新に失敗しました。\n{0}", ex.Message),
					"エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

				// 実レジストリ状態にUIを戻す（再入防止しながら）
				_isApplyingUi = true;
				_runAtStartupCheckBox.Checked = StartupRegistrar.IsEnabled();
				_isApplyingUi = false;
			}
		}


		private void OnMinimizeToTrayChanged(object sender, EventArgs e)
		{
			_settings.MinimizeToTray = _minimizeToTrayCheckBox.Checked;
		}

		private void OnSaveSettingsClick(object sender, EventArgs e)
		{
			_settings.Save();
			MessageBox.Show("設定を保存しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void OnResetSettingsClick(object sender, EventArgs e)
		{
			var result = MessageBox.Show("設定を初期値に戻しますか？", "確認", 
				             MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
			if (result == DialogResult.Yes) {
				_settings = new AppSettings();
				_minimizeToTrayCheckBox.Checked = _settings.MinimizeToTray;
				MessageBox.Show("設定を初期値に戻しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private bool _initialLoadCompleted = false;

		private void Form1_Load(object sender, EventArgs e)
		{
			// null-conditional elvis operator is not available in C# 5.0
			// _logService?.AddLog("フォームが読み込まれました。", Color.LightYellow);
			if (_logService != null)
				_logService.AddLog("フォームが読み込まれました。", Color.LightYellow);
		}

		private async void OnFormShown(object sender, EventArgs e)
		{
			if (_initialLoadCompleted)
				return;

			_initialLoadCompleted = true;
			if (_logService != null)
				_logService.AddLog("WSL Manager を起動しました。", Color.LightYellow);
			await Task.Delay(100);
			Reload();
			if (_trayService != null)
				_trayService.UpdateTrayMenu();
		}

		private void InitializeAvailableDistros()
		{
			RefreshAvailableDistros();
		}

		private void RefreshAvailableDistros()
		{
			try {
				_installStatus.Text = "確認中...";
                
				_availableDistros = WslInstallService.GetAvailableDistros();
				var installedDistros = WslService.ListDistros();
                
				foreach (var distro in _availableDistros) {
					distro.IsInstalled = installedDistros.Contains(distro.Name, StringComparer.OrdinalIgnoreCase);
				}
                
				DisplayAvailableDistros();
				_installStatus.Text = String.Format("利用可能なディストリ: {0}個", _availableDistros.Count);
                
				if (_availableDistros.Count == 0) {
					if (_logService != null)
						_logService.SafeAddLog("利用可能なディストリが見つかりませんでした。インターネット接続を確認してください。", Color.Orange);
				}
			} catch (Exception ex) {
				_installStatus.Text = "確認に失敗しました。";
				if (_logService != null)
					_logService.SafeAddLog(String.Format("利用可能ディストリ確認エラー: {0}", ex.Message), Color.LightCoral);
			}
		}

		private void DisplayAvailableDistros()
		{
			_installPanel.Controls.Clear();
            
			foreach (var distro in _availableDistros) {
				_installPanel.Controls.Add(MakeAvailableDistroRow(distro));
			}
		}

		private Control MakeAvailableDistroRow(AvailableDistro distro)
		{
			// このメソッドは簡略化されたバージョンで、基本的なインストール機能のみ提供
			var row = new Panel {
				Height = 60,
				Width = 750,
				Padding = new Padding(6),
				Margin = new Padding(2),
				BackColor = Color.FromArgb(51, 51, 55),
			};

			var lblName = new Label {
				Text = distro.FriendlyName,
				Location = new Point(8, 8),
				Size = new Size(250, 18),
				Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
			};

			var lblDesc = new Label {
				Text = distro.Description,
				Location = new Point(8, 28),
				Size = new Size(400, 16),
				Font = new Font("Microsoft Sans Serif", 7.5F, FontStyle.Regular),
				ForeColor = Color.LightGray,
				BackColor = Color.Transparent,
			};

			var btnInstall = new Button {
				Text = "インストール",
				Location = new Point(600, 12),
				Size = new Size(80, 32),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
				BackColor = Color.FromArgb(0, 122, 204),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleCenter,
			};
			btnInstall.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			btnInstall.Click += async (_, __) => {
				try {
					btnInstall.Enabled = false;
					btnInstall.Text = "インストール中...";
					if (_logService != null)
						_logService.SafeAddLog(String.Format("{0} のインストール開始...", distro.FriendlyName), Color.Yellow);
                    
					await Task.Run(() => WslInstallService.InstallDistro(distro.Name));
					if (_logService != null)
						_logService.SafeAddLog(String.Format("{0} のインストールが完了しました。", distro.FriendlyName), Color.LightGreen);
					Reload();
				} catch (Exception ex) {
					if (_logService != null)
						_logService.SafeAddLog(String.Format("{0} のインストールに失敗: {1}", distro.FriendlyName, ex.Message), Color.LightCoral);
				} finally {
					btnInstall.Text = "インストール";
					btnInstall.Enabled = true;
				}
			};

			row.Controls.Add(lblName);
			row.Controls.Add(lblDesc);
			row.Controls.Add(btnInstall);

			return row;
		}

		private void Reload()
		{
			try {
				UseWaitCursor = true;
				_status.Text = "取得中...";
                
				// 新しいディストリリストを取得
				var newDistros = WslService.ListDistros();
                
				// 新しいディストリが追加された場合のみ画面を再構築
				if (_allDistros.Length != newDistros.Length || !_allDistros.SequenceEqual(newDistros)) {
					_panel.Controls.Clear();
					_distroRows.Clear();
					_allDistros = newDistros;
                    
					if (_allDistros.Length == 0) {
						_status.Text = "WSL ディストリが見つかりません。";
						return;
					}
                    
					DisplayDistros(_allDistros);
				} else {
					// ディストリリストが変わっていない場合は状態のみ更新
					foreach (var distroName in _allDistros) {
						UpdateDistroStatus(distroName);
					}
					ResortAllInContainer(_panel);
				}
                
				_status.Text = String.Format("ディストリ数: {0}", _allDistros.Length);
				if (_trayService != null)
					_trayService.UpdateTrayMenu();
			} catch (Exception ex) {
				_status.Text = "取得に失敗しました。";
				MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} finally {
				UseWaitCursor = false;
			}
		}

		private void DisplayDistros(string[] distros)
		{
			_panel.Controls.Clear();
			_distroRows.Clear();
			foreach (var name in distros) {
				_panel.Controls.Add(MakeDistroRow(name));

			}
			ResortAllInContainer(_panel);
		}

		private void UpdateDistroStatus(string distroName)
		{
			if (_distroRows.ContainsKey(distroName)) {
				var newStatus = WslService.GetDistroStatus(distroName);
				_distroRows[distroName].UpdateStatus(newStatus);
				if (_trayService != null)
				if (_trayService != null)
					_trayService.UpdateTrayMenu();
			}
		}

		private void FilterDistros()
		{
			if (_allDistros.Length == 0)
				return;

			var searchText = _searchBox.Text.ToLower();
			if (string.IsNullOrWhiteSpace(searchText)) {
				DisplayDistros(_allDistros);
				_status.Text = String.Format("ディストリ数: {0}", _allDistros.Length);
			} else {
				var filtered = _allDistros.Where(d => d.ToLower().Contains(searchText)).ToArray();
				DisplayDistros(filtered);
				_status.Text = String.Format("ディストリ数: {0} / {1} (検索: \"{2}\")", filtered.Length, _allDistros.Length, _searchBox.Text);
			}
		}

		private Control MakeDistroRow(string distroName)
		{
			var row = new Panel {
				Height = 40,
				Width = 750,
				Padding = new Padding(4),
				Margin = new Padding(3),
				BackColor = Color.FromArgb(51, 51, 55),
				Tag = distroName
			};

			var isFav = _fav.IsFavorite(distroName);
			var lblStar = new Label {
				Text = isFav ? "★" : "☆",
				Location = new Point(6, 8),
				Size = new Size(20, 22),
				Font = new Font("Segoe UI", 12F, FontStyle.Regular),
				ForeColor = isFav ? Color.Gold : Color.Gray,
				BackColor = Color.Transparent,
				TextAlign = ContentAlignment.MiddleCenter,
				Cursor = Cursors.Hand
			};
			lblStar.Click += (_, __) => {
				bool nowFav = !_fav.IsFavorite(distroName);
				_fav.SetFavorite(distroName, nowFav);

				lblStar.Text = nowFav ? "★" : "☆";
				lblStar.ForeColor = nowFav ? Color.Gold : Color.Gray;

				// ★ 同じコンテナ内で即座に上部配置へ
				ResortRowsInSameContainer(row);
			};

			var lbl = new Label {
				Text = distroName,
				Location = new Point(28, 8),
				Size = new Size(180, 22),
				Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Regular),
				TextAlign = ContentAlignment.MiddleLeft,
				ForeColor = Color.White,
				BackColor = Color.Transparent,
			};

			var status = WslService.GetDistroStatus(distroName);
			var lblStatus = new Label {
				Text = status,
				Location = new Point(200, 8),
				Size = new Size(80, 22),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleCenter,
				BackColor = Color.Transparent,
			};

			switch (status.ToLower()) {
				case "running":
					lblStatus.ForeColor = Color.FromArgb(92, 184, 92);
					lblStatus.Text = "Running";
					break;
				case "stopped":
					lblStatus.ForeColor = Color.FromArgb(217, 83, 79);
					lblStatus.Text = "Stopped";
					break;
				default:
					lblStatus.ForeColor = Color.FromArgb(204, 204, 204);
					lblStatus.Text = "Unknown";
					break;
			}

			var btnBackground = new Button {
				Location = new Point(290, 4),
				Size = new Size(60, 28),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleCenter,
			};

			if (status.ToLower() == "running") {
				btnBackground.Text = "停止";
				btnBackground.BackColor = Color.FromArgb(217, 83, 79);
				btnBackground.FlatAppearance.BorderColor = Color.FromArgb(217, 83, 79);
			} else {
				btnBackground.Text = "起動";
				btnBackground.BackColor = Color.FromArgb(46, 125, 50);
				btnBackground.FlatAppearance.BorderColor = Color.FromArgb(46, 125, 50);
			}

			var btnLaunch = new Button {
				Text = "cmdで開く",
				Location = new Point(360, 4),
				Size = new Size(120, 28),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
				BackColor = Color.FromArgb(0, 122, 204),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleCenter,
			};
			btnLaunch.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			btnLaunch.Click += (_, __) => {
				try {
					WslService.LaunchInCmd(distroName);
				} catch (Exception ex) {
					MessageBox.Show(String.Format("プロセス起動エラー: {0}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};

			var btnDropdown = new Button {
				Text = "▼",
				Location = new Point(480, 4),
				Size = new Size(25, 28),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
				BackColor = Color.FromArgb(0, 100, 180),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleCenter,
			};
			btnDropdown.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);

			var contextMenu = new ContextMenuStrip();
			contextMenu.BackColor = Color.FromArgb(45, 45, 48);
			contextMenu.ForeColor = Color.White;

			var menuItemCmd = new ToolStripMenuItem("cmdで開く");
			menuItemCmd.BackColor = Color.FromArgb(45, 45, 48);
			menuItemCmd.ForeColor = Color.White;
			menuItemCmd.Click += (_, __) => WslService.LaunchInCmd(distroName);

			var menuItemDirect = new ToolStripMenuItem("直接WSL起動（別コンソール）");
			menuItemDirect.BackColor = Color.FromArgb(45, 45, 48);
			menuItemDirect.ForeColor = Color.White;
			menuItemDirect.Click += (_, __) => WslService.LaunchWslDirect(distroName);

			contextMenu.Items.Add(menuItemCmd);
			contextMenu.Items.Add(menuItemDirect);

			btnDropdown.Click += (sender, e) => {
				var btn = sender as Button;
				contextMenu.Show(btn, 0, btn.Height);
			};

			_tip.SetToolTip(btnLaunch, "cmdで開きます");
			_tip.SetToolTip(btnDropdown, "他の起動方法を選択");

			var txtDescription = new TextBox {
				Text = "",
				Location = new Point(515, 4),
				Size = new Size(240, 28),
				Font = new Font("Microsoft Sans Serif", 13F, FontStyle.Regular)
               
			};

			txtDescription.Text = _store.Get(distroName) ?? "";

			row.Controls.Add(lblStar);
			row.Controls.Add(lbl);
			row.Controls.Add(lblStatus);
			row.Controls.Add(btnBackground);
			row.Controls.Add(btnLaunch);
			row.Controls.Add(btnDropdown);
			row.Controls.Add(txtDescription);

			txtDescription.TextChanged += (_, __) => {
				_store.Set(distroName, txtDescription.Text);
			};

			var distroRow = new DistroRow(distroName, row, lblStatus, btnBackground, btnLaunch, btnDropdown, txtDescription,
				                LaunchWslBackgroundAsync, StopWslAsync, UpdateDistroStatus, _tip);
			_distroRows[distroName] = distroRow;
			distroRow.UpdateStatus(status);

			return row;
		}

		private async Task LaunchWslBackgroundAsync(string distroName)
		{
			try {
				await WslService.LaunchWslBackgroundAsync(distroName);
				if (_logService != null)
					_logService.SafeAddLog(String.Format("{0} をバックグラウンドで起動しました。", distroName), Color.LightGreen);
			} catch (Exception ex) {
				if (_logService != null)
					_logService.SafeAddLog(String.Format("バックグラウンド起動エラー: {0}", ex.Message), Color.LightCoral);
			}
		}

		private async Task StopWslAsync(string distroName)
		{
			try {
				await WslService.StopWslAsync(distroName);
				if (_logService != null)
					_logService.SafeAddLog(String.Format("{0} を停止しました。", distroName), Color.LightBlue);
			} catch (Exception ex) {
				if (_logService != null)
					_logService.SafeAddLog(String.Format("WSL停止エラー: {0}", ex.Message), Color.LightCoral);
			}
		}

		// ★ 同じコンテナ内で「お気に入り→非お気に入り」、同順位は名前順に並べ替える
		private void ResortRowsInSameContainer(Control anyRowInContainer)
		{
			Control host = null;
        	
			if (anyRowInContainer != null)
				host = anyRowInContainer.Parent;
			// Embedded statement cannot be a declaration or labeled statement (CS1023) 
			// var host = anyRowInContainer.Parent;
			if (host == null)
				return;

			var rows = host.Controls.Cast<Control>().ToList();

			rows.Sort((a, b) => {
				var nameA = a.Tag as string ?? "";
				var nameB = b.Tag as string ?? "";

				int favA = _fav.IsFavorite(nameA) ? 0 : 1; // 0 が上・1 が下
				int favB = _fav.IsFavorite(nameB) ? 0 : 1;

				int cmp = favA.CompareTo(favB);
				if (cmp != 0)
					return cmp;

				return string.Compare(nameA, nameB, StringComparison.CurrentCultureIgnoreCase);
			});

			// Clear/Add でもOKですが、Z順だけ変えるなら SetChildIndex が安全
			host.SuspendLayout();
			// 最終並び rows[0] が最上に来るよう、逆順で index=0 に積む
			for (int i = rows.Count - 1; i >= 0; i--) {
				host.Controls.SetChildIndex(rows[i], 0);
			}
			host.ResumeLayout();
		}

		// 初期表示後に一度だけ全体を整列したい場合に使うユーティリティ
		public void ResortAllInContainer(Control host)
		{
			if (host == null)
				return;
			if (host.Controls.Count == 0)
				return;
			ResortRowsInSameContainer(host.Controls[0]);
		}


		private void Form1_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized && _settings.MinimizeToTray) {
				Hide();
				ShowInTaskbar = false;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing && _settings.MinimizeToTray) {
				e.Cancel = true;
				Hide();
				ShowInTaskbar = false;
			} else if (e.CloseReason == CloseReason.UserClosing) {
				if (_trayService != null)
					_trayService.Dispose();
			}
		}
	}
}