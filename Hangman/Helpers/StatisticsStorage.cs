using Hangman.Models;
using System.IO;
using System.Text.Json;

namespace Hangman.Helpers
{
    public static class StatisticsStorage
    {
        private static string _filePath = "Profiles/statistics.json";

        public static List<UserStatistics> LoadAllStats()
        {
            if (!File.Exists(_filePath)) return new List<UserStatistics>();
            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<UserStatistics>>(json) ?? new List<UserStatistics>();
            }
            catch { return new List<UserStatistics>(); }
        }

        public static void SaveStats(List<UserStatistics> allStats)
        {
            string directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(allStats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
