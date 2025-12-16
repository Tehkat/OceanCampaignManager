using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MapManager.App.Models;

namespace MapManager.App.Services
{
    public class DataService
    {
        private readonly string _dataPath;

        public DataService()
        {
            _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "campaigns.json");
        }

        public List<Campaign> Load()
        {
            if (!File.Exists(_dataPath)) return new List<Campaign>();
            var json = File.ReadAllText(_dataPath);
            return JsonSerializer.Deserialize<List<Campaign>>(json) ?? new List<Campaign>();
        }

        public void Save(List<Campaign> campaigns)
        {
            var json = JsonSerializer.Serialize(campaigns, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dataPath, json);
        }
    }
}
