using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Utils;


using WSL_Manager.Services;
using WslManagerFramework.Models;
using WslManagerFramework.Services;
using WslManagerFramework.UI;

namespace WslManagerFramework {
	public partial class Form1 : Form {

		private readonly TabControl tabControl = new TabControl();
		private readonly FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
		private readonly Panel toolbarPanel = new Panel();

		private readonly Button button1 = new Button();
		private readonly Label label1 = new Label();
		private readonly ToolTip toolTip = new ToolTip();
		private readonly TextBox textBox1 = new TextBox();
		private readonly Label label2 = new Label();
		private readonly RichTextBox richTextBox = new RichTextBox();
		private string[] _allDistros = new string[0];
		private readonly Dictionary<string, DistroRow> _distroRows = new Dictionary<string, DistroRow>();
		private readonly DescriptionStore descriptionStore = new DescriptionStore("WslLauncher");
		private readonly FavoriteStore favoriteStore = new FavoriteStore("WslLauncher");
		// %AppData%\WslLauncher\favorites.json

		private CheckBox _runAtStartupCheckBox;
		private bool _isApplyingUi;

		private LogService logService;
		private TrayService trayService;
        
		private AppSettings appSettings;
		private CheckBox checkBox1;
		private Label label3;
        
		private FlowLayoutPanel _installPanel = new FlowLayoutPanel();
		private Button button1Available = new Button();
		private List<AvailableDistro> availableDistroList = new List<AvailableDistro>();
		private Dictionary<string, InstallProgress> _activeInstalls = new Dictionary<string, InstallProgress>();

		private ImageButton imageButton1;
		private ImageButton imageButton2;
		private ImageButton imageButton4;
		private ImageButton imageButton5;
		private ImageButton imageButton6;
		private ImageButton imageButton7;
		private ImageButton imageButton8;
		
		public Form1() {
			InitializeComponent();

			Text = "WSL Manager";
			Width =1280;
			Height = 640;

			appSettings = AppSettings.Load();

			var iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "tax.ico");
			if (System.IO.File.Exists(iconPath)) {
				Icon = new Icon(iconPath);
			}

			BackColor = Color.FromArgb(45, 45, 48);
			ForeColor = Color.White;

			InitializeTabs();
		}

		private void InitializeToolbarPanel() {
			toolbarPanel.SuspendLayout();
			imageButton1 = new ImageButton();
			imageButton2 = new ImageButton();
			imageButton4 = new ImageButton();
			imageButton5 = new ImageButton();
			imageButton6 = new ImageButton();
			imageButton7 = new ImageButton();
			imageButton8 = new ImageButton();

			((ISupportInitialize)(imageButton1)).BeginInit();
			((ISupportInitialize)(imageButton2)).BeginInit();
			((ISupportInitialize)(imageButton4)).BeginInit();
			((ISupportInitialize)(imageButton5)).BeginInit();
			((ISupportInitialize)(imageButton6)).BeginInit();
			((ISupportInitialize)(imageButton7)).BeginInit();
			((ISupportInitialize)(imageButton8)).BeginInit();
			// 
			// imageButton1
			// 
			imageButton1.DialogResult = DialogResult.None;
			imageButton1.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton1.Font = new Font("Segoe UI", 28F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton1.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton1.Location = new Point(22, 22);
			imageButton1.Margin = new Padding(6);
			imageButton1.Name = "imageButton1";
			imageButton1.NormalImage = null;
			imageButton1.Size = new Size(100, 50);
			imageButton1.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton1.TabIndex = 0;
			imageButton1.TabStop = false;
			imageButton1.Text = "+";
			imageButton1.Click += new EventHandler(imageButton1_Click);
			
			// 
			// imageButton2
			// 
			imageButton2.DialogResult = DialogResult.None;
			imageButton2.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton2.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton2.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton2.Location = new Point(216, 22);
			imageButton2.Margin = new Padding(6);
			imageButton2.Name = "imageButton2";
			imageButton2.NormalImage = null;
			imageButton2.Size = new Size(100, 50);
			imageButton2.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton2.TabIndex = 1;
			imageButton2.TabStop = true;
			imageButton2.Text = "\uE174";
			imageButton2.Click += new EventHandler(imageButton2_Click);
			// 
			// imageButton4
			// 
			imageButton4.DialogResult = DialogResult.None;
			imageButton4.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton4.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton4.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton4.Location = new Point(411, 22);
			imageButton4.Margin = new Padding(6);
			imageButton4.Name = "imageButton4";
			imageButton4.NormalImage = null;
			imageButton4.Size = new Size(100, 50);
			imageButton4.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton4.TabIndex = 4;
			imageButton4.TabStop = false;
			imageButton4.Text = "\uE102";
			imageButton4.Click += new EventHandler(imageButton4_Click);
			// 
			// imageButton5
			// 
			imageButton5.DialogResult = DialogResult.None;
			imageButton5.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton5.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton5.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton5.Location = new Point(605, 22);
			imageButton5.Margin = new Padding(6);
			imageButton5.Name = "imageButton5";
			imageButton5.NormalImage = null;
			imageButton5.Size = new Size(100, 50);
			imageButton5.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton5.TabIndex = 5;
			imageButton5.TabStop = true;
			imageButton5.Text = "\uE103";
			imageButton5.Click += new EventHandler(imageButton5_Click);
			// 
			// imageButton6
			// 
			imageButton6.DialogResult = DialogResult.None;
			imageButton6.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton6.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton6.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton6.Location = new Point(801, 22);
			imageButton6.Margin = new Padding(6);
			imageButton6.Name = "imageButton6";
			imageButton6.NormalImage = null;
			imageButton6.Size = new Size(100, 50);
			imageButton6.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton6.TabIndex = 6;
			imageButton6.TabStop = true;
			imageButton6.Text = "\uE184";
			imageButton6.Click += new EventHandler(imageButton6_Click);
			// 
			// imageButton7
			// 
			imageButton7.DialogResult = DialogResult.None;
			imageButton7.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton7.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton7.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton7.Location = new Point(979, 22);
			imageButton7.Margin = new Padding(6);
			imageButton7.Name = "imageButton7";
			imageButton7.NormalImage = null;
			imageButton7.Size = new Size(100, 50);
			imageButton7.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton7.TabIndex = 7;
			imageButton7.TabStop = true;
			imageButton7.Text = "\uE179";
			imageButton7.Click += new EventHandler(imageButton7_Click);

			// 
			// imageButton8
			// 
			imageButton8.DialogResult = DialogResult.None;
			imageButton8.DownImage = global::Utils.Properties.Resources.ExampleButtonDownA;
			imageButton8.Font = new Font("Segoe UI", 19F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
			imageButton8.HoverImage = global::Utils.Properties.Resources.ExampleButtonHoverA;
			imageButton8.Location = new Point(979, 22);
			imageButton8.Margin = new Padding(6);
			imageButton8.Name = "imageButton8";
			imageButton8.NormalImage = null;
			imageButton8.Size = new Size(100, 50);
			imageButton8.SizeMode = PictureBoxSizeMode.AutoSize;
			imageButton8.TabIndex = 7;
			imageButton8.TabStop = true;
			imageButton8.Text = "\uE107";
			imageButton8.Click += new EventHandler(imageButton8_Click);

			// AutoScaleDimensions = new SizeF(11F, 24F);
			// AutoScaleMode = AutoScaleMode.Font;
			// ClientSize = new Size(1280, 640);
			toolbarPanel.Dock = DockStyle.Top;
			// toolbarPanel.Height = 80;
			toolbarPanel.BackColor = Color.FromArgb(45, 45, 48);			
			toolbarPanel.ClientSize = new Size(1280, 80);
			toolbarPanel.Controls.Add(imageButton1);
			toolbarPanel.Controls.Add(imageButton2);
			toolbarPanel.Controls.Add(imageButton4);
			toolbarPanel.Controls.Add(imageButton5);
			toolbarPanel.Controls.Add(imageButton6);
			toolbarPanel.Controls.Add(imageButton7);
			toolbarPanel.Controls.Add(imageButton8);
			((ISupportInitialize)(imageButton1)).EndInit();
			((ISupportInitialize)(imageButton2)).EndInit();
			((ISupportInitialize)(imageButton4)).EndInit();
			((ISupportInitialize)(imageButton5)).EndInit();
			((ISupportInitialize)(imageButton6)).EndInit();
			((ISupportInitialize)(imageButton7)).EndInit();
			((ISupportInitialize)(imageButton8)).EndInit();
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Margin = new Padding(6);
			MaximizeBox = false;
    		toolbarPanel.ResumeLayout();

		}
		private void InitializeTabs() {
			
			toolbarPanel.Dock = DockStyle.Top;
			// toolbarPanel.Height = 80;
    		toolbarPanel.ClientSize = new Size(1280, 80);
	
    		toolbarPanel.BackColor = Color.FromArgb(45, 45, 48);
    		InitializeToolbarPanel();
			ClientSize = new Size(1280, 640);

			tabControl.Dock = DockStyle.Fill;
			tabControl.BackColor = Color.FromArgb(45, 45, 48);
			tabControl.ForeColor = Color.White;
			tabControl.ClientSize = new Size(1280, 580);

			Controls.Add(tabControl);
		    Controls.Add(toolbarPanel);
			toolbarPanel.BringToFront();
			tabControl.SendToBack();
			PerformLayout();
			// Main Tab
			var mainTab = new TabPage("Console");
			mainTab.BackColor = Color.FromArgb(45, 45, 48);
			mainTab.ForeColor = Color.White;
			tabControl.TabPages.Add(mainTab);

			// Install tab
			var installTab = new TabPage("Install");
			installTab.BackColor = Color.FromArgb(45, 45, 48);
			installTab.ForeColor = Color.White;
			tabControl.TabPages.Add(installTab);

			// Settings tab
			var settingsTab = new TabPage("Settings");
			settingsTab.BackColor = Color.FromArgb(45, 45, 48);
			settingsTab.ForeColor = Color.White;
			tabControl.TabPages.Add(settingsTab);

			InitializeMainTab(mainTab);
			InitializeInstallTab(installTab);
			InitializeSettingsTab(settingsTab);
			tabControl.PerformLayout();
            
			// the main tab is select by default.
			tabControl.SelectedIndex = 0;
            
			logService = new LogService(richTextBox);
			trayService = new TrayService(this, logService);
            
			WindowState = FormWindowState.Normal;
			ShowInTaskbar = true;
			Resize += Form1_Resize;
			FormClosing += Form1_FormClosing;
			Load += Form1_Load;
			Shown += OnFormShown;
		}

		private void InitializeMainTab(TabPage mainTab) {
			button1.Text = "↺";
			button1.Size = new Size(35, 30);
			button1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
			button1.BackColor = Color.FromArgb(63, 63, 70);
			button1.ForeColor = Color.White;
			button1.FlatStyle = FlatStyle.Flat;
			button1.FlatAppearance.BorderColor = Color.FromArgb(104, 104, 104);
			button1.Click += (_, __) => Reload();
			toolTip.SetToolTip(button1, "Refresh distribution list.");

			label2.Text = "Search:";
			label2.AutoSize = true;
			label2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			label2.ForeColor = Color.White;
			label2.Margin = new Padding(8, 12, 4, 8);

			textBox1.Width = 200;
			textBox1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			textBox1.BackColor = Color.FromArgb(62, 62, 66);
			textBox1.ForeColor = Color.White;
			textBox1.BorderStyle = BorderStyle.FixedSingle;
			textBox1.Margin = new Padding(4, 8, 8, 8);
			textBox1.TextChanged += (_, __) => FilterDistros();

			label1.AutoSize = true;
			label1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			label1.ForeColor = Color.FromArgb(204, 204, 204);
			label1.Margin = new Padding(8);

			var top = new FlowLayoutPanel {
				Dock = DockStyle.Top,
				Height = 48,
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(8),
				AutoSize = true,
				BackColor = Color.FromArgb(45, 45, 48)
			};
			top.Controls.Add(button1);
			top.Controls.Add(label2);
			top.Controls.Add(textBox1);
			top.Controls.Add(label1);

			flowLayoutPanel.Dock = DockStyle.Fill;
			flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel.WrapContents = false;
			flowLayoutPanel.AutoScroll = true;
			flowLayoutPanel.Padding = new Padding(8);
			flowLayoutPanel.BackColor = Color.FromArgb(37, 37, 38);

			// Log panel
			richTextBox.Dock = DockStyle.Bottom;
			richTextBox.Height = 90;
			richTextBox.ReadOnly = true;
			richTextBox.BackColor = Color.FromArgb(30, 30, 30);
			richTextBox.ForeColor = Color.White;
			richTextBox.Font = new Font("Lucida Console", 9F, FontStyle.Regular);
			richTextBox.BorderStyle = BorderStyle.FixedSingle;
			richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;

			mainTab.Controls.Add(flowLayoutPanel);
			mainTab.Controls.Add(richTextBox);
			mainTab.Controls.Add(top);
		}

		private void InitializeInstallTab(TabPage installTab) {
			button1Available.Text = "↺"; // unicode
			button1Available.Size = new Size(35, 30);
			button1Available.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular);
			button1Available.BackColor = Color.FromArgb(63, 63, 70);
			button1Available.ForeColor = Color.White;
			button1Available.FlatStyle = FlatStyle.Flat;
			button1Available.FlatAppearance.BorderColor = Color.FromArgb(104, 104, 104);
			button1Available.Click += (_, __) => RefreshAvailableDistros();
			toolTip.SetToolTip(button1Available, "Update the list of available distributions");
			
			label3 = new Label();
			label3.AutoSize = true;
			label3.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular);
			label3.ForeColor = Color.FromArgb(204, 204, 204);
			label3.Margin = new Padding(8);
			label3.Text = "List of available distributions";

			var installTop = new FlowLayoutPanel {
				Dock = DockStyle.Top,
				Height = 48,
				FlowDirection = FlowDirection.LeftToRight,
				Padding = new Padding(8),
				AutoSize = true,
				BackColor = Color.FromArgb(45, 45, 48)
			};
			installTop.Controls.Add(button1Available);
			installTop.Controls.Add(label3);

			_installPanel.Dock = DockStyle.Fill;
			_installPanel.FlowDirection = FlowDirection.TopDown;
			_installPanel.WrapContents = false;
			_installPanel.AutoScroll = true;
			_installPanel.Padding = new Padding(8);
			_installPanel.BackColor = Color.FromArgb(37, 37, 38);

			installTab.Controls.Add(_installPanel);
			installTab.Controls.Add(installTop);

			InitializeAvailableDistros();
			DisplayAvailableDistros();
		}

		private void InitializeSettingsTab(TabPage settingsTab) {
			var settingsPanel = new Panel {
				Dock = DockStyle.Fill,
				Padding = new Padding(20),
				BackColor = Color.FromArgb(37, 37, 38)
			};

			// Task tray setting
			var minimizeLabel = new Label {
				Text = "Window Settings",
				Location = new Point(20, 20),
				Size = new Size(200, 25),
				Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent
			};

			checkBox1 = new CheckBox {
				Text = "Minimize to tray",
				Location = new Point(20, 60),
				Size = new Size(300, 25),
				Font = new Font("Microsoft Sans Serif", 10F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Checked = appSettings.MinimizeToTray
			};
			checkBox1.CheckedChanged += OnMinimizeToTrayChanged;

			_runAtStartupCheckBox = new CheckBox {
				Text = "Launch automatically after login",
				Location = new Point(20, 80),
				Size = new Size(350, 25),
				Font = new Font("Microsoft Sans Serif", 10F),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
				Checked = appSettings.RunAtStartup
			};
			_runAtStartupCheckBox.CheckedChanged += OnRunAtStartupChanged;

			var saveButton = new Button {
				Text = "Save settings",
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
				Text = "Reset",
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
			settingsPanel.Controls.Add(checkBox1);

			settingsPanel.Controls.Add(_runAtStartupCheckBox);

			settingsPanel.Controls.Add(saveButton);
			settingsPanel.Controls.Add(resetButton);

			settingsTab.Controls.Add(settingsPanel);
		}

		private void OnRunAtStartupChanged(object sender, EventArgs e) {
			if (_isApplyingUi)
				return; 
			try {
				// Write to the registry 
				// add arguments if necessary, e.g., "--minimized"
				StartupRegistrar.SetEnabled(_runAtStartupCheckBox.Checked /*, args: "--minimized"*/);

				// Apply config & save
				appSettings.RunAtStartup = _runAtStartupCheckBox.Checked;
				appSettings.Save();
			} catch (Exception ex) {
				MessageBox.Show(String.Format("Failed to update the auto-start configuration: {0}", ex.Message),
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

				// Revert the UI
				_isApplyingUi = true;
				_runAtStartupCheckBox.Checked = StartupRegistrar.IsEnabled();
				_isApplyingUi = false;
			}
		}


		private void OnMinimizeToTrayChanged(object sender, EventArgs e) {
			appSettings.MinimizeToTray = checkBox1.Checked;
		}

		private void OnSaveSettingsClick(object sender, EventArgs e) {
			appSettings.Save();
			MessageBox.Show("Settings saved.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void OnResetSettingsClick(object sender, EventArgs e) {
			var result = MessageBox.Show("Do you want to restore the settings？", "Confirmation", 
				             MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
			if (result == DialogResult.Yes) {
				appSettings = new AppSettings();
				checkBox1.Checked = appSettings.MinimizeToTray;
				MessageBox.Show("Settings have been restored", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private bool _initialLoadCompleted = false;

		private void Form1_Load(object sender, EventArgs e) {
			// null-conditional elvis operator is not available in C# 5.0
			// logService?.AddLog("フォームが読み込まれました。", Color.LightYellow);
			if (logService != null)
				logService.AddLog("The form has been loaded", Color.LightYellow);
		}

		private async void OnFormShown(object sender, EventArgs e) {
			if (_initialLoadCompleted)
				return;

			_initialLoadCompleted = true;
			if (logService != null)
				logService.AddLog("WSL Manager has been launched", Color.LightYellow);
			await Task.Delay(100);
			Reload();
			if (trayService != null)
				trayService.UpdateTrayMenu();
		}

		private void InitializeAvailableDistros() {
			DetectAvailableDistros();
			if (WslService.CheckWslStatus()) {
				RefreshAvailableDistros();
			}
		}

		private void RefreshAvailableDistros() {
			try {
				label3.Text = "Loading...";
				if (!WslService.CheckWslStatus()) {
					label3.Text  = "WSL is unavailable";
					return;
				}
				availableDistroList = WslInstallService.GetAvailableDistros();
				var installedDistros = WslService.ListDistros();
                
				foreach (var distro in availableDistroList) {
					distro.IsInstalled = installedDistros.Contains(distro.Name, StringComparer.OrdinalIgnoreCase);
				}
                
				DisplayAvailableDistros();
				label3.Text = String.Format("Available distributions: {0} entries", availableDistroList.Count);
                
				if (availableDistroList.Count == 0) {
					if (logService != null)
						logService.SafeAddLog("No available distributions were found. Please check your internet connection.", Color.Orange);
				}
			} catch (Exception ex) {
				label3.Text = "Verification failed";
				if (logService != null)
					logService.SafeAddLog(String.Format("Error inventory available distributions: {0}", ex.Message), Color.LightCoral);
			}
		}

		private void DisplayAvailableDistros() {
			_installPanel.Controls.Clear();
            
			foreach (var distro in availableDistroList) {
				_installPanel.Controls.Add(MakeAvailableDistroRow(distro));
			}
		}

		private Control MakeAvailableDistroRow(AvailableDistro distro) {
			var panel = new Panel {
				Height = 60,
				Width = 750,
				Padding = new Padding(6),
				Margin = new Padding(2),
				BackColor = Color.FromArgb(51, 51, 55),
			};

			var label1 = new Label {
				Text = distro.FriendlyName,
				Location = new Point(8, 8),
				Size = new Size(250, 18),
				Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold),
				ForeColor = Color.White,
				BackColor = Color.Transparent,
			};

			var label2 = new Label {
				Text = distro.Description,
				Location = new Point(8, 28),
				Size = new Size(400, 16),
				Font = new Font("Microsoft Sans Serif", 7.5F, FontStyle.Regular),
				ForeColor = Color.LightGray,
				BackColor = Color.Transparent,
			};

			var button = new Button {
				Text = "Install",
				Location = new Point(600, 12),
				Size = new Size(80, 32),
				Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular),
				BackColor = Color.FromArgb(0, 122, 204),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleCenter,
			};
			button.FlatAppearance.BorderColor = Color.FromArgb(0, 122, 204);
			button.Click += async (_, __) => {
				try {
					button.Enabled = false;
					button.Text = "Installing...";
					if (logService != null)
						logService.SafeAddLog(String.Format("Starting install of {0}", distro.FriendlyName), Color.Yellow);
                    
					await Task.Run(() => WslInstallService.InstallDistro(distro.Name));
					if (logService != null)
						logService.SafeAddLog(String.Format("Done install {0}", distro.FriendlyName), Color.LightGreen);
					Reload();
				} catch (Exception ex) {
					if (logService != null)
						logService.SafeAddLog(String.Format("Failed to install {0}: {1}", distro.FriendlyName, ex.Message), Color.LightCoral);
				} finally {
					button.Text = "Install";
					button.Enabled = true;
				}
			};

			panel.Controls.Add(label1);
			panel.Controls.Add(label2);
			panel.Controls.Add(button);

			return panel;
		}

		private void Reload() {
			try {
				UseWaitCursor = true;
				label1.Text = "Loading...";
                
				// Recreate distribution list
				string[] newDistros = {};
				if (WslService.CheckWslStatus()) {
					newDistros = WslService.ListDistros();
				}
				// Cache
				if (_allDistros.Length != newDistros.Length || !_allDistros.SequenceEqual(newDistros)) {
					flowLayoutPanel.Controls.Clear();
					_distroRows.Clear();
					_allDistros = newDistros;
                    
					if (_allDistros.Length == 0) {
						label1.Text = "No WSL Distribution found";
						return;
					}
                    
					DisplayDistros(_allDistros);
				} else {
					// If the distribution list has not changed, update only the status
					foreach (var distroName in _allDistros) {
						UpdateDistroStatus(distroName);
					}
					ResortAllInContainer(flowLayoutPanel);
				}
                
				label1.Text = String.Format("Number of distributions: {0}", _allDistros.Length);
				if (trayService != null)
					trayService.UpdateTrayMenu();
			} catch (Exception ex) {
				label1.Text = "Load failed";
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} finally {
				UseWaitCursor = false;
			}
		}

		private void DisplayDistros(string[] distros) {
			flowLayoutPanel.Controls.Clear();
			_distroRows.Clear();
			foreach (var name in distros) {
				flowLayoutPanel.Controls.Add(MakeDistroRow(name));

			}
			ResortAllInContainer(flowLayoutPanel);
		}

		private void UpdateDistroStatus(string distroName) {
			if (_distroRows.ContainsKey(distroName)) {
				var newStatus = WslService.GetDistroStatus(distroName);
				_distroRows[distroName].UpdateStatus(newStatus);
				if (trayService != null)
				if (trayService != null)
					trayService.UpdateTrayMenu();
			}
		}

		private void FilterDistros() {
			if (_allDistros.Length == 0)
				return;

			var searchText = textBox1.Text.ToLower();
			if (string.IsNullOrWhiteSpace(searchText)) {
				DisplayDistros(_allDistros);
				label1.Text = String.Format("Number of distributions: {0}", _allDistros.Length);
			} else {
				var filtered = _allDistros.Where(d => d.ToLower().Contains(searchText)).ToArray();
				DisplayDistros(filtered);
				label1.Text = String.Format(@"Found: {0} / {1} (Searched: ""{2}"")", filtered.Length, _allDistros.Length, textBox1.Text);
			}
		}

		private Control MakeDistroRow(string distroName) {
			var row = new Panel {
				Height = 40,
				Width = 750,
				Padding = new Padding(4),
				Margin = new Padding(3),
				BackColor = Color.FromArgb(51, 51, 55),
				Tag = distroName
			};

			var isFav = favoriteStore.IsFavorite(distroName);
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
				bool nowFav = !favoriteStore.IsFavorite(distroName);
				favoriteStore.SetFavorite(distroName, nowFav);

				lblStar.Text = nowFav ? "★" : "☆";
				lblStar.ForeColor = nowFav ? Color.Gold : Color.Gray;

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
				btnBackground.Text = "Terminate";
				btnBackground.BackColor = Color.FromArgb(217, 83, 79);
				btnBackground.FlatAppearance.BorderColor = Color.FromArgb(217, 83, 79);
			} else {
				btnBackground.Text = "Boot";
				btnBackground.BackColor = Color.FromArgb(46, 125, 50);
				btnBackground.FlatAppearance.BorderColor = Color.FromArgb(46, 125, 50);
			}

			var btnLaunch = new Button {
				Text = "Open with cmd",
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
					MessageBox.Show(String.Format("Shell error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

			var menuItemCmd = new ToolStripMenuItem("Open with cmd");
			menuItemCmd.BackColor = Color.FromArgb(45, 45, 48);
			menuItemCmd.ForeColor = Color.White;
			menuItemCmd.Click += (_, __) => WslService.LaunchInCmd(distroName);

			var menuItemDirect = new ToolStripMenuItem("Launch WSL directly (new console)");
			menuItemDirect.BackColor = Color.FromArgb(45, 45, 48);
			menuItemDirect.ForeColor = Color.White;
			menuItemDirect.Click += (_, __) => WslService.LaunchWslDirect(distroName);

			contextMenu.Items.Add(menuItemCmd);
			contextMenu.Items.Add(menuItemDirect);

			btnDropdown.Click += (sender, e) => {
				var btn = sender as Button;
				contextMenu.Show(btn, 0, btn.Height);
			};

			toolTip.SetToolTip(btnLaunch, "Opens with cmd");
			toolTip.SetToolTip(btnDropdown, "Select another startup method");

			var txtDescription = new TextBox {
				Text = "",
				Location = new Point(515, 4),
				Size = new Size(240, 28),
				Font = new Font("Microsoft Sans Serif", 13F, FontStyle.Regular)
               
			};

			txtDescription.Text = descriptionStore.Get(distroName) ?? "";

			row.Controls.Add(lblStar);
			row.Controls.Add(lbl);
			row.Controls.Add(lblStatus);
			row.Controls.Add(btnBackground);
			row.Controls.Add(btnLaunch);
			row.Controls.Add(btnDropdown);
			row.Controls.Add(txtDescription);

			txtDescription.TextChanged += (_, __) => {
				descriptionStore.Set(distroName, txtDescription.Text);
			};

			var distroRow = new DistroRow(distroName, row, lblStatus, btnBackground, btnLaunch, btnDropdown, txtDescription,
				                LaunchWslBackgroundAsync, StopWslAsync, UpdateDistroStatus, toolTip);
			_distroRows[distroName] = distroRow;
			distroRow.UpdateStatus(status);

			return row;
		}

		private async Task LaunchWslBackgroundAsync(string name) {
			try {
				await WslService.LaunchWslBackgroundAsync(name);
				if (logService != null)
					logService.SafeAddLog(String.Format("{0} was launched in the background", name), Color.LightGreen);
			} catch (Exception ex) {
				if (logService != null)
					logService.SafeAddLog(String.Format("Background startup error: {0}", ex.Message), Color.LightCoral);
			}
		}

		private async Task StopWslAsync(string distroName) {
			try {
				await WslService.StopWslAsync(distroName);
				if (logService != null)
					logService.SafeAddLog(String.Format("{0} has been stopped", distroName), Color.LightBlue);
			} catch (Exception ex) {
				if (logService != null)
					logService.SafeAddLog(String.Format("WSL Stop error: {0}", ex.Message), Color.LightCoral);
			}
		}

		// remove items from "Favorite" and sort items
		private void ResortRowsInSameContainer(Control anyRowInContainer) {
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

				int favA = favoriteStore.IsFavorite(nameA) ? 0 : 1; 
				// 0 - ascending 1 - descending
				int favB = favoriteStore.IsFavorite(nameB) ? 0 : 1;

				int cmp = favA.CompareTo(favB);
				if (cmp != 0)
					return cmp;

				return string.Compare(nameA, nameB, StringComparison.CurrentCultureIgnoreCase);
			});

			host.SuspendLayout();
			for (int i = rows.Count - 1; i >= 0; i--) {
				host.Controls.SetChildIndex(rows[i], 0);
			}
			host.ResumeLayout();
		}

		public void ResortAllInContainer(Control host) {
			if (host == null)
				return;
			if (host.Controls.Count == 0)
				return;
			ResortRowsInSameContainer(host.Controls[0]);
		}


		private void Form1_Resize(object sender, EventArgs e) {
			if (WindowState == FormWindowState.Minimized && appSettings.MinimizeToTray) {
				Hide();
				ShowInTaskbar = false;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing && appSettings.MinimizeToTray) {
				e.Cancel = true;
				Hide();
				ShowInTaskbar = false;
			} else if (e.CloseReason == CloseReason.UserClosing) {
				if (trayService != null)
					trayService.Dispose();
			}
		}

		private bool DetectAvailableDistros(){
			List<DistroData> distroDataList =  new List<DistroData>();
			string registryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Lxss";

			try {
				if (logService != null)
					logService.AddLog("Examine Registry", Color.LightYellow);

				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath)) {
					if (key != null) {
						string[] subKeyNames = key.GetSubKeyNames();

						foreach (string subKeyName in subKeyNames) {
							using (RegistryKey subKey = key.OpenSubKey(subKeyName)) {
								if (subKey != null) {
									object basePathValue = subKey.GetValue("BasePath");
									object defaultUidValue = subKey.GetValue("DefaultUid");
									object distributionNameValue = subKey.GetValue("DistributionName");
									object stateValue = subKey.GetValue("State");
									object versionValue = subKey.GetValue("Version");
									object packageFamilyValue = subKey.GetValue("PackageFamilyName");

									if (basePathValue != null && defaultUidValue != null &&
									                               distributionNameValue != null && stateValue != null && versionValue != null) {
										string hash = subKeyName;
										string basePath = basePathValue.ToString();
										string defaultUid = defaultUidValue.ToString();
										string distributionName = distributionNameValue.ToString();
										string state = stateValue.ToString();
										string version = versionValue.ToString();
										string packageFamily = packageFamilyValue != null ? packageFamilyValue.ToString() : "";
										var distroData = new DistroData();
										distroData.DistroImage = "";
										distroData.DistroName = distributionName;
										Debug.WriteLine(String.Format("Detected entry: {0} {1}",subKeyName, basePath));
										if (logService != null)
										logService.AddLog(String.Format("Detected entry: {0} {1}",subKeyName, basePath), Color.LightYellow);
								
										distroDataList.Add(distroData);
									} else {
										
				if (logService != null)
					Debug.WriteLine(String.Format("Can't find distro info for '{0}' at registry on '{1}", subKeyName, registryPath));
					logService.SafeAddLog(String.Format("Can't find distro info for '{0}' at registry on '{1}", subKeyName, registryPath), Color.LightCoral);									}
								}
							}
						}

					} else {
						Debug.WriteLine("Can't find hashed keys for distros.");
						MessageBox.Show("Can't find hashed keys for distros.",
							"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			} catch (Exception e) {
				Debug.WriteLine(String.Format("Failed to read the registry key {0}: {1}", registryPath, e.Message));
				MessageBox.Show(String.Format("Failed to read the registry key {0}: {1}", registryPath, e.Message),
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

	        return (bool)(distroDataList.Count != 0 );
		}

		private void imageButton1_Click(object sender, EventArgs e) {
			
		    // Controls.Add(toolbarPanel);
			MessageBox.Show("New");
		}

		private void imageButton2_Click(object sender, EventArgs e) {
			Controls.Add(toolbarPanel);
			MessageBox.Show("Reload");
		}
		
		private void imageButton4_Click(object sender, EventArgs e) {
			MessageBox.Show("Start");
		}

		private void imageButton5_Click(object sender, EventArgs e) {
			MessageBox.Show("Stop");
		}

		private void imageButton6_Click(object sender, EventArgs e) {
			MessageBox.Show("Shell");
		}

		private void imageButton7_Click(object sender, EventArgs e) {
			MessageBox.Show("Configure");
		}
		private void imageButton8_Click(object sender, EventArgs e) {
			MessageBox.Show("Recycle");
		}
	}
	public class DistroData {
		public string DistroImage { get; set; }
		public string DistroName { get; set; }
		public string DistroState { get; set; }
		public int DistroWslVersion { get; set; }
		private string guid = "{9d51c0ac-cf84-46ab-bbfb-b8d417429ea5}"; 
		// key in HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Lxss
		public string Guid { get {return guid;} set {guid = value; } }
	}
}
