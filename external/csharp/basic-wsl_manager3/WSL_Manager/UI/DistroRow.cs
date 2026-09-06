using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WSL_Manager.Services;

namespace WslManagerFramework.UI
{
	public class DistroRow
	{
		public string DistroName { get; protected set; }
		public Panel Panel { get; protected set; }
		public Label StatusLabel { get; protected set; }
		public Button BackgroundButton { get; protected set; }
		public Button LaunchButton { get; protected set; }
		public Button DropdownButton { get; protected set; }
		// 'WslManagerFramework.UI.DistroRow.DropdownButton.get' must declare a body because it is not marked abstract or extern.
		// Automatically implemented properties must define both get and set accessors. (CS0840)

		public TextBox TxtDescription { get; set; }

		private Func<string, Task> _launchAction;
		private Func<string, Task> _stopAction;
		private System.Action<string> _updateStatusAction;
		private ToolTip _tooltip;

		public DistroRow(string distroName, Panel panel, Label statusLabel, Button backgroundButton, Button launchButton, Button dropdownButton, TextBox txtDescription,
			Func<string, Task> launchAction, Func<string, Task> stopAction, System.Action<string> updateStatusAction, ToolTip tooltip)
		{
			DistroName = distroName;
			Panel = panel;
			StatusLabel = statusLabel;
			BackgroundButton = backgroundButton;
			LaunchButton = launchButton;
			DropdownButton = dropdownButton;
			TxtDescription = txtDescription;
			_launchAction = launchAction;
			_stopAction = stopAction;
			_updateStatusAction = updateStatusAction;
			_tooltip = tooltip;
            
			BackgroundButton.Click += OnBackgroundButtonClick;
		}

		public void UpdateStatus(string newStatus)
		{
			StatusLabel.Text = newStatus;
            
			switch (newStatus.ToLower()) {
				case "running":
					StatusLabel.ForeColor = System.Drawing.Color.FromArgb(92, 184, 92);
					StatusLabel.Text = "Running";
					break;
				case "stopped":
					StatusLabel.ForeColor = System.Drawing.Color.FromArgb(217, 83, 79);
					StatusLabel.Text = "Stopped";
					break;
				default:
					StatusLabel.ForeColor = System.Drawing.Color.FromArgb(204, 204, 204);
					StatusLabel.Text = "Unknown";
					break;
			}

			UpdateBackgroundButton(newStatus);
		}

		private void UpdateBackgroundButton(string status)
		{
			BackgroundButton.Click -= OnBackgroundButtonClick;
            
			if (status.ToLower() == "running") {
				BackgroundButton.Text = "停止";
				BackgroundButton.BackColor = System.Drawing.Color.FromArgb(217, 83, 79);
				BackgroundButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(217, 83, 79);
				_tooltip.SetToolTip(BackgroundButton, "WSLを停止します");
			} else {
				BackgroundButton.Text = "起動";
				BackgroundButton.BackColor = System.Drawing.Color.FromArgb(46, 125, 50);
				BackgroundButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(46, 125, 50);
				_tooltip.SetToolTip(BackgroundButton, "WSLをバックグラウンドで起動します");
			}
            
			BackgroundButton.Click += OnBackgroundButtonClick;
		}

		private async void OnBackgroundButtonClick(object sender, EventArgs e)
		{
			BackgroundButton.Enabled = false;
			var originalText = BackgroundButton.Text;
			BackgroundButton.Text = "処理中...";
            
			try {
				if (originalText == "停止") {
					await _stopAction(DistroName);
				} else {
					await _launchAction(DistroName);
				}
                
				await Task.Delay(1000);
                
				if (BackgroundButton.InvokeRequired) {
					BackgroundButton.Invoke(new Action(() => _updateStatusAction(DistroName)));
				} else {
					_updateStatusAction(DistroName);
				}
			} finally {
				BackgroundButton.Enabled = true;
			}
		}
	}
}