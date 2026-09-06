using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WSL_Manager.Services
{

    public class DescriptionStore
    {
        private readonly string _jsonPath;
        private Dictionary<string, string> _map = new Dictionary<string, string>();

        public DescriptionStore(string appName = "WSL Manager")
        {
            _jsonPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                appName, "descriptions.json");
            Load();
        }

        public string Get(string key)
        {
            string v;
            return _map.TryGetValue(key, out v) ? v : "";
        }

        public void Set(string key, string value)
        {
            _map[key] = value ?? "";
            Save();
        }

        private void Load()
        {
            try
            {
                var dir = Path.GetDirectoryName(_jsonPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (File.Exists(_jsonPath))
                {
                    var json = File.ReadAllText(_jsonPath, Encoding.UTF8);
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    _map = data ?? new Dictionary<string, string>();
                }
            }
            catch
            {
                _map = new Dictionary<string, string>();
            }
        }

        private void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_jsonPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonConvert.SerializeObject(_map, Formatting.Indented);
                File.WriteAllText(_jsonPath, json, Encoding.UTF8);
            }
            catch
            {
            }
        }
    }

}



