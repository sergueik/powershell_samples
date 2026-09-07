using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WslManagerFramework.Models {
	[Serializable]
	public class AppSettings {
		// public bool MinimizeToTray { get; set; } = true;
		// public bool RunAtStartup { get; set; } = true;
		public bool minimizeToTray = true;
		public bool runAtStartup = true;
		public bool MinimizeToTray { get { return  minimizeToTray; } set { minimizeToTray = value; } }
		public bool RunAtStartup { get { return runAtStartup; } set { runAtStartup = value; } }
		private static string settingsPath = null;
		/*
        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WSLManager", "settings.xml");
*/
		public static AppSettings Load() {
			settingsPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"WSLManager", "settings.xml");
			try {
				if (!File.Exists(settingsPath))
					return new AppSettings();

				var serializer = new XmlSerializer(typeof(AppSettings));
				using (var reader = new FileStream(settingsPath, FileMode.Open)) {
					return (AppSettings)serializer.Deserialize(reader);
				}
			} catch {
				return new AppSettings();
			}
		}

		public void Save() {
			try {
				var directory = Path.GetDirectoryName(settingsPath);
				if (!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				var serializer = new XmlSerializer(typeof(AppSettings));
				using (var writer = new FileStream(settingsPath, FileMode.Create)) {
					serializer.Serialize(writer, this);
				}
			} catch (Exception e) {
				MessageBox.Show(String.Format("Failed to save settings: {0}", e.Message), "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}
