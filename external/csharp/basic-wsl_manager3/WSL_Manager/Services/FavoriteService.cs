using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WSL_Manager.Services
{
	public class FavoriteStore
	{
		private readonly string _jsonPath;
		private HashSet<string> _set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public FavoriteStore(string appName = "YourAppName")
		{
			_jsonPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				appName, "favorites.json");
			Load();
		}

		// public bool IsFavorite(string key) => _set.Contains(key);
		public bool IsFavorite(string key)
		{
			return (bool)_set.Contains(key);
		}
		public void SetFavorite(string key, bool isFav)
		{
			if (isFav)
				_set.Add(key);
			else
				_set.Remove(key);
			Save();
		}

		private void Load()
		{
			try {
				var dir = Path.GetDirectoryName(_jsonPath);
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				if (File.Exists(_jsonPath)) {
					var json = File.ReadAllText(_jsonPath, Encoding.UTF8);
					var data = JsonConvert.DeserializeObject<HashSet<string>>(json);
					_set = data ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				}
			} catch {
				_set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			}
		}

		private void Save()
		{
			try {
				var dir = Path.GetDirectoryName(_jsonPath);
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				var json = JsonConvert.SerializeObject(_set, Formatting.Indented);
				File.WriteAllText(_jsonPath, json, Encoding.UTF8);
			} catch {
				// 必要ならログ
			}
		}
	}
}


